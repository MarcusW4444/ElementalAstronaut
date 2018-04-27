using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ethereal_Ice_ArtificialPlatform : MonoBehaviour {

    // Use this for initialization
    public Vector3 Displacement;
    public bool DisplaceWhenDV = true;
    public bool DV = false;
    public float DetectRange = 5f;
    //private float DisplaceValue = 0f;
    public Collider2D MyCollider;
    public int RequiredEtherealLevel = 2;
    public bool RemoveIfQualified = false; 
    public Vector3 startlocation;
    void Start () {
        startlocation = this.transform.position;
        

    }
	
	// Update is called once per frame
	void FixedUpdate () {
        EtherealManager eth = EtherealManager.TheEtherealManager;
        if (eth == null) return;
        
        if ((Astronaut.TheAstronaut != null) && (Astronaut.TheAstronaut.Alive))
        {

            
            Vector3 dif = (startlocation - Astronaut.TheAstronaut.transform.position);
            if (dif.magnitude < DetectRange)
            {
                DV = true;
            } else
            {
                DV = false;
            }
        }
        if (eth.EtherealCurrentLevel < RequiredEtherealLevel)
        {
            
            DV = (!DV);
        } else
        {
            if (RemoveIfQualified)
            {
                this.gameObject.SetActive(false);
            }
        }
        float sp = 3f * Time.fixedDeltaTime;
        Vector3 loc = startlocation;
        if (DV == DisplaceWhenDV)
        {
            loc = startlocation+Displacement;
        } else
        {
            loc = startlocation;
        }
        Vector3 dif2 = (loc - this.transform.position);

        if (dif2.magnitude < sp)
        {
            this.transform.position = loc;
        } else
        {
            this.transform.position = (this.transform.position+(dif2.normalized*sp));
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(.25f, .25f, 1f);
        
        Vector3 bo = new Vector3(MyCollider.bounds.size.x, MyCollider.bounds.size.y, 1f);
        Gizmos.DrawWireCube(this.transform.position+Displacement, bo);
        

        //Gizmos.DrawLine(new Vector3(this.transform.position.x + .5f, bottomy, this.transform.position.z), new Vector3(this.transform.position.x - .5f, bottomy, this.transform.position.z));
        //Gizmos.DrawLine(new Vector3(this.transform.position.x + .5f, topy, this.transform.position.z), new Vector3(this.transform.position.x - .5f, topy, this.transform.position.z));
        //Gizmos.DrawLine(new Vector3(this.transform.position.x, bottomy, this.transform.position.z), new Vector3(this.transform.position.x, topy, this.transform.position.z));
    }
}
