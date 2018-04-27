using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JungleTreeProjectile : EnemyProjectile {

    // Use this for initialization
    private float StartTime = -10f;
    public bool IsSpore = false;
	void Start () {
        StartTime = Time.time;
	}

    // Update is called once per frame
    public float DetonationTime = 2f;

    void Update () {
		if (Live)
        {
            if (!IsSpore)
            if ((Time.time - StartTime) >= (IsSubdivided?4f:DetonationTime))
            {
                
                explode();
            }
        }
	}
    public const float WINDFORCEFACTOR = 20f;
    public float driftTime = -10f;
    public bool drifting = false;
    private void FixedUpdate()
    {
        if (IsSpore)
        {
            IsSubdivided = true;
            
            
            if (!drifting)
            {
                MyConstantForce.enabled = false;
                MyConstantForce.force = new Vector2();
                MyRigidbody.velocity = (MyRigidbody.velocity * (1f - (Time.fixedDeltaTime * 2f)));
                if (MyRigidbody.velocity.magnitude < 0.1f)
                {
                    driftTime = Time.time;
                    drifting = true;
                    
                }
                

            } else
            {
                MyConstantForce.enabled = true;
                MyConstantForce.force = Vector2.Lerp(MyConstantForce.force, (Random.insideUnitCircle.normalized * (WINDFORCEFACTOR*.2f*(1f+ (Astronaut.AggressionLevelF*1f)))), .5f)+((new Vector2(0f,-1f)*.25f)*(1f-Astronaut.AggressionLevelF));
                if ((Time.time - driftTime) >= 15f)
                {
                    explode();
                }
            }
           
            
        }
        else
        {

            if (Live)
            {
                if (IsSubdivided)
                {
                    MyConstantForce.force = Vector2.Lerp(MyConstantForce.force, (Random.insideUnitCircle.normalized * WINDFORCEFACTOR), 1f);
                }
                Astronaut plr = Astronaut.TheAstronaut;
                Vector3 dif = (plr.transform.position - this.transform.position);
                if (dif.magnitude < 8f)
                {
                    //nerfed. They came from nowhere at full speed and you had no time to react.
                    MyRigidbody.AddForce(new Vector2(dif.x, dif.y).normalized * WINDFORCEFACTOR * 1f * Astronaut.AggressionLevelF);
                }
            }
        }
    }

    public Rigidbody2D MyRigidbody;
    public ConstantForce2D MyConstantForce;
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
        GameObject.Destroy(NegativeParticles.gameObject, 5f);
        GameObject.Destroy(ParticleTrail.gameObject,5f);

        GameObject.Destroy(this.gameObject);
    }
    public void explode()
    {
        if (!Live) return;
        if (IsSpore)
        {
            Live = false;
            ParticleExplosion.Play(true);
            Remove();
        }
        else
        {
            if (IsSubdivided)
            {
                Live = false;
                ParticleExplosion.Play(true);
                Remove();
            }
            else
            {
                int subs = (6 + (Astronaut.AggressionLevel * 2));
                int subsets = (1);

                for (int i = 0; i < (subs * subsets); i++)
                {
                    JungleTreeProjectile proj = GameObject.Instantiate(this.gameObject, this.transform.position, this.transform.rotation).GetComponent<JungleTreeProjectile>();
                    proj.transform.localScale = (this.transform.localScale * (.3f));
                    proj.DamageRatio = (DamageRatio * (1f / ((float)(subs * subsets))));
                    float ang = (360f * (((float)i) / ((float)(subs))));
                    proj.MyRigidbody.velocity = (new Vector2(Mathf.Cos((ang / 360f) * 2f * Mathf.PI), Mathf.Sin((ang / 360f) * 2f * Mathf.PI)) * this.MyRigidbody.velocity.magnitude * ((1 + (i / subs)) / subsets));
                    proj.IsSubdivided = true;
                    proj.StartTime = Time.time;
                    proj.MyConstantForce.force = (Random.insideUnitCircle.normalized * WINDFORCEFACTOR);
                    //proj
                }
                IsSubdivided = true;
                Live = false;
                ParticleExplosion.Play(true);
                Remove();
            }
        }
    }



    public float DamageRatio = 1f;
    public bool IsSubdivided = false;
    private float SporeAffectLevel = 0f;
    public ParticleSystem SporeParticles;
    public void setSporeEffectLevel(float l)
    {
        SporeAffectLevel = l;
        if (SporeParticles)
        if (SporeAffectLevel > 0f)
        {
            SporeParticles.Play();
        } else
        {
            SporeParticles.Stop();
        }
    }
    public float SporeFactor = 1f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Astronaut plr = collision.GetComponent<Astronaut>();

        if ((plr != null) && (plr.Alive) && (!plr.Invulnerable) && (Live))
        {
            
            Vector3 dif = (plr.transform.position - this.transform.position);
            plr.TakeDamage(40f*DamageRatio,0f* (dif.normalized * 10f + new Vector3(0f, plr.JumpSpeed/4f,0f)*DamageRatio));
            if (plr.Alive)
            {
                plr.affectSpore(4f*DamageRatio* SporeFactor * Astronaut.AggressionLevelF);
            }

            explode();
        } else if ((collision.gameObject.CompareTag("Geometry")) || (collision.gameObject.GetComponent<IceBlock>() != null) || (collision.gameObject.GetComponent<IcePillar>() != null))
        {

            explode();
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnTriggerEnter2D(collision.collider);

    }

}
