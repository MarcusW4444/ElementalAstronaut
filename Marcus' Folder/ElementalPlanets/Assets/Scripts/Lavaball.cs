using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lavaball : EnemyProjectile {

    // Use this for initialization
    //public bool Live = true;
    public bool Launched = false;

    public SpriteRenderer MySpriteRenderer;
    public Vector3 StartPosition;
	void Start () {
        Live = true;
        Launched = false;
        StartTime = Time.time;
        StartPosition = this.transform.position;
        MyCollider.enabled = false;
        MySpriteRenderer.transform.Rotate(0f,0f,360f*Random.value,Space.World);
        MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
    }

    private float StartTime = -10f;





    // Update is called once per frame
    public Collider2D MyCollider;
    public void Launch(Vector3 direction)
    {
        if (Launched) return;
        Launched = true;
        MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
        MyRigidbody.velocity = new Vector2(direction.x,direction.y);
        MyCollider.enabled = true;
        Rotvelocity = Random.Range(-100f,100f);
    }
    public float Rotvelocity = 0f;
    void Update()
    {
        if (Live)
        {

            if ((Time.time - StartTime) >= 3f)
            {

                //crash();
            }
        }
    }

    private void FixedUpdate()
    {
        //if (Live)
            //MyRigidbody.velocity = (MyRigidbody.velocity * (1f + Time.fixedDeltaTime * (1f + (2f * Astronaut.AggressionLevelF))));
            if (Launched)
        {
            MySpriteRenderer.transform.Rotate(0f,0f,Rotvelocity*Time.fixedDeltaTime,Space.World);
        }
    }
    public Rigidbody2D MyRigidbody;
    public ParticleSystem ParticleTrail, ParticleExplosion, NegativeParticles;
    private bool HasCrashed=false;
    public void crash() {
        if (HasCrashed) return;
        HasCrashed = true;
        
        foreach (ParticleSystem p in this.GetComponentsInChildren<ParticleSystem>())
        {
            
                p.transform.SetParent(null);
                    p.Stop();
                GameObject.Destroy(p.gameObject, 5f);


        }


        Astronaut plr = Astronaut.TheAstronaut;
        if ((plr != null) && (plr.Alive))
        {
            Vector3 dif = (this.transform.position - plr.transform.position);
            float sc = 1f;
            float pre = 1f;

            float radius = (3f * sc);//*pre
            float dist = (dif.magnitude / radius);
            if (dist < 1f)
            {


                if ((Time.time - plr.lastDamageTakenTime) >= 1.5f)
                {
                    Vector3 diff = (plr.transform.position - this.transform.position);
                    
                    float df = (1f - dist);
                    df = Mathf.Pow(df, 1f / (1f + (1f * Astronaut.AggressionLevelF)));
                    if (plr.TakeDamage(80 * df, new Vector3(0f, plr.JumpSpeed * 1f, 0f)))
                    {
                        
                    }

                }
            }
        }
        GameObject u = GameObject.Instantiate(ParticleExplosion,this.transform.position,ParticleExplosion.transform.rotation).gameObject;
        GameObject.Destroy(u.gameObject,10f);



        GameObject.Destroy(this.gameObject);
    }


    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!Live) return;
        Astronaut a = col.gameObject.GetComponent<Astronaut>();

        if ((a != null) && (a.Alive))
        {
            crash();
            return;
        }
        
        LavaPlatform lp = col.gameObject.GetComponent<LavaPlatform>();
        if ((lp != null) && (!lp.Destroyed))
        {
            lp.DestroyPlatform();
        crash();
        return;
        }

        
        
        if (col.gameObject.layer == LayerMask.NameToLayer("Geometry"))
        {
            crash();
            return;
        }
        
        
        


    }
}
