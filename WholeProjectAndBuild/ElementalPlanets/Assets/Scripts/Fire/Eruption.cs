using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eruption : MonoBehaviour {

    // Use this for initialization
    public MicroVolcano MyMicroVolcano;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        MyMicroVolcano.OnVolcanoEruptionCollision(collision);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        MyMicroVolcano.OnVolcanoEruptionTriggered(collision);
    }
}
