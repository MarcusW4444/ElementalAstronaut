using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Void_Skeleton : GenericEnemy {

    // Use this for initialization
    public enum ManipulationFormation { Wisdom, Frontal,Orbital, Prong,Swift,};
	void Start () {
        OriginalPosition = this.transform.position;
        OriginalRotation = this.transform.rotation;
        Rushing = false;
        MyAnimator.SetBool("Rushing", false);
        MyAnimator.SetBool("Moving", false);
        ManualFlipping = true;
    }
    private float LastResetLocationTime = -10f;
    public void ResetLocation()
    {

        VoidField vf = GameObject.Instantiate<VoidField>(VoidExplosion, this.transform.position, VoidExplosion.transform.rotation);
        vf.Duration = .5f;
        this.transform.SetPositionAndRotation(OriginalPosition, OriginalRotation);
        vf = GameObject.Instantiate<VoidField>(VoidExplosion, OriginalPosition, VoidExplosion.transform.rotation);
        vf.Duration = .5f;
        //Teleport sound
    }
    public VoidField VoidExplosion;
    private Vector3 OriginalPosition;
    private Quaternion OriginalRotation;
    public ManipulationFormation ChargeManipulationFormation;
	// Update is called once per frame
	
    private float transitiontime;
    private bool Transitioning = false;
    private bool TransitioningIntoCharge = false;
    private bool defyinggravity = false;
    private const float readytime = .4f;
    private void FixedUpdate()
    {

        Astronaut plr = Astronaut.TheAstronaut;
        Vector3 dif = (plr.transform.position - this.transform.position);

        if (Alive && !isStunned())
        {

            if ((plr != null) && (plr.Alive))
            {


                bool acknowledgeplr = false;
                //Debug.Log("Mag: " + dif.magnitude);
                if ((dif.magnitude < 10f) && (!Astronaut.TheAstronaut.Quelling))
                {
                    acknowledgeplr = true;
                    //Walk Towards

                    if (!spotted)
                    {
                        SpotTime = Time.time;
                        spotted = true;
                    }
                    bool los = (Physics2D.Linecast(this.transform.position, plr.transform.position, LayerMask.GetMask(new string[] { "Geometry" })).collider == null);
                    //Line of sight is required for free directional charging

                    if (!Rushing)
                    {

                        
                        
                            int d = ((int)Mathf.Sign(Vector3.Dot(this.transform.right, dif.normalized)));
                            if ((d != 0) && ((int)Mathf.Sign(MoveDirection.x) == Mathf.Sign(d)))
                            {
                                passtime = Time.time;
                            }
                            else
                            {
                                if ((Time.time - passtime) > 1f)
                                {
                                    ForwardFacing = d;
                                }
                            }
                        MoveDirection = new Vector3(ForwardFacing*3f*(1f+(.5f*Astronaut.AggressionLevelF)),0f,0f)*(AccelerationValue)*(1f - FreezeFactor);


                        if (((Time.time - FinishRushTime) >= 4f * (1f - .75f * Astronaut.AggressionLevelF) && ((Time.time - SpotTime)>= (2f*Astronaut.AggressionLevelF)))&& los)
                        {

                            rushAttack();
                        }

                    }
                    





                } else
                {
                    spotted = true;
                    if (((Astronaut.TheAstronaut.transform.position - this.transform.position).magnitude > 10f) && ((OriginalPosition - this.transform.position).magnitude > 15f))
                    {
                        if (((Time.time - LastResetLocationTime) >= 5f))
                        {
                            ResetLocation();
                            LastResetLocationTime = Time.time;

                        }

                    }
                }

                if (Rushing)
                {
                    float rushspeed = 8f * (1f+(1.5f*Astronaut.AggressionLevelF))* AccelerationValue * (1f - FreezeFactor);

                    float dot = Vector2.Dot(new Vector2(RushDirection.x, RushDirection.y).normalized, new Vector2(dif.x, dif.y).normalized);
                    if (((Time.time - RushingTime) >= 4f) || ((dif.magnitude > 2f) && (dot < 0f)))
                    {
                        //stop Rushing
                        stopRushing();
                    } else
                    {
                        //Crash into the geometry
                        //If Vita Level is high enough, cause a massive seismic pull. on the player if he is nearby, and form a VoidField
                        Collider2D col = Physics2D.OverlapPoint(new Vector2(this.transform.position.x, this.transform.position.y) + (new Vector2(RushDirection.normalized.x, RushDirection.normalized.y) * 2f), LayerMask.GetMask(new string[] { "Geometry" }));
                        
                        if (col != null)
                        {
                            onImpact();
                            stopRushing();

                        }
                        MoveDirection = (RushDirection*rushspeed);
                    }

                    if ((Time.time - RushingTime) <= readytime)
                    {
                        MoveDirection = new Vector3();
                    } else
                    {
                        AccelerationValue = Mathf.Clamp01(AccelerationValue+(Time.fixedDeltaTime*1f));
                    }
                    MyAnimator.SetBool("Moving", false);
                } else
                {

                    if ((Time.time - FinishRushTime) <= readytime)
                    {
                        MoveDirection = new Vector3();
                    } else
                    {

                    }


                    MyAnimator.SetBool("Moving", MoveDirection.magnitude > 0f);

                }
            }





        } else
        {
            if (Rushing)
            {
                stopRushing();
            }
            MyAnimator.SetBool("Moving",false);
            MyAnimator.SetBool("Rushing", false);
        }

        if (MoveDirection.x != 0f)
        {
            this.transform.localScale = new Vector3(Mathf.Abs(this.transform.localScale.x) * Mathf.Sign(MoveDirection.x), this.transform.localScale.y, this.transform.localScale.z);
        } else
        {
            if (Rushing)
            {
                
                this.transform.localScale = new Vector3(Mathf.Abs(this.transform.localScale.x) * Mathf.Sign(RushDirection.x), this.transform.localScale.y, this.transform.localScale.z);
            }

        }
        
        MyRigidbody.velocity = new Vector2(MoveDirection.x, (!defyinggravity)?MyRigidbody.velocity.y:MoveDirection.y);
        
    }
    private float AccelerationValue = 0f;
    private bool spotted = false;
    private float SpotTime = -10f;
    private float passtime = -10f;
    private int ForwardFacing = -1;
    private Vector3 RushDirection=new Vector3();
    private float RushingTime = -10f;
    private float FinishRushTime = -10f;
    public bool Rushing = false;
    private bool WasRunning = false;
    public Animator MyAnimator;
    private Vector3 AimDirection = Vector3.left;
    private Vector3 MoveDirection;
    public VoidProjectile FormingProjectilePrefab;
    public VoidProjectile[] PersonalProjectiles;
    private float lastProjectileFormTime = -10f;
    public VoidField MyVoidField;
    public void rushAttack()
    {
        //initiate the rush attack
        defyinggravity = true;
        Rushing = true;
        RushingTime = Time.time;
        MyAnimator.SetBool("Rushing", true);
        RushParticles.Play(true);
        MyRigidbody.velocity = new Vector2();
        AccelerationValue = 0f;
        if (Astronaut.AggressionLevel >= 1) {
            //Free Directions
            Astronaut plr = Astronaut.TheAstronaut;
            Vector3 dif = (plr.transform.position - this.transform.position);
            RushDirection = dif.normalized;//new Vector3(ForwardFacing, 0f, 0f).normalized;
        } else
        {
            //Only laterally

            RushDirection = new Vector3(ForwardFacing, 0f, 0f).normalized;
        }
        if (Astronaut.AggressionLevel >= 2)
        {
            MyVoidField.Activate();

        }
    }
    public ParticleSystem RushParticles;
    public void stopRushing()
    {
        Rushing = false;
        defyinggravity = false;
        MyRigidbody.velocity = new Vector2();
        MyVoidField.Deactivate();
        FinishRushTime = Time.time;
        RushParticles.Stop(true);
        MyAnimator.SetBool("Rushing", false);
    }
    public void onImpact()
    {
        if (Astronaut.AggressionLevel > 1)
        {
            //Create a Void Field
            VoidField vf = GameObject.Instantiate<VoidField>(MyVoidField,null);
            vf.Activate();
            vf.StartTime = Time.time;
            vf.Duration = 4f;
        }
        if (Astronaut.AggressionLevel >= 3)
        {
            //Seismic Pull
            Astronaut plr = Astronaut.TheAstronaut;
            Vector3 dif = (plr.transform.position - this.transform.position);
            Astronaut.TheAstronaut.MyRigidbody.velocity = (-dif * 1f);
            SeismicQuakeParticles.Play();
        }
        Astronaut.TheAstronaut.addCamShake(2f * Astronaut.AggressionLevelF, 1f, 1f, .5f, 1f);
    }
    public ParticleSystem ImpactParticles;
    public void FormProjectile()
    {
        

    }
    public ParticleSystem HornParticles;
    public ParticleSystem PotentialParticles;
    public ParticleSystem SeismicQuakeParticles;
    public override void Kill()
    {
        if (!Alive) return;
        Alive = false;
        //base.Kill();
        Astronaut.PlayKillSound(2);
        Astronaut.TheAstronaut.dropResistance(0.4f / (1f + HitsDone), this.transform.position, Astronaut.Element.Void);
        deathKnockback();
        
        MyVoidField.Deactivate();
        MyVoidField.transform.SetParent(null);
        GameObject.Destroy(MyVoidField.gameObject, 4f);
    
}
    public ParticleSystem TeleportEffect;
    private float lastchargehittime = -10f;
    private void OnCollisionHit(Collider2D collision)
    {
        Astronaut a = collision.gameObject.GetComponent<Astronaut>();
        if ((a != null) && (a.Alive))
        {
            if (!isStunned())
                if ((Time.time - lastchargehittime) >= 1.5f)
                {
                    Vector3 dif = (a.transform.position - this.transform.position);
                    HitsDone += 1f;
                    lastchargehittime = Time.time;
                    if (Rushing && ((Time.time - RushingTime) >= readytime))
                    {
                        Vector3 dire = new Vector3(0f, a.JumpSpeed * 1f*(1f+1f*Astronaut.AggressionLevelF), 0f);
                        if (Mathf.Abs(Vector3.Dot(dif.normalized,Vector3.up)) < .5f)
                            dire = new Vector3(a.JumpSpeed* .5f * (1f + 1f * Astronaut.AggressionLevelF) * Mathf.Sign(RushDirection.x), a.JumpSpeed /8f, 0f);
                        MyVoidField.Deactivate();
                        Am.am.oneshot(Am.am.M.VoidSkeleton_Contact);
                        //ImpactParticles.Play();
                        a.PressParticles.Emit(20);
                        if (a.TakeDamage(30f, dire)) {

                            HitsDone += 4f;
                        } else
                        {
                            //Teleport
                            //Am.am.oneshot(Am.am.VoidWarpSound)
                        }
                    } else if (a.TakeDamage(10f, dif.normalized * 5f + new Vector3(0f, a.JumpSpeed, 0f)))
                    {
                        HitsDone += 4f;
                    }
                    //if (Astronaut.AggressionLevel > 2)
                    //{
                        
                    //} else
                    //{
                        
                    //}

                }

        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        OnCollisionHit(collision.collider);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionHit(collision.collider);
    }
}
