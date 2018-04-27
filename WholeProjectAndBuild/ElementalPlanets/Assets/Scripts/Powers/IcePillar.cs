using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IcePillar : GenericEnemy {

    // Use this for initialization
    private float StartTime = -10f;
	void Start () {
        StartTime = Time.time;
        if (Formulating)
            Am.am.oneshot(Am.am.M.chooseSound(Am.am.M.FreezeSound1, Am.am.M.FreezeSound2, Am.am.M.FreezeSound3, Am.am.M.FreezeSound4),.5f);
        else 
        Am.am.sound(Am.am.M.MakeIcePillar);
    }

    // Update is called once per frame
    private bool Removing = false;
    public float Lifetime = 5f;
    public bool FreezeOnForm = false;
    public float FormulationScale = 1f;
    public float FormulationHeight = 1f;
    public float FormulationDuration = .25f;

    void Update () {
		
        if (Removing)
        {
            float sc = Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z);
            float eu = ((sc - Time.deltaTime) / sc);
            if (eu <= 0f)
            {
                GameObject.Destroy(this.gameObject);
            } else
            {
                transform.localScale = transform.localScale * eu;
            }
            
        } else
        {

            if (Formulating)
            {
                float eu = Mathf.Clamp01((Time.time - StartTime) / FormulationDuration);

                if (FreezeOnForm)
                {
                    if (Astronaut.TheAstronaut.Alive && !Astronaut.TheAstronaut.Frozen)
                    if (((Astronaut.TheAstronaut.transform.position - this.transform.position).magnitude < 1.25f))
                    {
                            Astronaut.TheAstronaut.freeze(1.1f);
                    }
                }
                transform.localScale = (new Vector3(1f, FormulationHeight,1f) * FormulationScale *eu);
                if (eu >= 1f)
                {
                    Formulating = false;
                }
            }
            if ((Time.time-StartTime) >= Lifetime)
            {
                Remove();
            }

        }
	}
    public bool Formulating = false;
    private float FormulationValue = -10f;
    public ParticleSystem ShatterParticles;
    public void Shatter()
    {
        //Play the Icicle breaking sound
        if (Removing) return;
        if (ShatterParticles != null)
            ShatterParticles.Play();
        ShatterParticles.transform.SetParent(null);
        GameObject.Destroy(ShatterParticles.gameObject, 10f);
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
        Collider2D col = this.GetComponent<Collider2D>();
        if (col != null)
        {
            col.enabled = false;
        }
        
    }
    
    public override void Kill()
    {
        //base.Kill();

        Am.am.oneshot(Am.am.M.chooseSound(Am.am.M.IcicleBreak1, Am.am.M.IcicleBreak2), .5f);
        if (ShatterParticles)
        {
            ShatterParticles.Play(true);
            ShatterParticles.transform.SetParent(null);
            GameObject.Destroy(ShatterParticles.gameObject,10f);
        }

        GameObject.Destroy(this.gameObject);

    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        Astronaut a = collision.gameObject.GetComponent<Astronaut>();
        if ((a != null) && (a.Alive))
        {
            if (this.Alive)
            {
                Kill();
            }

        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionStay2D(collision);
    }
}
