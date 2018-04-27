using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Jungle_Bush : GenericEnemy {

	// Use this for initialization
	void Start () {
        Attacking = false;
    }

    // Update is called once per frame
    public bool Attacking = false;
    private float AttackTime = -10f;
    private float PeekTime = -10f;
    private bool Peeking = false;
    private float BlinkTime;
    private bool Blinking = false;
    private float PeekDuration = 1f;
    private float BlinkDuration = .5f;
    private bool LookingRightLeft = false;
    private float LookRightLeftDuration = .4f;
    private float LookingRightLeftTime = -10f;
    void FixedUpdate () {
		
        if (Alive && !isStunned())
        {


            if (Attacking)
            {
                MyAttackCollider.enabled = true;
                if ((Time.time - AttackTime) >= .5f)
                {
                    MyAttackCollider.enabled = false;
                    Attacking = false;
                } else
                {
                    MyAttackCollider.enabled = ((Time.time - AttackTime) >= .4f);
                }
            } else
            {

                Astronaut plr = Astronaut.TheAstronaut;

                if ((Time.time - AttackTime) >= 1.5f)
                {
                    //If the player is within range

                     if ((plr != null) && (plr.Alive) && ((Time.time - plr.ReviveTime)>= 2f))
                    {
                        Vector3 dif = (plr.transform.position - this.transform.position);
                        if (dif.magnitude < 1f)
                        {
                            Attack();
                        }

                        
                    }

                        
                }
            }

            if (Peeking)
            {
                if ((Time.time - PeekTime) >= PeekDuration)
                {
                    Peeking = false;
                    PeekTime = Time.time;
                    PeekDuration = (1f + (2.5f * Random.value));
                }
            } else
            {
                if ((Time.time - PeekTime) >= PeekDuration)
                {
                    Peeking = true;
                    BlinkTime = Time.time;
                    PeekTime = Time.time;
                    PeekDuration = (1.5f + (2.5f * Random.value));
                }
            }
            if (Blinking)
            {
                if ((Time.time - BlinkTime) >= .2f)
                {
                    BlinkTime = Time.time;
                    Blinking = false;
                    BlinkDuration = (.5f + (1.5f * Random.value));
                }
            } else
            {
                if ((Time.time - BlinkTime) >= BlinkDuration)
                {
                    BlinkTime = Time.time;
                    Blinking = true;
                }
                
            }

            if ((Time.time - LookingRightLeftTime) >= LookRightLeftDuration)
            {
                LookingRightLeft = !LookingRightLeft;
                LookingRightLeftTime = Time.time;
                if (Random.value < .25f)
                {
                    LookRightLeftDuration = (.4f+(.8f*Random.value));
                } else
                {
                    LookRightLeftDuration = .4f;
                }
            }


            MyAnimator.SetBool("Peeking",Peeking);
            MyAnimator.SetBool("Blinking", Blinking);
            MyAnimator.SetBool("PeekLeftRight", !LookingRightLeft);
        }
	}
    

    public void Attack()
    {
        if (!Alive) return;
        Attacking = true;
        AttackTime = Time.time;
        MyAnimator.SetTrigger("Attack");
        Am.am.oneshot(Am.am.M.BushGhostSneakAttack);
    }
    public Animator MyAnimator;
    public Collider2D MyCollider, MyAttackCollider;
    public void OnAttackTriggerEnter()
    {

    }

    public Enemy_Jungle_Ghost SurpriseGhost;
    public Sprite OriginalBushSprite;
    public override void Kill()
    {
        if (!Alive) return;
        GameObject.Instantiate(SurpriseGhost, this.transform.position,SurpriseGhost.transform.rotation);
        Alive = false;
        MyCollider.enabled = false;
        MyAttackCollider.enabled = false;
        MyAnimator.enabled = false;
        MySpriteRenderer.sprite = OriginalBushSprite;
        //deathKnockback();
        //Astronaut.TheAstronaut.dropResistance(.3f / (1f + HitsDone), this.transform.position, Astronaut.Element.Grass);
    }
}
