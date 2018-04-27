using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Geyser : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame

    //public Steam SteamPrefab;
    public float InitialDelay = 0f;
    public bool NotStopping = false;
    public const float TelegraphDuration = 1.0f;
    public const float SteamingDuration = 2.0f;
    public const float SteamPauseDuration = 2.0f;
    private float SteamTime = -10f;
    public bool CurrentlySteaming = false;
    //public GameObject IceCap
	void Update () {
		
	}

    public bool hazardFrozen = false;
    public void hazardFreeze()
    {

    }
}
