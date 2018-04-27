using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour {
    public bool Live = true;
    public float Freezability = 10f;
    public float FreezeFactor = 0f;
    public bool Disabled = false;
    
    public void slowFreeze(float v1)
    {
        FreezeFactor = Mathf.Clamp01(FreezeFactor+(v1*Freezability));
    }
}
