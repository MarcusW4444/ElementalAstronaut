using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JungleMonkeyProjectile : EnemyProjectile {

    // Use this for initialization
    private float StartTime = -10f;
	void Start () {
        StartTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (Live)
        {
            if ((Time.time - LastRotateTime) > .05f)
            {
                //Debug.Log("Banana Spin!");
                this.transform.Rotate(0f, 0f, 30f * ((LastRotateTime == -10f) ? Random.Range(0, 360 / 30) : 1), Space.World);
                LastRotateTime = Time.time;
            }
            if ((Time.time - StartTime) >= 3f)
            {
                
                explode();
            }
        }
	}
    private float LastRotateTime = -10f;
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
        
        ParticleExplosion.Play(true);
        Remove();
    }

    public float DamageRatio = 1f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Astronaut plr = collision.GetComponent<Astronaut>();

        if ((plr != null) && (plr.Alive) && (!plr.Invulnerable) && (Live))
        {
            
            Vector3 dif = (plr.transform.position - this.transform.position);
            plr.TakeDamage(15f*DamageRatio, (dif.normalized * 10f + new Vector3(0f, plr.JumpSpeed/4f, 0f))*0f);
            if (plr.Alive)
            {
                
            }

            explode();
        } else if ((collision.gameObject.CompareTag("Geometry")) || (collision.gameObject.GetComponent<IceBlock>() != null) || (collision.gameObject.GetComponent<IcePillar>() != null))
        {

            explode();
        }
    }
    
}
