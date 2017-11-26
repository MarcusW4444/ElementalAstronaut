using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameBar : MonoBehaviour {

    // Use this for initialization

    public ParticleSystem MyParticles;
    public bool FlameActive = false;
    public Collider2D MyCollider;
    public bool IgniteEnemies = true;
    public bool TerrainScorching = false;
    public bool IncinerateEnemies = false;
    public bool ConvectionField = false;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public ParticleSystem BurningParticles;
    public float FirePowerLevel = 0f;
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
            if (me.MeltParticles != null) me.MeltParticles.Emit(1);
            
            me.TakeDamage((70f+(200f*FirePowerLevel)) * Time.fixedDeltaTime, this.transform.forward * 0.001f);
        }
        BreakableIceWall bw = collision.GetComponent<BreakableIceWall>();
        if (bw != null)
        {
            bw.TakeDamage(40f+(80f*FirePowerLevel) * Time.fixedDeltaTime, this.transform.forward * 0.001f);
        }
        IceShard ic = collision.GetComponent<IceShard>();
        if (ic != null)
        {
            ic.transform.localScale = (ic.transform.localScale * (.5f+(.45f*(1f-FirePowerLevel))));
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
