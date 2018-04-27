using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Jungle_Monkey : GenericEnemy
{

	// Use this for initialization
	void Start () {
        BaseColor = MySpriteRenderer.color;
    }

    // Update is called once per frame
    public ClimbingVine MyVine;
    public float LimitMin = 0f, LimitMax = 0f;
    public Vector3 AimDirection = new Vector3(1f,0f,0f);
    public int ClimbDirection = 0;
    void FixedUpdate()
    {
        
        if (Alive && !isStunned())
        {
            if (MyVine != null)
            {
                Collider2D col = MyVine.GetComponent<Collider2D>();
                if (col != null)
                {
                    LimitMin = col.bounds.min.y;
                    LimitMax = col.bounds.max.y;
                }
            }


            if (ThrowingProjectile)
            {

                if ((Time.time - ThrowTime) >= .25f)
                {
                    ThrowingProjectile = false;
                    ThrowTime = Time.time;


                    //Fire the Projectile
                    JungleMonkeyProjectile p = GameObject.Instantiate(ProjectilePrefab, this.transform.position, new Quaternion());
                    float sp = 5f * (1f + (2f * Astronaut.AggressionLevelF));
                    p.MyRigidbody.velocity = AimDirection * sp;
                    Am.am.oneshot(Am.am.M.MonkeyThrow);
                    if (Astronaut.AggressionLevel > 0)
                    {
                        for (int i = 0; i < Astronaut.AggressionLevel; i++) {
                            JungleMonkeyProjectile c = GameObject.Instantiate(p, p.transform.position, p.transform.rotation);
                            c.MyRigidbody.velocity = (p.MyRigidbody.velocity + (Random.insideUnitCircle * p.MyRigidbody.velocity.magnitude * .5f));
                        }
                    }
                    
                }


            }
            else
            {

                Astronaut plr = Astronaut.TheAstronaut;

                if ((Time.time - ThrowTime) >= .75f)
                if ((plr != null) && (plr.Alive) && ((Time.time - plr.ReviveTime) >= 2f))
                {
                    Vector3 dif = (plr.transform.position - this.transform.position);
                    if (dif.magnitude < 8f)
                    {
                            //if (Mathf.Abs(dif.x) < 2f)
                            //{
                            if ((Time.time - climbtime) >= .5f)
                            {
                                climbtime = Time.time;
                                ClimbDirection = 1 * ((int)Mathf.Sign(dif.y));
                            }
                        //}
                        //else
                        //{
                        //climbdirection = -1*((int)Mathf.Sign(dif.y));
                        //}
                        if ((Time.time - ThrowTime) >= Mathf.Lerp(2.5f,1.75f,Astronaut.AggressionLevelF))
                        {
                            ThrowingProjectile = true;
                            ThrowTime = Time.time;
                            MyAnimator.SetTrigger("Throw");
                        }

                        AimDirection = dif.normalized;
                    }

                    






                }
            }

            if (!ThrowingProjectile)
            {
                if (ClimbDirection != 0)
                {
                    
                    float cl = CLIMBINGSPEED*(Mathf.Lerp(1f,2f,Astronaut.AggressionLevelF))* (1f - FreezeFactor);
                    if (ClimbDirection > 0)
                    {
                        if (this.transform.position.y < LimitMax) {
                            this.transform.position = (this.transform.position + (Vector3.up * (Mathf.Min(Time.fixedDeltaTime * cl, LimitMax - this.transform.position.y))));
                            MyAnimator.SetInteger("Climbing", 1);
                        } else
                        {
                            MyAnimator.SetInteger("Climbing", 0);
                        }
                    } else if (ClimbDirection < 0)
                    {
                        if (this.transform.position.y > LimitMin)
                        {
                            this.transform.position = (this.transform.position + (Vector3.down * (Mathf.Min(Time.fixedDeltaTime * cl,this.transform.position.y - LimitMin))));
                            MyAnimator.SetInteger("Climbing",-1);
                            
                        }
                        else
                        {
                            MyAnimator.SetInteger("Climbing", 0);
                        }
                    }
                    
                } else
                {
                    ClimbDirection = 0;
                    MyAnimator.SetInteger("Climbing", 0);
                }

            } else
            {
                ClimbDirection = 0;
                MyAnimator.SetInteger("Climbing", 0);
                
            }
        } else
        {
            ClimbDirection = 0;
            MyAnimator.SetInteger("Climbing", 0);
        }
    }
    private float climbtime = -10f;
    public const float CLIMBINGSPEED = 2f;
    public Animator MyAnimator;
    private bool ThrowingProjectile = false;
    private float ThrowTime = -10f;
    public JungleMonkeyProjectile ProjectilePrefab;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        ClimbingVine v = collision.gameObject.GetComponent<ClimbingVine>();
        if (v != null)
        {
            MyVine = v;
        }
    }

    public override void Kill()
    {
        if (!Alive) return;
        Alive = false;
        Astronaut.PlayKillSound(1);
        deathKnockback();
        Astronaut.TheAstronaut.dropResistance(.25f / (1f + HitsDone), this.transform.position, Astronaut.Element.Grass);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(.5f, 1f, 0f);
        float bottomy = LimitMin;
        float topy = LimitMax;
        Gizmos.DrawLine(new Vector3(this.transform.position.x+.5f, bottomy, this.transform.position.z), new Vector3(this.transform.position.x-.5f, bottomy, this.transform.position.z));
        Gizmos.DrawLine(new Vector3(this.transform.position.x+.5f,topy, this.transform.position.z), new Vector3(this.transform.position.x-.5f, topy, this.transform.position.z));
        Gizmos.DrawLine(new Vector3(this.transform.position.x, bottomy,this.transform.position.z), new Vector3(this.transform.position.x, topy, this.transform.position.z));
    }

}
