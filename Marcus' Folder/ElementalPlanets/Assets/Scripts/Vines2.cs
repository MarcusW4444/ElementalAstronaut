using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vines2 : MonoBehaviour {

    // Use this for initialization
    public LineRenderer MyLineRenderer;
    public Astronaut MyAstronaut;
    public float TravelDistance = 0f;
    public float MaxTravelDistance = 10f;
    public float TravelSpeed = 30f;
    public float RetractionRate = 80f;
    private Vector3 startPosition=new Vector3();
    public Vector3 VinePosition;
    public Vector3 VineAnchorOffset;
    public Vector3 TravelDirection;
    public GameObject VineAttachedToSolid=null;
    public GenericEnemy VineAttachedToEnemy = null;
    public bool Sustaining = true;
    public bool Attached = false;
    public bool Retracting = false;
    public int VitaPowerLevel = 0;

    
    void Start () {
        TravelDistance = 0f;
        startPosition = this.transform.position;
        VinePosition = this.transform.position;
        VineAttachedToSolid = null;
        VineAttachedToEnemy = null;
    }

    // Update is called once per frame

    public bool GrappleOnWalls = false;
    public bool ControlPush = false;
    public bool PushesEnemies = false;
    public bool ConstrictsEnemies = false;
    public bool EnergyDrain = false;
    public bool ClusterConstriction = false;
    public Vines2 ClusterVineParent = null;
    public List<Vines2> MyClusterVineChildren = new List<Vines2>();
    private Vector3 lastvictimpressposition = new Vector3();
	void FixedUpdate () {


        if (Retracting)
        {


            TravelDistance = Mathf.Max(0f, TravelDistance- (RetractionRate * Time.deltaTime));

            if (TravelDistance <= 0f)
            {
                GameObject.Destroy(this.gameObject);
                return;
            }

            VinePosition = Vector3.Lerp(MyAstronaut.transform.position,VinePosition, (TravelDistance / MaxTravelDistance));
            Attached = false;
        } else
        {
            
            if (Attached)
            {
                //Retracting = true;

                if (VineAttachedToSolid != null)
                {
                    Vector3 dif = (VineAnchorOffset - MyAstronaut.transform.position);
                    //MyAstronaut.MyRigidbody.AddForce(dif * MyAstronaut.MyRigidbody.velocity.magnitude * MyAstronaut.MyRigidbody.mass * Vector3.Dot(MyAstronaut.MyRigidbody.velocity.normalized,-dif.normalized) *1f); 

                    float towards = Vector3.Dot(MyAstronaut.MyRigidbody.velocity.normalized, -dif.normalized);
                    //if (dif.y > 0f)
                        MyAstronaut.MyRigidbody.AddForce(dif.normalized*(50f) * MyAstronaut.MyRigidbody.mass);
                    //if (dif.y > 0f)
                    MyAstronaut.MyRigidbody.AddForce((((Vector3.Dot(dif.normalized,Vector3.up)+1f)/2f))*Vector2.up* MyAstronaut.MyRigidbody.mass * 70f*(1f/(0.1f+Mathf.Max(Mathf.Abs(dif.x),0.1f))));
                    float speed = MyAstronaut.MyRigidbody.velocity.magnitude;

                    //float closedist = 5f;

                    //Pull Up
                    //Vector3 cross = Vector3.Cross(dif.normalized,Vector3.forward);
                    //MyAstronaut.MyRigidbody.AddForce(MyAstronaut.MyRigidbody.mass * speed*10f*Vector3.Lerp(Vector3.up ,dif.normalized,Mathf.Clamp01((dif.magnitude)/closedist)));
                    VinePosition = VineAnchorOffset;

                    //How high in what sense? Units? 
                    //Maybe you could have high cliffs inside the ice caves?
                    //I think he should. Let me add that..

                } else if ((VineAttachedToEnemy != null) && (VineAttachedToEnemy.Alive)) {
                    Vector3 dif = (VineAttachedToEnemy.transform.position - MyAstronaut.transform.position);
                    VinePosition = (VineAttachedToEnemy.transform.position+ VineAnchorOffset);
                    //?
                    //MyAstronaut.MyRigidbody.AddForce(dif * MyAstronaut.MyRigidbody.mass * 10f);//Pull yourself towards an enemy
                    float stopthresh = 2f;
                    float delt = (Time.fixedDeltaTime * 10f);
                    float pushdps = 100f;
                    if (ControlPush)
                    {
                        VineAttachedToEnemy.MyRigidbody.AddForce(dif * VineAttachedToEnemy.MyRigidbody.mass * 10f);//Pull the enemy towards you
                        Vector3 difpress = (lastvictimpressposition - VineAttachedToEnemy.transform.position);
                        float d = (difpress.magnitude/(5f*Time.fixedDeltaTime));
                        lastvictimpressposition = VineAttachedToEnemy.transform.position;
                        if (d < 1f)
                        {
                            
                            VineAttachedToEnemy.TakeDamage(Time.fixedDeltaTime* pushdps*(1f-d), new Vector2());
                        }
                        
                    }
                    else
                    {
                        if (dif.magnitude < (stopthresh + delt))
                        {
                            //VineAttachedToEnemy.MyRigidbody.AddForce(-VineAttachedToEnemy.MyRigidbody.velocity * VineAttachedToEnemy.MyRigidbody.mass * 10f);
                            if (dif.magnitude > stopthresh)
                                VineAttachedToEnemy.MyRigidbody.MovePosition(VineAttachedToEnemy.MyRigidbody.position + (-new Vector2(dif.x, dif.y).normalized * (dif.magnitude - stopthresh)));
                        }
                        else
                        {
                            //VineAttachedToEnemy.MyRigidbody.AddForce(-dif * VineAttachedToEnemy.MyRigidbody.mass * 10f);//Pull the enemy towards you
                            VineAttachedToEnemy.MyRigidbody.MovePosition(VineAttachedToEnemy.MyRigidbody.position + (-new Vector2(dif.x, dif.y).normalized * delt));
                        }
                    }
                    VineAttachedToEnemy.StunTime = Mathf.Max(VineAttachedToEnemy.StunTime,Time.time+.4f);
                } else
                {
                    Retracting = false;

                }
                if (!Sustaining)
                {
                    MyAstronaut.UsedDoubleJump = false;
                    Retracting = true;
                    Attached = false;
                    
                    VineAttachedToSolid = null;
                    VineAttachedToEnemy = null;
                }

            }
            else
            {

                float delta = (TravelSpeed * Time.deltaTime);
                float d = (TravelDistance + delta);
                bool ending = false;
                
                if (d >= MaxTravelDistance)
                {
                    d = (MaxTravelDistance - TravelDistance);
                    ending = true;
                }
                else
                {
                    d = delta;
                }
                TravelDistance = (TravelDistance + d);


                Ray2D r = new Ray2D(VinePosition, TravelDirection.normalized);

                
                RaycastHit2D rh = Physics2D.Raycast(r.origin, r.direction, d, LayerMask.GetMask(new string[] { "Geometry","Enemy"}));
                if (rh.collider != null)
                {
                    GenericEnemy en = rh.collider.gameObject.GetComponentInParent<GenericEnemy>();
                    if ((en != null) &&(en.Alive))
                    {
                        Attached = true;
                        VineAnchorOffset = (new Vector3(rh.point.x, rh.point.y, 0f) - en.transform.position);
                        VineAttachedToEnemy = en;
                        lastvictimpressposition = en.transform.position;
                        VineAttachedToSolid = null;
                    }
                    else
                    {
                        
                        Attached = true;
                        VineAnchorOffset = (new Vector3(rh.point.x, rh.point.y, 0f));

                        VineAttachedToSolid = rh.collider.gameObject;
                        VineAttachedToEnemy = null;
                    }


                }
                else
                {
                    //Miss.
                    if (!Sustaining)
                    {
                        Retracting = true;
                    }
                }

                if ((!Attached) && (ending))
                {
                    Retracting = true;
                    
                }

                
                
                VinePosition = startPosition + (TravelDirection * TravelDistance);
            }
        }
        
        
        
        if (MyAstronaut != null)
        {
            MyLineRenderer.SetPosition(1, MyAstronaut.transform.position);
        }
        MyLineRenderer.SetPosition(0, VinePosition);
    }
}
