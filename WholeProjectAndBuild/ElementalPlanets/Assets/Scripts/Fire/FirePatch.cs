using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePatch : Hazard {

    // Use this for initialization
    public static List<FirePatch> existingFirePatches = new List<FirePatch>();
    public float Lifetime = 3f;
    private float StartTime = -10f;
    public Collider2D MyCollider;
    public ParticleSystem SingeParticles;
	void Start () {
        StartTime = Time.time;
        
        existingFirePatches.Add(this);
        OnParticleEffectLevelChanged();
        BurningSoundLight = AudioManager.AM.createAudioSource(Am.am.M.LavaPatchBurnLight,this.gameObject, new Vector3(0f, 0f, 0f), Am.am.M.PlayerAudioMixer,1f,1f,true);
        BurningSoundHeavy = AudioManager.AM.createAudioSource(Am.am.M.LavaPatchBurnHeavy, this.gameObject, new Vector3(0f,0f,0f), Am.am.M.PlayerAudioMixer, 0f, 1f, true);
        BurningSoundHeavy.loop = true;
        BurningSoundLight.loop = true;
        BurningSoundLight.Play();
        BurningSoundHeavy.Play();
    }
    private AudioSource BurningSoundLight, BurningSoundHeavy;
    bool prevparticlelowlevel = false;
    public void OnParticleEffectLevelChanged()
    {
        if (prevparticlelowlevel != GameManager.TheGameManager.UsingLowParticleEffects)
        {
            //*((!prevparticlelowlevel)?(1f/2f):2f)
            //reduce or restore particle emission/duration/size
            //
            foreach (ParticleSystem ps in this.GetComponentsInChildren<ParticleSystem>())
            {
                ParticleSystem.EmissionModule e = ps.emission;
                e.rateOverTimeMultiplier = (e.rateOverTimeMultiplier * ((!prevparticlelowlevel) ? (1f / 8f) : 8f));
                //e.rateOverDistanceMultiplier = (e.rateOverDistanceMultiplier * ((!prevparticlelowlevel) ? (1f / 4f) : 4f));

            }

            prevparticlelowlevel = GameManager.TheGameManager.UsingLowParticleEffects;
        }

    }
    public Enemy_Fire_Slug MyFireSlug = null;

    // Update is called once per frame
    public bool Live = true;
	void Update () {
		if (!Live)
        {
            //...
            if (Removing)
            {

                if (MagmaSprite.transform.localScale.y < (sy*.01f))
                {
                    
                    GameObject.Destroy(this.gameObject);
                } else
                {
                    MagmaSprite.transform.localScale = new Vector3(MagmaSprite.transform.localScale.x, MagmaSprite.transform.localScale.y-(sy*Time.deltaTime), 
                        MagmaSprite.transform.localScale.z);
                    BurningSoundLight.volume = (BurningSoundLight.volume - (sy * Time.deltaTime));
                    BurningSoundHeavy.volume = (BurningSoundHeavy.volume - (sy * Time.deltaTime));

                }
            }
        } else
        {
            BurningSoundHeavy.volume = Mathf.Lerp(BurningSoundHeavy.volume, 1f, Time.deltaTime/2f);
            if ((Time.time - StartTime)>=Lifetime)
            {
                Live = false;
                Remove();
            }
        }
	}
    public SpriteRenderer MagmaSprite;
    private bool Removing = false;
    private float sy = 1f;
    public void Remove()
    {
        Live = false;
        Removing = true;
        sy = MagmaSprite.transform.localScale.y;
        foreach (ParticleSystem ps in this.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Stop(true);
            ps.transform.SetParent(null);
            GameObject.Destroy(ps.gameObject,10f);

        }
        
        
        if (existingFirePatches.Contains(this))
        {
            existingFirePatches.Remove(this);
        }
        
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!Live) return;
        Astronaut a = collision.gameObject.GetComponent<Astronaut>();
        if ((a != null))
        {
            BurningSoundHeavy.volume = Mathf.Lerp(BurningSoundHeavy.volume, 1f, .75f);
            if (a.Alive)
            if (((Time.time - a.lastDamageTakenTime) >= .05f) && ((Time.time - a.ReviveTime) >= 1.5f))
                {
                    Vector3 dif = (a.transform.position - this.transform.position);
                    
                    MyFireSlug.HitsDone += .1f;

                    
                if (a.TakeDamage(5f, new Vector3(0f, 0f, 0f)))
                    {
                    MyFireSlug.HitsDone += 4f;
                    }

                }
            SingeParticles.transform.position = new Vector3(a.transform.position.x, this.transform.position.y, this.transform.position.z);
            SingeParticles.Emit(1);
            return;
        }
        Enemy_Fire_Slug fs = collision.gameObject.GetComponent<Enemy_Fire_Slug>();
        if ((fs != null) &&(fs.Alive))//if ((MyFireSlug != null) && collision.gameObject.Equals(MyFireSlug))
        {
            fs.IsInFirePatch = true;
        }
    }
    public override void permafreezeUnique()
    {
        base.permafreezeUnique();
        foreach (ParticleSystem ps in this.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Stop(true);
            ps.transform.SetParent(null);
            GameObject.Destroy(ps.gameObject, 10f);

        }
        GameObject.Destroy(this.gameObject,5f);

    }
}
