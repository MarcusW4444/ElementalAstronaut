using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSkullProjectile : EnemyProjectile {

    // Use this for initialization
    private float StartTime = -10f;
    public float LifeTime = 3f;
	void Start () {
        StartTime = Time.time;
	}

    // Update is called once per frame
    public bool EtherealBehavior = false;
	void Update () {
		if (Live)
        {
            if ((Time.time - StartTime) >= LifeTime)
            {
                
                explode();
            }
        }
	}
    public Rigidbody2D MyRigidbody;
    public ParticleSystem ParticleTrail, ParticleExplosion,NegativeParticles;
    public void Remove()
    {

        ParticleExplosion.transform.SetParent(null);
        //ParticleExplosion.Stop();
        GameObject.Destroy(ParticleExplosion.gameObject, 5f);

        ParticleTrail.transform.SetParent(null);
        ParticleTrail.Stop();
        NegativeParticles.Stop();
        NegativeParticles.transform.SetParent(null);
        GameObject.Destroy(ParticleTrail.gameObject,5f);

        GameObject.Destroy(this.gameObject);
    }
    public void explode()
    {
        if (!Live) return;
        Live = false;
        
        ParticleExplosion.Play();
        Remove();
    }
    public float FreezeTimeMultiplier = 1.0f;
    public float DamageMultiplier = 1.0f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Astronaut plr = collision.GetComponent<Astronaut>();

        if ((plr != null) && (plr.Alive) && (!plr.Invulnerable) && (Live))
        {
            if (!EtherealBehavior)
            {



                Vector3 dif = (plr.transform.position - this.transform.position);
                plr.TakeDamage(30f*DamageMultiplier, dif.normalized * 10f + new Vector3(0f, plr.JumpSpeed / 4f, 0f));
                if (plr.Alive)
                {
                    if ((!plr.Frozen) && ((Time.time - plr.UnfreezeTime) >= 1.5f))
                        plr.freeze(1.1f*FreezeTimeMultiplier);
                }
            } else
            {
                plr.SendBackToEtherealCheckpoint();
            }

            explode();
        } else if ((collision.gameObject.CompareTag("Geometry")) || (collision.gameObject.GetComponent<IceBlock>() != null) || (collision.gameObject.GetComponent<IcePillar>() != null))
        {

            explode();
        }
    }
    
}
