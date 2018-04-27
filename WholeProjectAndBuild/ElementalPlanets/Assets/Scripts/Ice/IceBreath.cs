using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBreath : MonoBehaviour {



    public ParticleSystem FrostParticles;
    public ParticleSystem IceBreathHitParticles;
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


        if (!freezehit)
        {
            FreezeFactor = Mathf.Max(FreezeFactor - (Time.deltaTime / 2f), 0f);
        }
        freezehit = false;
    }
    private bool freezehit = false;
    public float FreezeFactor = 0f;
    private const float IceBreathDPS = 40f;
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
            IceBreathHitParticles.transform.position = a.transform.position + ( (a.transform.position - this.transform.position).normalized*.5f);
            IceBreathHitParticles.Emit(1);
            freezehit = true;
            FreezeFactor += Time.deltaTime * 1.2f;
            if (FreezeFactor >= 1f)
            if (a.Alive)
            {
                a.freeze(.5f+(FreezeFactor-1f));
            }
        }
    }
}
