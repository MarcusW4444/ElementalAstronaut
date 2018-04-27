using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Ice_FlamingSkull : GenericEnemy {

    // Use this for initialization
    public enum State {None,FloatingBackAndForth,SwoopCharging,Firing};
    public State MyState = State.FloatingBackAndForth;
	void Start () {
        StartPosition = this.transform.position;
        StateTime = Time.time;
        setState(State.FloatingBackAndForth, .5f + (2f * Random.value));
    }
    private Vector3 StartPosition;
    private bool GoLeftGoRight;

    // Update is called once per frame
    public IceSkullProjectile SkullProjectile;
    public ParticleSystem ShootWindUpGlow,ShootFlash;
    public const float MoveSpeed = 5f;
    private float StateTime = -10f;
    private float StateDuration = 1f;
    public Collider2D MyCollider;
    void Update()
    {
        
        if (Alive)
        {
            hitdirection = (hitdirection * (1f - Time.deltaTime));
            DamageFlashStep();
        }
    }
    public EnemyProjectile EtherealIceProjectile;
    private float lastetherealshoottime = -10f;
    public const float ETHEREALSHOOTINTERVAL = .4f;
    public const float ETHEREALPROJECTILESPEED = 5f;
    public Vector3 EtherealMoveTo;
    void FixedUpdate()
    {
        Astronaut plr = Astronaut.TheAstronaut;
        if (EtherealBehavior)
        {

            if (EtherealWillingToTeach)
            {
                MyCollider.enabled = true;
                if (LessonIndex == 0)
                {

                } else if (LessonIndex == 1)
                {
                    Vector3 dif = (plr.transform.position - this.transform.position);
                    if (Alive && !isStunned())
                    {
                        if (dif.x > 0f)
                        {
                            if ((Time.time - lastetherealshoottime) >= ETHEREALSHOOTINTERVAL)
                            {

                                IceSkullProjectile p = GameObject.Instantiate(SkullProjectile, this.transform.position, new Quaternion());
                                p.MyRigidbody.velocity = new Vector2(ETHEREALPROJECTILESPEED, 0f);
                                p.EtherealBehavior = true;
                                lastetherealshoottime = Time.time;

                            }
                        }
                    }

                        
                    
                }
            } else
            {
                MyCollider.enabled = false;
                if (!Frozen)
                MySpriteRenderer.flipX = (Mathf.Sign((plr.transform.position - this.transform.position).x) < 0f);
            }
            
        } else 
        if (Alive && !isStunned())
        {


            bool stateexpired = (Time.time >= (StateTime + StateDuration));

            switch (MyState)
            {
                case State.None: { break; }
                case State.FloatingBackAndForth: {

                        float sdif = (this.transform.position.x - StartPosition.x);
                        if (Mathf.Abs(sdif) > 2f)
                        {
                            float sig = Mathf.Sign(sdif);
                            GoLeftGoRight = (sig >= 0f);
                        }
                        MyRigidbody.AddForce(new Vector2(GoLeftGoRight ? -1f : 1f, 0f) * 5f*(1f + (2f * Astronaut.AggressionLevelF)));


                        if (stateexpired)
                        {
                            bool ch = false;

                            if ((plr != null) && (plr.Alive) && (!Astronaut.TheAstronaut.Quelling))
                            {


                                Vector3 dif = (plr.transform.position - this.transform.position);
                                if (dif.magnitude < 10f)
                                {
                                    RaycastHit2D rh = Physics2D.Linecast(this.transform.position, plr.transform.position, LayerMask.GetMask(new string[] { "Geometry" }));
                                    Debug.Log(rh.distance);
                                    if (rh.distance <= 0f)
                                    {
                                        Debug.Log("Take aim");
                                        ParticleSystem s = ShootWindUpGlow;
                                        setState(State.Firing, s.main.duration);
                                        ch = true;
                                        //Debug.Log("Visible");
                                    } else
                                    {
                                        Debug.Log("hiding...");
                                    }
                                }
                            }
                            if (!ch)
                            setState(State.FloatingBackAndForth, .5f + (2f * Random.value));
                        }


                        if (MyRigidbody.velocity.x != 0f)
                        {
                            MySpriteRenderer.flipX = (Mathf.Sign(MyRigidbody.velocity.x) < 0f);
                        }

                        break; }
                case State.SwoopCharging:
                    {
                        break;
                    }
                case State.Firing:
                    {

                        if ((plr != null) && (plr.Alive))
                        {
                            Vector3 dif = (plr.transform.position - this.transform.position);
                            AimDirection = dif.normalized;

                        }
                        MySpriteRenderer.flipX = (Mathf.Sign(AimDirection.x) < 0f);
                        if (stateexpired)
                        {
                            ShootWindUpGlow.Play();
                            
                            
                            
                            IceSkullProjectile p = GameObject.Instantiate(SkullProjectile,this.transform.position,new Quaternion());
                            float sp = 5f* (1f + (2f * Astronaut.AggressionLevelF));
                            p.MyRigidbody.velocity = AimDirection*sp;

                            setState(State.FloatingBackAndForth,.5f+(2f*Random.value));
                        }
                        else
                        {
                            //MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
                            MyRigidbody.velocity = new Vector2();
                        }

                        break;
                    }
            }


            //MyRigidbody.velocity = new Vector2(movedir * movespeed, MyRigidbody.velocity.y);
        }
        else
        {
            setState(State.FloatingBackAndForth, .5f + (2f * Random.value));
        }
    }
    private Vector3 AimDirection = Vector3.left;
    
    public void setState(State st,float dur)
    {
        
        StateTime = Time.time;
        StateDuration = dur;
        if ((st == State.Firing) &&(MyState != State.Firing))
        {
            MyRigidbody.velocity = new Vector2();
            ShootWindUpGlow.Play();
        }

        MyState = st;
    }
    
    public override void Kill()
    {
        if (!Alive) return;
        Alive = false;
        //base.Kill();
        Astronaut.PlayKillSound(2);
        Astronaut.TheAstronaut.dropResistance(.5f / (1f + HitsDone), this.transform.position, Astronaut.Element.Ice);
        deathKnockback();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        Astronaut a = collision.gameObject.GetComponent<Astronaut>();
        if ((a != null) && (a.Alive))
        {
            if (EtherealBehavior)
            {
                //teleport the player backward
                if (EtherealWillingToTeach)
                {
                    a.SendBackToEtherealCheckpoint();
                }
            }
            else
            {
                if (!isStunned())
                    if ((Time.time - a.lastDamageTakenTime) >= 2f)
                    {
                        Vector3 dif = (a.transform.position - this.transform.position);
                        HitsDone += 1f;
                        if (Astronaut.AggressionLevel > 0)
                            if ((!a.Frozen) && ((Time.time - a.UnfreezeTime) >= 1.5f))
                                a.freeze(.5f * Astronaut.AggressionLevel);
                        if (a.TakeDamage(20f, dif.normalized * 10f + new Vector3(0f, a.JumpSpeed, 0f)))
                        {
                            HitsDone += 4f;
                        }

                    }
            }

        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionEnter2D(collision);
    }


    private float LastShotTime = -10f;
    private Vector3 TargetingDirection;

}
