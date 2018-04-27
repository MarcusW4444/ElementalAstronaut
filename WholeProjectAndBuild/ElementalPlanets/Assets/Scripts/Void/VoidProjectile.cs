using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidProjectile : EnemyProjectile {

    // Use this for initialization
    public float StartTime = -10f;
    public VoidField VoidFieldPrefab;
    public bool ManipulatedByVoidSkeleton = false;
    public bool ManipulatedByVoidEye = false;
    //If these things get frozen, have the enemy manipulating them just disregard it and replace it
    public float ScaleFactor = 1f;
    //public ProjectileBlocker
    public float OrbitRadius = 1f;
    public float FormFactor = 0f;
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
    public bool Launched = true;
    // Update is called once per frame
    void Update()
    {
        if ((Live) && (Launched))
        {
            
            if ((Time.time - StartTime) >= 3f)
            {

                explode();
            }
        }
        this.transform.localScale = (Vector3.one * ScaleFactor * FormFactor);
        
            if (Disposing)
            {
                FormFactor = Mathf.Clamp01(FormFactor - Time.deltaTime);
                this.transform.localScale = (Vector3.one * ScaleFactor * FormFactor);
                if (FormFactor <= 0f)
                {
                    Remove();
                }
            }
            else
            {
                FormFactor = Mathf.Clamp01(FormFactor + Time.deltaTime);

            }
        
        
    }

    private void FixedUpdate()
    {
        //if (Live)
        //MyRigidbody.velocity = (MyRigidbody.velocity * (1f + Time.fixedDeltaTime*(1f+(2f*Astronaut.AggressionLevelF))));
    }
    public Rigidbody2D MyRigidbody;
    public ParticleSystem ParticleTrail, ParticleExplosion, NegativeParticles;
    public void Remove()
    {

        ParticleExplosion.transform.SetParent(null);
        //ParticleExplosion.Stop();
        GameObject.Destroy(ParticleExplosion.gameObject, 5f);

        ParticleTrail.transform.SetParent(null);
        ParticleTrail.Stop();
        NegativeParticles.Stop();
        NegativeParticles.transform.SetParent(null);
        GameObject.Destroy(ParticleTrail.gameObject, 5f);

        GameObject.Destroy(this.gameObject);
    }
    public float Damage = 10f;
    //public ParticleSystem ImpactParticles;
    public void explode()
    {
        if (!Live) return;
        Live = false;

        ParticleExplosion.Play(true);
        if (Astronaut.AggressionLevel > 0)
        {
            VoidField vf = GameObject.Instantiate<VoidField>(VoidFieldPrefab, this.transform.position, VoidFieldPrefab.transform.rotation);
            vf.Duration = (2f + (.5f * Astronaut.AggressionLevel));
            vf.ForceFactor = (1f + (.2f * Astronaut.AggressionLevel));
        }

        Remove();
    }

    private float lastHitTime = -10f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Astronaut plr = collision.GetComponent<Astronaut>();

        if ((plr != null) && (plr.Alive) && (!plr.Invulnerable) && (Live) && ((Time.time - lastHitTime)>=.4f))
        {
            lastHitTime = Time.time;
            Vector3 dif = (plr.transform.position - this.transform.position);

            plr.TakeDamage(Damage, new Vector3());
            //Am.am.oneshot(Am.am.M.FireballHit);
            //Am.am.oneshot(Am.am.M.LavaBurn);

            if (plr.Alive)
            {
                //plr.freeze(2f);
            }
            explode();

            if (Launched)
            {
                
            } else
            {

            }
        }
        else if ((collision.gameObject.CompareTag("Geometry")) || (collision.gameObject.GetComponent<IceBlock>() != null) || (collision.gameObject.GetComponent<IcePillar>() != null))
        {
            if ((collision.gameObject.GetComponent<IceBlock>() != null) || (collision.gameObject.GetComponent<IcePillar>() != null))
            {
                Am.am.oneshot(Am.am.M.PillarHit);
            } else
            {
                //Am.am.oneshot(Am.am.M.FireballHit);
            }

            if (Launched)
            {
                explode();
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerEnter2D(collision);
    }

    public void releaseManipulation()
    {
        //Check if the 

        if (Physics2D.OverlapPoint(this.MyRigidbody.position, LayerMask.GetMask(new string[] { "Geometry" })) == null)
        {
            if (ManipulatedByVoidSkeleton)
            {

            }
            else if (ManipulatedByVoidEye)
            {
                //Launch these projectiles
            }
        }

    }
}
