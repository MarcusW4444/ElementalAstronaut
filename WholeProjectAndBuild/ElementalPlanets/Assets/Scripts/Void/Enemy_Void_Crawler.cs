using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Void_Crawler : GenericEnemy {

    //The Void Crawler can drag you toward danger using its tendrils or pull you towards it in order to drain you
    //It can also attatch to walls and the ceiling (it will teleport back o)

	// Use this for initialization
	void Start () {
        OriginalPosition = this.transform.position;
        OriginalRotation = this.transform.rotation;
        StartPosition = this.transform.position;
        targetacquisition = 0f;
        TendrilOffsets = new Vector3[DragTendrils.Length];
        ManualFlipping = true;
        //AnchoredToVine = false;
        LastAttachedLocation = this.transform.position;
    }
    public ParticleSystem TeleportEffect;
    public const float MoveSpeed = 4f;
    public int MoveDirection = 0;
    public Animator MyAnimator;
    // Update is called once per frame
    public LineRenderer[] DragTendrils;
    public Vector3[] TendrilOffsets;
    public Astronaut DragTarget = null;
    private Vector3 OriginalPosition;
    private Quaternion OriginalRotation;
    public Transform DownVector;
    
    
    private Vector3 StartPosition;
    
    private float targetacquisition = 0f;
    public bool TendrilConnected = true;
    
    void FixedUpdate()
    {

        if (Alive && !isStunned())
        {

            Astronaut plr = Astronaut.TheAstronaut;


            
           





                //If the player is within range

                if ((plr != null) && (plr.Alive) && ((Time.time - plr.ReviveTime) >= 2f) && (!plr.Quelling))
                {
                    Vector3 dif = (plr.transform.position - this.transform.position);

                    tdif = dif;

                    if ((dif.magnitude < 8f) && (Physics2D.Linecast(this.transform.position, plr.transform.position, LayerMask.GetMask(new string[] { "Geometry" })).collider == null))
                        {
                        
                        int d = ((int)Mathf.Sign(Vector3.Dot(this.transform.right, dif.normalized)));

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

                            //acquire the target
                            if (targetacquisition >= 1f)
                            {
                                
                                tendrilAttack(plr);
                                targetacquisition = 0f;
                            } else
                            {
                        float df = Time.fixedDeltaTime;
                        float agg = 1f+(2f*Astronaut.AggressionLevelF);
                        if (!TendrilConnected)
                            df *= .33f;
                        
                            
                            targetacquisition = Mathf.Clamp01(targetacquisition+(df*agg));
                            }
                    
                    float ra = 5f;
                    float ef = 1f;
                    TendrilSpew.transform.localScale = Vector3.Lerp(TendrilSpew.transform.localScale, Vector3.one * ef, .5f);
                    float mg = (dif.magnitude / (.5f * ra*ef));
                            
                            if (mg < 1f)
                            {
                                float dps = 30f*ef;
                                plr.TakeDamage(dps*Time.fixedDeltaTime*(1f-mg),new Vector3());
                                TendrilSpew.Emit(3);
                                
                            }
                        }
                        else
                        {
                    targetacquisition = 0f;
                    DragTarget = null;
                    TendrilConnected = false;
                    NumberOfTendrils = 0;
                            Vector3 du = (this.transform.position - StartPosition);
                            
                            if (du.magnitude > 3f)
                            {
                                //int d = ((int)Mathf.Sign(-du.x));
                                int d = ((int)Mathf.Sign(Vector3.Dot(this.transform.right, dif.normalized)));
                                if (DragTarget == null)MoveDirection = d;
                            }
                        }

                
            }
                else
                {
                if (DragTarget != null) MoveDirection = 0;    //MoveDirection = 0;
                if (Alive)
                {
                    if (Astronaut.TheAstronaut != null)
                    {
                        if (((LastAttachedLocation - this.transform.position).magnitude > 2f))
                        {
                            if (((Time.time - LastResetLocationTime) >= 5f))
                            {
                                ResetLocation();
                                LastResetLocationTime = Time.time;

                            }

                        }

                    }
                }
                 
                }



            





            //if (!Attacking)
            checkForEdge();




            if (MoveDirection != 0)
            {

                
                    MyAnimator.SetBool("Moving", true);
                Vector2 vel = MyRigidbody.velocity;
                Vector3 crs = Vector3.Cross(this.transform.right,Vector3.forward);
                Vector3 vvertical = new Vector3(vel.x, vel.y,0f) *Vector3.Dot(crs,vel);
                MyRigidbody.velocity = ((this.transform.right * MoveSpeed * (1f-FreezeFactor)* MoveDirection * (.75f + (.75f * Astronaut.AggressionLevelF))) + vvertical);//new Vector2(, MyRigidbody.velocity.y);

                int se = (int)Mathf.Sign(-MoveDirection);
                if ((DragTarget != null) && false) {
                    int su = (int)Mathf.Sign(-(Vector3.Dot(this.transform.right, new Vector3(DragTarget.transform.position.x, DragTarget.transform.position.y,0f))));
                    this.transform.localScale = new Vector3(Mathf.Abs(this.transform.localScale.x) * su, this.transform.localScale.y, this.transform.localScale.z);
                    MyAnimator.SetBool("Pulling", (se != su));
                } else
                {
                    this.transform.localScale = new Vector3(Mathf.Abs(this.transform.localScale.x) * se, this.transform.localScale.y, this.transform.localScale.z);
                    
                        MyAnimator.SetBool("Pulling", false);
                    
                }
                

            }
            else
            {
                
                    //MyRigidbody.velocity = new Vector2(0f, MyRigidbody.velocity.y);

                    MyAnimator.SetBool("Moving", false);
                MyAnimator.SetBool("Pulling", false);
            }

        }
        else
        {
            TendrilConnected = false;
            MoveDirection = 0;
            MyAnimator.SetBool("Moving", false);
            
        }

        
        if (TendrilConnected && (DragTarget != null) &&(DragTarget.Alive) && (Physics2D.Linecast(this.transform.position, DragTarget.transform.position, LayerMask.GetMask(new string[] { "Geometry" })).collider == null))
        {
            
            TendrilSpew.transform.LookAt(DragTarget.transform.position);
            Vector3 dif = (DragTarget.transform.position - TendrilSpawnLocation.position);
            float pullspeed = ((12f * (1f + (1f * Astronaut.AggressionLevelF))))*((1.0f*NumberOfTendrils)/4f);
            MoveDirection = 0;
            if ((DragTarget.MyRigidbody.bodyType == RigidbodyType2D.Dynamic) && (DragTarget.MyRigidbody.simulated))
            {
                DragTarget.transform.position = (DragTarget.transform.position - (new Vector3(dif.normalized.x, dif.normalized.y, 0f) * pullspeed * Time.fixedDeltaTime));
                //AnchoredToVine = true;
            } else
            {
                //AnchoredToVine = false;
                TendrilConnected = false;
                DragTarget = null;  
            }
        } else
        {
            TendrilConnected = false;
            DragTarget = null;
        }
            for (int i = 0; i < DragTendrils.Length; i++)
            {
                if ((NumberOfTendrils >= (i+1)) && (TendrilConnected) && (DragTarget != null))
                {
                    DragTendrils[i].enabled = true;
                DragTendrils[i].useWorldSpace = true;
                DragTendrils[i].SetPositions(new Vector3[2] { TendrilSpawnLocation.position, DragTarget.transform.position + TendrilOffsets[i]});
                } else
                {
                    DragTendrils[i].enabled = false;
                }
            }
        


    }
    private float LastResetLocationTime = -10f;
    public void ResetLocation()
    {
        Debug.Log("Crawler return");
        MoveDirection = 0;
        TeleportEffect.transform.position = this.transform.position;
        TeleportEffect.Emit(10);
        TeleportEffect.transform.position = OriginalPosition;
        TeleportEffect.Emit(10);
        VoidField vf = GameObject.Instantiate<VoidField>(VoidExplosion, this.transform.position, VoidExplosion.transform.rotation);
        vf.Duration = .5f;
        this.transform.SetPositionAndRotation(OriginalPosition,OriginalRotation);
        vf = GameObject.Instantiate<VoidField>(VoidExplosion, OriginalPosition, VoidExplosion.transform.rotation);
        vf.Duration = .5f;
        

        //Teleport sound
    }
    public override void onIncinerated()
    {
        base.onIncinerated();
        MyRigidbody.velocity = (new Vector2(this.transform.up.x, this.transform.up.y).normalized *20f);
        //Move based on your attachment direction
    }
    private int NumberOfTendrils = 0;
    public void tendrilAttack(Astronaut target)
    {
        DragTarget = target;
        Debug.Log("Tendril Attack");
        if (TendrilConnected)
        {
            if (NumberOfTendrils < 4)
            {
                TendrilOffsets[NumberOfTendrils] = new Vector3(target.MyCollider.bounds.size.x*(Random.value-.5f), target.MyCollider.bounds.size.y * (Random.value - .5f), 0f); 
                NumberOfTendrils = NumberOfTendrils + 1;

            }
            
        } else
        {
            Debug.Log("Tendril Connected");
            TendrilConnected = true;
            TendrilOffsets[NumberOfTendrils] = new Vector3(target.MyCollider.bounds.size.x * (Random.value - .5f), target.MyCollider.bounds.size.y * (Random.value - .5f), 0f);
            NumberOfTendrils = 1;
        }
        

    }

    public void checkForEdge()
    {
        if (MoveDirection != 0)
        {
            RaycastHit2D rh = Physics2D.Linecast(this.transform.position + (this.transform.right*MoveDirection*2f), this.transform.position + (this.transform.right*MoveDirection*2f)+(this.transform.up*-3f), LayerMask.GetMask(new string[] { "Geometry" }));
            if (rh.collider == null)
            {
                //No collision here. please do not walk over the edge
                MoveDirection = 0;
            }
        }
        RaycastHit2D rhe = Physics2D.Linecast(this.transform.position, this.transform.position + (this.transform.up * -1f), LayerMask.GetMask(new string[] { "Geometry" }));
        if (rhe.collider != null)
        {
            LastAttachedLocation = this.transform.position;
        } else
        {
            if (((Time.time - LastResetLocationTime) >= 3f))
            {
                ResetLocation();
                LastResetLocationTime = Time.time;

            }//this.transform.rotation = new Quaternion();
        }
    }
    public Vector3 LastAttachedLocation;
    public ParticleSystem TendrilSpew, LeechParticles;
    public Transform TendrilSpawnLocation;
    public override void Kill()
    {
        if (!Alive) return;
        Alive = false;
        //ReleaseSpores(false);
        //ReleaseSpores(false);
        Astronaut.PlayKillSound(1);
        deathKnockback();
        Astronaut.TheAstronaut.dropResistance(.4f, this.transform.position, Astronaut.Element.Void);
        if (Astronaut.AggressionLevel > 1)
        {
            VoidField vf = GameObject.Instantiate<VoidField>(VoidExplosion, this.transform.position, VoidExplosion.transform.rotation);
            vf.Duration = (2f + (4f * Astronaut.AggressionLevelF));
        }
    }
    public VoidField VoidExplosion;

    private float jumpe = 0f;
    private float passtime = -10f;
    private Vector3 tdif = new Vector3();
}
