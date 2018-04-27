using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementGoal : MonoBehaviour {

    // Use this for initialization
    public ParticleSystem AbsorptionParticles;
    public Astronaut.Element MyElementPrize = Astronaut.Element.None;
    public bool PickedUp = false;
    public bool IsSkillElement = false; //Is this one of those elements that you can find that can teach you?
    public int SkillIndex = 1;
    public bool HomingInOnAstronaut = false;
    public float HomingTime = -10f;
    public bool AvoidingAstronaut = false;
    public bool ApproachToHome = false;
    public float ApproachDistance = 5f;
    public bool Collectable = true;
    public GenericEnemy[] EtherealLessonTeachers;
    public Transform EtherealCheckpoint;

    void Start () {
        PickedUp = false;
        HomingTime = -10f;
    }
	
	// Update is called once per frame
	void Update () {
        
        if (!PickedUp)
        {
            if (Astronaut.TheAstronaut != null)
            {
                if (AvoidingAstronaut)
                {
                    Vector3 dif = (this.transform.position - Astronaut.TheAstronaut.transform.position);
                    float mag = dif.magnitude;
                    float thresh = 4f;
                    if (mag < thresh)
                    {
                        this.transform.position = (this.transform.position + (dif.normalized * thresh * (1f - (mag / thresh))));
                    }
                } else if (HomingInOnAstronaut)
                {
                    float f = Mathf.Clamp01((Time.time - HomingTime) / 3f);
                    this.transform.position = Vector3.Lerp(this.transform.position, Astronaut.TheAstronaut.transform.position, f);
                } else if (ApproachToHome)
                {
                    Vector3 dif = (this.transform.position - Astronaut.TheAstronaut.transform.position);
                    if (dif.magnitude < ApproachDistance)
                    {
                        HomingInOnAstronaut = true;
                        HomingTime = Time.time;
                    }


                }
            }

        }
		
	}
    public const bool ShowingTutorialZone = false;
    public EtherealManager MyEtherealManager;
    private void OnTriggerEnter2D(Collider2D col)
    {
        Astronaut a = col.gameObject.GetComponent<Astronaut>();
        if ((a != null)&&(a.Alive) && (Collectable))
        {
            if ((!PickedUp) && (!AvoidingAstronaut))
            {

                PickedUp = true;
                
                if ((MyEtherealManager != null) && (ShowingTutorialZone))
                {
                    if (IsSkillElement)
                    {
                        //Grant the skill...?
                        //Increase the player's Vita Power...?
                        
                        AudioManager.TheAudioManager.playGeneralSoundOneShot(AudioManager.AM.ElementGoalCollect, AudioManager.AM.PlayerAudioMixer, 1f, 1f, false, 10f);
                        Astronaut.TheAstronaut.SetEtherealSkillLevel(SkillIndex);
                        Astronaut.TheAstronaut.MyEtherealCheckPoint = this.EtherealCheckpoint;
                        
                        foreach (GenericEnemy en in EtherealLessonTeachers)
                        {
                            en.EtherealWillingToTeach = true;
                            if (en.MySpriteRenderer != null)
                            {
                                en.MySpriteRenderer.material.SetFloat("_EtherealFactor", 0f);
                            }
                        }
                        AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.ElementObtained, AudioManager.AM.PlayerAudioMixer, volume: 1f, pitch: 1f, looped: false, destroyafter: 7f);

                    }
                    else
                    {

                        MyEtherealManager.initiateEtherealTutorial();
                    }

                    //Absorb!
                    ParticleSystem ps = GameObject.Instantiate(AbsorptionParticles, a.transform.position, new Quaternion()).GetComponent<ParticleSystem>();
                    
                    
                    ps.gameObject.SetActive(true);
                    ps.transform.SetParent(a.transform);
                    ps.Play(true);

                    GameObject.Destroy(ps.gameObject, 15f);

                } else
                {
                    a.collectElement(MyElementPrize);
                    ParticleSystem ps = GameObject.Instantiate(AbsorptionParticles, a.transform.position, new Quaternion()).GetComponent<ParticleSystem>();
                    AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.ElementObtained, AudioManager.AM.PlayerAudioMixer, volume: 1f, pitch: 1f, looped: false, destroyafter: 7f);

                    ps.gameObject.SetActive(true);
                    ps.transform.SetParent(a.transform);
                    ps.Play(true);
                    Astronaut.TheAstronaut.PlayerHasControl = false;
                    GameObject.Destroy(ps.gameObject, 15f);
                }
                

                
                foreach (ParticleSystem p in this.gameObject.GetComponentsInChildren<ParticleSystem>())
                {
                    p.transform.SetParent(null);
                    p.Stop();
                    GameObject.Destroy(p.gameObject, 15f);
                }
                this.gameObject.SetActive(false);
            }
        }
    }
}
