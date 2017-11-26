using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lava : Hazard {


    public ParticleSystem BurnParticles;
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
        
        if ((a != null))
        {


            Vector3 dif = (a.transform.position - this.transform.position);
            BurnParticles.transform.position = collision.contacts[0].point;
            BurnParticles.transform.position = collision.contacts[0].point;
            //BurnParticles.Emit(50);
            float d = 20f;
            if (a.TakeDamage(10f, (a.Health > d) ? new Vector3(a.MyRigidbody.velocity.x, 50f, 0f) : new Vector3(0f,1f,0f)))
            {

                //...
                //...WHAT HAPPENS IF YOU DIE IN LAVA?
            }
            else
            {

            }
            BurnParticles.transform.position = collision.contacts[0].point;
            BurnParticles.Emit(20);

        } else 
        {
            GenericEnemy ge = collision.gameObject.GetComponent<GenericEnemy>();
            if ((ge != null) && (ge))
            {
                ge.Kill();
            }
        }

        
    }
    
}
