using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBreath : MonoBehaviour {



    public ParticleSystem FrostParticles;
    public Collider2D MyCollider;
    public bool BreathActive = false;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
        if (BreathActive != FrostParticles.isPlaying)
        {
            
            if (BreathActive)
                FrostParticles.Play();
            else
                FrostParticles.Stop();
        }
        MyCollider.enabled = BreathActive;




    }
    private const float IceBreathDPS = 20f;
    public void OnTouched(Collider2D collision)
    {
        Astronaut a = collision.gameObject.GetComponent<Astronaut>();
        if ((a != null) && (a.Alive))
        {
            //if ((Time.time - a.lastDamageTakenTime) >= 2f)
            //{
                //Vector3 dif = (a.transform.position - this.transform.position);
                a.TakeDamage(IceBreathDPS*Time.fixedDeltaTime, Vector3.zero);
            //}

        }
    }
}
