using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityTrigger : MonoBehaviour {

    // Use this for initialization
    public CapsuleCollider2D MyVelocityCollider;


    private Vector3 lastposition;
	void Start () {
        lastposition = this.transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        Vector3 dif = (this.transform.position - lastposition);
        MyVelocityCollider.offset = new Vector2((dif.magnitude- MyVelocityCollider.size.y)/2f, 0f);
        MyVelocityCollider.size = new Vector2(dif.magnitude, MyVelocityCollider.size.y);
        lastposition = this.transform.position;
        
        Vector3 diff = this.transform.position - lastposition;
        if (diff.magnitude > 0f)
        {
            diff.Normalize();

            float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot_z - 90);
        }
    }
}
