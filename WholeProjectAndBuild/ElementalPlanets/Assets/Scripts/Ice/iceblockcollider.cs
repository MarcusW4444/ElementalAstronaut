using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class iceblockcollider : MonoBehaviour {

    // Use this for initialization
    public IceBlock MyParentIceBlock;
    private void OnCollisionStay2D(Collision2D collision)
    {

        Astronaut a = collision.gameObject.GetComponent<Astronaut>();
        if ((a != null))
        {
            float dot = (Vector2.Dot(-collision.contacts[0].normal, Vector2.up));
            Debug.Log(dot);
            if (dot < .25f)
            {
                MyParentIceBlock.shatter();
            }

        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionStay2D(collision);
    }
}
