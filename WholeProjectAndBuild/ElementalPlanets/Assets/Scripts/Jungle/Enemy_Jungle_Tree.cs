using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Jungle_Tree : GenericEnemy
{


    // Use this for initialization
    void Start()
    {
        Attacking = false;
        //MyTyphoonField = GameObject.Instantiate<TyphoonField>(TyphoonFieldPrefab,this.transform.position,TyphoonFieldPrefab.transform.rotation);
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
    public ParticleSystem RegenerationParticles;
    void FixedUpdate()
    {

        if (Alive)
        {


            if (Attacking)
            {
                
                if ((Time.time - AttackTime) >= (.5f * (1f - (.75f * (Astronaut.AggressionLevelF)))))
                {
                    
                    Attacking = false;
                }
                else
                {
                    
                }
            }
            else
            {

                Astronaut plr = Astronaut.TheAstronaut;

                
                    //If the player is within range

                    if ((plr != null) && (plr.Alive) && ((Time.time - plr.ReviveTime) >= 2f) && (!isStunned()))
                    {
                        Vector3 dif = (plr.transform.position - this.transform.position);

                        if ((dif.magnitude < 10f) && ((dif.x*Mathf.Sign(this.transform.localScale.x)) < 0))
                        {
                        //Attack();
                        if ((Time.time - AttackTime) >= (2.5f * (1f - (.5f * (Astronaut.AggressionLevelF)))))
                        {
                            ReleaseSpores(false);
                        }
                        if (!MyTyphoonField.TyphoonWindsActive)
                        {
                            Vector3 v = Vector3.left;
                            MyTyphoonField.startTyphoon(v * (5f * (1f + (4f * Astronaut.AggressionLevelF))) , 2f + (3f * Astronaut.AggressionLevelF));
                        }

                    }


                        
                    }


                

                
            }

            


            
        }
    }


    public void Attack()
    {
        if (!Alive) return;
        //Please. Just release spores that home in on the player..
        //The projectiles are obnoxious.
        Attacking = true;
        AttackTime = Time.time;
        MyAnimator.SetTrigger("Attack");
        JungleTreeProjectile p = GameObject.Instantiate(ProjectilePrefab, this.transform.position+(Vector3.down*1f), new Quaternion());
        float sp = 4f * (1f + (2f * Astronaut.AggressionLevelF));
        p.transform.localScale = (p.transform.localScale * ((1f + (Astronaut.AggressionLevelF))));
        Astronaut plr = Astronaut.TheAstronaut;

        if ((plr != null) && (plr.Alive) && ((Time.time - plr.ReviveTime) >= 2f))
        {
            Vector3 dif = (plr.transform.position - this.transform.position);
            
            p.MyRigidbody.velocity = (dif.normalized * sp);
            p.DetonationTime = (dif.magnitude / Mathf.Max(p.MyRigidbody.velocity.magnitude, 1f));

        } else {
            p.MyRigidbody.velocity = new Vector2(-1f * Mathf.Sign(this.transform.localScale.x), 0f) * sp;
        }
        
    }

    public TyphoonField MyTyphoonField;
    //public TyphoonField TyphoonFieldPrefab;
    public JungleTreeProjectile SporePrefab;
    public ParticleSystem SporeExplosion;
    private float SporeAttackTime = -10f;
    public void ReleaseSpores(bool releaseinconeorcircle)
    {
        //if (!Alive) return;
        
        SporeAttackTime = Time.time;
        AttackTime = Time.time;
        
        MyAnimator.SetBool("WingUp", true);
        //Create a Cloud of Spores that slows the astronaut
        SporeExplosion.Play(true);
        int subs = (5 + (Astronaut.AggressionLevel * 3));


        for (int i = 0; i < (subs); i++)
        {
            JungleTreeProjectile proj = GameObject.Instantiate(SporePrefab, this.transform.position, this.transform.rotation).GetComponent<JungleTreeProjectile>();
            // proj.transform.localScale = (this.transform.localScale * (.3f));
            proj.DamageRatio = (1f * (1f / ((float)(subs))));
            float af = (((float)i + (1 * (Random.value))) / ((float)(subs)));
            float ang = (releaseinconeorcircle ? 45f + (90f * af) : (360f * af));
            proj.MyConstantForce.enabled = true;
            proj.MyRigidbody.velocity = ((new Vector2(Mathf.Cos((ang / 360f) * 2f * Mathf.PI), Mathf.Sin((ang / 360f) * 2f * Mathf.PI)) * 2.5f) * (((float)(i + 1)) / ((float)subs)) * (.5f + Random.value * 3f) * (1f + Astronaut.AggressionLevelF));


            proj.MyConstantForce.force = (Random.insideUnitCircle.normalized * JungleTreeProjectile.WINDFORCEFACTOR);
            //proj
        }

    }
    public Animator MyAnimator;
    public JungleTreeProjectile ProjectilePrefab;

    public override void Kill()
    {
        if (!Alive) return;
        Alive = false;
        Astronaut.PlayKillSound(3);
        deathKnockback();
        Astronaut.TheAstronaut.dropResistance(.7f / (1f + HitsDone), this.transform.position, Astronaut.Element.Grass);
    }

}
