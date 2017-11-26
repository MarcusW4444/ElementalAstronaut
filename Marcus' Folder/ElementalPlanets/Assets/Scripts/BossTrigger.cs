using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossTrigger : MonoBehaviour {

    public BossGolem BossToActivate;
    // Use this for initialization
    public Collider2D[] InvisibleColliders;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Astronaut a = collision.gameObject.GetComponent<Astronaut>();
        if ((a != null) &&(a.Alive))
        {
            if (!BossToActivate.hasbeenintroduced)
            {
                BossToActivate.introduceBoss();
                foreach (Collider2D c in InvisibleColliders)
                {
                    c.enabled = !c.enabled;
                }
            }
        }
    }
}
