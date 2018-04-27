using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : Hazard {


    public ParticleSystem BurnParticles;
    public void Start()
    {
        PermafreezeSprite = null;
        OnParticleEffectLevelChanged();
    }
    bool prevparticlelowlevel = false;
    public void OnParticleEffectLevelChanged()
    {
        if (prevparticlelowlevel != GameManager.TheGameManager.UsingLowParticleEffects)
        {
            //*((!prevparticlelowlevel)?(1f/2f):2f)
            //reduce or restore particle emission/duration/size
            //
            foreach (ParticleSystem ps in this.GetComponentsInChildren<ParticleSystem>())
            {
                ParticleSystem.EmissionModule e = ps.emission;
                e.rateOverTimeMultiplier = (e.rateOverTimeMultiplier * ((!prevparticlelowlevel) ? (1f / 8f) : 8f));
                //e.rateOverDistanceMultiplier = (e.rateOverDistanceMultiplier * ((!prevparticlelowlevel) ? (1f / 4f) : 4f));
                
            }

            prevparticlelowlevel = GameManager.TheGameManager.UsingLowParticleEffects;
        }

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        Astronaut a = collision.gameObject.GetComponent<Astronaut>();
        
        if ((a != null) && (a.Alive))
        {
            
                //if ((Time.time - a.lastDamageTakenTime) >= 2f)
                //{
                    
            
                //}

        }
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        Astronaut a = collision.gameObject.GetComponent<Astronaut>();
        if (HazardActive)
        if ((a != null))
        {


            Vector3 dif = (a.transform.position - this.transform.position);
            BurnParticles.transform.position = collision.contacts[0].point;
            BurnParticles.transform.position = collision.contacts[0].point;
            //BurnParticles.Emit(50);
            float d = 25f;
            if (a.Alive)Am.am.oneshot(Am.am.M.LavaBurn);
            if (a.TakeDamage(d, (a.Health > d) ? new Vector3(Mathf.Abs(a.MyRigidbody.velocity.x)* Mathf.Sign(-(a.transform.position-a.LastGroundedLocation).x), 10f, 0f) : new Vector3(0f,1f,0f)))
            {

                    //...
                    //...WHAT HAPPENS IF YOU DIE IN LAVA?
                    //You teleport
                    a.transform.position = a.LastGroundedLocation;
                    a.MyRigidbody.velocity = new Vector2();
            }
            else
            {

            }
            BurnParticles.transform.position = collision.contacts[0].point;
            BurnParticles.Emit(20/(prevparticlelowlevel?4:1));

        } else 
        {

            GenericEnemy ge = collision.gameObject.GetComponent<GenericEnemy>();
            if ((ge != null) && (ge.Alive))
            {
                    Am.am.oneshot(Am.am.M.LavaBurn);
                    ge.Kill();
            }
        }

        
    }
    
}
