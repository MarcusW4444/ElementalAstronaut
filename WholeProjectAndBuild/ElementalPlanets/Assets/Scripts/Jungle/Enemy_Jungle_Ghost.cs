using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Jungle_Ghost : GenericEnemy
{
    public enum State { None, Surprising, Waiting,Chasing,Firing,Fading,};

    public State MyState = State.Surprising;
    void Start()
    {
        StartPosition = this.transform.position;
        StateTime = Time.time;
        originalspritescale = MySpriteRenderer.transform.localScale;
        //setState(State.Waiting, .35f);
    }
    private Vector3 StartPosition;
    private bool GoLeftGoRight;

    // Update is called once per frame
    public JungleGhostProjectile JungleGhostProjectilePrefab;
    public ParticleSystem ShootWindUpGlow, ShootFlash;
    public const float MoveSpeed = 5f;
    private float StateTime = -10f;
    private float StateDuration = .5f;
    private bool FadingInOut;
    public ParticleSystem NegativeParticles;
    void Update()
    {

        if (Alive)
        {
            hitdirection = (hitdirection * (1f - Time.deltaTime));
            DamageFlashStep();
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
                case State.Waiting:
                    {
                        /*
                        float sdif = (this.transform.position.x - StartPosition.x);
                        if (Mathf.Abs(sdif) > 2f)
                        {
                            float sig = Mathf.Sign(sdif);
                            GoLeftGoRight = (sig >= 0f);
                        }
                        MyRigidbody.AddForce(new Vector2(GoLeftGoRight ? -1f : 1f, 0f) * 10f);

                        */

                        if (stateexpired)
                        {

                            bool ch = false;
                            if (FadeValue >= 1f)
                            {
                                NegativeParticles.Stop();
                                    FadingInOut = true;
                                    
                                    setState(State.Fading, .5f);
                                ch = true;
                            }
                            
                            
                            if (!ch)
                            if ((plr != null) && (plr.Alive))
                            {


                                Vector3 dif = (plr.transform.position - this.transform.position);
                                if (dif.magnitude < 10f)
                                {
                                    //RaycastHit2D rh = Physics2D.Linecast(this.transform.position, plr.transform.position, LayerMask.GetMask(new string[] { "Geometry" }));
                                    //Debug.Log(rh.distance);
                                   if (true)// if (rh.distance <= 0f) //Has line of sight
                                    {

                                        if (Random.value < .5f)
                                        {
                                            ParticleSystem s = ShootWindUpGlow;
                                            MyAnimator.SetTrigger("Firing");
                                            setState(State.Firing, s.main.duration);



                                        } else
                                        {
                                            setState(State.Chasing, 2f);
                                        }
                                        ch = true;
                                        //Debug.Log("Visible");
                                    }
                                    else
                                    {
                                        Debug.Log("hiding...");
                                    }
                                }
                            }
                            if (!ch)
                                setState(State.Waiting, .5f);
                        }


                        
                        
                        break;
                    }
                    
                case State.Chasing:
                    {


                        if ((plr != null) && (plr.Alive))
                        {

                            float sdif = (this.transform.position.x - plr.transform.position.x);
                            float ydif = (this.transform.position.y - plr.transform.position.y);
                            if (Mathf.Abs(sdif) > 2f)
                            {
                                float sig = Mathf.Sign(sdif);
                                //GoLeftGoRight = (sig >= 0f);

                                if (sdif != 0f)
                                {
                                    this.transform.localScale = new Vector3(1f * Mathf.Sign(sdif), 1f, 1f);//(Mathf.Sign(MyRigidbody.velocity.x) < 0f);
                                    if (Mathf.Abs(sdif) > 4f)
                                    {
                                        MyRigidbody.AddForce(new Vector2(-1f * Mathf.Sign(sdif), 0f)* (1f - FreezeFactor) * 4f);
                                        if (stateexpired)
                                        {
                                            ParticleSystem s = ShootWindUpGlow;
                                            MyAnimator.SetTrigger("Firing");
                                            setState(State.Firing, s.main.duration);
                                        }
                                    } else
                                    {
                                        
                                        MyRigidbody.AddForce(new Vector2(1f * Mathf.Sign(sdif), 0f) * 10f);
                                        if (stateexpired)
                                        {
                                            FadingInOut = false;
                                            setState(State.Fading, .5f);
                                        }
                                    }
                                    
                                    
                                    
                                }
                            }
                            

                            if (Mathf.Abs(ydif) > 1f)
                            {
                                
                                MyRigidbody.AddForce(new Vector2(0f, (ydif > 0f) ? -1f : 1f)* ((Mathf.Abs(sdif) < 3f)?-1f:1f) * 5f);
                            }




                        } else
                        {

                            setState(State.Waiting,.5f);
                        }


                        

                        
                        break;
                    }
                case State.Firing:
                    {

                        if ((plr != null) && (plr.Alive))
                        {
                            Vector3 dif = (plr.transform.position - this.transform.position);
                            AimDirection = dif.normalized;
                            this.transform.localScale = new Vector3(-1f * Mathf.Sign(dif.x), 1f, 1f);//(Mathf.Sign(MyRigidbody.velocity.x) < 0f);
                        }
                        //MySpriteRenderer.flipX = (Mathf.Sign(AimDirection.x) < 0f);
                        if (stateexpired)
                        {
                            ShootWindUpGlow.Play();



                            JungleGhostProjectile p = GameObject.Instantiate(JungleGhostProjectilePrefab, this.transform.position, new Quaternion());
                            float sp = 5f;
                            p.MyRigidbody.velocity = AimDirection * sp;
                            if (Astronaut.AggressionLevel > 0)
                            {
                                int e = Astronaut.AggressionLevel*2;
                                Vector3 off = Vector3.Cross(new Vector3(p.MyRigidbody.velocity.normalized.x, p.MyRigidbody.velocity.normalized.y,0f),Vector3.forward);

                                for (int i = 0; i <= e; i++)
                                {
                                    JungleGhostProjectile c = GameObject.Instantiate(p, p.transform.position, p.transform.rotation);
                                    c.MyRigidbody.velocity = ((p.MyRigidbody.velocity + (new Vector2(off.x, off.y) * (((((float)i)/((float)e)))-.5f)*2f ))*1.2f);
                                }
                            }

                            setState(State.Waiting, .5f);
                        }
                        else
                        {
                            //MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
                            MyRigidbody.velocity = new Vector2();
                        }

                        break;
                    }

                case State.Fading:
                    {

                        float f = ((Time.time - StateTime) / StateDuration);
                        //Debug.Log("Fading");
                        if (FadingInOut)
                        { 

                            FadeValue = 1f-f;
                            if (f >= 1f)
                            {
                                //Faded in
                                NegativeParticles.Play();
                                setState(State.Waiting, .5f);
                            } 
                        }  else
                        {
                            FadeValue = f;
                            if (f >= 1f)
                            {
                                //Faded Out
                                
                                setState(State.Waiting, .5f);
                                Teleport();
                            }
                            
                        }


                        break;
                    }
                case State.Surprising: {



                        if (stateexpired)
                        {
                            this.MyCollider.enabled = true;
                            setState(State.Waiting,.25f);
                        } else
                        {
                            this.transform.position = (this.transform.position + (Vector3.up * Time.fixedDeltaTime * 2f));

                        }


                        break; }
            }


            //MyRigidbody.velocity = new Vector2(movedir * movespeed, MyRigidbody.velocity.y);
        }
        else
        {
            FadeValue = 0f;
            setState(State.Waiting, .5f);
        }
        MyAnimator.SetFloat("MoveSpeed",Mathf.Abs(MyRigidbody.velocity.x)); 
            if (FadeValue > 0f)
        {
            BaseColor = Color.Lerp(Color.white,new Color(1f,1f,1f,0f),FadeValue);
            MySpriteRenderer.transform.localScale = new Vector3(originalspritescale.x*(1f+(8f*FadeValue)),originalspritescale.y*(1f-FadeValue),originalspritescale.z);
            if (Alive) MyCollider.enabled = false;
        } else
        {
            
            MySpriteRenderer.transform.localScale = originalspritescale;
            if (Alive) MyCollider.enabled = true;
        }

    }
    public float[] distances;
    public float[] distancefloats;
    public int teleindex = 0;
    public float randval = 0f;
    public void Teleport()
    {

        int numberofdirectionchoices = 16;
        float maxdistance = 7f;
        distances = new float[numberofdirectionchoices];
        //look in several different directions
        float ang = (360f / ((float)numberofdirectionchoices));
        float tot = 0f;
        for (int i = 0; i < numberofdirectionchoices; i++)
        {
            float rang = (Mathf.PI*2f*(((float)i)/((float)numberofdirectionchoices)));
            
            RaycastHit2D rh = Physics2D.Raycast(new Vector2(this.transform.position.x, this.transform.position.y),new Vector2(Mathf.Cos(rang), Mathf.Sin(rang)),maxdistance,LayerMask.GetMask("Geometry"));
            if (rh.collider != null)
            {
                Debug.Log("Tele yes");
                distances[i] = rh.distance;
            } else
            {
                Debug.Log("Tele none");
                distances[i] = maxdistance;
            }
            tot+=distances[i];
        }

        


            
        
        if (tot > 0f)
        {
            
            distancefloats = new float[numberofdirectionchoices];
            for (int i = 0; i < numberofdirectionchoices; i++)
            {
                distancefloats[i] = (distances[i] / tot);
            }
            float rv = Random.value;
            int ind = 0;
            float f = 0f;
            randval = rv;
            while ((ind < numberofdirectionchoices) && (f < rv))
            {
                float a = distancefloats[ind];
                if (rv > a)
                {
                    rv -= a;
                    ind++;
                } else
                {
                    float rang = (Mathf.PI * 2f * (((float)ind) / ((float)numberofdirectionchoices)));
                    teleindex = ind;
                    Vector2 teledir = new Vector2(Mathf.Cos(rang), Mathf.Sin(rang));
                    this.transform.position = this.transform.position + (new Vector3(teledir.x, teledir.y, 0f)*Mathf.Max(0f,(distances[ind]-.5f))*(.5f+(.5f*Random.value)));
                    break;
                }
            }


        }


            //commit to the teleport
        }
    public Vector3 originalspritescale;
    public float FadeValue = 0f;
    public Collider2D MyCollider;
    private Vector3 AimDirection = Vector3.left;

    public void setState(State st, float dur)
    {

        StateTime = Time.time;
        StateDuration = dur;
        if ((st == State.Firing) && (MyState != State.Firing))
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
        base.Kill();
        Astronaut.PlayKillSound(2);
        MyCollider.enabled = false;
        Astronaut.TheAstronaut.dropResistance(.3f / (1f + HitsDone), this.transform.position, Astronaut.Element.Grass);
        deathKnockback();
    }


    private float LastShotTime = -10f;
    private Vector3 TargetingDirection;
}
