using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingArea : MonoBehaviour {

    // Use this for initialization
    public Collider2D MyCollider;
    public void OnTriggered(Collider2D col)
    {
        Astronaut a = col.GetComponent<Astronaut>();
        if ((a != null))
        {
            a.fallCollision(this,MyCollider);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnTriggered(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnTriggered(other);
    }
}
