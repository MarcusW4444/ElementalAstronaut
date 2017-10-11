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
    
    void Start () {
        TravelDistance = 0f;
        startPosition = this.transform.position;
        VinePosition = this.transform.position;
        VineAttachedToSolid = null;
        VineAttachedToEnemy = null;
    }
	
	// Update is called once per frame

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
                    MyAstronaut.MyRigidbody.AddForce((((Vector3.Dot(dif.normalized,Vector3.up)+1f)/2f))*Vector2.up* MyAstronaut.MyRigidbody.mass * 50f*(10f/(0.1f+Mathf.Max(Mathf.Abs(dif.x),0.001f))));
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
                    VineAttachedToEnemy.MyRigidbody.AddForce(-dif * VineAttachedToEnemy.MyRigidbody.mass * 10f);//Pull the enemy towards you
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
            MyLineRenderer.SetPosition(0, MyAstronaut.transform.position);
        }
        MyLineRenderer.SetPosition(1, VinePosition);
    }
}
