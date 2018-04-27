using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Void_Phantom : GenericEnemy {

	// Use this for initialization
	void Start () {

        ManualFlipping = true;
    }

    public bool AttackingRanged;
    //public PhantomProjection MyPhantomProjections[]
    public Animator MyAnimator;
	// Update is called once per frame
	

    public bool Attacking = false;
    public bool ThrowingProjectile = false;
    private float AttackingTime = -10f;
    private float ProjectileShotTime = -10f;

    private void FixedUpdate()
    {

        Astronaut plr = Astronaut.TheAstronaut;


        if (Alive && !isStunned())
        {


            if (Attacking)
            {
                

                if (ThrowingProjectile)
                {
                    //Launch Projectile
                    float tu = .3f;
                    if (((Time.time - AttackingTime) >= tu) && (((Time.time- AttackingTime) - Time.fixedDeltaTime) < tu))
                    {
                        aimdir = (Astronaut.TheAstronaut.transform.position - this.transform.position).normalized;
                        ProjectileShotTime = Time.time;
                        VoidSlashProjectile sl = GameObject.Instantiate<VoidSlashProjectile>(SlashProjectilePrefab,this.transform.position,SlashProjectilePrefab.transform.rotation);
                        lastrangedattack = Time.time;
                        Vector2 vf = (new Vector2(aimdir.x, aimdir.y).normalized * (10f * (1f + (1f * Astronaut.AggressionLevelF))));
                        sl.MyRigidbody.velocity = vf;
                        /*
                        Vector2 r = Random.insideUnitCircle;

                        sl = GameObject.Instantiate<VoidSlashProjectile>(sl, this.transform.position+new Vector3(r.x,r.y,0f), SlashProjectilePrefab.transform.rotation);
                        r = Random.insideUnitCircle;
                        
                        sl.MyRigidbody.velocity = (sl.MyRigidbody.velocity+new Vector2(r.x,r.y)* sl.MyRigidbody.velocity.magnitude*.25f* (Astronaut.AggressionLevelF));
                        */
                        SlashFlare.transform.position = this.transform.position+ new Vector3(Mathf.Sign(aimdir.x),0f,0f);
                        SlashFlare.Play();
                        
                        Am.am.oneshot(Am.am.M.VoidPhantom_SlashLaunch);
                        Debug.Log("SHOOT");
                    }

                    if ((Time.time - AttackingTime) >= .8f)
                    {
                        Attacking = false;
                    }


                } else {
                    //Slash

                    if (((Time.time - AttackingTime) >= .2f) && ((Time.time - AttackingTime) <= .4f))
                    {
                        Collider2D col = Physics2D.OverlapCircle(new Vector2(this.transform.position.x+(.5f*Mathf.Sign(-this.transform.localScale.x)),this.transform.position.y),1f, LayerMask.GetMask(new string[] { "Player" }));
                        if (col != null)
                        {
                            Astronaut a = col.gameObject.GetComponent<Astronaut>();

                            if ((a != null) && (a.Alive) &&(!slashhit))
                            {
                                Vector3 dif = (plr.transform.position - this.transform.position);

                                plr.TakeDamage(15f, -dif*1f);
                                SlashEffect.transform.position = a.transform.position;
                                SlashEffect.Play();
                                Am.am.oneshot(Am.am.M.VoidPhantom_SlashHit);
                                slashhit = true;
                            }

                        }
                        if (!playedswing)
                        {
                            Am.am.oneshot(Am.am.M.VoidPhantom_SlashSwing);
                            playedswing = true;
                        }

                    }
                    



                    if ((Time.time - AttackingTime) >= .5f)
                    {
                        Attacking = false;
                    }

                }


                


            }
            else
            {




                if ((plr != null) && (plr.Alive) && (!Astronaut.TheAstronaut.Quelling))
                {
                    Vector3 dif = (plr.transform.position - this.transform.position);


                    if (dif.magnitude < 10f)
                    {


                        if (dif.magnitude <1.35f)
                        {

                            sliceAttack();

                        } else if (dif.magnitude < 8f)
                        {
                            
                            MoveDirection = ((dif.normalized * ((dif.magnitude < 5f) ?6f:2f)) * (1f + (1f * Astronaut.AggressionLevelF)));

                            if ((Time.time - lastrangedattack) >= (1.5f*(1f-Astronaut.AggressionLevelF)))
                            {
                                sliceProjectileAttack();
                            }
                            

                        } else
                        {
                            MoveDirection = ((dif.normalized * 3f)* (1f+(1f * Astronaut.AggressionLevelF)));
                        }






                    } else
                    {
                        MoveDirection = new Vector3();
                    }


                }


            }




           if (MoveDirection.x != 0f)
            {
                this.transform.localScale = new Vector3(Mathf.Abs(this.transform.localScale.x) * Mathf.Sign(-MoveDirection.x), this.transform.localScale.y, this.transform.localScale.z);
            }
            MyRigidbody.velocity = new Vector2(MoveDirection.x,MoveDirection.y)* (1f - FreezeFactor);
        } else
        {
            
        }


    }
    private bool playedswing = false;
    private Vector3 MoveDirection = new Vector3();
    private bool slashhit = false;
    public ParticleSystem SlashEffect;
    public ParticleSystem SlashFlare;
    private float lastrangedattack = -10f;
    private float PhantomSpawnTime;
    public VoidSlashProjectile SlashProjectilePrefab;
    private Vector3 aimdir;
    public void sliceAttack()
    {
        AttackingTime = Time.time;
        Attacking = true;
        playedswing = false;
        ThrowingProjectile = false;
        slashhit = false;
        
        MoveDirection = new Vector3();
        MyAnimator.SetTrigger("Attack");
        

    }
    public void sliceProjectileAttack()
    {
        Attacking = true;
        ThrowingProjectile = true;
        MyAnimator.SetTrigger("Attack");
        AttackingTime = Time.time;
        
    }

    public void createPhantoms()
    {

    }
    public ParticleSystem NegativeParticles;
    public override void Kill()
    {
        if (!Alive) return;
        Alive = false;

        Astronaut.PlayKillSound(2);
        NegativeParticles.Stop();
        GameObject.Destroy(NegativeParticles.gameObject, 4f);
        deathKnockback();
        Astronaut.TheAstronaut.dropResistance(.4f / (1f + HitsDone), this.transform.position, Astronaut.Element.Void);
        
        //MyVoidField.Deactivate();
        //MyVoidField.transform.SetParent(null);
        //GameObject.Destroy(MyVoidField.gameObject, 4f);
    }


}
