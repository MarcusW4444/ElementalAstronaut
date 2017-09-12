using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlock : MonoBehaviour {

    // Use this for initialization
    public float LastTickTime = -10f;
	void Start () {
        LastTickTime = Time.time;

    }
	
	// Update is called once per frame
	void Update () {
		

        if ((Time.time - LastTickTime) >= 5f)
        {
            ParticleSystem[] ps = this.gameObject.GetComponentsInChildren<ParticleSystem>();
            foreach (ParticleSystem p in ps)
            {
                p.Stop();
                p.transform.SetParent(null);
                GameObject.Destroy(p.gameObject,5f);
            }
            GameObject.Destroy(this.gameObject);
        }
	}
}
