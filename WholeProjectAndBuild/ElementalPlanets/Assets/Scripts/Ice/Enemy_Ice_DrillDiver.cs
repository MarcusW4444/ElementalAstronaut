using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Ice_DrillDiver : GenericEnemy {
    // Use this for initialization
    public enum State { None, Waiting, DrillingThrough,LateralRepositioning};
    public State MyState = State.Waiting;
    public Collider2D Chains;


    void Start()
    {
        StartPosition = this.transform.position;
        StateTime = Time.time;
        setState(State.DrillingThrough, .5f + (2f * Random.value));
        //Emerged = false;
    }
    private Vector3 StartPosition;
    private bool GoLeftGoRight;

    // Update is called once per frame
    
    public ParticleSystem ShootWindUpGlow, ShootFlash;
    public const float MoveSpeed = 5f;
    private float StateTime = -10f;
    private float StateDuration = 1f;
    void Update()
    {

        if (Alive)
        {
            hitdirection = (hitdirection * (1f - Time.deltaTime));

        }
    }
    public bool Emerged = true;
    public Transform Turner;
    public Ethereal_Ice_HazardPit MyEtherealPit;
    void FixedUpdate()
    {
        Astronaut plr = Astronaut.TheAstronaut;
        if (EtherealBehavior)
        {
            Frozen = false;
            if (EtherealWillingToTeach)
            {
                

                if (MyEtherealPit.Permafrozen)
                {
                    FreezeTime = Time.time + 1f;
                } else
                {
                    this.transform.position = (StartPosition + (new Vector3(0f, -1f, 0f) * Mathf.Abs((Time.time % .5f) - .25f) / .25f));
                    this.transform.rotation = Quaternion.Euler(0f,0f,180f);
                }
                //Freezability = .1f;
            } else
            {
                Vector3 dif = ((plr.transform.position) - this.transform.position);
                if (dif.magnitude < 1f)
                {
                    //Just Dig away
                    this.transform.position = (this.transform.position + (new Vector3(0f, -1f, 0f) * (Time.fixedDeltaTime * DigSpeed)));
                }
           }
        } else 
        if (Alive && !isStunned())
        {
            bool stateexpired = (Time.time >= (StateTime + StateDuration));

            bool frontcollision = false;
            bool backcollision = false;
            bool leftcollision = false;
            bool rightcollision = false;
            Vector2 mypos = new Vector2(this.transform.position.x, this.transform.position.y);
            Vector2 dir = new Vector2(Turner.forward.x, Turner.forward.y);
            float depth = .8f;
            frontcollision = (Physics2D.OverlapPoint(mypos+(dir*depth),LayerMask.GetMask(new string[] { "Geometry"}))!=null);
            backcollision = (Physics2D.OverlapPoint(mypos - (dir * depth), LayerMask.GetMask(new string[] { "Geometry" })) != null);
            Vector3 crs3 = Vector3.Cross(new Vector3(dir.x,dir.y,0f).normalized,Vector3.forward);
            Vector2 crs = new Vector2(crs3.x,crs3.y);
            leftcollision = (Physics2D.OverlapPoint(mypos - (crs * depth), LayerMask.GetMask(new string[] { "Geometry" })) != null);
            rightcollision = (Physics2D.OverlapPoint(mypos + (crs * depth), LayerMask.GetMask(new string[] { "Geometry" })) != null);

            switch (MyState)
            {
                case State.None: { break; }
                case State.Waiting:
                    {
                        //standby until the player shows up

                        if (stateexpired)
                        {
                            
                            
                            if (DiggingParticles.isPlaying) DiggingParticles.Stop();
                            if ((plr != null) && (plr.Alive)&& (!Astronaut.TheAstronaut.Quelling))
                            {
                                Vector3 dif = ((plr.transform.position) - this.transform.position);
                                if (dif.magnitude < 15f)
                                {
                                    targpos = new Vector2(plr.transform.position.x, plr.transform.position.y) + (plr.MyRigidbody.velocity * (dif.magnitude / RepositionSpeed));
                                    setState(State.LateralRepositioning,.5f);
                                    //RaycastHit2D rh = Physics2D.Linecast(this.transform.position, plr.transform.position, LayerMask.GetMask(new string[] { "Geometry" }));
                                    //Debug.Log("Get moving");
                                    /*
                                    if (rh.distance <= 0f)
                                    {

                                    }
                                    */

                                }


                            }
                        }
                        break;
                    }
                case State.DrillingThrough:
                    {
                        
                        if (frontcollision && backcollision)
                        {
                            if (Emerged)
                            {
                                //Debug.Log("Wait");
                                setState(State.Waiting, .5f);
                                Turner.Rotate(0f,0f,180f,Space.World);
                                if (!DiggingParticles.isPlaying) DiggingParticles.Play();
                            } else
                            {
                                
                                MyRigidbody.MovePosition(new Vector2(this.transform.position.x, this.transform.position.y) + (new Vector2(Turner.forward.x, Turner.forward.y)*Time.fixedDeltaTime*DigSpeed*(1f+(Astronaut.AggressionLevelF*1f))));
                            }
                            DiggingParticles.Emit(5);

                            if (rightcollision && (!leftcollision))
                            {
                                MyRigidbody.MovePosition(new Vector2(this.transform.position.x, this.transform.position.y) - (new Vector2(Turner.right.x, Turner.right.y) * Time.fixedDeltaTime * DigSpeed * (1f + (Astronaut.AggressionLevelF * 1f))));
                            } else if ((!rightcollision) && leftcollision)
                            {
                                MyRigidbody.MovePosition(new Vector2(this.transform.position.x, this.transform.position.y) + (new Vector2(Turner.right.x, Turner.right.y) * Time.fixedDeltaTime * DigSpeed * (1f + (Astronaut.AggressionLevelF * 1f))));
                            }

                        } else if (frontcollision || backcollision)
                        {       
                            if (!DiggingParticles.isPlaying) DiggingParticles.Play();
                            MyRigidbody.MovePosition(new Vector2(this.transform.position.x, this.transform.position.y) + (new Vector2(Turner.forward.x, Turner.forward.y) * Time.fixedDeltaTime * DigSpeed * (1f + (Astronaut.AggressionLevelF * 1f))));
                            DiggingParticles.Emit(5);

                            if (rightcollision && (!leftcollision))
                            {
                                MyRigidbody.MovePosition(new Vector2(this.transform.position.x, this.transform.position.y) - (new Vector2(Turner.right.x, Turner.right.y) * Time.fixedDeltaTime * DigSpeed * (1f + (Astronaut.AggressionLevelF * 1f))));
                            }
                            else if ((!rightcollision) && leftcollision)
                            {
                                MyRigidbody.MovePosition(new Vector2(this.transform.position.x, this.transform.position.y) + (new Vector2(Turner.right.x, Turner.right.y) * Time.fixedDeltaTime * DigSpeed * (1f + (Astronaut.AggressionLevelF * 1f))));
                            }

                        } else
                        {
                            
                            if (DiggingParticles.isPlaying) DiggingParticles.Stop();
                            Emerged = true;
                            MyRigidbody.MovePosition(new Vector2(this.transform.position.x, this.transform.position.y) + (new Vector2(Turner.forward.x, Turner.forward.y) * Time.fixedDeltaTime*FlySpeed * (1f + (Astronaut.AggressionLevelF * 2f))));
                        }




                        if (stateexpired)
                        {
                            setState(State.Waiting, .5f);
                            Turner.Rotate(0f, 0f, 180f, Space.World);
                            //Turner.Rotate()
                        }




                        break;
                    }
                case State.LateralRepositioning:
                    {
                        //Move laterally in order to hit the player or just forget repositioning 
                        Emerged = false;
                        if (!DiggingParticles.isPlaying) DiggingParticles.Play();
                        if ((plr != null) && (plr.Alive) && (!Astronaut.TheAstronaut.Quelling))
                        {
                            Vector3 dif = ((plr.transform.position) - this.transform.position);
                            
                            dif = ((targpos) - this.transform.position);
                            float frontdot = Vector3.Dot(dif.normalized, Turner.forward.normalized);
                            
                            if (frontdot < 0f)
                            {
                                Turner.Rotate(0f, 0f, 180f, Space.World);
                            }
                            if (dif.magnitude < 10f)
                            {
                                Vector3 latdir = Turner.right;
                                float dot = Vector3.Dot(dif.normalized,latdir.normalized);
                                //float dot = Vector3.Dot(dif.normalized, latdir.normalized);
                                Vector3 hitpos = Vector3.Project(dif, Turner.forward);

                                Vector3 plrdif = ((plr.transform.position) - this.transform.position);
                                Vector3 plrhitpos = Vector3.Project(plrdif, Turner.forward);

                                float tdif = (hitpos - dif).magnitude;
                                float udif = (plrhitpos - dif).magnitude;
                                //Reposition
                                //Debug.Log();
                                float mdelt = (RepositionSpeed * Time.fixedDeltaTime * (1f + (Astronaut.AggressionLevelF * 1f)));
                                //Debug.Log(""+tdif+" "+mdelt);
                                bool notdeadend = (Physics2D.OverlapPoint(mypos + (new Vector2(latdir.x, latdir.y) * Mathf.Sign(dot) * Time.fixedDeltaTime*5f*RepositionSpeed * (1f + (Astronaut.AggressionLevelF * 1f))), LayerMask.GetMask(new string[] { "Geometry" })) != null);
                                
                                    
                                    if ((tdif <= (mdelt * 2f))|| (udif <= (mdelt * 2f)))
                                    {
                                        Emerged = false;
                                        //Debug.Log("Drill");
                                        setState(State.DrillingThrough, 5f);
                                        this.transform.position = this.transform.position + (latdir * Mathf.Sign(dot) * tdif);
                                    //DiggingParticles.Emit(3);
                                }
                                    else if (notdeadend)
                                    {
                                    //DiggingParticles.Emit(1);
                                    this.transform.position = this.transform.position + (latdir * Mathf.Sign(dot) * RepositionSpeed * Time.fixedDeltaTime);
                                    }


                                if (rightcollision && (!leftcollision))
                                {
                                    MyRigidbody.MovePosition(new Vector2(this.transform.position.x, this.transform.position.y) - (new Vector2(Turner.right.x, Turner.right.y) * Time.fixedDeltaTime * DigSpeed * (1f + (Astronaut.AggressionLevelF * 1f))));
                                }
                                else if ((!rightcollision) && leftcollision)
                                {
                                    MyRigidbody.MovePosition(new Vector2(this.transform.position.x, this.transform.position.y) + (new Vector2(Turner.right.x, Turner.right.y) * Time.fixedDeltaTime * DigSpeed * (1f + (Astronaut.AggressionLevelF * 1f))));
                                }


                            } else
                            {

                                if (frontcollision && backcollision)
                                {
                                    if (stateexpired)
                                        setState(State.Waiting, .5f);
                                }
                                else
                                {
                                    
                                    setState(State.DrillingThrough,3f);
                                    Emerged = true;

                                }
                            }

                        }
                        else
                        {

                            
                        }
                        if (frontcollision && backcollision)
                        {
                            if (stateexpired)
                            setState(State.Waiting, .5f);
                        }
                        else
                        {

                            setState(State.DrillingThrough, 3f);
                            Emerged = true;
 
                        }   


                        break;
                    }
            }


            //MyRigidbody.velocity = new Vector2(movedir * movespeed, MyRigidbody.velocity.y);
            DamageFlashStep();
        }
        else
        {
            //setState(State.Waiting, .5f + (2f * Random.value));
        }
    }
    private Vector3 targpos=new Vector3();
    public ParticleSystem DiggingParticles;
    private Vector3 AimDirection = Vector3.left;
    private const float FlySpeed=10f, DigSpeed=2f, RepositionSpeed = 5f;
    public void setState(State st, float dur)
    {
         
        StateTime = Time.time;
        StateDuration = dur;
        

        MyState = st;
    }

    public override void Kill()
    {
        if (!Alive) return;
        Alive = false;
        //base.Kill();
        Astronaut.PlayKillSound(1);
        Astronaut.TheAstronaut.dropResistance(.3f / (1f + HitsDone), this.transform.position, Astronaut.Element.Ice);
        deathKnockback();
    }


    private float LastShotTime = -10f;
    private Vector3 TargetingDirection;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Astronaut a = collision.gameObject.GetComponent<Astronaut>();
        if ((a != null) && (a.Alive))
        {

            if (EtherealBehavior)
            {
                if (EtherealWillingToTeach)
                {
                    if (!isStunned())
                    {
                        a.SendBackToEtherealCheckpoint();
                    }
                }
            }
            else
            {


                if (!isStunned())
                    if ((Time.time - a.lastDamageTakenTime) >= 2f)
                    {
                        Vector3 dif = (a.transform.position - this.transform.position);
                        HitsDone += 1f;
                        if (a.TakeDamage(20f, dif.normalized * 4f + new Vector3(0f, 0f, 0f)))
                        {
                            HitsDone += 4f;
                        }

                    }
            }

        }
    }


}
