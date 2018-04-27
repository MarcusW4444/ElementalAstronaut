using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ethereal_Ice_HazardPit : Hazard {

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    public Collider2D MyCollider;
	void Update () {
        if (EtherealManager.TheEtherealManager == null) return;
        MyCollider.enabled = (EtherealManager.TheEtherealManager.EtherealLock && (EtherealManager.TheEtherealManager.EtherealCurrentLevel >= 3));
	}


    private void OnCollision(Collider2D col)
    {
        Astronaut a = col.gameObject.GetComponent<Astronaut>();

        if ((a != null) && (a.Alive))
        {

            if (!Permafrozen)
            {
                a.SendBackToEtherealCheckpoint();
            }
            

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollision(collision.collider);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        OnCollision(collision.collider);
    }


}
