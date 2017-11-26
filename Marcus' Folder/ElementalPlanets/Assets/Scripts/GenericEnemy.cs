using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericEnemy : MonoBehaviour {

    // Use this for initialization
    public bool Alive = true;
    public float Health = 100f;
    public float MaxHealth = 100f;
    public Rigidbody2D MyRigidbody;
    public float StunTime = -10f;
    void Start () {
        Health = MaxHealth;
        HitsDone = 0f;


    }

    // Update is called once per frame
    public SpriteRenderer MySpriteRenderer;
    
    public void DamageFlashStep()
    {

        if (MySpriteRenderer)
        {
            if (Alive)
            {
                float ti = Mathf.Clamp01(((Time.time - lastDamageTakenTime)/(.1f+(.4f*(1f-(Health/MaxHealth))))));
                //if (ti > 0f)
                    //ti = Mathf.Pow(ti, 1f / 3f);
                Color shade = Color.Lerp(Color.black,new Color(1f,0f,0f),(Health/MaxHealth));
                MySpriteRenderer.color = Color.Lerp(shade,Color.white,Mathf.Clamp01(ti));

            }
        }
    }

    void Update () {

        if (Alive) {
            hitdirection = (hitdirection*(1f-Time.deltaTime));
            if (MyRigidbody.velocity.x != 0f)
            {
                MySpriteRenderer.flipX = (Mathf.Sign(MyRigidbody.velocity.x) < 0f);
            }
            freezeStep();
            DamageFlashStep();
        }
    }

    

    public bool isStunned()
    {
        return (((StunTime - Time.time) >= 0f)||Frozen);
    }


    public virtual void Kill()
    {
        if (!Alive) return;
        Alive = false;
        deathKnockback();
        Astronaut.TheAstronaut.dropResistance(.3f/(1f+HitsDone), this.transform.position, Astronaut.Element.Ice);


    }
    public float HitsDone = 0f;

    protected bool dontstopparticles = false;
    public void deathKnockback()
    {
        Collider2D[] cols = this.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in cols)
        {
            col.enabled = false;
        }
        Rigidbody2D rb = this.GetComponentInChildren<Rigidbody2D>();
        TrailRenderer[] tr = this.GetComponentsInChildren<TrailRenderer>();
        foreach (TrailRenderer t in tr)
        {
            if (!t.transform.Equals(this.transform))
            {
                t.transform.SetParent(null);
                GameObject.Destroy(t.gameObject, t.time);
            } else
            {
                t.enabled = false;
            }
        }
        foreach (ParticleSystem p in this.GetComponentsInChildren<ParticleSystem>())
        {
            if (!p.transform.Equals(this.transform))
            {
                p.transform.SetParent(null);
                if (!dontstopparticles)
                p.Stop();
                GameObject.Destroy(p.gameObject, p.main.duration);

            }
            else
            {
                //p.enabled = false;
                p.Stop();
            }
        }
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            
            rb.velocity = new Vector2(hitdirection.x, hitdirection.y);
            rb.gravityScale = 4f;
        }
        SpriteRenderer[] spr = this.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer s in spr)
        {
            s.color = Color.Lerp(s.color, Color.black, 0.75f);
            s.transform.Rotate(new Vector3(0f, 0f, 90f * Mathf.Sign(Random.value - .5f)));
        }
        GameObject.Destroy(this.gameObject,5f);
    }
    protected Vector2 hitdirection=new Vector2();
    private float lastDamageTakenTime = -10f;
    public void TakeDamage(float dmg,Vector2 dir)
    {
        if (!Alive) return;

        float hp = (Health - dmg);
        hitdirection = dir;
        if (hp <= 0f)
        {
            Kill();
        } else
        {
            lastDamageTakenTime = Time.time;
            Health = hp;
            
        }


    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Astronaut a = collision.gameObject.GetComponent<Astronaut>();
        if ((a != null) &&(a.Alive))
        {
            if (!isStunned())
            if ((Time.time - a.lastDamageTakenTime) >= 2f)
            {
                Vector3 dif = (a.transform.position - this.transform.position);
                    HitsDone += 1f;
                    if (a.TakeDamage(20f,dif.normalized*10f+new Vector3(0f, a.JumpSpeed, 0f)))
                    {
                        HitsDone += 4f;
                    }

            }

        }
    }

    public SpriteRenderer FreezeSprite;
    public ParticleSystem FreezeFlashEffect;
    public ParticleSystem MeltParticles;
    
    public bool Frozen = false;
    private RigidbodyType2D rbodtype = RigidbodyType2D.Dynamic;
    private float FreezeTime = -10f;
    private float usedgrav = 0f;
    public void freeze(float dur)
    {

        
        FreezeTime = Time.time + dur;
        if (Frozen) return;
        Frozen = true;
        foreach (Animator anim in this.GetComponentsInChildren<Animator>())
        {
            anim.enabled = false; //May not apply to all animators
        }
        FreezeSprite.enabled = true;
        rbodtype = MyRigidbody.bodyType;
        usedgrav = this.MyRigidbody.gravityScale;
        MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
        this.MyRigidbody.velocity = new Vector2(0f, 0f);
        this.MyRigidbody.gravityScale = 1f;
        
    }
    public void unfreeze()
    {
        if (!Frozen) return;
        foreach (Animator anim in this.GetComponentsInChildren<Animator>())
        {
            anim.enabled = true; //May not apply to all animators
        }
        usedgrav = this.MyRigidbody.gravityScale = usedgrav;
        Frozen = false;
        FreezeSprite.enabled = false;
        MyRigidbody.bodyType = rbodtype;
    }

    public void freezeStep()
    {
        if ((Frozen) && ((Time.time - FreezeTime) >= 0f))
        {
            unfreeze();
        }
        incinerateStep();
    }
    public float IncinerationTime = -10f;
    public ParticleSystem IncinerationParticles;
    public bool isIncinerating() { //Enemies act sporadically
        return (IncinerationTime > Time.time);
    }
    private bool wasincincerating = false;

    public void incinerateStep()
    {
        bool inc = isIncinerating();
        if (inc != wasincincerating)
        {
            if (inc)
            {
                IncinerationParticles.Play();
            } else
            {
                IncinerationParticles.Stop();
            }
        } 
        wasincincerating = inc;
    }
    public void incinerate(float ince)
    {

        if (Time.time > IncinerationTime)
        {
            IncinerationTime = Time.time + ince;
        } else
        {
            IncinerationTime += ince;
        }

    }

}
