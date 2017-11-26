using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Fire_Fireball : GenericEnemy {

    // Use this for initialization
    public enum State { None, Waiting, Invading, Exploding,Aftermathing};
    public State MyState = State.Waiting;
    


    void Start()
    {
        StartPosition = this.transform.position;
        StateTime = Time.time;
        originalspritesize = MySpriteRenderer.transform.localScale;
        setState(State.Waiting, .5f + (2f * Random.value));
        
    }
    private float LookDirection = 0f;
    private Vector3 StartPosition;
    private bool GoLeftGoRight;
    public bool SurpriseFromLava = false;
    private bool surprising = false;


    // Update is called once per frame


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
    }
    private bool Telegraphed = false;
    private float TelegraphDelay = .25f;
    public Collider2D MyCollider;
    public ParticleSystem SurpriseParticles;
    void FixedUpdate()
    {
        Astronaut plr = Astronaut.TheAstronaut;

        if (Alive && !isStunned())
        {
            bool stateexpired = (Time.time >= (StateTime + StateDuration));

            switch (MyState)
            {
                case State.None: { break; }
                case State.Waiting:
                    {
                        MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
                        /*
                        float sdif = (this.transform.position.x - StartPosition.x);
                        if (Mathf.Abs(sdif) > 2f)
                        {
                            float sig = Mathf.Sign(sdif);
                            GoLeftGoRight = (sig >= 0f);
                        }
                        MyRigidbody.AddForce(new Vector2(GoLeftGoRight ? -1f : 1f, 0f) * 3f);
                        */
                        

                        //if (stateexpired)
                        //{
                        bool ch = false;

                        if ((plr != null) && (plr.Alive))
                        {


                            Vector3 dif = (plr.transform.position - this.transform.position);
                            if (((SurpriseFromLava && (Mathf.Abs(dif.x) <3f))||(!SurpriseFromLava))&&(dif.magnitude < 15f))
                            {
                                RaycastHit2D rh = Physics2D.Linecast(this.transform.position, plr.transform.position, LayerMask.GetMask(new string[] { "Geometry" }));
                                
                                if ((rh.distance <= 0f))
                                {
                                    //The player is present. Invade their space
                                    //Debug.Log("Approach");
                                    //ParticleSystem s = ShootWindUpGlow;
                                    InvadeStartTime = Time.time;
                                    setState(State.Invading, 2f);
                                    ch = true;
                                    //Debug.Log("Visible");
                                }
                                else
                                {
                                    //Debug.Log("hiding...");
                                }
                            }
                        }
                        if (!ch)
                         setState(State.Waiting, .5f);
                        //}


                        //if (MyRigidbody.velocity.x != 0f)
                        //{
                        //MySpriteRenderer.flipX = (Mathf.Sign(MyRigidbody.velocity.x) < 0f);
                        //}

                        break;
                    }
                case State.Invading:
                    {
                        bool ch = false;
                        MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
                        
                        if ((plr != null) && (plr.Alive))
                        {
                            Vector3 dif = (plr.transform.position - this.transform.position);

                            

                            if (dif.magnitude < 10f)
                            {
                                RaycastHit2D rh = Physics2D.Linecast(this.transform.position, plr.transform.position, LayerMask.GetMask(new string[] { "Geometry" }));
                                LookDirection = (int)((dif.x != 0) ? Mathf.Sign(dif.x) : LookDirection);
                                
                                if ((Time.time - LastLoveDuckTime) >= LOVEDUCKINTERVAL)
                                {
                                    LoveDuck = !LoveDuck;
                                    LastLoveDuckTime = Time.time;
                                    if (!SurpriseFromLava)
                                    {
                                        
                                        Vector3 crs = Vector3.Cross(dif, Vector3.forward);
                                        MoveDirection = (((dif) + (crs * LoveDuckOffset * (LoveDuck ? 1f : -1f))).normalized * MoveSpeed);
                                        MySpriteRenderer.sortingOrder = 0;
                                        MyCollider.enabled = true;
                                        
                                    } else
                                    {

                                        if (surprising)
                                        {
                                            ch = true;
                                            setState(State.Waiting, .5f);
                                            SurpriseFromLava = false;
                                            MyCollider.enabled = true;
                                        }
                                        else
                                        {
                                            MoveDirection = new Vector3(0f, 1f * MoveSpeed * 2f, 0f);
                                            surprising = true;
                                            MyCollider.enabled = false;
                                        }
                                        
                                        
                                    }
                                } else
                                {
                                    if (SurpriseFromLava)
                                    if (SurpriseParticles) SurpriseParticles.Emit(5);

                                }

                                if (rh.distance <= 0f)
                                {
                                    //
                                    //ParticleSystem s = ShootWindUpGlow;
                                    if (dif.magnitude < 5f)
                                    {
                                        NearTime = Mathf.Clamp01(NearTime+(Time.deltaTime/1f));
                                    } else
                                    {
                                        NearTime = Mathf.Clamp01(NearTime - (Time.deltaTime / 1f));
                                    }
                                        if ((dif.magnitude < 2f) && ((NearTime >= 1f)))
                                    {

                                        ch = true;
                                        setState(State.Exploding, 2f-((1f * Astronaut.AggressionLevelF)));
                                    }
                                    else
                                    {

                                        //MyRigidbody.AddForce(new Vector2(dif.normalized.x, dif.normalized.y) * 5f);
                                        //setState();
                                    }


                                    //Debug.Log("Visible");
                                }
                                else
                                {

                                    setState(State.Invading, .5f);
                                }
                            }
                            else
                            {

                                setState(State.Waiting, .5f);
                            }
                        }


                        
                            


                        

                        if (LookDirection != 0f)
                            MySpriteRenderer.flipX = (LookDirection > 0f);

                    
                    if (!ch)
                        {
                            
                            MyRigidbody.velocity = new Vector3(MoveDirection.x,MoveDirection.y,0f);
                            


                        }

                        if (stateexpired)
                        {
                            SurpriseFromLava = false;
                            StateTime = Time.time;
                        }


                        break;
                    }
                case State.Exploding:
                    {
                        //MyRigidbody.bodyType = RigidbodyType2D.Static;
                        //MyRigidbody.velocity = new Vector2();
                        if (!PepperField.isPlaying) PepperField.Play();
                        if (!CoalescingEffect.isPlaying) CoalescingEffect.Play(true);
                        bool ch = false;
                        float f = ((Time.time - StateTime) / StateDuration);

                        MySpriteRenderer.transform.localScale = originalspritesize*(1f+((1f+(1f*Astronaut.AggressionLevelF))*f));
                        ExplosionEffect.transform.localScale = (Vector3.one*(.25f+(.75f*f)));
                        if ((!Telegraphed) && (((StateTime + StateDuration) - Time.time) < TelegraphDelay))
                        {
                            Telegraphed = true;
                            TelegraphEffect.Play(true);
                        }
                        if (stateexpired)
                        {
                            explode();
                            setState(State.Aftermathing, 1f);
                        } 




                        break;
                    }
                case State.Aftermathing:
                    {

                        MySpriteRenderer.transform.localScale = originalspritesize;
                        if (stateexpired)
                        {
                            setState(State.Waiting, 1f);
                        }
                        break;
                    }
            }


            
            


        }
        else
        {
            if (Alive)
            {
                if (PepperField.isPlaying) PepperField.Stop();
                if (CoalescingEffect.isPlaying) CoalescingEffect.Stop(true);
                setState(State.Waiting, .5f);
                if (isStunned())
                {
                    MySpriteRenderer.transform.localScale = originalspritesize;
                }
            }
        }

        freezeStep();
    }
    private Vector3 originalspritesize;
    private float NearTime = 0f;
    private float InvadeStartTime = -10f;
    public ParticleSystem CoalescingEffect,PepperField,ExplosionEffect, TelegraphEffect, NegativeParticles;

    private Vector3 MoveDirection = new Vector3();
    private Vector3 AimDirection = Vector3.left;
    private bool exploded = false;
    private float LastLoveDuckTime = -10f;
    private float LoveDuckOffset = .1f;
    private float LOVEDUCKINTERVAL = .3f;


    public void explode()
    {
        if (exploded) return;
        CoalescingEffect.Stop(true);
        PepperField.Stop();
        PepperField.Clear();
        
        ExplosionEffect.Play(true);
        exploded = true;
        
        Astronaut plr = Astronaut.TheAstronaut;
        

        //if (Alive && !isStunned())
        //{
            Vector3 dif = (this.transform.position - plr.transform.position);
        float sc = 1f;
        float pre = 1f;
        /*
         * if (premature)
        {
            float f = ((Time.time - StateTime) / StateDuration);
            pre = (.25f + (.75f * f));
            //ExplosionEffect.transform.localScale = (Vector3.one * pre);
        } else
        {
            
            ExplosionEffect.transform.localScale = (Vector3.one );
        }
        */
            float radius = (4f*sc);//*pre
            float dist = (dif.magnitude/radius);
            if (dist < 1f)
            {


                //if ((Time.time - plr.lastDamageTakenTime) >= 1.5f)
                //{
                    Vector3 diff = (plr.transform.position - this.transform.position);
                    HitsDone += 1f;
                    float df = (1f - dist);
                    //df = Mathf.Pow(df, 1f/(1f + (1f * Astronaut.AggressionLevelF)));
                    if (plr.TakeDamage(80f*df, (diff.normalized * 30f) + new Vector3(0f, plr.JumpSpeed*2f, 0f)))
                    {
                        HitsDone += 4f;
                    }

                //}
            }

        //}


        
    }
    public CircleCollider2D ConvectionCollider;
    public ParticleSystem ConvectionGlow;
    private void OnTriggerStay2D(Collider2D col)
    {
        //return;
        if (!Alive) return;
        //Deal Damage over time
        if (SurpriseFromLava && !surprising) return;
        Astronaut plr = col.gameObject.GetComponent<Astronaut>();
        if ((plr != null) && (plr.Alive))
        {

            Vector3 diff = (plr.transform.position - this.transform.position);
            
            float dist = (diff.magnitude / ConvectionCollider.radius);
            if (dist >= 1f) return;
            float df = (1f - dist);
            //df = Mathf.Pow(df, 1f / (1f + (1f * Astronaut.AggressionLevelF)));
            if (plr.TakeDamage(20f*df*Time.deltaTime, Vector3.zero))
            {
                HitsDone += 4f;
            } else
            {
                HitsDone += Time.deltaTime * df;
            }
        }
        if (plr != null)
        {

            ConvectionGlow.transform.position = plr.transform.position;
            ConvectionGlow.Emit(1);
            ConvectionGlow.transform.position = this.transform.position;
        }
    }
    private bool LoveDuck = false;
    public void setState(State st, float dur)
    {

        StateTime = Time.time;
        StateDuration = dur;
        if ((st == State.Exploding) && (MyState != State.Exploding))
        {
            //MyRigidbody.velocity = new Vector2();
            Telegraphed = false;
            MyRigidbody.bodyType = RigidbodyType2D.Static;
            exploded = false;
            //ShootWindUpGlow.Play();
        }

        if (st == State.Invading)
        {
            Vector2 c = Random.insideUnitCircle;
            invadeoffset = (new Vector3(c.x, c.y, 0f) * 2f);
            if (MyState != State.Invading)
            {
                InvadeStartTime = Time.time;
            }
        }

        MyState = st;
    }
    private Vector3 invadeoffset = new Vector3();
    public override void Kill()
    {
        if (!Alive) return;
        Alive = false;
        //base.Kill();
        CoalescingEffect.Stop(true);
        PepperField.Stop();
        NegativeParticles.Stop();
        MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
        Astronaut.TheAstronaut.dropResistance(.4f / (1f + HitsDone), this.transform.position, Astronaut.Element.Fire);
        exploded = false;
        //if (this.MyState == State.Exploding)
        //{
            premature = true;
            //explode();
        //}
        dontstopparticles = true;
        deathKnockback();
    }
    private bool premature = false;

    private float LastShotTime = -10f;
    private Vector3 TargetingDirection;

    


}
