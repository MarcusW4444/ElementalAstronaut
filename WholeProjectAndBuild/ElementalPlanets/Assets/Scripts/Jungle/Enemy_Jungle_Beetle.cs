using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Jungle_Beetle : GenericEnemy
{

    // Use this for initialization
    void Start()
    {
        Attacking = false;
        BaseColor = MySpriteRenderer.color;
        OriginalPosition = this.transform.position;
        StartPosition = this.transform.position;
    }
    private Vector3 OriginalPosition;
    // Update is called once per frame
    public bool Attacking = false;
    private float AttackTime = -10f;
    public bool SporeAttacking = false;
    private float SporeAttackTime = -10f;
    public const float MoveSpeed = 4f;
    public int MoveDirection = 0;

    void Update()
    {

        if (Alive)
        {
            hitdirection = (hitdirection * (1f - Time.deltaTime));
            if (MyRigidbody.velocity.x != 0f)
            {
                //MySpriteRenderer.flipX = (Mathf.Sign(MyRigidbody.velocity.x) < 0f);
            }
            freezeStep();
            DamageFlashStep();
        }
        
        //MoveDirection = -1;
    }

    public void checkForEdge()
    {
        if (MoveDirection != 0)
        {
            RaycastHit2D rh = Physics2D.Linecast(this.transform.position + new Vector3(MoveDirection * 2f, 3f, 0f), this.transform.position + new Vector3(MoveDirection * 2f, -3f, 0f), LayerMask.GetMask(new string[] { "Geometry" }));
            if (rh.collider == null)
            {
                //No collision here. please do not walk over the edge
                MoveDirection = 0;
            }
        }
    }
    private Vector3 StartPosition;
    
    void FixedUpdate()
    {

        if (Alive && !isStunned())
        {

            Astronaut plr = Astronaut.TheAstronaut;


            if (SporeAttacking)
            {
                //Release Spores
                if ((Time.time - SporeAttackTime) >= 1.5f)
                {
                    SporeAttacking = false;
                    //MoveDirection = 0;
                    MyAnimator.SetBool("WingUp", false);
                }
            }
            else if (Attacking)
            {
                //Jumping attack    
                //MoveDirection = ((int)Mathf.Sign(MyRigidbody.velocity.x));
                if (SporeJumpLoaded)
                if((Time.time - AttackTime) >= .1f)
                if (MyRigidbody.velocity.y < 0f)
                {
                        SporeJumpLoaded = false;
                        ReleaseSpores(false);
                }
                if ((Time.time - AttackTime) >= .75f)
                {
                    Attacking = false;
                    MyAnimator.SetBool("WingUp", false);
                }
            }
            else
            {





                //If the player is within range

                if ((plr != null) && (plr.Alive) && ((Time.time - plr.ReviveTime) >= 2f))
                {
                    Vector3 dif = (plr.transform.position - this.transform.position);

                    tdif = dif;
                    
                    if (Physics2D.Linecast(this.transform.position, plr.transform.position, LayerMask.GetMask(new string[] { "Geometry" })).collider == null)
                        if (dif.magnitude < 6.5f)
                    {

                        int d = ((int)Mathf.Sign(dif.x));
                        if ((d != 0) && (MoveDirection == d))
                        {
                            passtime = Time.time;
                        }
                        else
                        {
                            if ((Time.time - passtime) > 1f)
                            {
                                MoveDirection = d;
                            }
                        }

                        jumpe = Mathf.Max(dif.y, 0f);
                        if ((dif.magnitude < 4f) && (Mathf.Min((Time.time - AttackTime), (Time.time - SporeAttackTime)) > 2.5f))
                        {
                            if ((Astronaut.AggressionLevel > 2) || false)
                            {

                                if (Random.value < .5f)
                                {
                                    
                                    Attack();
                                }
                                else
                                {
                                    ReleaseSpores(true);
                                }

                            }
                            else
                            {
                                Attack();
                            }

                        }
                        else
                        {

                        }
                    }
                    else
                    {

                        Vector3 du = (this.transform.position - StartPosition);
                        if (Mathf.Abs(du.x) > 3f)
                        {
                            int d = ((int)Mathf.Sign(-du.x));
                            MoveDirection = d;
                        }
                    }


                }
                else
                {
                    MoveDirection = 0;
                }



            }





            //if (!Attacking)
            checkForEdge();




            if (MoveDirection != 0)
            {

                if (!Attacking)
                {
                    MyAnimator.SetBool("Moving", true);
                    MyRigidbody.velocity = new Vector2(MoveSpeed * (1f - FreezeFactor) * MoveDirection * (.75f + (1.75f * Astronaut.AggressionLevelF)), MyRigidbody.velocity.y);

                    this.transform.localScale = new Vector3(Mathf.Abs(this.transform.localScale.x) * Mathf.Sign(-MoveDirection), this.transform.localScale.y, this.transform.localScale.z);
                }

            }
            else
            {
                if (!Attacking)
                {
                    MyRigidbody.velocity = new Vector2(0f, MyRigidbody.velocity.y);

                    MyAnimator.SetBool("Moving", false);
                }
            }

        }
        else
        {
            MoveDirection = 0;
            MyAnimator.SetBool("Moving", false);
            MyAnimator.SetBool("WingUp", false);
        }


    }
    private float jumpe = 0f;
    private float passtime = -10f;
    private Vector3 tdif = new Vector3();
    public void Attack()
    {
        if (!Alive) return;
        if (Mathf.Abs(MyRigidbody.velocity.y) > .010f) return;
        Attacking = true;
        AttackTime = Time.time;
        //Debug.Log(jumpe);
        SporeJumpLoaded = true;
        MoveDirection = 0;
        //Mathf.Sign(tdif.x) * 2f * MoveSpeed;
        ReleaseSpores(true);
        MyRigidbody.velocity = new Vector2(Mathf.Sign(tdif.x)*MoveSpeed* (1f - FreezeFactor) * (1+(.5f*Astronaut.AggressionLevel)), 10f*(1f+ (jumpe*.2f)));
        MyAnimator.SetBool("WingUp", true);
    }
     
    public JungleTreeProjectile SporePrefab;
    public ParticleSystem SporeExplosion;
    private bool SporeJumpLoaded = false;
    public void ReleaseSpores(bool releaseinconeorcircle)
    {
        //if (!Alive) return;
        SporeAttacking = true;
        SporeAttackTime = Time.time;
        
        MyRigidbody.velocity = new Vector2(0f, MyRigidbody.velocity.y);
        MyAnimator.SetBool("WingUp", true);
        //Create a Cloud of Spores that slows the astronaut
        SporeExplosion.Play(true);
        int subs = (5 + (Astronaut.AggressionLevel*3));
        

        for (int i = 0; i < (subs ); i++)
        {
            JungleTreeProjectile proj = GameObject.Instantiate(SporePrefab, this.transform.position, this.transform.rotation).GetComponent<JungleTreeProjectile>();
           // proj.transform.localScale = (this.transform.localScale * (.3f));
            proj.DamageRatio = (.25f * (1f / ((float)(subs))));
            float af = (((float)i+(1*(Random.value))) / ((float)(subs)));
            float ang = (releaseinconeorcircle ? 45f+(90f * af):(360f*af));
            proj.MyConstantForce.enabled = true;
            proj.MyRigidbody.velocity = ((new Vector2(Mathf.Cos((ang / 360f) * 2f * Mathf.PI), Mathf.Sin((ang / 360f) * 2f * Mathf.PI)) * 2.5f)*(((float) (i+1))/((float)subs))* (.5f+Random.value*5f)*(1f+Astronaut.AggressionLevelF));


            proj.MyConstantForce.force = (Random.insideUnitCircle.normalized * JungleTreeProjectile.WINDFORCEFACTOR);
            //proj
        }

    }
    public Animator MyAnimator;


    public override void Kill()
    {
        if (!Alive) return;
        Alive = false;
        ReleaseSpores(false);
        ReleaseSpores(false);
        Astronaut.PlayKillSound(1);
        deathKnockback();
        Astronaut.TheAstronaut.dropResistance(.25f / (1f + HitsDone), this.transform.position, Astronaut.Element.Grass);
    }
}
