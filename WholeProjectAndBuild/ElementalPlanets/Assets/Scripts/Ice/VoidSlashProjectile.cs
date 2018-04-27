using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidSlashProjectile : EnemyProjectile
{


    // Use this for initialization
    public float StartTime = -10f;
    
    
    
    
    void Start()
    {
        StartTime = Time.time;
        OnParticleEffectLevelChanged();
    }
    public bool Disposing = false;
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
                e.rateOverDistanceMultiplier = (e.rateOverDistanceMultiplier * ((!prevparticlelowlevel) ? (1f / 4f) : 4f));

            }

            prevparticlelowlevel = GameManager.TheGameManager.UsingLowParticleEffects;
        }

    }

    // Update is called once per frame
    private float LastRotateTime = -10f;
    void Update()
    {
        if (Live)
        {
            if (Time.time - LastRotateTime > .025f)
            {
                //Debug.Log("Banana Spin!");
                this.transform.Rotate(0f, 0f, 30f * ((LastRotateTime == -10f) ? Random.Range(0, 360 / 30) : 1), Space.World);
                LastRotateTime = Time.time;
            }
            

            if ((Time.time - StartTime) >= 5f)
            {

                slice();
            }
        }
        


    }

    private void FixedUpdate()
    {
        if (Live)
        {
            //MyRigidbody.velocity = (MyRigidbody.velocity * (1f + Time.fixedDeltaTime * (1f + (2f * Astronaut.AggressionLevelF))));
            Vector3 dif = (Astronaut.TheAstronaut.transform.position - this.transform.position);
            float dot = Vector2.Dot(MyRigidbody.velocity.normalized, new Vector2(dif.x,dif.y).normalized);
            if (dot < 0f) {
                if (MyRigidbody.velocity.x != 0f)
                {
                    MyRigidbody.velocity = new Vector2(MyRigidbody.velocity.x * (1f - Time.fixedDeltaTime), MyRigidbody.velocity.y * (1.0f + (Time.fixedDeltaTime * 4f)));
                }
            }
        }
    }
    public Rigidbody2D MyRigidbody;
    public ParticleSystem SliceFlare, NegativeParticles;
    public TrailRenderer ParticleTrail;
    public void Remove()
    {

        SliceFlare.transform.SetParent(null);
        //ParticleExplosion.Stop();
        GameObject.Destroy(SliceFlare.gameObject, 5f);

        ParticleTrail.transform.SetParent(null);
        
        
        NegativeParticles.Stop();
        NegativeParticles.transform.SetParent(null);
        GameObject.Destroy(ParticleTrail.gameObject, 5f);
        GameObject.Destroy(NegativeParticles.gameObject, 5f);

        GameObject.Destroy(this.gameObject);
    }
    public float Damage = 10f;
    //public ParticleSystem ImpactParticles;
    public void slice()
    {
        if (!Live) return;
        Live = false;

        SliceFlare.Play(true);
        

        Remove();
    }

    private float lastHitTime = -10f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Astronaut plr = collision.GetComponent<Astronaut>();

        if ((plr != null) && (plr.Alive) && (!plr.Invulnerable) && (Live)&&(lastHitTime < 0f))
        {
            lastHitTime = Time.time;
            Vector3 dif = (plr.transform.position - this.transform.position);
            Am.am.oneshot(Am.am.M.VoidPhantom_SlashHit);
            Am.am.oneshot(Am.am.M.VoidPhantom_SlashHitMelee);
            plr.TakeDamage(Damage, ((new Vector3(MyRigidbody.velocity.x, MyRigidbody.velocity.y,0f).normalized*-plr.JumpSpeed) + new Vector3(0f,plr.JumpSpeed/8,0f))*(.1f+(.9f*Astronaut.AggressionLevelF)));
            //Am.am.oneshot(Am.am.M.FireballHit);
            //Am.am.oneshot(Am.am.M.LavaBurn);

            if (plr.Alive)
            {
                //plr.freeze(2f);
            }
            SliceFlare.Play(true);

            //slice();
            
        }
        else if ((collision.gameObject.CompareTag("Geometry")) || (collision.gameObject.GetComponent<IceBlock>() != null) || (collision.gameObject.GetComponent<IcePillar>() != null))
        {
            if ((collision.gameObject.GetComponent<IceBlock>() != null) || (collision.gameObject.GetComponent<IcePillar>() != null))
            {
                Am.am.oneshot(Am.am.M.PillarHit);
            }
            else
            {
                //Am.am.oneshot(Am.am.M.FireballHit);
            }

            
                slice();
            
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerEnter2D(collision);
    }
    
}
