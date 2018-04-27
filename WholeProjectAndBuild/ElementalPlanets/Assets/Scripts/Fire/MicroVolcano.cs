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
    public AudioSource TelegraphSound, VolcanoBlowingSound;
    void Start () {

        EruptingTime = Time.time;
        if (Delaying)
        {
            Erupting = false;
        }

        TelegraphSound = AudioManager.AM.createAudioSource(Am.am.M.VolcanoTelegraph, this.gameObject, new Vector3(0f, 0f, 0f), Am.am.M.PlayerAudioMixer, 0f, 1f, true);//createGeneralAudioSource(AudioManager.AM.VolcanoTelegraph, AudioManager.AM.PlayerAudioMixer, 0f, 1f, true);
        VolcanoBlowingSound = AudioManager.AM.createAudioSource(Am.am.M.VolcanoBlowing, this.gameObject, new Vector3(0f, 0f, 0f), Am.am.M.PlayerAudioMixer, 0f, 1f, true);//AudioManager.AM.createGeneralAudioSource(AudioManager.AM.VolcanoBlowing, AudioManager.AM.PlayerAudioMixer, 0f, 1f, true);
        TelegraphSound.Play();
        VolcanoBlowingSound.Play();


        // TheSoundSource.Play();
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
                    Am.am.M.crossfade(VolcanoBlowingSound, 10f, .1f);
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
                        TelegraphParticles.Stop();
                        Am.am.M.crossfade(VolcanoBlowingSound, 0f, .5f);
                        Am.am.M.crossfade(TelegraphSound,0f,.25f);
                        Telegraphing = false;
                    }
                }
                else
                {
                    if (!Telegraphing)
                    {
                        if (((Time.time - EruptingTime) >= (IdleDuration / speedrate)) && (!Astronaut.TheAstronaut.Quelling))
                        {
                            EruptingTime = Time.time;
                            Telegraphing = true;
                            TelegraphParticles.Play();
                            Am.am.M.crossfade(TelegraphSound, 10f, .25f);
                        }
                    }
                    else if ((Time.time - EruptingTime) >= (TelegraphDuration / speedrate))
                    {
                        Erupting = true;
                        Am.am.M.crossfade(VolcanoBlowingSound, 10f, .1f);
                        EruptingTime = Time.time;
                        //TelegraphParticles.Stop();
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
            if (MySpriteRenderer.enabled)
            {
                Am.am.M.crossfade(VolcanoBlowingSound, 10f, 0f);
            }
            MySpriteRenderer.enabled = false;
            if (TelegraphParticles.isPlaying)
            {
                TelegraphParticles.Stop();
                Am.am.M.crossfade(TelegraphSound, 0f, .25f);
            }
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
        float dps = (200f * Time.fixedDeltaTime);
        if ((a != null))
        {
            if (!Frozen)
            {
                if ((Time.time - a.ReviveTime) >= 2f)
                if (a.Alive)
                {
                    Vector3 dif = (a.transform.position - this.transform.position);
                    
                    Vector3 cross = Vector3.Cross(this.transform.up,Vector3.forward);
                        Am.am.oneshot(Am.am.M.LavaBurn);
                        if (a.TakeDamage(dps,cross*0f* 5f*Mathf.Sign(Vector3.Dot(cross,dif))))
                    {
        
                    }
                } else
                {
                    //BurnParticles
                }

            }
                

        } else
        {
            GenericEnemy ge = col.gameObject.GetComponent<GenericEnemy>();
            if ((ge != null) && (ge.Alive))
            {
                //Am.am.oneshot(Am.am.M.LavaBurn);
                //ge.Kill();
                ge.TakeDamage(dps,new Vector2());
                Am.am.oneshot(Am.am.M.LavaBurn);
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
