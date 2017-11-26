using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericMovingEnemy : GenericEnemy {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		

        if (Alive && !isStunned())
        {
            float movedir = 0f;
            float movespeed = 5f;
            float t = ((Time.time % 3f) / 3f);
            if (t <= .5f)
            {
                movedir = 1;
            } else {
                movedir = -1;
            }
            MyRigidbody.velocity = new Vector2(movedir* movespeed, MyRigidbody.velocity.y);
        }
	}

    public override void Kill()
    {
        if (!Alive) return;
        Alive = false;
        //base.Kill();
        deathKnockback();
    }
}
