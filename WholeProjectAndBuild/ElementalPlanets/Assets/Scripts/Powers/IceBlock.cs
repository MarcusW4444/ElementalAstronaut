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

    public void Remove()
    {
        ParticleSystem[] ps = this.gameObject.GetComponentsInChildren<ParticleSystem>();
        foreach (ParticleSystem p in ps)
        {
            p.Stop();
            p.transform.SetParent(null);
            GameObject.Destroy(p.gameObject, 5f);

        }
        GameObject.Destroy(this.gameObject);
    }
    
	void Update () {
		

        if ((Time.time - LastTickTime) >= 5f)
        {
            Remove();
            
        }
	}
    private bool beenshattered = false;
    public void shatter()
    {
        if (beenshattered) return;
        //base.Kill();
        beenshattered = true;
        Am.am.oneshot(Am.am.M.chooseSound(Am.am.M.IcicleBreak1, Am.am.M.IcicleBreak2), .5f);
        //if (ShatterParticles)
        //{
        //ShatterParticles.Play(true);
        //ShatterParticles.transform.SetParent(null);
        //GameObject.Destroy(ShatterParticles.gameObject, 10f);
        //}

        Remove();
        //GameObject.Destroy(this.gameObject);

    }
    
}
