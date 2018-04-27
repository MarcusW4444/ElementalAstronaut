using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class GameManager : MonoBehaviour {

    // Use this for initialization
    public bool IceWorldCompleted = false, FireWorldCompleted = false, JungleWorldCompleted = false, VoidWorldCompleted = false;
    public int StimPackInventory = 0, ShotgunInventory = 0, MachineGunInventory = 0;
    public float LaserRifleInventory = 0f;
    public float TeslaInventory = 0f;
    public int GrenadeLauncherInventory = 0;
    public int IceVitaLevelAchieved = 0, FireVitaLevelAchieved = 0, JungleVitaLevelAchieved = 0, VoidVitaLevelAchieved = 0;
    public Astronaut.Element AffinityElement = Astronaut.Element.None;
    public int TotalDeaths = 0;
    public PlanetSelector.EntranceType PlanetSelectorEntrance=PlanetSelector.EntranceType.ResumingGame; 
    public static GameManager TheGameManager = null;
    public bool HasSeenIntro = false;
    public bool FinishedGame = false;
    public Astronaut.Element ElementStage = Astronaut.Element.None;
    
    void Start () {
		if (TheGameManager != null)
        {
            
            GameObject.Destroy(this.gameObject);
            return;
        }
        TutorialSystem.TutorialHintArray = new bool[(int)TutorialSystem.TutorialTip.count];
        for (int i = 0; i < TutorialSystem.TutorialHintArray.Length; i++) TutorialSystem.TutorialHintArray[i] = false;
        TutorialSystem.TutorialQueuedArray = new bool[(int)TutorialSystem.TutorialTip.count];
        for (int i = 0; i < TutorialSystem.TutorialQueuedArray.Length; i++) TutorialSystem.TutorialQueuedArray[i] = false;

        AudioListener.volume = GameVolume;
        GameManager.TheGameManager = this;
        GameObject.DontDestroyOnLoad(this.gameObject);
	}
    public void setVolume(float inc,int dir)
    {
        if (dir == 0)
        {
            GameVolume = inc;
        } else 
        {
            GameVolume += (inc*dir);
        } 
        GameVolume = Mathf.Clamp01(GameVolume);
        AudioListener.volume = GameVolume;

    }

    public float GameVolume = .75f;
    public void leaveEndingScene()
    {
        PlanetSelectorEntrance = PlanetSelector.EntranceType.FinishingGame;
        FinishedGame = true;
        SceneManager.LoadScene("StartScene");
    }
    public void completeStage(Astronaut.Element stageelement)
    {
        bool completinggame = false;
        switch (stageelement)
        {
            case Astronaut.Element.Fire:
                {
                    FireVitaLevelAchieved = Mathf.Max(1 + Astronaut.TheAstronaut.VitaLevel,FireVitaLevelAchieved);
                    FireWorldCompleted = true;
                    break;
                }
            case Astronaut.Element.Ice:
                {
                    IceVitaLevelAchieved = Mathf.Max(1 + Astronaut.TheAstronaut.VitaLevel, IceVitaLevelAchieved);
                    IceWorldCompleted = true;

                    break;
                }
            case Astronaut.Element.Grass:
                {
                    JungleVitaLevelAchieved = Mathf.Max(1 + Astronaut.TheAstronaut.VitaLevel, JungleVitaLevelAchieved);
                    JungleWorldCompleted = true;
                    break;
                }
            case Astronaut.Element.Void:
                {
                    //If you complete this stage, you've beaten the game
                    VoidVitaLevelAchieved = Mathf.Max(1+Astronaut.TheAstronaut.VitaLevel, VoidVitaLevelAchieved);
                    VoidWorldCompleted = true;
                    completinggame = true;
                    
                    break;
                }
        }

        /*
        //FOR TESTING PURPOSES
        if ((JungleWorldCompleted) && (FireWorldCompleted) && (IceWorldCompleted) && (VoidWorldCompleted))
        {
            completinggame = true;
        }
        */

        if (completinggame)
        {
            PlanetSelectorEntrance = PlanetSelector.EntranceType.BeatVoidPlanet;
            FinishedGame = true;
            SceneManager.LoadScene("StartScene");
            
        }
        else
        {
            SceneManager.LoadScene("PlanetSelectScene");
        }
        ShotgunInventory = Astronaut.TheAstronaut.ShotgunAmmo;
        MachineGunInventory = Astronaut.TheAstronaut.GatlingAmmo;
        LaserRifleInventory = Astronaut.TheAstronaut.LaserAmmo;
        GrenadeLauncherInventory = Astronaut.TheAstronaut.GrenadeLauncherAmmo;
        TeslaInventory = Astronaut.TheAstronaut.TeslaAmmo;
        StimPackInventory = Astronaut.TheAstronaut.StimPacks;
    }


    public void goToEndingScreen()
    {
        SceneManager.LoadScene("EndingScene");
    }

    public void cheat_PrepareForVoidStage()
    {
        IceWorldCompleted = true;
        IceVitaLevelAchieved = 3;
        FireWorldCompleted = true;
        FireVitaLevelAchieved = 3;
        FireWorldCompleted = false;
        JungleVitaLevelAchieved = 0;
    }



    public GameObject TutorialTipGroup;
    public Text TutorialTipText;
    private bool showingTutorialTip;
    private float tutorialTipTime = -10f;
    private float tutorialTipDuration = 8f;
    private float tutorialTipScale = 0f;
    private bool terminateTip = false;
    public AnimationCurve TutorialScaleCurve;
    private Queue<TutorialSystem.TutorialTip> TutorialQueue = new Queue<TutorialSystem.TutorialTip>();
    int tipsshown = 0;
    public void showTutorialTip(TutorialSystem.TutorialTip tiptype)
    {


        if (TutorialSystem.TutorialHintArray[(int)tiptype]) return;
        if (TutorialSystem.TutorialQueuedArray[(int)tiptype]) return;
        TutorialSystem.TutorialHintArray[(int)tiptype] = true;


        if (ignoringAllTips) return;
        if (showingTutorialTip)
        {
            if (TutorialSystem.TutorialQueuedArray[(int)tiptype]) return;
            TutorialSystem.TutorialQueuedArray[(int)tiptype] = true;
            TutorialQueue.Enqueue(tiptype);
            return;
        }
        terminateTip = false;
        TutorialSystem.TutorialQueuedArray[(int)tiptype] = true;

        float dur = 6f;
        string missingtext = "<missing tutorial tip>";
        string tx = missingtext;
        switch (tiptype)
        {
            case TutorialSystem.TutorialTip.WalkMovement:
                {
                    tx = "Press A or D to move left or right";
                    break;
                }
            case TutorialSystem.TutorialTip.Shoot:
                {
                    tx = "[LEFT CLICK] to Shoot your current weapon";
                    break;
                }
            case TutorialSystem.TutorialTip.SkipTip:
                {
                    dur = 4f;
                    tx = "Press [TAB] to close Tips like this";
                    break;
                }
            case TutorialSystem.TutorialTip.IgnoreTips:
                {
                    tx = "Rather figure the rest out on your own? Press [ENTER] to stop seeing these Tips";
                    break;
                }
            case TutorialSystem.TutorialTip.SwapWeapons:
                {
                    tx = "Press [2], [3], [4], or [5] to switch back to a stronger weapon you have found";
                    break;
                }
            case TutorialSystem.TutorialTip.SwapPistol:
                {
                    tx = "You found a powerful Weapon that has limited ammo. Conserve ammo by swapping to your Pistol by pressing [1]";
                    break;
                }
            case TutorialSystem.TutorialTip.Injured:
                {
                    tx = "You are hurt. Avoid damage for a few seconds to Regenerate Health or activate a Stim Pack by pressing [E] to restore your health quicker.";
                    dur = 8f;
                    break;
                }
            case TutorialSystem.TutorialTip.Revive:
                {
                    tx = "You Died. Tap [SPACE] to Revive.";
                    dur = 5f;
                    break;
                }
            case TutorialSystem.TutorialTip.OnRevived:
                {
                    tx = "Pick up your dropped Vita energy before it disappears to retain your power!";
                    dur = 5f;
                    break;
                }
            case TutorialSystem.TutorialTip.VitaPower:
                {
                    tx = "Enemies drop Vita Energy which will increase the amount of power you get from the element of this planet.";
                    break;
                }
            case TutorialSystem.TutorialTip.VitaDifficulty:
                {
                    tx = "Your Vita Energy level has increased, this makes enemies more aggressive towards you.";
                    break;
                }
            case TutorialSystem.TutorialTip.VitaDropped:
                {
                    tx = "Dieing causes you to drop the Vita Energy you collected, making enemies less aggressive again. Revive quickly to retrieve the Vita Energy";
                    break;
                }
            case TutorialSystem.TutorialTip.TooManyDeaths:
                {
                    tx = "Each time you die causes enemies to do less damage, however they drop less energy when killed";
                    break;
                }
            case TutorialSystem.TutorialTip.Jump:
                {
                    tx = "Press SPACE to Jump in the air. Hold it down to go higher!";
                    break;
                }
            case TutorialSystem.TutorialTip.DoubleJump:
                {
                    tx = "Press SPACE while in midair to use your Boost pack for a Double Jump";
                    break;
                }
            case TutorialSystem.TutorialTip.ChoosePlanet:
                {
                    tx = "Click on a Planet to begin!";
                    dur = 3f;
                    break;
                }
            case TutorialSystem.TutorialTip.ScrollElement:
                {
                    tx = "Use the SCROLL WHEEL to switch between elements";
                    break;
                }
            case TutorialSystem.TutorialTip.UseElement:
                {
                    tx = "Hold the RIGHT Mouse Button to use your Elemental power";
                    break;
                }
            case TutorialSystem.TutorialTip.PistolFastFire:
                {
                    tx = "Fire your Pistol faster by tapping faster at the expense of less Accuracy";
                    break;
                }
                
            default:
                {
                    tx = "<"+tiptype.ToString()+" Tip>";
                    break;
                }


        }

        tutorialTipTime = Time.time;
        tutorialTipDuration = dur;
        showingTutorialTip = true;
        TutorialTipText.text = tx;
        tipsshown++;
        if (tipsshown >= 3) {
            GameManager.TheGameManager.showTutorialTip(TutorialSystem.TutorialTip.SkipTip);
        }
        if (tipsshown >= 5)
        {
            GameManager.TheGameManager.showTutorialTip(TutorialSystem.TutorialTip.IgnoreTips);
        }
        AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.TipIn, AudioManager.AM.PlayerAudioMixer, 1f, 1f, false, 4f);//Play the tutorial tip-in sound

    }
    private bool ignoringAllTips = false;
    public void ignoreTutorialTip(TutorialSystem.TutorialTip tiptype)
    {
        
        TutorialSystem.TutorialHintArray[(int)tiptype] = true;//We don't need to see this tip. We know how to use this skill.
    }
    public void tutorialStep()
    {
        if (showingTutorialTip)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                terminateTip = true;
            }
                
            if (Input.GetKeyDown(KeyCode.Return))
            {
                ignoringAllTips = true;
                terminateTip = true;
            }
            if (!TutorialTipGroup.activeInHierarchy)
                TutorialTipGroup.SetActive(true);

            if (((Time.time - tutorialTipTime) >= tutorialTipDuration)||(terminateTip))
            {
                tutorialTipScale = Mathf.Clamp01(tutorialTipScale - (Time.deltaTime*3f));
                if (tutorialTipScale <= 0f)
                {
                    showingTutorialTip = false;
                    
                }
            } else
            {
                tutorialTipScale = Mathf.Clamp01(tutorialTipScale + (Time.deltaTime * 2f));
            }
        }
        else
        {
            if (TutorialTipGroup.activeInHierarchy)
                TutorialTipGroup.SetActive(false);
            if (TutorialQueue.Count > 0)
            {
                showTutorialTip(TutorialQueue.Dequeue());
            }
        }
        TutorialTipGroup.transform.localScale = (Vector3.one * TutorialScaleCurve.Evaluate(tutorialTipScale));
    }
    public bool UsingLowParticleEffects = false;
    public void setUsingLowParticleEffects(bool b)
    {
        if (UsingLowParticleEffects != b)
        {
            UsingLowParticleEffects = b;
            BroadcastMessage("OnParticleEffectLevelChanged");
        }

    }
    public void OnParticleEffectLevelChanged()
    {
        
        Debug.Log("Particle Effects: "+ ((!UsingLowParticleEffects)?"High":"Low"));
    }
    // Update is called once per frame
    [HideInInspector]public bool VoidWorldCheatEnabled = false;
    void Update () {
        
        tutorialStep();
        if (Input.GetKeyDown(KeyCode.Home))
        {
            setUsingLowParticleEffects(true);
        } else if (Input.GetKeyDown(KeyCode.End))
        {
            setUsingLowParticleEffects(false);
        }
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            cheat_PrepareForVoidStage();
            
        }
        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            setVolume(.05f,-1);

        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            setVolume(.05f, 1);

        }
        if (PauseMenu == null)
        {
            return;
        }
        
        if (!leavingplanet)
            if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!leavingplanet)
            if (!paused)
            {
                pauseGame();
            } else if (askingareyousure)
            {

            } else
            {
                pauseGame();
            }
        }

        if (leavingplanet)
        {
            PauseFade.enabled = true;
            if (PauseFade.color.a >= 1f)
            {
                paused = false;
                leavingplanet = false;
                SceneManager.LoadScene("PlanetSelectScene");
                Cursor.visible = true;
            }
            else
            {
                PauseFade.color = new Color(PauseFade.color.r, PauseFade.color.g, PauseFade.color.b, Mathf.Clamp01(PauseFade.color.a + Time.unscaledDeltaTime));
            }

        }
        else
        {
            
            PauseFade.enabled = false;
        }


        PauseMenu.SetActive(paused);
        if (paused)
        {
            PauseBack.enabled = true;
            MenuGroup.SetActive(!askingareyousure);
            AreYouSureGroup.SetActive(askingareyousure);
        }
        else
        {
            PauseBack.enabled = false;
            MenuGroup.SetActive(false);
            AreYouSureGroup.SetActive(false);
        }


    }

    public bool paused = false;
    public GameObject PauseMenu;
    public bool askingareyousure = false;
    public bool leavingplanet = false;
    public Image PauseFade, PauseBack;
    public void pauseGame()
    {
        
        paused = !paused;
        if (paused)
        {
            
                
            
            if ((SceneManager.GetActiveScene().buildIndex == SceneManager.GetSceneByName("PlanetSelectScene").buildIndex)|| (SceneManager.GetActiveScene().buildIndex == SceneManager.GetSceneByName("StartScene").buildIndex))
            {
                LeavePlanetButton.interactable = false;
                
            } else
            {
                LeavePlanetButton.interactable = true;
                Time.timeScale = 0f;
                Cursor.visible = true;
            }

            PauseMenu.SetActive(true);
            MenuGroup.SetActive(true);
            askingareyousure = false;
        }
        else
        {
                
                PauseMenu.SetActive(false);
            
            
            if ((SceneManager.GetActiveScene().buildIndex == SceneManager.GetSceneByName("PlanetSelectScene").buildIndex) || (SceneManager.GetActiveScene().buildIndex == SceneManager.GetSceneByName("StartScene").buildIndex))
            {
                //...
            }
            else
            {
                Cursor.visible = true;
                Time.timeScale = 1f;
            }
        }

        

    }

    
    public Text AreYouSureText;
    public GameObject MenuGroup;
    public GameObject AreYouSureGroup;
    //public bool pause
    public Button LeavePlanetButton;
    public void OnResumeButtonPressed()
    {
        if (paused)
        pauseGame();

    }
    bool askingtoquit;
    public void OnLeavePlanetButtonPressed()
    {
        if (leavingplanet) return;
        askingareyousure = true;
        askingtoquit = false;
        AreYouSureText.text = "Are you sure you want to leave this planet?";
    }
    public void OnQuitGameButtonPressed()
    {
        if (leavingplanet) return;
        askingareyousure = true;
        askingtoquit = true;
        AreYouSureText.text = "Are you sure you want to Quit the Terra Vita?";

    }
    public void OnYesButtonPressed()
    {
        if (leavingplanet) return;
        pauseGame();
        if (askingtoquit)
        {
            Application.Quit();
        } else
        {
            leavingplanet = true;
            PauseFade.enabled = true;

        }
    }
    public void OnNoButtonPressed()
    {
        askingareyousure = false;
    }
}
