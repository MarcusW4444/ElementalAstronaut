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
    private bool Emerged = true;
    public Transform Turner;
    void FixedUpdate()
    {
        Astronaut plr = Astronaut.TheAstronaut;

        if (Alive && !isStunned())
        {
            bool stateexpired = (Time.time >= (StateTime + StateDuration));

            bool frontcollision = false;
            bool backcollision = false;
            Vector2 mypos = new Vector2(this.transform.position.x, this.transform.position.y);
            Vector2 dir = new Vector3(Turner.forward.x, Turner.forward.y);
            float depth = .4f;
            frontcollision = (Physics2D.OverlapPoint(mypos+(dir*depth),LayerMask.GetMask(new string[] { "Geometry"}))!=null);
            backcollision = (Physics2D.OverlapPoint(mypos - (dir * depth), LayerMask.GetMask(new string[] { "Geometry" })) != null);


            switch (MyState)
            {
                case State.None: { break; }
                case State.Waiting:
                    {
                        //standby until the player shows up

                        if (stateexpired)
                        {
                            
                            if (DiggingParticles.isPlaying) DiggingParticles.Stop();
                            if ((plr != null) && (plr.Alive))
                            {
                                Vector3 dif = ((plr.transform.position) - this.transform.position);
                                if (dif.magnitude < 10f)
                                {
                                    setState(State.LateralRepositioning,0f);
                                    //RaycastHit2D rh = Physics2D.Linecast(this.transform.position, plr.transform.position, LayerMask.GetMask(new string[] { "Geometry" }));

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
                                setState(State.Waiting, .5f);
                                Turner.Rotate(0f,0f,180f,Space.World);
                                if (!DiggingParticles.isPlaying) DiggingParticles.Play();
                            } else
                            {
                                
                                MyRigidbody.MovePosition(new Vector2(this.transform.position.x, this.transform.position.y) + (new Vector2(Turner.forward.x, Turner.forward.y)*Time.fixedDeltaTime*DigSpeed));
                            }
                            DiggingParticles.Emit(5);
                        } else if (frontcollision || backcollision)
                        {       
                            if (!DiggingParticles.isPlaying) DiggingParticles.Play();
                            MyRigidbody.MovePosition(new Vector2(this.transform.position.x, this.transform.position.y) + (new Vector2(Turner.forward.x, Turner.forward.y) * Time.fixedDeltaTime * DigSpeed));
                            DiggingParticles.Emit(10);
                        } else
                        {
                            
                            if (DiggingParticles.isPlaying) DiggingParticles.Stop();
                            Emerged = true;
                            MyRigidbody.MovePosition(new Vector2(this.transform.position.x, this.transform.position.y) + (new Vector2(Turner.forward.x, Turner.forward.y) * Time.fixedDeltaTime*FlySpeed));
                        }

                        if (stateexpired)
                        {

                            //Turner.Rotate()
                        }
                        

                        




                        break;
                    }
                case State.LateralRepositioning:
                    {
                        //Move laterally in order to hit the player or just forget repositioning 
                        Emerged = false;
                        if (!DiggingParticles.isPlaying) DiggingParticles.Play();
                        if ((plr != null) && (plr.Alive))
                        {
                            Vector3 dif = ((plr.transform.position) - this.transform.position);
                            Vector3 targpos = new Vector2(plr.transform.position.x, plr.transform.position.y) + (plr.MyRigidbody.velocity *(dif.magnitude/FlySpeed));
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
                                Vector3 hitpos = Vector3.Project(Turner.forward,dif);
                                float tdif = (hitpos - dif).magnitude;
                                //Reposition
                                float mdelt = RepositionSpeed * Time.fixedDeltaTime;
                                //Debug.Log(""+tdif+" "+mdelt);
                                bool notdeadend = (Physics2D.OverlapPoint(mypos + (new Vector2(latdir.x, latdir.y) * Mathf.Sign(dot) * Time.fixedDeltaTime*2f*RepositionSpeed), LayerMask.GetMask(new string[] { "Geometry" })) != null);
                                
                                    
                                    if (tdif <= mdelt * 1.25f)
                                    {
                                        Emerged = false;
                                        //Debug.Log("Drill");
                                        setState(State.DrillingThrough, 0f);
                                        this.transform.position = this.transform.position + (latdir * Mathf.Sign(dot) * tdif);
                                    DiggingParticles.Emit(5);
                                }
                                    else if (notdeadend)
                                    {
                                    DiggingParticles.Emit(5);
                                    this.transform.position = this.transform.position + (latdir * Mathf.Sign(dot) * RepositionSpeed * Time.fixedDeltaTime);
                                    }
                                

                            } else
                            {
                                setState(State.Waiting,.5f);
                            }

                        }
                        

                        break;
                    }
            }


            //MyRigidbody.velocity = new Vector2(movedir * movespeed, MyRigidbody.velocity.y);
        }
        else
        {
            setState(State.Waiting, .5f + (2f * Random.value));
        }
    }
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
        deathKnockback();
    }


    private float LastShotTime = -10f;
    private Vector3 TargetingDirection;


}
