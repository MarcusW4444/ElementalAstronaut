using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Fire_Hound : GenericEnemy {

    // Use this for initialization
    public enum State {None,Waiting,Approaching,Firing};
    public State MyState = State.Waiting;
	void Start () {
        
        StateTime = Time.time;
        setState(State.Waiting, .5f);
    }
    
    

    // Update is called once per frame
    public FireHoundProjectile FireProjectile;
    public ParticleSystem ShootFlash,BiteFlash;
    public const float MoveSpeed = 5f;
    private float StateTime = -10f;
    private float StateDuration = 1f;
    void Update()
    {

        if (Alive)
        {
            hitdirection = (hitdirection * (1f - Time.deltaTime));
            DamageFlashStep();
        }

        
        freezeStep();
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
                case State.Waiting: {
                        

                        if (stateexpired)
                        {
                            bool ch = false;

                            if ((plr != null) && (plr.Alive))
                            {


                                Vector3 dif = (plr.transform.position - this.transform.position);
                                //Debug.Log("Mag: " + dif.magnitude);
                                if ((dif.magnitude < 10f)&& (!Astronaut.TheAstronaut.Quelling))
                                {
                                    RaycastHit2D rh = Physics2D.Linecast(this.transform.position, plr.transform.position, LayerMask.GetMask(new string[] { "Geometry" }));
                                    
                                    if (rh.distance <= 0f)
                                    {
                                        
                                        if (dif.magnitude > 5f)
                                        {
                                            ch = true;
                                            setState(State.Firing, .35f+(1f*(1f-Astronaut.AggressionLevelF)));

                                        } else
                                        {
                                            ch = true;
                                            
                                            setState(State.Approaching, 1f);
                                        }

                                            
                                        ch = true;
                                        //Debug.Log("Visible");
                                    } else
                                    {
                                        ch = true;
                                        //MoveDirection = ((int)Mathf.Sign(dif.x));
                                        //setState(State.Approaching, 1f);
                                    }
                                }
                            }
                            if (!ch)
                            setState(State.Waiting, .5f);
                        }
                        MyAnimator.SetBool("Running",false);
                        MyRigidbody.velocity = new Vector3(0f, MyRigidbody.velocity.y);
                        break; }
                case State.Approaching:
                    {
                        bool ch = false;

                        //Debug.Log("Approach!");
                        WasRunning = true;
                        if (stateexpired)
                            {

                            if ((plr != null) && (plr.Alive) && (!Astronaut.TheAstronaut.Quelling))
                            {


                                
                                    RaycastHit2D rh = Physics2D.Linecast(this.transform.position, plr.transform.position, LayerMask.GetMask(new string[] { "Geometry" }));

                                if (rh.distance <= 0f)
                                {
                                    Vector3 dif = (plr.transform.position - this.transform.position);
                                    if (dif.magnitude > 5f)
                                    {
                                        //Debug.Log("Take aim");
                                        //ParticleSystem s = ShootWindUpGlow;

                                        setState(State.Firing, .35f+ (1f * (1f - Astronaut.AggressionLevelF)));
                                        ch = true;
                                        //Debug.Log("Visible");
                                    }
                                    else
                                    {
                                        //Debug.Log("hiding...");
                                        ch = true;

                                        setState(State.Approaching, 1f);

                                    }

                                } else
                                {

                                }
                            }
                            if (!ch)
                                setState(State.Waiting, .5f);
                            
                            
                            }

                        
                        if (!ch)
                        if (MoveDirection != 0)
                        {
                            MyAnimator.SetBool("Running", true);
                            MyRigidbody.velocity = new Vector3(MoveSpeed* (1f - FreezeFactor) * MoveDirection *(.75f+(1.75f*Astronaut.AggressionLevelF)),MyRigidbody.velocity.y);
                            this.transform.localScale = new Vector3(Mathf.Abs(this.transform.localScale.x) * Mathf.Sign(-MoveDirection), this.transform.localScale.y, this.transform.localScale.z);//MySpriteRenderer.flipX = (Mathf.Sign(dif.x) < 0f);
                                
                        }

                        break;
                    }
                case State.Firing:
                    {


                        bool ch = false;
                        if (stateexpired)
                        if ((plr != null) && (plr.Alive) && (!Astronaut.TheAstronaut.Quelling))
                        {


                            Vector3 dif = (plr.transform.position - this.transform.position);
                            if (dif.magnitude < 8f)
                            {
                                RaycastHit2D rh = Physics2D.Linecast(this.transform.position, plr.transform.position, LayerMask.GetMask(new string[] { "Geometry" }));
                                    
                                if ((rh.distance <= 0f) && (dif.magnitude < 5f))
                                    {
                                        //Debug.Log("Take aim");
                                        //ParticleSystem s = ShootWindUpGlow;
                                        WasRunning = false;
                                        setState(State.Firing, .35f+ (1f * (1f - Astronaut.AggressionLevelF)));
                                    ch = true;
                                    //Debug.Log("Visible");
                                }
                                else
                                {
                                        ch = true;
                                        setState(State.Approaching, .5f);
                                        
                                    }
                            }

                            if (stateexpired)
                            {
                                setState(State.Waiting, .5f);
                            } else
                                {
                                    MyAnimator.SetBool("Running", false);
                                }
                        } else
                        {
                            
                            
                                setState(State.Waiting,.5f);
                            
                        }
                        
                        break;
                    }
            }


            //MyRigidbody.velocity = new Vector2(movedir * movespeed, MyRigidbody.velocity.y);
        }
        else
        {
            setState(State.Waiting, .5f);
        }
        
    }
    private bool WasRunning = false;
    public Animator MyAnimator;
    private Vector3 AimDirection = Vector3.left;
    public Transform OutputTransform;
    public void setState(State st,float dur)
    {
        
        StateTime = Time.time;
        StateDuration = dur;
        Astronaut plr = Astronaut.TheAstronaut;
        if ((st == State.Firing) && (MyState != State.Firing))
        {
            
            MyRigidbody.velocity = new Vector2(0f, MyRigidbody.velocity.y);
            if ((plr != null) && (plr.Alive))
            {


                Vector3 dif = (plr.transform.position - this.transform.position);
                if (dif.magnitude < 10f)
                {
                    RaycastHit2D rh = Physics2D.Linecast(this.transform.position, plr.transform.position, LayerMask.GetMask(new string[] { "Geometry" }));


                    if (dif.x != 0f)
                    {
                        this.transform.localScale = new Vector3(Mathf.Sign(-dif.x)*Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);//MySpriteRenderer.flipX = (Mathf.Sign(dif.x) < 0f);
                    }

                    if (rh.distance <= 0f)
                    {
                        MyAnimator.SetBool("Running", false);
                        MyAnimator.SetTrigger("Shoot");
                        if (!Biting)
                        {
                            FireHoundProjectile p = GameObject.Instantiate(FireProjectile, OutputTransform.position, FireProjectile.transform.rotation).GetComponent<FireHoundProjectile>();
                            p.MyRigidbody.velocity = (dif.normalized * 5f);
                            Am.am.oneshot(Am.am.M.FireHoundShootFireball);
                            ShootFlash.Emit(1);
                        } else
                        {
                            Am.am.oneshot(Am.am.M.BiteSound);
                            BiteFlash.Play();
                            Biting = false;
                        }
                        
                        
                    }
                }

            }
        }
        else if ((st == State.Approaching) && (MyState != State.Approaching))
        {
            MoveDirection = 0;
            Vector3 dif = (plr.transform.position - this.transform.position); 
            MoveDirection = ((int)Mathf.Sign(dif.x));

            if (MoveDirection == 0)
            {
                st = State.Waiting;
                
            }
        }


            MyState = st;
    }
    
    private int MoveDirection = 0;
    public override void Kill()
    {
        if (!Alive) return;
        Alive = false;
        //base.Kill();
        Astronaut.PlayKillSound(2);
        Astronaut.TheAstronaut.dropResistance(0.4f / (1f + HitsDone), this.transform.position, Astronaut.Element.Fire);
        deathKnockback();
    }

    private bool Biting = false;
    private void OnCollisionStay2D(Collision2D collision)
    {
        Astronaut a = collision.gameObject.GetComponent<Astronaut>();
        if ((a != null) && (a.Alive))
        {
            if (!isStunned())
                if ((Time.time - a.lastDamageTakenTime) >= 2f)
                {
                    Vector3 dif = (a.transform.position - this.transform.position);
                    HitsDone += 1f;
                    Biting = true;
                    setState(State.Firing,.33f);
                    if (a.TakeDamage(20f, dif.normalized * 5f + new Vector3(0f, a.JumpSpeed, 0f)))
                    {
                        HitsDone += 4f;
                    }

                }

        }
    }


    private float LastShotTime = -10f;
    private Vector3 TargetingDirection;

}
