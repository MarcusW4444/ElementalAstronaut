using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderJumpCollider : MonoBehaviour {

    // Use this for initialization
    public Collider2D MyCollider;
	
    private bool tangible = true;
	void FixedUpdate () {
		if (Astronaut.TheAstronaut != null)
        {
            //MyCollider.bounds.max.y;
            //Astronaut.TheAstronaut.MyCollider.bounds.min.y;
            if (tangible)
            {
                if (MyCollider.bounds.max.y > Astronaut.TheAstronaut.MyCollider.bounds.min.y)
                {
                    tangible = false;
                    Physics2D.IgnoreCollision(Astronaut.TheAstronaut.MyCollider, MyCollider, true);
                }
            } else
            {
                if (MyCollider.bounds.max.y < Astronaut.TheAstronaut.MyCollider.bounds.min.y)
                {
                    tangible = true;
                    Physics2D.IgnoreCollision(Astronaut.TheAstronaut.MyCollider, MyCollider, false);
                }
            }
            
        }
	}
}
