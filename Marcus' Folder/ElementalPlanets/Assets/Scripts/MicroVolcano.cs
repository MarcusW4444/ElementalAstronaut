using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MicroVolcano : Hazard {

    // Use this for initialization
    public bool Erupting = false;
    public bool Telegraphing = false;
    public bool AlwaysErupting = false;
    private float EruptingTime = -10f;
    public float EruptionDuration = 2f;
    public float DelayTime = 0f;
    public bool Delaying = true;
    public float IdleDuration = 3f;
    public float TelegraphDuration = 2f;
    public Collider2D MyCollider, MyTrigger;
    public Eruption MyEruption;
    public Animator MyAnimator;
    public SpriteRenderer MySpriteRenderer;
    public ParticleSystem TelegraphParticles;
    void Start () {

        EruptingTime = Time.time;
        if (Delaying)
        {
            Erupting = false;
        }
    }

    // Update is called once per frame

    void Update() {
        if (!Permafrozen)
        {


            float speedrate = 1f;
            speedrate *= 1f + (3f * Astronaut.AggressionLevelF);

            if (Delaying)
            {
                if ((Time.time - EruptingTime) >= (DelayTime / speedrate))
                {
                    Erupting = true;
                    EruptingTime = Time.time;
                    Delaying = false;
                }
            }
            else
            {

                if (Erupting)
                {

                    if ((Time.time - EruptingTime) >= (EruptionDuration / speedrate))
                    {
                        Erupting = false;
                        EruptingTime = Time.time;
                        Telegraphing = false;
                    }
                }
                else
                {
                    if (!Telegraphing)
                    {
                        if ((Time.time - EruptingTime) >= (IdleDuration / speedrate))
                        {
                            EruptingTime = Time.time;
                            Telegraphing = true;
                            TelegraphParticles.Play();
                        }
                    }
                    else if ((Time.time - EruptingTime) >= (TelegraphDuration / speedrate))
                    {
                        Erupting = true;
                        EruptingTime = Time.time;
                        TelegraphParticles.Stop();
                        Telegraphing = false;
                    }


                }
            }



            bool e = (Erupting || AlwaysErupting);
            MySpriteRenderer.enabled = e;
            MyAnimator.enabled = !Permafrozen;

        } else
        {
            MyAnimator.enabled = false;
            MySpriteRenderer.enabled = false;
            if (TelegraphParticles.isPlaying) TelegraphParticles.Stop();
        }
        //MyAnimator.SetBool("Erupting",e);
	}

    public void OnVolcanoEruptionCollision(Collision2D col)
    {

    }

    public override void permafreezeUnique()
    {
        base.permafreezeUnique();

    }

    public ParticleSystem BurnParticles;
    public void OnVolcanoEruptionTriggered(Collider2D col)
    {
        if (!(Erupting || AlwaysErupting)) return;
        if (Frozen) return;
        Astronaut a = col.gameObject.GetComponent<Astronaut>();
        if ((a != null))
        {
            if (!Frozen)
            {
                if ((Time.time - a.ReviveTime) >= 2f)
                if (a.Alive)
                {
                    Vector3 dif = (a.transform.position - this.transform.position);
                    float dps = (200f * Time.fixedDeltaTime);
                    Vector3 cross = Vector3.Cross(this.transform.up,Vector3.forward);
                    if (a.TakeDamage(dps,cross*0f* 5f*Mathf.Sign(Vector3.Dot(cross,dif))))
                    {
        
                    }
                } else
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

}
