using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour {

    // Use this for initialization
    public float HealthValue = 60f;
	void Start () {
        Live = true;
        
    }
    
    public ParticleSystem GlowEffect;
	

    public bool Live = true;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Live) return;
        Astronaut a = collision.gameObject.GetComponent<Astronaut>();

        if ((a != null) && (a.Alive))
        {
            Live = false;
            a.pickupHealth(this);
            
            GlowEffect.transform.SetParent(null);
            GlowEffect.Stop();
            GameObject.Destroy(GlowEffect.gameObject,3f);
            GameObject.Destroy(this.gameObject);
        }
    }


}
