using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireHoundProjectile : EnemyProjectile {

    // Use this for initialization
    private float StartTime = -10f;

    void Start()
    {
        StartTime = Time.time;
        OnParticleEffectLevelChanged();
    }
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
    void Update()
    {
        if (Live)
        {
            
            if ((Time.time - StartTime) >= 3f)
            {

                explode();
            }
        }
    }

    private void FixedUpdate()
    {
        if (Live)
        MyRigidbody.velocity = (MyRigidbody.velocity * (1f + Time.fixedDeltaTime*(1f+(2f*Astronaut.AggressionLevelF))));
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
    public void explode()
    {
        if (!Live) return;
        Live = false;

        ParticleExplosion.Play(true);
        Remove();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Astronaut plr = collision.GetComponent<Astronaut>();

        if ((plr != null) && (plr.Alive) && (!plr.Invulnerable) && (Live))
        {

            Vector3 dif = (plr.transform.position - this.transform.position);
            plr.TakeDamage(25f, dif.normalized * 1f + new Vector3(0f, plr.JumpSpeed / 4f, 0f));
            Am.am.oneshot(Am.am.M.FireballHit);
            Am.am.oneshot(Am.am.M.LavaBurn);

            if (plr.Alive)
            {
                //plr.freeze(2f);
            }

            explode();
        }
        else if ((collision.gameObject.CompareTag("Geometry")) || (collision.gameObject.GetComponent<IceBlock>() != null) || (collision.gameObject.GetComponent<IcePillar>() != null))
        {
            if ((collision.gameObject.GetComponent<IceBlock>() != null) || (collision.gameObject.GetComponent<IcePillar>() != null))
            {
                Am.am.oneshot(Am.am.M.PillarHit);
            } else
            {
                Am.am.oneshot(Am.am.M.FireballHit);
            }

            explode();
        }
    }
}
