using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameBar : MonoBehaviour {

    // Use this for initialization

    public ParticleSystem MyParticles;
    public bool FlameActive = false;
    public Collider2D MyCollider;
    public bool IgniteEnemies = true;
    public bool TerrainScorching = false;
    public bool IncinerateEnemies = false;
    public bool ConvectionField = false;
    public bool IsFlamePatch = false;
    public float StartTime = -10f;
	void Start () {
        SingeSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.LavaBurn, AudioManager.AM.EnvironmentAudioMixer, .2f, 1.5f, false);
        StartTime = Time.time;
    }
	
	// Update is called once per frame
	void Update () {
        if (IsFlamePatch)
        {
            if ((Time.time - StartTime) >= (4f * Astronaut.FirePowerFactor))
            {
                Remove();
            }
        }
	}

    public void Remove()
    {
        FlameActive = false;
        foreach (ParticleSystem ps in this.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Stop(true);
            ps.transform.SetParent(null);
            GameObject.Destroy(ps.gameObject,5f);
        }
        GameObject.Destroy(this.gameObject);
    }

    public ParticleSystem BurningParticles;
    public float FirePowerLevel = 0f;
    public AudioSource SingeSound;
    public ParticleSystem IncinerationParticlesPrefab;
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (IsFlamePatch)
        {
            FlameBar fb = collision.gameObject.GetComponent<FlameBar>();
            if (fb!= null)
            {
                if (fb.IsFlamePatch)
                {
                    if (fb.StartTime > this.StartTime)
                    {
                        Remove();
                        Debug.Log("Flame Patch override");
                    }
                }
                return;
            } 

            
        }
        if (!FlameActive) return;
        GenericEnemy ge = collision.GetComponent<GenericEnemy>();
        if (ge != null)
        {
            ge.LastBurnTime = Time.time;
            
            float inc = ((120f + (400f * Astronaut.FirePowerFactor)) * Time.fixedDeltaTime * (1f / Mathf.Max(ge.Burnability, 0.0001f)));
            if (ge.burn(inc / 200f, .5f*(1f+(3f*Astronaut.FirePowerFactor))))
            {
                ge.BurnDirection = ((int)Mathf.Sign(this.transform.forward.x));
                if (ge.BurnDirection == 0)
                    ge.BurnDirection = 1;
                //SingeSound.PlayOneShot(Am.am.M.LavaBurn, 1f);
            }
            if (!ge.AnchoredToVine) {
                Rigidbody2D rb = ge.gameObject.GetComponent<Rigidbody2D>();
                if(rb != null)
                {
                    rb.AddForce(new Vector2(this.transform.forward.x, this.transform.forward.y).normalized * 50f * (Astronaut.FirePowerFactor));
                    //Move the object if necessary.
                    
                }
                
                }
            if (ge.isIncinerating())
            {
                if (ge is IcePillar)
                {
                    (ge as IcePillar).Remove();
                    Am.am.oneshot(Am.am.M.MeltSound);
                }
                Am.am.oneshot(Am.am.M.LavaBurn,IsFlamePatch?.1f:1f);
            }
            //ge.TakeDamage(, this.transform.forward*0.001f);
            
                SingeSound.PlayOneShot(SingeSound.clip, IsFlamePatch ? .1f : 1f);
        }
        MeltableIceWall me = collision.GetComponent<MeltableIceWall>();

        if (me != null)
        {
            if (me.MeltParticles != null) me.MeltParticles.Emit(1);
            
            me.TakeDamage((120f + (400f * Astronaut.FirePowerFactor)) * Time.fixedDeltaTime, this.transform.forward * 0.001f);
            SingeSound.PlayOneShot(SingeSound.clip, .4f);

        }
        BreakableIceWall bw = collision.GetComponent<BreakableIceWall>();
        if (bw != null)
        {
            bw.TakeDamage((80f + (160f * Astronaut.FirePowerFactor)) * Time.fixedDeltaTime, this.transform.forward * 0.001f);
            SingeSound.PlayOneShot(SingeSound.clip, .2f);
        }
        BurnableLog bl = collision.GetComponent<BurnableLog>();
        if (bl != null)
        {
            bl.TakeDamage((80f + (400f * Astronaut.FirePowerFactor)) * Time.fixedDeltaTime, this.transform.forward * 0.001f);
            SingeSound.PlayOneShot(SingeSound.clip, .2f);
        }
        VoidGolem vg = collision.GetComponent<VoidGolem>();
        if ((vg != null) && (vg.VoidElementType == VoidGolem.VoidElement.Ice))
        {

            if (vg.Ice_FreezingSkinActive) {
                float dps = ((40f + (400f * Astronaut.FirePowerFactor)) * Time.fixedDeltaTime);
                vg.SkinMeltingParticles.Emit(1);
                if (vg.IceSkinHealth <= dps) {
                    vg.IceSkinHealth = 0f;
                    vg.meltSkinOff();
                    Am.am.oneshot(Am.am.M.MeltSound);
                } else
                {
                    vg.IceSkinHealth = vg.IceSkinHealth - dps;
                    
                    SingeSound.PlayOneShot(SingeSound.clip, .2f);
                }
            }
            
            //vg.TakeDamage(, this.transform.forward * 0.001f);
            
        }

        
        IceShard ic = collision.GetComponent<IceShard>();
        if (ic != null)
        {
            ic.transform.localScale = (ic.transform.localScale * (.1f+(.85f*(1f-FirePowerLevel))));
            ic.Live = false;
            ic.Melting = true;
            SingeSound.Play();
            if (ic.transform.localScale.magnitude <= .1f) {
                Am.am.oneshot(Am.am.M.MeltSound);
                GameObject.Destroy(ic.gameObject);
            }
        }

        IceBlock ib = collision.GetComponent<IceBlock>();
        if (ib != null)
        {
            ib.Remove();
            SingeSound.Play();
            //
            //SingeSound.Play();
            ib.transform.localScale = (ib.transform.localScale * .5f);
            if (ib.transform.localScale.magnitude <= .1f)
            {
                Am.am.oneshot(Am.am.M.MeltSound);
                ib.Remove();
                //GameObject.Destroy(ib.gameObject);
            }
        }


    }
}
