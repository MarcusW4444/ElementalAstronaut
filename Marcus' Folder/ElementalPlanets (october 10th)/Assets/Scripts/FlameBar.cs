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
            ge.TakeDamage(160f*Time.fixedDeltaTime, this.transform.forward*0.001f);
        }
        MeltableIceWall me = collision.GetComponent<MeltableIceWall>();
        if (me != null)
        {
            me.TakeDamage(220f * Time.fixedDeltaTime, this.transform.forward * 0.001f);
        }
        BreakableIceWall bw = collision.GetComponent<BreakableIceWall>();
        if (bw != null)
        {
            bw.TakeDamage(70f * Time.fixedDeltaTime, this.transform.forward * 0.001f);
        }
        IceShard ic = collision.GetComponent<IceShard>();
        if (ic != null)
        {
            ic.transform.localScale = (ic.transform.localScale * .5f);
            if (ic.transform.localScale.magnitude <= .1f)
                GameObject.Destroy(ic.gameObject);
        }

        IceBlock ib = collision.GetComponent<IceBlock>();
        if (ib != null)
        {
            ib.transform.localScale = (ib.transform.localScale * .5f);
            if (ib.transform.localScale.magnitude <= .1f)
                GameObject.Destroy(ic.gameObject);
        }

    }
}
