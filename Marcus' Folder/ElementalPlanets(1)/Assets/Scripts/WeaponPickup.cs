using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour {

    // Use this for initialization
    public Astronaut.SpecialWeapon WeaponType;
	void Start () {
        Live = true;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public bool Live = true;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Live) return;
        Astronaut a = collision.gameObject.GetComponent<Astronaut>();

        if ((a != null) && (a.Alive))
        {
            Live = false;
            a.pickUpWeapon(this.WeaponType);
            GameObject.Destroy(this.gameObject);
        }
    }
}
