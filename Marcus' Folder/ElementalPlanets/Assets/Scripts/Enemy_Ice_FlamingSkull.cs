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
    void Update()
    {

        if (Alive)
        {
            hitdirection = (hitdirection * (1f - Time.deltaTime));
            DamageFlashStep();
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
                case State.FloatingBackAndForth: {

                        float sdif = (this.transform.position.x - StartPosition.x);
                        if (Mathf.Abs(sdif) > 2f)
                        {
                            float sig = Mathf.Sign(sdif);
                            GoLeftGoRight = (sig >= 0f);
                        }
                        MyRigidbody.AddForce(new Vector2(GoLeftGoRight ? -1f : 1f, 0f) * 10f);


                        if (stateexpired)
                        {
                            bool ch = false;

                            if ((plr != null) && (plr.Alive))
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
                            float sp = 10f;
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
        Astronaut.TheAstronaut.dropResistance(.5f / (1f + HitsDone), this.transform.position, Astronaut.Element.Ice);
        deathKnockback();
    }
    

    private float LastShotTime = -10f;
    private Vector3 TargetingDirection;

}
