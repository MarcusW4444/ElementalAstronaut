﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour {

    // Use this for initialization
    public BossGolem MyBoss;
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}



    private void OnTriggerStay2D(Collider2D collider)
    {

        MyBoss.onSlashed(collider);

        

    }
}
