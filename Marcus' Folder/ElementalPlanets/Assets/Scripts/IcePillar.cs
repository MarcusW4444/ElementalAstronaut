using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePillar : MonoBehaviour {

    // Use this for initialization
    private float StartTime = -10f;
	void Start () {
        StartTime = Time.time;
		
	}

    // Update is called once per frame
    private bool Removing = false;
    public float Lifetime = 5f;
	void Update () {
		
        if (Removing)
        {
            
        } else
        {

            if ((Time.time-StartTime) >= Lifetime)
            {
                Remove();
            }
        }
	}
    public ParticleSystem ShatterParticles;
    public void Shatter()
    {
        //Play the Icicle breaking sound
        if (Removing) return;
        if (ShatterParticles != null)
            ShatterParticles.Play();

        Remove();
        //GameObject.Destroy(this.gameObject);

    }
    public void Remove()
    {
        if (Removing) return;
        Removing = true;
        foreach (ParticleSystem p in this.GetComponentsInChildren<ParticleSystem>())
        {
            if (!p.transform.Equals(this.transform))
            {
                p.transform.SetParent(null);
                if (p.main.loop)
                    p.Stop();


            }
            else
            {
                //p.enabled = false;
                p.Stop();
            }
            GameObject.Destroy(p.gameObject, p.main.duration);
        }
        GameObject.Destroy(this.gameObject);
    }
}
