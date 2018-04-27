using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombustionField : MonoBehaviour {

    // Use this for initialization
    private float StartTime = -10f;
	void Start () {
        StartTime = Time.time;
        Exploded = false;

    }

    public float Delay = 0f;
    public float SparkleTime = 2f;
    // Update is called once per frame
    private bool IsSparkling = false;
    private bool Exploded = false;
    public ParticleSystem SparkleParticles, ExplosionParticles,WarningFlash;
    private bool HasFlashed = false;

	void Update () {
		if ((Time.time - StartTime) >= Delay)
        {
            if (!IsSparkling)
            {
                SparkleParticles.Play(true);
                IsSparkling = true;
            }
        }
        if (!HasFlashed)
        {
            if ((Time.time - (StartTime + Delay)) >= (SparkleTime-.5f))
            {
                HasFlashed = true;
                if (WarningFlash)
                    WarningFlash.Play(true);
            }
        }
        if (!Exploded)
        if ((Time.time - (StartTime+Delay))>=(SparkleTime))
        {
            Explode();
        }
	}
    public float BlastRadius = 4f;
    public void Explode()
    {
        if (Exploded) return;
        Exploded = true;
        ExplosionParticles.Play(true);
        SparkleParticles.Stop();
        Astronaut plr = Astronaut.TheAstronaut;
        Am.am.oneshot(Am.am.M.VoidFireBallExplosion);

        //if (Alive && !isStunned())
        //{
        Vector3 dif = (this.transform.position - plr.transform.position);
        float sc = 1f;

        float radius = BlastRadius;//(2f * sc);//*pre
        float dist = (dif.magnitude / radius);
        if (dist < 1f)
        {


            //if ((Time.time - plr.lastDamageTakenTime) >= 1.5f)
            //{
            Vector3 diff = (plr.transform.position - this.transform.position);
            
            float df = (1f - dist);
            //df = Mathf.Pow(df, 1f/(1f + (1f * Astronaut.AggressionLevelF)));
            if (plr.TakeDamage(20f * df, new Vector3(0f, plr.JumpSpeed *.25f* df, 0f)))
            {
                
            }
            Am.am.oneshot(Am.am.M.LavaBurn);

            //}
        }

        //}



    

        Remove();
    }

    public void Remove()
    {
        foreach (ParticleSystem ps in this.gameObject.GetComponentsInChildren<ParticleSystem>())
        {
            //ps.Stop(true);
            ps.gameObject.transform.SetParent(null);
            GameObject.Destroy(ps.gameObject,10f);
        }
        GameObject.Destroy(this.gameObject);
    }
}
