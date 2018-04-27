using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class VoidBush : GenericEnemy
{

    // Use this for initialization
    public Transform StandByTransform;
    private Vector3 StandByLocation, DestinationLocation;

    void Start()
    {
        OriginalScale = transform.localScale;
        int sh = (2 * (1 + Astronaut.AggressionLevel));
        int v = Random.Range(0,sh);
        TargetScale = (new Vector3(1+v,1+(sh-v),1f)*.4f);
        IncineratesHealth = true;
        Extended = 0f;
        BaseColor = MySpriteRenderer.color;
        StartTime = Time.time;
    }
    private float StartTime;
    public Vector3 OriginalScale;
    public Vector3 TargetScale = new Vector3(3f,3f,1f);
    public Collider2D MyCollider;
    private float Extended = 0f;
    public bool Extending = false;
    void FixedUpdate()
    {
        Freezability = (2f * (1f - (.75f * Astronaut.AggressionLevelF)));
        if (Alive)
        {
            //Regenerate Health
            if (!Shattered)
            {
                if ((Time.time - LastBurnTime) >= (1f - (1f * Astronaut.AggressionLevelF)))
                {
                    if (!Frozen)
                    {
                        Health = Mathf.Min(Health + ((MaxHealth * .25f) * Time.fixedDeltaTime*.25f * (1f + (1.5f * Astronaut.AggressionLevelF))), MaxHealth);
                    }
                }
            }
            else if ((Time.time - ShatteredTime) >= (8f * (1f - .75f * Astronaut.AggressionLevelF)))
            {
                Shattered = false;
            }


            if (!Frozen)
                Extended = Mathf.Lerp(Extended, Health / MaxHealth, Mathf.Clamp01(Time.fixedDeltaTime * 8f));
            this.transform.localScale = (TargetScale * Extended);//Vector3.Lerp(StandByLocation, DestinationLocation, Extended);
            //ExtendedCollider.transform.localScale = new Vector3((this.transform.position - StandByLocation).magnitude,1f,1f);
           
        }


        
        if ((Lifetime > 0f)&&((Time.time - StartTime) >= Lifetime))
        {
            Health = Health*.95f;
            Alive = false;

            this.transform.localScale = (TargetScale * Extended);//Vector3.Lerp(StandByLocation, DestinationLocation, Extended);
            if (Health<0.01f)
            {
                MyCollider.enabled = false;
                GameObject.Destroy(this.gameObject);
            }
        } else
        {
            if ((Extended > 0f) && (Alive))
            {
                MyCollider.enabled = true;
                //ExtendedCollider.enabled = true; 
            }
            else
            {
                MyCollider.enabled = false;
                //ExtendedCollider.enabled = false;
            }
        }

    }
    public float Lifetime = 10f;
    public void popOut()
    {
        if (Extending) return;
        Extending = true;
    }
    public Transform ExtendingSprite;
    public bool Enchanted = false;
    public override void Kill()
    {
        GameObject.Destroy(this.gameObject);
    }
    public override void onIncinerated()
    {
        //base.onIncinerated();
        GameObject.Destroy(this.gameObject);
    }
    public bool Shattered = false;
    public float ShatteredTime = -10f;
    public override void TakeDamage(float dmg, Vector2 dir)
    {
        if (!Alive) return;
        if (Shattered) return;
        if (!Frozen)
            //if (Astronaut.AggressionLevelAbsolute > 3)
            //{
            dmg = (dmg * (1f / (1f + ((Astronaut.AggressionLevel) * .25f))));
        //}
        if (IncinerationFactor > 0f) dmg *= 1f + (3f * IncinerationFactor);
        float hp = (Health - dmg);
        hitdirection = dir;
        if (hp <= 0f)
        {
            Health = 0f;//Kill();
            Extended = 0f;
            if (Frozen)
            {
                Frozen = false;
                Shattered = true;
                ShatteredTime = Time.time;
                Am.am.oneshot(Am.am.M.IceShatter);
                
            }
            Kill();
        }
        else
        {
            lastDamageTakenTime = Time.time;
            Health = hp;

        }



    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Astronaut a = collision.gameObject.GetComponent<Astronaut>();
        if ((a != null) && (a.Alive))
        {
            if (!Frozen)
                //if ((Time.time - a.lastDamageTakenTime) >= .5f)
                //{

                a.MyRigidbody.velocity = new Vector2(a.MyRigidbody.velocity.x, a.MyRigidbody.velocity.y*.99f); //(a.MyRigidbody.velocity * .99f);
                    /*
                    Vector3 dif = (a.transform.position - this.transform.position);
                    //HitsDone += 1f;
                    if (a.TakeDamage(20f, dif.normalized * 5f + new Vector3(0f, a.JumpSpeed * .3f, 0f)))
                    {
                        HitsDone += 4f;
                    }
                    */

                //}

        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerStay2D(collision);
    }
    
}

