using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EtherealManager : MonoBehaviour {

    // Use this for initialization
    public bool EtherealLock = false;
    public bool EtherealTutorialStarted = false;
    public static EtherealManager TheEtherealManager;
    public GameObject EtherealGroup; // has all of the platforms
    public Transform FadeFullTransform, FadeEmptyTransform;
    public Material EtherealGeometryMaterial;
    public Material EtherealEnemyMaterial;
    public Material EtherealBoundaryMaterial;
    public Material[] EtherealBackgroundMaterials;
    public ElementGoal EtherealSkillOrb1, EtherealSkillOrb2, EtherealSkillOrb3, EtherealSkillOrb4;
    public Transform[] EtherealFadeFulls;
    public Transform[] EtherealFadeEmpties;
    public Transform MyEtherealFadeFull = null, MyEtherealFadeEmpty = null;
    public Astronaut.Element EtherealElement;
    public float EtherealFade = 0f;
	void Start () {
        EtherealTutorialStarted = false;
        FinishedEtherealTutorial = false;
        EtherealFlashScale = 0f;
        TheEtherealManager = this;
        EmptinessBackgroundSound = AudioManager.TheAudioManager.createGeneralAudioSource(AudioManager.AM.EtherealEmptyBackground, AudioManager.AM.PlayerAudioMixer, 0f, 1f, true);
        EtherealGroup.SetActive(false);
    }
    private float EtherealStartTime = -10f;
    private bool astronautinitiated = false;
    private float EtherealFlashScale = 0f;
    public int EtherealMaxLevel = 0;
    public int EtherealCurrentLevel = 0;
    public Transform EtherealStartingCheckpoint;
    public void disableRemainingEnemies()
    {
        foreach (GenericEnemy e in GameObject.FindObjectsOfType<GenericEnemy>())
        {
            if (e.transform.parent == null)
            {
                e.gameObject.SetActive(false);
            }
        }
        
    }
    public void initiateEtherealTutorial()
    {
        if (EtherealTutorialStarted) return;
        if (!ElementGoal.ShowingTutorialZone) return;
        EtherealFlash.transform.position = (Astronaut.TheAstronaut.transform.position + new Vector3(0f, 0f, -4.5f));
        EtherealTutorialStarted = true;
        EtherealStartTime = Time.time;
        EtherealLock = false;
        EtherealConverted = false;
        EtherealFlashScale = 0f;
        FinishedEtherealTutorial = false;
        
        disableRemainingEnemies();
        Astronaut.TheAstronaut.PlayerHasControl = false;
        astronautinitiated = false;
        List<Material> backs = new List<Material>();
        backs.Add(Astronaut.TheAstronaut.IceBackgroundInterior.material);
        backs.Add(Astronaut.TheAstronaut.IceBackgroundExterior.material);
        backs.Add(Astronaut.TheAstronaut.FireBackgroundExterior.material);
        backs.Add(Astronaut.TheAstronaut.FireBackgroundInterior.material);
        backs.Add(Astronaut.TheAstronaut.JungleBranchesBackground.material);
        backs.Add(Astronaut.TheAstronaut.JungleSwampBackground.material);
        backs.Add(Astronaut.TheAstronaut.JungleTreeTopsBackground.material);
        EtherealBackgroundMaterials = backs.ToArray();
        //stop the music
        AudioManager.TheAudioManager.StopMusic();
        AudioManager.TheAudioManager.playGeneralSoundOneShot(AudioManager.AM.ElementGoalCollect, AudioManager.AM.PlayerAudioMixer,1f,1f,false,10f);
        Astronaut.TheAstronaut.MyEtherealCheckPoint = this.EtherealStartingCheckpoint;
        EmptinessBackgroundSound.Play();
        int ethereallevel = 0;
        EtherealCurrentLevel = 0;
        ethereallevel = Mathf.FloorToInt(Astronaut.TheAstronaut.VitaLevel);
        if (ethereallevel >= 1)
        {
            EtherealSkillOrb1.Collectable = true;
            EtherealSkillOrb1.AvoidingAstronaut = false;
            EtherealSkillOrb1.ApproachToHome = true;
            


        } else
        {
            EtherealSkillOrb1.Collectable = false;
            EtherealSkillOrb1.AvoidingAstronaut = true;
            EtherealSkillOrb1.ApproachToHome = false;

        }

        if (ethereallevel >= 2)
        {
            EtherealSkillOrb2.Collectable = true;
            EtherealSkillOrb2.AvoidingAstronaut = false;
            EtherealSkillOrb2.ApproachToHome = true;

        }
        else
        {
            EtherealSkillOrb2.Collectable = false;
            EtherealSkillOrb2.AvoidingAstronaut = true;
            EtherealSkillOrb2.ApproachToHome = false;
        }
        if (ethereallevel >= 3)
        {
            EtherealSkillOrb3.Collectable = true;
            EtherealSkillOrb3.AvoidingAstronaut = false;
            EtherealSkillOrb3.ApproachToHome = true;

        }
        else
        {
            EtherealSkillOrb3.Collectable = false;
            EtherealSkillOrb3.AvoidingAstronaut = true;
            EtherealSkillOrb3.ApproachToHome = false;
        }
        if (ethereallevel >= 4)
        {
            EtherealSkillOrb4.Collectable = true;
            EtherealSkillOrb4.AvoidingAstronaut = false;
            EtherealSkillOrb4.ApproachToHome = true;

        }
        else
        {
            EtherealSkillOrb4.Collectable = false;
            EtherealSkillOrb4.AvoidingAstronaut = true;
            EtherealSkillOrb4.ApproachToHome = false;
        }

        if (ethereallevel < 4)
        {
            MyEtherealFadeEmpty = EtherealFadeEmpties[ethereallevel];
            MyEtherealFadeFull = EtherealFadeFulls[ethereallevel];
        } else
        {
            MyEtherealFadeEmpty = null;
            MyEtherealFadeFull = null;
        }

        switch (EtherealElement)
        {
            case Astronaut.Element.Fire:
                {
                    
                    break;
                }
            case Astronaut.Element.Ice:
                {
                    break;
                }
            case Astronaut.Element.Grass:
                {
                    break;
                }
        }
        EtherealMaxLevel = ethereallevel;
    }

    // Update is called once per frame
    public Color EtherealColor1, EtherealColor2;
    public SpriteRenderer EtherealFlash;
    private bool EtherealConverted = false;
    private void OnApplicationQuit()
    {
        EtherealGeometryMaterial.SetInt("_EtherealLock", 0);
        EtherealGeometryMaterial.SetFloat("_EtherealFade", 0f);
        EtherealEnemyMaterial.SetFloat("_EtherealFactor",0f);
        //EtherealGeometryMaterial.SetColor("_EtherealColor1", EtherealColor1);
        //EtherealGeometryMaterial.SetColor("_EtherealColor2", EtherealColor2);

        foreach (Material m in EtherealBackgroundMaterials)
        {
            m.SetInt("_EtherealLock", 0);
            m.SetFloat("_EtherealFade", 0f);
            //m.SetColor("_EtherealColor1", EtherealColor1);
            //m.SetColor("_EtherealColor2", EtherealColor2);
        }
    }
    public AudioSource EmptinessBackgroundSound;
    public GameObject[] BarriersToRemove;
    public bool FinishedEtherealTutorial = false;
    public void finishEtherealTutorial()
    {
        if (FinishedEtherealTutorial) return;
        FinishedEtherealTutorial = true;
        EtherealTutorialStarted = false;
        EtherealLock = false;
        EtherealFlashScale = 1f;
        Astronaut.TheAstronaut.EtherealLock = false;
        Astronaut.TheAstronaut.PlayerHasControl = false;
        Astronaut.TheAstronaut.FinishingStage = true;
        EtherealTutorialFinishTime = Time.time;
        Am.am.oneshot(Am.am.M.ReviveSound);
        //AudioManager.TheAudioManager.StopMusic();
        AudioManager.AM.crossfade(AudioManager.AM.CurrentMusic, 0f, 1f);
        AudioManager.AM.crossfade(EmptinessBackgroundSound, 0f, 1f);
        //EmptinessBackgroundSound.Stop();
    }
    public float EtherealTutorialFinishTime = -10f;
    void Update () {

        if (EtherealLock)
        {
            if ((MyEtherealFadeEmpty != null) && (MyEtherealFadeFull != null))
            {
                Vector3 pos = Astronaut.TheAstronaut.transform.position;
                Vector3 dif = (pos - FadeFullTransform.position);
                Vector3 ethdif = (MyEtherealFadeEmpty.position - MyEtherealFadeFull.position);
                Vector3 mid = Vector3.Project(dif, ethdif.normalized);
                float dot = Vector3.Dot(mid.normalized, ethdif.normalized);//Unnormalized?
                if (dot < 0f)
                {
                    EtherealFade = 0f;
                }
                else
                {

                    EtherealFade = Mathf.Clamp01(mid.magnitude / ethdif.magnitude);
                }
            } else
            {
                EtherealFade = 0f;
            }
            /*
            if (EtherealTutorialStarted)
            {
                if (false)
                if (!astronautinitiated)
                {
                    if ((Time.time - EtherealStartTime) >= 3f)
                    {
                         
                            
                    }
                }
            }
            */
        } else
        {
            EtherealFade = 0;
        }
        float highscale = 150f;
        if (EtherealTutorialStarted) {
            
            if (EtherealConverted)
            {
                EtherealFlashScale = Mathf.Clamp01(EtherealFlashScale-(Time.deltaTime/2f));
                if (EtherealFlashScale <= 0f)
                {
                    if (!astronautinitiated)
                    {
                        Astronaut.TheAstronaut.PlayerHasControl = true;
                        astronautinitiated = true;
                    }
                    else
                    {
                        //Just a placeholder
                        if (Astronaut.TheAstronaut.transform.position.x <= 0f)
                        {
                            if (!FinishedEtherealTutorial)
                            {
                                finishEtherealTutorial();
                            }
                        }
                    }
                }
            } else
            {
                EtherealFlashScale = Mathf.Clamp01(EtherealFlashScale + (Time.deltaTime /2f));
                if ((EtherealFlashScale >= 1f) && (ElementGoal.ShowingTutorialZone))
                {
                    EtherealConverted = true;
                    EtherealLock = true;
                    astronautinitiated = false;
                    foreach (GameObject c in BarriersToRemove)
                    {
                        c.SetActive(false);
                    }
                    EtherealGroup.SetActive(true);
                    Astronaut.TheAstronaut.engageEtherealTutorial(this);
                    
                    AudioClip cl = AudioManager.AM.IceEtherealMusic;
                    switch (EtherealElement)
                    {
                        case Astronaut.Element.Ice:
                            {
                                cl = AudioManager.AM.IceEtherealMusic;
                                break;
                            }
                        case Astronaut.Element.Fire:
                            {
                                cl = AudioManager.AM.FireEtherealMusic;
                                break;
                            }
                        case Astronaut.Element.Grass:
                            {
                                cl = AudioManager.AM.JungleEtherealMusic;
                                break;
                            }

                    }
                    AudioManager.AM.playMusic(cl, 0f, 1f, true);
                }
            }

            EmptinessBackgroundSound.volume = Mathf.Lerp(EmptinessBackgroundSound.volume,EtherealFade, Time.deltaTime * 1f);
            AudioManager.AM.CurrentMusic.volume = Mathf.Lerp(AudioManager.AM.CurrentMusic.volume, 1f-EtherealFade, Time.deltaTime*1f);
        } else
        {
            //EtherealFlashScale = 0f;
            EtherealFlashScale = Mathf.Clamp01(EtherealFlashScale - (Time.deltaTime * 2f));
        }
        EtherealFlash.transform.localScale = (Vector3.one* (EtherealFlashScale * highscale));
        if (Astronaut.TheAstronaut != null)
        {
            EtherealFlash.transform.position = (Astronaut.TheAstronaut.transform.position + new Vector3(0f, 0f, -4.5f));
        }
        EtherealGeometryMaterial.SetInt("_EtherealLock",EtherealLock?1:0);
        EtherealGeometryMaterial.SetFloat("_EtherealFade", EtherealLock ? EtherealFade : 0f);
        EtherealEnemyMaterial.SetFloat("_EtherealFactor", EtherealLock ? (EtherealFade) : 0f);

        foreach (Material m in EtherealBackgroundMaterials)
        {
            m.SetInt("_EtherealLock", EtherealLock ? 1 : 0);
            m.SetFloat("_EtherealFade", EtherealLock ? EtherealFade : 0f);
        }
        EtherealBoundaryMaterial.SetVector("_AstronautLocation",Astronaut.TheAstronaut.transform.position);
        


    }
}
