using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeam : MonoBehaviour {

    // Use this for initialization
    public ParticleSystem HitDamageParticles, FastParticles, GlowParticles;
    public LineRenderer MyLineRenderer;
    public Transform ParticleCatcher;

    public bool Active = false;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
