using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBeam : MonoBehaviour {

    // Use this for initialization
    public LineRenderer MyBeamRenderer;

    public ParticleSystem HitDamageParticles, FastParticles, GlowParticles;
    
    public Transform ParticleCatcher;
    public bool Active = false;
    
    public ParticleSystem ShootParticles;
    public ParticleSystem HitGeometryParticles;
    public ParticleSystem FreezingParticles;
}
