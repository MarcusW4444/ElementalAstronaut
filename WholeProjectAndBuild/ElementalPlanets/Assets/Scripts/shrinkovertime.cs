using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shrinkovertime : MonoBehaviour {

    // Use this for initialization
    public float Duration = 2f;
    public float Delay = 2f;
    private Vector3 originalsize;
	void Start () {
        originalsize = this.transform.localScale;

        StartTime = Time.time;
    }
    public float StartTime = -10f;	
	// Update is called once per frame
	void Update () {
        float t = (Time.time - StartTime);
        if (t > Delay)
        {
            t = ((t - Delay) / Duration);

            if (t >= 1f)
            {
                GameObject.Destroy(this.gameObject);
            } else
            {
                this.transform.localScale = (originalsize * (1f - t));
            }

        }
		
	}
}
