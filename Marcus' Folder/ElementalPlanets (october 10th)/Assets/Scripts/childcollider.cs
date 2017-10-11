using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class childcollider : MonoBehaviour {

    // Use this for initialization
    public FlameBar Receiver;
    private void OnTriggerStay2D(Collider2D collision)
    {
        Receiver.OnTriggerStay2D(collision);
    }
}
