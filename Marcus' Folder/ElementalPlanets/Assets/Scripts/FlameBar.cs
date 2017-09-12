using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameBar : MonoBehaviour {

    // Use this for initialization

    public ParticleSystem MyParticles;
    public bool FlameActive = false;
    public Collider2D MyCollider; 
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public ParticleSystem BurningParticles;
    public void OnTriggerStay2D(Collider2D collision)
    {
        if (!FlameActive) return;
        GenericEnemy ge = collision.GetComponent<GenericEnemy>();
        if (ge != null)
        {
            ge.TakeDamage(160f*Time.deltaTime,this.transform.forward*0.001f);
        }
    }
}
