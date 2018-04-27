using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour {

    // Use this for initialization
    private bool WaitingForInput = false;
    private bool WatchingIntro=false;
    private bool WatchingOutro = false;
    private bool SkippingIntro = false;
    private bool WatchingVoidReversion = false;
    private bool HeadingToPlanetSelect = false;
    public Image BlackFade;
    public Canvas canvasGroup;
    private float OriginalWindow;
    void Start () {
        canvasGroup.gameObject.SetActive(true);
        BlackFade.color = new Color(0f, 0f, 0f, 1f);
        FadingInOut = true;
        OriginalWindow = MyCamera.orthographicSize;
        StartTime = Time.time;
        if (GameManager.TheGameManager.PlanetSelectorEntrance == PlanetSelector.EntranceType.BeatVoidPlanet)
        {
            //SceneManager.LoadScene("EndingScreen");
            WatchingOutro = false;
            WatchingIntro = false;
            WatchingVoidReversion = true;
            clearPlanetParticleEffects();
            TitleScreenAnimator.enabled = false;
            VoidCloakParticles.Play(true);
            PlanetSprite.color = new Color(1f,1f,1f,0f);
            DryPlanetSprite.color = new Color(1f,1f,1f,1f);
            TitleObject.SetActive(false);
        }
        else if (GameManager.TheGameManager.PlanetSelectorEntrance == PlanetSelector.EntranceType.FinishingGame)
        {
            WatchingOutro = true;
            WatchingIntro = false;
            WatchingVoidReversion = false;
            TitleScreenAnimator.SetTrigger("StartOutro");
            Debug.Log("Show the Outro!");
            
            AudioManager.AM.playMusic(AudioManager.AM.MaxLevelMusic, .9f, 1f, true);
            
        } else
        {
            WatchingOutro = false;
            //This is the first time you are at this screen
            AudioManager.AM.playMusic(AudioManager.AM.PlanetSelectMusic, 0f, 1f, true);
            AudioManager.AM.crossfade(AudioManager.AM.CurrentMusic, 1f, 10f);
            WatchingOutro = false;
            WaitingForInput = true;

        }
        
        
        
        
        
    }

    // Update is called once per frame
    public bool FadingInOut = true;
    private bool VoidPlanetReverted = false;
    private float StartTime = -10f;
    private float reverttime = -10f;
    public ParticleSystem VoidCloakParticles, ReversionFlare;
    public SpriteRenderer PlanetSprite,DryPlanetSprite;
    public GameObject TitleObject;
    public Camera MyCamera;
	void Update () {

        if (WatchingVoidReversion)
        {
            
            if (!VoidPlanetReverted)
            {

                if ((Time.time - StartTime) >= 4f)
                {
                    //AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.ElementObtained, AudioManager.AM.PlayerAudioMixer, volume: 1f, pitch: 1f, looped: false, destroyafter: 7f);
                    AudioManager.TheAudioManager.playGeneralSoundOneShot(AudioManager.AM.ElementGoalCollect, AudioManager.AM.PlayerAudioMixer, 1f, 1f, false, 10f);
                    VoidPlanetReverted = true;
                    ReversionFlare.Play(true);
                    VoidCloakParticles.Stop();
                    reverttime = Time.time;
                }
                

            } else
            {
                if ((Time.time - reverttime) >= 8f)
                {
                    FadingInOut = false;
                    MyCamera.orthographicSize = Mathf.Lerp(OriginalWindow, .1f, Mathf.Clamp01(((Time.time - (reverttime+8f))*.8f)/1f));
                } 

            }
            
        }
        else
        {
            bool pr = (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space));
            pr = (pr && (!GameManager.TheGameManager.paused));


            if (pr)
            {
                if (!WatchingIntro)
                {
                    if (!GameManager.TheGameManager.HasSeenIntro)
                    {
                        GameManager.TheGameManager.HasSeenIntro = true;
                        TitleScreenAnimator.SetTrigger("StartIntro");
                        WatchingIntro = true;

                    }
                    else
                    {
                        FadingInOut = false;
                        HeadingToPlanetSelect = true;
                    }

                }
                else
                {
                    FadingInOut = false;
                    SkippingIntro = true;
                    HeadingToPlanetSelect = true;
                }

            }
        }




        if (FadingInOut)
        {
            float a = Mathf.Clamp01(BlackFade.color.a - Time.deltaTime);
            BlackFade.color = new Color(0f, 0f, 0f, a);


            
            //
        } else
        {
            float a = Mathf.Clamp01(BlackFade.color.a + Time.deltaTime);

            BlackFade.color = new Color(0f, 0f, 0f, a);
            if (WatchingVoidReversion)
            {
                if (a >= 1f)
                {
                    if (!faded)
                    {
                        faded = true;
                        //Debug.Log("That's it.");
                        //Debug.Break();
                        
                            GameManager.TheGameManager.goToEndingScreen();
                        
                    }
                }
            }
            else
            {
                if (a >= 1f)
                {
                    if (!faded)
                    {
                        faded = true;
                        if (HeadingToPlanetSelect)
                        {
                            SceneManager.LoadScene("PlanetSelectScene");
                        }
                    }
                }
            }
        }
        
        BlackFade.enabled = (BlackFade.color.a > 0f);

    }
    private bool faded = false; //just call the exit function once; not every frame after then



    //Animation Functions
    public Animator TitleScreenAnimator;
    public ParticleSystem[] PlanetParticles;
    public void stopPlanetParticleEffects()
    {
        foreach (ParticleSystem p in PlanetParticles)
        {
            p.Stop();
        }
    }
    public void clearPlanetParticleEffects()
    {
        foreach (ParticleSystem p in PlanetParticles)
        {
            p.Stop();
            p.Clear(true);
        }
    }

    public void crossfadeMainMusicAway()
    {
     
        AudioManager.AM.crossfade(AudioManager.AM.CurrentMusic, 0f, 2f);
    }
    public void playParticleSystem(ParticleSystem ps)
    {
        ps.Play();

    }
    public ParticleSystem JungleDisruption, FireDisruption, IceDisruption;
    public void playJungleDisruption()
    {
        JungleDisruption.Play();
    }
    public void playFireDisruption()
    {
        FireDisruption.Play();
    }
    public void playIceDisruption()
    {
        IceDisruption.Play();
    }
    public ParticleSystem LostJungleElement, LostIceElement, LostFireElement;
    
    public void releaseLostElements()
    {
        //just play the particle
        LostIceElement.Play(true);
        LostJungleElement.Play(true);
        LostFireElement.Play(true);


    }
    public void reclaimLostElements()
    {
        //just play the particle
        LostIceElement.Stop();//Play(true);
        LostJungleElement.Stop();
        LostFireElement.Stop();


    }

    public void stopParticleSystem(ParticleSystem ps)
    {
        ps.Stop();

    }
    public void playIntroTheme()
    {
        AudioManager.AM.playMusic(AudioManager.AM.IntroSequenceMusic, 1f, 1f, true);
    }
    public void OnIntroOver()
    {
        FadingInOut = false;
        faded = false;
        HeadingToPlanetSelect = true;
    }
    public void OnOutroOver()
    {

    }
}
