﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSkullProjectile : MonoBehaviour {

    // Use this for initialization
    private float StartTime = -10f;
	void Start () {
        StartTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (Live)
        {
            if ((Time.time - StartTime) >= 3f)
            {
                
                explode();
            }
        }
	}
    public Rigidbody2D MyRigidbody;
    public ParticleSystem ParticleTrail, ParticleExplosion;
    public void Remove()
    {

        ParticleExplosion.transform.SetParent(null);
        //ParticleExplosion.Stop();
        GameObject.Destroy(ParticleExplosion.gameObject, 5f);

        ParticleTrail.transform.SetParent(null);
        ParticleTrail.Stop();
        GameObject.Destroy(ParticleTrail.gameObject,5f);

        GameObject.Destroy(this.gameObject);
    }
    public void explode()
    {
        if (!Live) return;
        Live = false;
        
        ParticleExplosion.Play();
        Remove();
    }
    public bool Live = true;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Astronaut plr = collision.GetComponent<Astronaut>();

        if ((plr != null) && (plr.Alive) && (!plr.Invulnerable) && (Live))
        {
            
            Vector3 dif = (plr.transform.position - this.transform.position);
            plr.TakeDamage(30f, dif.normalized * 10f + new Vector3(0f, plr.JumpSpeed/4f, 0f));
            explode();
        } else if ((collision.gameObject.CompareTag("Geometry")) || (collision.gameObject.GetComponent<IceBlock>() != null))
        {

            explode();
        }
    }
    
}