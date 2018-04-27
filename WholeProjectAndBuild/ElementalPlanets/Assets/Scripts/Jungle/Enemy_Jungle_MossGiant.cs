using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Jungle_MossGiant : GenericEnemy
{

	// Use this for initialization
	void Start () {
        Attacking = false;
        BaseColor = MySpriteRenderer.color;
        OriginalPosition = this.transform.position;
        SpeedValue = 0f;
        if (Submerssion >= 0f)
        {
            
            MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
            //MyCollider.enabled = false;
        }
    }
    public Collider2D MyCollider; 
    private Vector3 OriginalPosition;
    // Update is called once per frame
    public bool Attacking = false;
    private float AttackTime = -10f;
    public const float MoveSpeed = 4f;
    public const float MoveAccelerationRate = 8f;
    public float SpeedValue = 0f;

    private int MoveDirection = 0;
    private int LookDirection = 0;
    private float MoveDirectionTime = -10f;
    public float Submerssion = 0f;
    public bool AppearsWhenToLeft=true, AppearsWhenToRight = true;
    public float AppearRange = 4f;
    public void checkForEdge()
    {
        if (MoveDirection != 0)
        {
            RaycastHit2D rh = Physics2D.Linecast(this.transform.position+new Vector3(MoveDirection*2f,3f,0f), this.transform.position + new Vector3(MoveDirection * 2f, -3f, 0f), LayerMask.GetMask(new string[] { "Geometry" }));
            if (rh.distance <= 0f)
            {
                //No collision here. please do not walk over the edge
                MoveDirection = 0;
            }
        }
    }
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
    }

    void FixedUpdate()
    {

        if (Alive && !isStunned())
        {

            Astronaut plr = Astronaut.TheAstronaut;

            if (Submerssion > 0f)
            {
                

                if ((plr != null) && (plr.Alive) && ((Time.time - plr.ReviveTime) >= 2f))
                {
                    Physics2D.IgnoreCollision(MyCollider, Astronaut.TheAstronaut.MyCollider, true);

                    Vector3 dif = (OriginalPosition - plr.transform.position);


                    if ((dif.magnitude < AppearRange)|| (Health < MaxHealth) || (Submerssion < 1f))
                    {


                        RaycastHit2D rh = Physics2D.Linecast(OriginalPosition, plr.transform.position, LayerMask.GetMask(new string[] { "Geometry" }));
                        //Debug.Log(rh.distance);
                        //Debug.Log(rh.collider);
                        bool correctside = false;
                        if (AppearsWhenToLeft && (dif.x < 0f)) correctside = true;
                        if (AppearsWhenToRight && (dif.x > 0f)) correctside = true;
                        if (((rh.collider == null) && (correctside))|| (Health < MaxHealth)||(Submerssion < 1f))
                        {

                            float df = .125f;
                            if (((dif.magnitude < (AppearRange * .125f)))|| (Health < MaxHealth))
                            {
                                df = 1f;
                            } else if (dif.magnitude < (AppearRange * .25f))
                            {
                                df = .5f;
                            } else if (dif.magnitude < (AppearRange*.5f))
                            {
                                df = .25f;
                            } else
                            {
                                //df = 0f;
                            }
                            df *= 1f; 
                            float e = (df * Time.fixedDeltaTime);

                            e = Mathf.Min(e,Submerssion);
                            //this.transform.position = Vector3.Lerp(OriginalPosition, (OriginalPosition + (Vector3.down * 2f)), Submerssion);
                            Submerssion = Mathf.Clamp01(Submerssion-e);
                            if (Submerssion <= 0f)
                            {
                                Submerssion = 0f;
                                Physics2D.IgnoreCollision(MyCollider, Astronaut.TheAstronaut.MyCollider, false);
                                MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
                            }
                            
                        }
                        LookDirection = ((int)Mathf.Sign(-dif.x));
                    }
                }

                this.transform.position = Vector3.Lerp(OriginalPosition, (OriginalPosition + (Vector3.down * 2f)), Submerssion);
            }  else
            {
                if (Attacking)
                {
                    MyAttackCollider.enabled = true;
                    if ((Time.time - AttackTime) >= .5f)
                    {
                        MyAttackCollider.enabled = false;
                        Attacking = false;
                    }
                    else
                    {
                        MyAttackCollider.enabled = ((Time.time - AttackTime) >= .4f);
                    }
                    MoveDirection = 0;
                }
                else
                {

                    float at = (1.2f-(.6f*Astronaut.AggressionLevelF));
                    
                    if ((Time.time - AttackTime) >= at)
                    {
                        //If the player is within range

                        if ((plr != null) && (plr.Alive) && ((Time.time - plr.ReviveTime) >= 2f))
                        {
                            Vector3 dif = (plr.transform.position - this.transform.position);



                            if (dif.magnitude < 6f)
                            {
                                int mi = ((int)Mathf.Sign(dif.x));
                                if (mi == MoveDirection)
                                {
                                    MoveDirectionTime = Time.time;
                                }
                                else
                                {
                                    if ((Time.time - MoveDirectionTime) >= .8f)
                                    {


                                        MoveDirection = mi;

                                    }
                                }
                                if (dif.magnitude < 2f)
                                {
                                    Attack();
                                }
                                else
                                {

                                }
                            }


                        }
                        else
                        {

                        }


                    }
                }
            }

            checkForEdge();









            if (MoveDirection != 0)
            {
                MyAnimator.SetBool("Walking", true);
                
                float spe = (MoveSpeed * (1f - FreezeFactor)* (.75f + (1.75f * Astronaut.AggressionLevelF)));
                SpeedValue = Mathf.Min(SpeedValue + (MoveAccelerationRate*(1f+(1.5f* Astronaut.AggressionLevelF)) * Time.fixedDeltaTime), spe);
                MyRigidbody.velocity = new Vector2(SpeedValue * MoveDirection, MyRigidbody.velocity.y);
                this.transform.localScale = new Vector3(Mathf.Abs(this.transform.localScale.x) * Mathf.Sign(-MoveDirection), this.transform.localScale.y, this.transform.localScale.z);
                
            } else
            {
                SpeedValue = 0f;
                MyRigidbody.velocity = new Vector2(0f, MyRigidbody.velocity.y);
                MyAnimator.SetBool("Walking", false);
                if (LookDirection != 0)
                {
                    this.transform.localScale = new Vector3(Mathf.Abs(this.transform.localScale.x) * Mathf.Sign(-LookDirection), this.transform.localScale.y, this.transform.localScale.z);
                    LookDirection = 0;
                }
            }

        } else
        {
            MoveDirection = 0;
            MyAnimator.SetBool("Walking", false);
        }

        
    }


    public void Attack()
    {
        if (!Alive) return;
        Attacking = true;
        Am.am.oneshot(Am.am.M.MossGiantSwing);
        AttackTime = Time.time;
        MyAnimator.SetTrigger("Attack");
    }
    public Animator MyAnimator;
    public Collider2D MyAttackCollider;

    public override void Kill()
    {
        if (!Alive) return;
        Alive = false;
        Astronaut.PlayKillSound(3);
        deathKnockback();
        Astronaut.TheAstronaut.dropResistance(.4f / (1f + HitsDone), this.transform.position, Astronaut.Element.Grass);
    }

    public override void TakeDamage(float dmg, Vector2 dir)
    {

        if (Submerssion > 0f) //Damage resistance while submerged.
        {
            dmg *= (1f-(.9f*Submerssion));
        }
        base.TakeDamage(dmg,dir);

    }


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
                    if (a.TakeDamage(20f, dif.normalized * 3f + new Vector3(0f, a.JumpSpeed * .1f, 0f)))
                    {
                        HitsDone += 4f;
                    }

                }

        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionStay2D(collision);
    }
}
