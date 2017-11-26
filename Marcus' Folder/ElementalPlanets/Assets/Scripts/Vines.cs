using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vines : MonoBehaviour {

    // Use this for initialization
    public bool GrappleOrGrab=true; //True for Grappling, False for Pulling
    public int PullTowardsOrAway=0; // 
    public bool Broken = false;
    public HingeJoint2D MyHingeJoint;
    public SpringJoint2D MySpringJoint;
    public LineRenderer MyLineRenderer;
    public Astronaut MyAstronaut;
    public Rigidbody2D MyRigidbody;
    private void Start()
    {
        Broken = false;
    }
    private void FixedUpdate()
    {

        if (Broken)
        {


        }
        else
        {

            if (GrappleOrGrab)
            {
                //Grapple
                if (PullTowardsOrAway > 0)
                {

                }
                else if (PullTowardsOrAway < 0)
                {

                }
                else
                {

                }
            }
            else
            {
                //Grab
                if (PullTowardsOrAway > 0)
                {

                }
                else if (PullTowardsOrAway < 0)
                {

                }
                else
                {

                }
            }

            Vector3 dif = (this.transform.position - MyAstronaut.transform.position);
            Ray2D r = new Ray2D(MyAstronaut.transform.position,dif.normalized);
            RaycastHit2D rh = Physics2D.Raycast(r.origin, r.direction, dif.magnitude, LayerMask.GetMask(new string[] { "Geometry" }));
            if (rh.collider != null)
            {
                Break();
            }

        }
        MyLineRenderer.enabled = !Broken;
        if (!Broken) {
            MyLineRenderer.SetPositions(new Vector3[]{ MyAstronaut.transform.position, this.transform.position});
        }
    }


    public void Break()
    {
        if (Broken) return;
        Broken = true;
        
    }
}
