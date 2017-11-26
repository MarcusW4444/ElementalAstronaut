using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaPillar : Hazard {

    // Use this for initialization
    private float StartTime = -10f;
    public float TelegraphTime = 2f;

    public float PillarDuration = 2f;
    public bool Rising = false;
    public bool Falling = false;
    private float Offset = 0f;
    private float MaxOffset = 7f;
    public ParticleSystem TelegraphParticles,GlowingParticles;
	void Start () {
        StartTime = Time.time;
        Rising = false;
        Falling = false;
        this.transform.localScale = (this.transform.localScale * .5f);

    }

    // Update is called once per frame
    private const float RISESPEED = 10f;
    private bool removed = false;
    void FixedUpdate () {
        float time = (Time.time - StartTime);
        if (Falling)
        {
            Debug.Log("Going Down");
            if (Offset > 0f)
            {
                //Rise
                float df = (Time.fixedDeltaTime * RISESPEED);
                if ((Offset) < df)
                {
                    df = Offset;
                    
                }
                Offset -= df;
                this.transform.position = (this.transform.position - (Vector3.up * df));
                
            }


            if (Offset <= 0f)
            {
                //Orphan the particles.
                if (!removed)
                {
                    removed = true;
                    foreach (ParticleSystem ps in this.GetComponentsInChildren<ParticleSystem>())
                    {
                        ps.Stop(true);
                        ps.transform.SetParent(null);
                        GameObject.Destroy(ps.gameObject, 10f);

                    }
                    
                    GameObject.Destroy(this.gameObject);
                }
            }

        }
        else
        {
            if (Rising)
            {
                if (Offset < MaxOffset)
                {
                    //Rise
                    float df = (Time.fixedDeltaTime * RISESPEED);
                    if ((MaxOffset - Offset) < df)
                    {
                        df = (MaxOffset - Offset);
                        
                    }
                    Offset += df;
                    this.transform.position = this.transform.position + (Vector3.up * df);
                }
                if (time >= PillarDuration)
                {
                    GlowingParticles.Stop();
                    StartTime = Time.time;
                    Falling = true;
                } else
                {
                    if (!GlowingParticles.isPlaying)
                        GlowingParticles.Play();
                }

            }
            else
            {
                if (time >= TelegraphTime)
                {

                    TelegraphParticles.Stop();
                    Rising = true;
                    StartTime = Time.time;
                }
                else
                {
                    if (!TelegraphParticles.isPlaying)
                        TelegraphParticles.Play();
                }
                
            }
        }
	}
    
    public ParticleSystem BurnParticles;
    void OnTriggerStay2D(Collider2D col)
    {
        //if ((!Rising) || Falling) return;
        Astronaut a = col.gameObject.GetComponent<Astronaut>();
        if ((a != null))
        { 
            if (!Frozen)
            {
                //if ((Time.time - a.lastDamageTakenTime) >= 2f)
                if (a.Alive)
                {
                    Vector3 dif = (a.transform.position - this.transform.position);
                    float dps = (100f * Time.fixedDeltaTime);

                    if (a.TakeDamage(dps, new Vector3(Mathf.Sign(dif.x) , 0f, 0f) * 4f))
                    {

                    }
                }
                else
                {
                    //BurnParticles
                }

            }


        }

    }


    public SpriteRenderer FreezeSprite;
    public ParticleSystem FreezeFlashEffect;
    public ParticleSystem MeltParticles;

    public bool Frozen = false;
    private float FreezeTime = -10f;

    public void freeze(float dur)
    {


        FreezeTime = Time.time + dur;
        if (Frozen) return;
        Frozen = true;
        foreach (Animator anim in this.GetComponentsInChildren<Animator>())
        {
            anim.enabled = false; //May not apply to all animators
        }
        FreezeSprite.enabled = true;



    }
    public void unfreeze()
    {
        if (!Frozen) return;
        foreach (Animator anim in this.GetComponentsInChildren<Animator>())
        {
            anim.enabled = false; //May not apply to all animators
        }

        Frozen = false;
        FreezeSprite.enabled = false;

    }

    public void freezeStep()
    {
        if ((Frozen) && ((Time.time - FreezeTime) >= 0f))
        {
            unfreeze();
        }

    }

    public override void permafreezeUnique()
    {
        base.permafreezeUnique();
        foreach (ParticleSystem ps in this.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Stop(true);
            ps.transform.SetParent(null);
            GameObject.Destroy(ps.gameObject, 10f);

        }
        foreach (Collider2D col in this.GetComponentsInChildren<Collider2D>()) {
            col.enabled = false;
        }

        GameObject.Destroy(this.gameObject, 3f);

    }
}
