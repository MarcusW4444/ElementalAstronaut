using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Ice_Heart : GenericEnemy {

    // Use this for initialization
    public enum State { None, FloatingBackAndForth, Invading, Chaining};
    public State MyState = State.FloatingBackAndForth;
    public Collider2D[] Chains;
	

    void Start()
    {
        StartPosition = this.transform.position;
        StateTime = Time.time;
        setState(State.FloatingBackAndForth, .5f + (2f * Random.value));
    }
    private Vector3 StartPosition;
    private bool GoLeftGoRight;

    // Update is called once per frame

    public ParticleSystem WindUpGlow;
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
    void FixedUpdate()
    {
        Astronaut plr = Astronaut.TheAstronaut;

        if (Alive && !isStunned())
        {
            bool stateexpired = (Time.time >= (StateTime + StateDuration));

            switch (MyState)
            {
                case State.None: { break; }
                case State.FloatingBackAndForth:
                    {
                        MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
                        float sdif = (this.transform.position.x - StartPosition.x);
                        if (Mathf.Abs(sdif) > 2f)
                        {
                            float sig = Mathf.Sign(sdif);
                            GoLeftGoRight = (sig >= 0f);
                        }
                        MyRigidbody.AddForce(new Vector2(GoLeftGoRight ? -1f : 1f, 0f) * 3f);
                        ChainsRetracting = true;

                        //if (stateexpired)
                        //{
                        bool ch = false;

                            if ((plr != null) && (plr.Alive))
                            {
                                

                                Vector3 dif = (plr.transform.position - this.transform.position);
                                if (dif.magnitude < 15f)
                                {
                                    RaycastHit2D rh = Physics2D.Linecast(this.transform.position, plr.transform.position, LayerMask.GetMask(new string[] { "Geometry" }));
                                    
                                    if (rh.distance <= 0f)
                                    {
                                        //The player is present. Invade their space
                                        Debug.Log("Approach");
                                        //ParticleSystem s = ShootWindUpGlow;
                                        setState(State.Invading, 2f);
                                        ch = true;
                                        //Debug.Log("Visible");
                                    }
                                    else
                                    {
                                        Debug.Log("hiding...");
                                    }
                                }
                            }
                            //if (!ch)
                               // setState(State.FloatingBackAndForth, .5f + (2f * Random.value));
                        //}


                        //if (MyRigidbody.velocity.x != 0f)
                        //{
                            //MySpriteRenderer.flipX = (Mathf.Sign(MyRigidbody.velocity.x) < 0f);
                        //}

                        break;
                    }
                case State.Invading:
                    {
                        MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
                        ChainsRetracting = true;
                        if ((plr != null) && (plr.Alive))
                        {


                            Vector3 dif = ((plr.transform.position + invadeoffset) - this.transform.position);
                            if (dif.magnitude < 10f)
                            {
                                RaycastHit2D rh = Physics2D.Linecast(this.transform.position, plr.transform.position, LayerMask.GetMask(new string[] { "Geometry" }));
                                    
                                if (rh.distance <= 0f)
                                {
                                    //
                                    //ParticleSystem s = ShootWindUpGlow;
                                    if (dif.magnitude < 2f) {
                                        setState(State.Chaining, 2f);
                                    } else
                                    {
                                        
                                        MyRigidbody.AddForce(new Vector2(dif.normalized.x, dif.normalized.y) * 5f);
                                        //setState();
                                    }
                                    
                                    
                                    //Debug.Log("Visible");
                                }
                                else
                                {
                                    
                                     setState(State.FloatingBackAndForth, .5f + (2f * Random.value));
                                }
                            } else
                            {

                                setState(State.FloatingBackAndForth, .5f + (2f * Random.value));
                            }
                        }

                        if (stateexpired)
                        {
                            Vector2 c = Random.insideUnitCircle;
                            invadeoffset = (new Vector3(c.x, c.y, 0f) * 2f);
                            StateTime = Time.time;
                        }


                        break;
                    }
                case State.Chaining:
                    {
                        MyRigidbody.bodyType = RigidbodyType2D.Static;
                        //MyRigidbody.velocity = new Vector2();
                        if ((plr != null) && (plr.Alive) && ((plr.transform.position - this.transform.position).magnitude < 4f)) 
                        {
                            ChainsRetracting = false;


                            
                        } else
                        {
                            ChainsRetracting = true;
                            if (isfullyretracted)
                            {
                                setState(State.Invading,2f);
                            }
                        }

                        //if (stateexpired)
                        //{
                          //  ChainsRetracting = true;
                        //} 
                        
                            
                        
                        
                        break;
                    }
            }


            //MyRigidbody.velocity = new Vector2(movedir * movespeed, MyRigidbody.velocity.y);

            
                Collider2D cha;
                float maxdist = 12f;
                float expansionrate = 10f*Time.fixedDeltaTime;
            bool fr = true;
                for (int i = 0; i < Chains.Length; i++)
                {
                    cha = Chains[i];
                    RaycastHit2D hit = Physics2D.Raycast(cha.transform.position,cha.transform.right,maxdist,LayerMask.GetMask(new string[]{"Geometry"}));
                    float mx = maxdist;
                    if (hit.distance <= 0)
                    {
                        //visible
                    } else
                    {
                        //limited
                        mx = hit.distance;
                    }

                
                
                    float curr = cha.transform.localScale.x;
                    bool hitmax = false;
                    float c = Mathf.Clamp(curr + (ChainsRetracting ? -expansionrate : expansionrate), 0f, maxdist);
                if ((curr < mx) && (c >= mx) && (mx < maxdist))
                {
                    hitmax = true;
                    curr = mx;
                } else
                {
                    curr = c;
                }
                
                       if (hitmax)
                        {
                            if (impactSparks)
                            {
                                impactSparks.transform.position = hit.point;
                                impactSparks.Emit(50);
                            }
                        }
                    cha.enabled = (curr > 0f);
                
                cha.transform.localScale = new Vector3(Mathf.Min(curr,mx), cha.transform.localScale.y, cha.transform.localScale.z);
                if (curr > 0f) fr = false;
                    
                    

                }
            isfullyretracted = fr;


        }
        else
        {
            setState(State.FloatingBackAndForth, .5f + (2f * Random.value));
        }
    }
    public ParticleSystem impactSparks;
    private bool isfullyretracted = true;
    private Vector3 AimDirection = Vector3.left;
    private bool ChainsRetracting = false;
    public void setState(State st, float dur)
    {

        StateTime = Time.time;
        StateDuration = dur;
        if ((st == State.Chaining) && (MyState != State.Chaining))
        {
            //MyRigidbody.velocity = new Vector2();
            MyRigidbody.bodyType = RigidbodyType2D.Static;
            ChainsRetracting = false;
            //ShootWindUpGlow.Play();
        }

        if (st == State.Invading)
        {
            Vector2 c = Random.insideUnitCircle;
            invadeoffset = (new Vector3(c.x, c.y, 0f)*2f);
        }

        MyState = st;
    }
    private Vector3 invadeoffset=new Vector3();
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
