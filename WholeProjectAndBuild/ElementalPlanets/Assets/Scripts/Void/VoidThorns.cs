using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidThorns : GenericEnemy {

    // Use this for initialization
    public Transform StandByTransform;
    private Vector3 StandByLocation, DestinationLocation;

    void Start()
    {
        StandByLocation = StandByTransform.position;
        DestinationLocation = this.transform.position;
        IncineratesHealth = true;
    }

    public Collider2D MyCollider;
    private float Extended = 0f;
    public bool Extending = false;
    void FixedUpdate()
    {

        if (Alive)
        {
            //Regenerate Health
            if (!Shattered) {
                if ((Time.time - LastBurnTime) >= (2f - (1f * Astronaut.AggressionLevelF)))
                {
                    if (!Frozen)
                    {
                        Health = Mathf.Min(Health + ((MaxHealth * .25f) * Time.fixedDeltaTime * (1f + (1.5f * Astronaut.AggressionLevelF))), MaxHealth);
                    }
                }
            } else if ((Time.time - ShatteredTime) >= (8f*(1f-.75f*Astronaut.AggressionLevelF)))
            {
                Shattered = false;
            }
            
            
            if ((!Frozen) && (Extending))
            Extended = Mathf.Lerp(Extended,Health/MaxHealth,Mathf.Clamp01(Time.fixedDeltaTime*4f));
            this.transform.position = Vector3.Lerp(StandByLocation, DestinationLocation, Extended);
            //ExtendedCollider.transform.localScale = new Vector3((this.transform.position - StandByLocation).magnitude,1f,1f);
            if (Lifetime < 0f)
            if (!Extending)
            {
                Vector3 dif = (Astronaut.TheAstronaut.transform.position - DestinationLocation);
                if (dif.magnitude < 6f)
                {
                    popOut();
                }
            }
            
        }

        if (Lifetime > 0f)
        {
            if((Time.time - StartTime) >= Lifetime)
            {
                Extending = false;
            }
        }

        
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
    public float StartTime = -10f;
    public float Lifetime = -1f;

    public void popOut()
    {
        if (Extending) return;
        Extending = true;
        StartTime = Time.time;
    }
    public Transform ExtendingSprite;
    public bool Enchanted = false;
    public bool BossThorns = false;
    public override void Kill()
    {
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
        }
        else
        {
            lastDamageTakenTime = Time.time;
            Health = hp;

        }


    
}

    private void OnCollisionStay2D(Collision2D collision)
    {
        Astronaut a = collision.gameObject.GetComponent<Astronaut>();
        if ((a != null) && (a.Alive))
        {
            if (!Frozen)
                if ((Time.time - a.lastDamageTakenTime) >= .5f)
                {

                    Vector3 dif = (a.transform.position - this.transform.position);
                    HitsDone += 1f;
                    if (a.TakeDamage(20f, dif.normalized * 5f + new Vector3(0f, a.JumpSpeed * .3f, 0f)))
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
    private void OnDrawGizmos()
    {
        

        if (MyCollider != null)
        {
            Vector3 bo = new Vector3(MyCollider.bounds.size.x, MyCollider.bounds.size.y, 1f);
            if (StandByTransform != null)
            {

                Gizmos.color = new Color(1f, 0f, .8f);
                Gizmos.DrawWireCube(this.transform.position, bo);
                Gizmos.DrawLine(this.transform.position, StandByTransform.position);
                Gizmos.color = new Color(1f, 1f, 1f);
                Gizmos.DrawWireCube(StandByTransform.position, bo);
                
                
                
            }
        }
        

    }
}
