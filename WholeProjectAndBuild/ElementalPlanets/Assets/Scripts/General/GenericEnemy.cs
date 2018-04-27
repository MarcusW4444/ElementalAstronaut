using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericEnemy : MonoBehaviour {

    // Use this for initialization
    public bool Alive = true;
    public bool EtherealBehavior = false;
    public bool EtherealWillingToTeach = false;
    public int LessonIndex = 0;
    public float Health = 100f;
    public float MaxHealth = 100f;
    public float VitaWorth = 0.25f;
    public Rigidbody2D MyRigidbody;
    [HideInInspector]public float StunTime = -10f;
    [HideInInspector]public bool VineStrangle = false;
    [HideInInspector]public float LastBurnTime = -10f;
    [HideInInspector]public ParticleSystem mydrainparticles;
    public bool AnchoredToVine = false;
    [HideInInspector]
    public int BurnDirection = 0;
    void Start () {
        Health = MaxHealth;
        HitsDone = 0f;


    }

    // Update is called once per frame
    public SpriteRenderer MySpriteRenderer;
    protected Color BaseColor = Color.white;
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
                shade = new Color(shade.r, shade.g, shade.b,BaseColor.a);


                //MySpriteRenderer.color = Color.Lerp(Color.Lerp(shade,BaseColor,Mathf.Clamp01(ti)),new Color(.5f,.5f,1f),FreezeFactor);
                Color cool = Color.Lerp(shade, BaseColor, Mathf.Clamp01(ti));
                Color ash = Color.Lerp(cool, new Color(0f, 0f, 0f, shade.a), IncinerationFactor);
                MySpriteRenderer.color = ash;
                MySpriteRenderer.material.SetFloat("_FrozenFactor", Frozen ? 1f:FreezeFactor*.5f);
                MySpriteRenderer.material.SetInt("_VineStrangle", (VineStrangle ? 1 : 0));

            } else
            {
                MySpriteRenderer.material.SetInt("_VineStrangle", 0);
            }
             
            
        }
    }
    protected bool ManualFlipping = false;
    void Update () {

        if (Alive) {
            hitdirection = (hitdirection*(1f-Time.deltaTime));
            if (!ManualFlipping)
            if (MyRigidbody.velocity.x != 0f)
            {
                MySpriteRenderer.flipX = (Mathf.Sign(MyRigidbody.velocity.x) < 0f);
            }
            freezeStep();
            DamageFlashStep();
        } else
        {
            if (MySpriteRenderer)
            {
                MySpriteRenderer.material.SetInt("_VineStrangle", 0);

            }
        }

    }

    

    public virtual bool isStunned()
    {
        return (((StunTime - Time.time) >= 0f)||Frozen||isIncinerating());
    }


    public virtual void Kill()
    {
        if (!Alive) return;
        Alive = false;
        deathKnockback();
        Astronaut.TheAstronaut.dropResistance(VitaWorth / (1f+HitsDone), this.transform.position, Astronaut.Element.Ice);


    }
    public float HitsDone = 0f;

    protected bool dontstopparticles = false;
    public void deathKnockback()
    {
        if (Frozen) { Am.am.oneshot(Am.am.M.IceShatter); }
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
    protected float lastDamageTakenTime = -10f;
    public virtual void TakeDamage(float dmg,Vector2 dir)
    {
        if (!Alive) return;
        //if (Astronaut.AggressionLevelAbsolute > 3)
        //{
            dmg = (dmg * (1f / (1f + ((Astronaut.AggressionLevelAbsolute) * .25f))));
        //}
        if (IncinerationFactor > 0f) dmg *= 1f+((1f+(2f*Astronaut.FirePowerFactor))*IncinerationFactor);
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
                    if (a.TakeDamage(20f,dif.normalized*5f+new Vector3(0f, a.JumpSpeed*.3f, 0f)))
                    {
                        HitsDone += 4f;
                    }

            }

        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionStay2D(collision);
    }

    [HideInInspector]public SpriteRenderer FreezeSprite;
    [HideInInspector]public ParticleSystem FreezeFlashEffect;
    [HideInInspector]public ParticleSystem MeltParticles;
    
    [HideInInspector]public bool Frozen = false;
    public float Freezability = 1f;
    public float Burnability = 1f;
    [HideInInspector]public float FreezeFactor = 0f;
    private float FreezingTime = -10f;
    private RigidbodyType2D rbodtype = RigidbodyType2D.Dynamic;
    [HideInInspector]public float FreezeTime = -10f;
    private float usedgrav = 0f;
    public virtual void freeze(float dur)
    {

        
        FreezeTime = Time.time + dur;
        if (Frozen) return;
        Frozen = true;
        foreach (Animator anim in this.GetComponentsInChildren<Animator>())
        {
            anim.enabled = false; //May not apply to all animators
        }
        Am.am.sound(Am.am.M.chooseSound(Am.am.M.FreezeSound1, Am.am.M.FreezeSound2, Am.am.M.FreezeSound3, Am.am.M.FreezeSound4));
        //FreezeSprite.enabled = true;
        rbodtype = MyRigidbody.bodyType;
        usedgrav = this.MyRigidbody.gravityScale;
        if (rbodtype == RigidbodyType2D.Dynamic)
        {
            MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
        }


        this.MyRigidbody.velocity = new Vector2(0f, 0f);
        this.MyRigidbody.gravityScale = 1f;
        MyFreezeFlare.Play(true);
        MyFreezeDust.Play(true);
        MyFreezeSparkles.Play(true);



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
        MyFreezeDust.Stop(true);
        if (FreezeSprite != null)
        {
            FreezeSprite.enabled = false;
        }
        MyRigidbody.bodyType = rbodtype;
    }

    public void freezeStep()
    {
        if ((Frozen) && ((Time.time - FreezeTime) >= 0f))
        {
            FreezeFactor = 0f;
            unfreeze();
        } else
        {
            if ((Time.time - FreezingTime) >= .5f)
            {
                FreezeFactor = Mathf.Clamp01(FreezeFactor-(Time.fixedDeltaTime/Freezability));
            }
        }


        incinerateStep();
    }
    public bool slowFreeze(float freezeamt,float duration)
    {
        if (Frozen)
        {
            FreezeTime += (freezeamt* Freezability);
            FreezingTime = Time.time;
            return false;
        } else
        {
            float f = (FreezeFactor + (freezeamt*Freezability));
            if (f >= 1f)
            {
                FreezeFactor = 1f;
                freeze(duration);
                return true;
            } else
            {
                FreezeFactor = f;
                FreezingTime = Time.time;
                return false;
            }
        }

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
        if (!inc)
        {
            if (IncinerationFactor > 0f)
            {
                if ((Time.time - LastBurnTime) >= .5f) {
                    IncinerationFactor = Mathf.Clamp01(IncinerationFactor-Time.deltaTime);
                    
                }
            }
        }
        if (!IncinerationParticles)
        {
            IncinerationParticles = GameObject.Instantiate<ParticleSystem>(Astronaut.TheAstronaut.IncinerationParticlesPrefab, this.transform.position, Astronaut.TheAstronaut.IncinerationParticlesPrefab.transform.rotation, this.transform);
        }
            if (inc != wasincincerating)
            {
                if (inc)
                {
                    IncinerationParticles.Play();
                }
                else
                {
                    IncinerationParticles.Stop();
                }
            }
        
        wasincincerating = inc;
    }
    public float IncinerationFactor = 0f;
    public bool burn(float factor,float ince)
    {
        LastBurnTime = Time.time;
        if (isIncinerating())
        {

            IncineratesHealth = false;
            incinerate(ince);
            onIncineratedAgain();
            IncinerationFactor = 1f;
            return false;
        } else {
            float f = (IncinerationFactor + factor);
            if (f >= 1f)
            {
                if (IncineratesHealth)
                {
                    this.TakeDamage(200f * factor, new Vector2());
                    //incinerate(ince);
                    IncinerationFactor = 1f;
                }
                else
                {
                    onIncinerated();
                    incinerate(ince);
                    IncinerationFactor = 1f;
                }
                
                return true;
            } else
            {
                IncinerationFactor = f;
                return false;
            }
                }
        
    }
    public bool IncineratesHealth = false;
    public void incinerate(float ince)
    {

        if (Time.time > IncinerationTime)
        {
            IncinerationTime = Time.time + ince;
        } else
        {
            IncinerationTime = Mathf.Max(Time.time+ince, IncinerationTime);
        }

    }

    public virtual void onIncinerated()
    {
        if (MyRigidbody.bodyType == RigidbodyType2D.Dynamic)
        {
            if (MyRigidbody.velocity.y < 1f)
            {
                MyRigidbody.velocity = new Vector2(((Random.value*2f)-1f)*3f,20f);
            }
        }
    }
    public virtual void onIncineratedAgain()
    {

    }

    public ParticleSystem MyFreezeFlare, MyFreezeSparkles, MyFreezeDust;
    

}
