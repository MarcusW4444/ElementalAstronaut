using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbingVine : MonoBehaviour {

    // Use this for initialization
    public bool TransitionClimbingVine = false;
    public bool TransitionToBranches = false;

    private void OnTriggered(Collider2D col)
    {
        Astronaut a = col.GetComponent<Astronaut>();
        if ((a != null) && (a.Alive))
        {
            a.vineCollision(this);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggered(collision);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggered(collision);
    }
}
