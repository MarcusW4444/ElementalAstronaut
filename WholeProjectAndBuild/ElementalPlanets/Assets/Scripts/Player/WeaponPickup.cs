using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickup : MonoBehaviour {

    // Use this for initialization
    public Astronaut.SpecialWeapon WeaponType;
	void Start () {
        Live = true;
        StartPos = this.transform.position;
    }
    private Vector3 StartPos;
    public ParticleSystem GlowEffect;
	void Update () {
        this.transform.position = StartPos + (new Vector3(0f,.2f,0f) * Mathf.Sin(Mathf.PI * (Time.time / .75f)));

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
            //a.setWeapon(this.WeaponType);
            GlowEffect.transform.SetParent(null);
            GlowEffect.Stop();
            GameObject.Destroy(GlowEffect.gameObject,3f);
            GameObject.Destroy(this.gameObject);
        }
    }


}
