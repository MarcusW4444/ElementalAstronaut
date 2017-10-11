﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Ice_Blob : GenericEnemy {
    // Use this for initialization
    public enum State { None, Idling, PreparingToJump, Jumped};
    public State MyState = State.Idling;
    


    void Start()
    {
        StartPosition = this.transform.position;
        StateTime = Time.time;
        setState(State.Idling, 1.25f);
    }
    private Vector3 StartPosition;
    

    // Update is called once per frame
    public IceSkullProjectile SkullProjectile;
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
    public Animator MyAnimator;
    void FixedUpdate()
    {
        Astronaut plr = Astronaut.TheAstronaut;

        if (Alive && !isStunned())
        {
            bool stateexpired = (Time.time >= (StateTime + StateDuration));

            switch (MyState)
            {
                case State.None: { break; }
                case State.Idling:
                    {
                        MyAnimator.SetBool("Jumping",false);

                        //MyRigidbody.AddForce(new Vector2(0f, 0f) * 10f);
                        MyRigidbody.velocity = new Vector2(0f, MyRigidbody.velocity.y);

                        if (stateexpired)
                        {
                            bool ch = false;

                            if ((plr != null) && (plr.Alive))
                            {


                                Vector3 dif = (plr.transform.position - this.transform.position);
                                if (dif.magnitude < 10f)
                                {
                                    RaycastHit2D rh = Physics2D.Linecast(this.transform.position, plr.transform.position, LayerMask.GetMask(new string[] { "Geometry" }));
                                    
                                    if (rh.distance <= 0f)
                                    {
                                        //Debug.Log("Take aim");
                                        //ParticleSystem s = ShootWindUpGlow;
                                        setState(State.PreparingToJump, .5f);
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
                                setState(State.Idling, .4f);
                        }


                        if (MyRigidbody.velocity.x != 0f)
                        {
                            MySpriteRenderer.flipX = (Mathf.Sign(MyRigidbody.velocity.x) < 0f);
                        }

                        break;
                    }
                case State.PreparingToJump:
                    {
                        MyAnimator.SetBool("Jumping", false);
                        MyRigidbody.velocity = new Vector2(0f, MyRigidbody.velocity.y);

                        if ((plr != null) && (plr.Alive))
                        {


                            Vector3 dif = (plr.transform.position - this.transform.position);
                            if (dif.magnitude < 10f)
                            {
                                LookDirection = (int)((dif.x != 0)?Mathf.Sign(dif.x): LookDirection);


                            }

                            if (LookDirection != 0f)
                            MySpriteRenderer.flipX = (LookDirection > 0f);

                            }

                        if (stateexpired)
                        {
                            this.MyRigidbody.velocity = new Vector2(5f*LookDirection,10f);
                            //Jump!!
                            setState(State.Jumped,0f);
                        }

                        break;
                    }
                case State.Jumped:
                    {
                        MyAnimator.SetBool("Jumping", true);

                        

                        if (MyRigidbody.velocity.y <= 0f)
                        {
                            RaycastHit2D rh = Physics2D.Linecast(this.transform.position, this.transform.position + Vector3.down * .6f, LayerMask.GetMask(new string[] { "Geometry" }));

                            if (rh.distance <= 0f)
                            {
                                //in the air.
                            } else if (rh.distance <= .6f)
                            {
                                //Landed
                                setState(State.Idling,.4f);
                            }
                        }
                        

                        break;
                    }
            }


            //MyRigidbody.velocity = new Vector2(movedir * movespeed, MyRigidbody.velocity.y);
        }
        else
        {
            setState(State.Idling, .5f + (2f * Random.value));
        }
    }
    public int LookDirection = 1;
    private Vector3 AimDirection = Vector3.left;

    public void setState(State st, float dur)
    {

        StateTime = Time.time;
        StateDuration = dur;
        /*
        if ((st == State.Firing) && (MyState != State.Firing))
        {
            MyRigidbody.velocity = new Vector2();
            ShootWindUpGlow.Play();
        }
        */
        MyState = st;
    }

    public GameObject Subglob;
    public void subdivide()
    {

    }

    public override void Kill()
    {
        if (!Alive) return;
        Alive = false;
        //base.Kill();
        //
        deathKnockback();
    }


    private float LastShotTime = -10f;
    private Vector3 TargetingDirection;


}
