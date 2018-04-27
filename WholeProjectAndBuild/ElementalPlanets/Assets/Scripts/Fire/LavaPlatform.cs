using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaPlatform : MonoBehaviour {

    // Use this for initialization

    public bool Destroyed = false;
    private Vector3 OriginalPosition;
    private Quaternion OriginalRotation;
    public Rigidbody2D MyRigidbody;
    public Collider2D MyCollider;
    private Vector3 OriginalScale;
	void Start () {
        OriginalPosition = this.transform.position;
        OriginalRotation = this.transform.rotation;
        OriginalScale = this.transform.localScale;
	}

    // Update is called once per frame
    private float DestructionTime = -10f;
    private float RestorationTime = -10f;
	void Update () {
		
        if (Destroyed)
        {
            MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
            if ((Time.time - DestructionTime) >= 5f)
            {
                RestorePlatform();
            }

        } else
        {
            float f = ((Time.time - RestorationTime) / 1f);
            MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
            this.transform.position = OriginalPosition;
            this.transform.rotation = OriginalRotation;
            MyRigidbody.velocity = new Vector2();
            MyRigidbody.angularVelocity = 0f;
            if (f < 1f)
            {
                this.transform.localScale = Vector3.Lerp(Vector3.zero,OriginalScale,f);
            } else
            {
                this.transform.localScale = OriginalScale;   
            }
        }

	}
    public void RestorePlatform()
    {
        if (!Destroyed) return;
        Destroyed = false;
        RestorationTime = Time.time;
        MyCollider.enabled = true;
        MyRigidbody.velocity = new Vector2();
        MyRigidbody.angularVelocity = 0f;
        MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
        MyRigidbody.velocity = new Vector2();
        MyRigidbody.angularVelocity = 0f;
        this.transform.localScale = Vector3.one*.0001f;
        this.transform.position = OriginalPosition;
        this.transform.rotation = OriginalRotation;
    }

    public void DestroyPlatform()
    {
        if (Destroyed) return;
        Destroyed = true;
        DestructionTime = Time.time;
        MyCollider.enabled = false;
        MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
        MyRigidbody.velocity = ((Random.insideUnitCircle + new Vector2(0f, 1f))*4f);
        MyRigidbody.angularVelocity = (Random.Range(-100f, 100f));
    }
}
