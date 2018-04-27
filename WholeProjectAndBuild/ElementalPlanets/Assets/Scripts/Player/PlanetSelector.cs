using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlanetSelector : MonoBehaviour {

    // Use this for initialization
    public bool SelectingPlanets = false;
    private Vector3 OriginalCameraPosition;
    private float OriginalWindow;
    private float startTime = -10f;
	void Start () {
        canvasGroup.gameObject.SetActive(true);
        startTime = Time.time;
        startSelectingPlanets();

    }
    public bool UsingController;
    public int ControllerPlanetSelectionIndex; //Which planet.
    
    public void startSelectingPlanets()
    {
        OriginalCameraPosition = MyCamera.transform.position;
        OriginalWindow = MyCamera.orthographicSize;
        AudioManager.AM.playMusic(AudioManager.AM.PlanetSelectMusic, 0f, 1f, true);
        AudioManager.AM.crossfade(AudioManager.AM.CurrentMusic, 1f, 10f);
        //AudioManager.AM.MusicAudioMixer.audioMixer.SetFloat();
        Cursor.visible = true;
        IceAccess = true;
        FireAccess = true;
        JungleAccess = true;
        VoidAccess = false;

        bool vo = true;
        if (GameManager.TheGameManager.IceWorldCompleted)
        { 
            
            IceAccess = false;
            
        } else
        {
            vo = false;
        }

        if (GameManager.TheGameManager.FireWorldCompleted)
        {
            
            FireAccess = false;
        }
        else
        {
            vo = false;
        }

        if (GameManager.TheGameManager.JungleWorldCompleted)
        {
            
            JungleAccess = false;
        }
        else
        {
            vo = false;
        }

        
        VoidAccess = vo;
        



        if (GameManager.TheGameManager.VoidWorldCompleted|| GameManager.TheGameManager.FinishedGame|| GameManager.TheGameManager.VoidWorldCheatEnabled)
        {
            FireAccess = true;
            IceAccess = true;
            JungleAccess = true;
            VoidAccess = true;
        }
        //VoidAccess = false;//The stage isn't up yet//Yes it is


        /*
        if (GameManager.TheGameManager.IceWorldCompleted)
        {
            if (GameManager.TheGameManager.FireWorldCompleted)
            {
                FireAccess = false;
                IceAccess = false;
                JungleAccess = true;
            } else
            {
                FireAccess = true;
                IceAccess = false;
            }
        } else
        {
            FireAccess = false;
            IceAccess = true;
            JungleAccess = false;
        }
        */
        FireAnimator.SetBool("Identified",true);
        VoidAnimator.SetBool("Identified", VoidAccess); if (VoidAccess)VoidParticles.Play();

        IceAnimator.SetBool("Identified", true);
        GrassAnimator.SetBool("Identified",true);
        SelectingPlanets = true;
        BlackFade.color = new Color(0f, 0f, 0f, 1f);
    }
    public bool StartingGame = false;
    public int StageDestination = 0;
    public GameObject ComingSoonTextGrass, ComingSoonTextVoid;
    public SpriteRenderer FirePlanet, GrassPlanet, IcePlanet, VoidPlanet;
    public SpriteRenderer FireLock, GrassLock, IceLock, VoidLock;
    public bool FireAccess=false, JungleAccess=false, IceAccess=false, VoidAccess=false;
    public Animator FireAnimator, VoidAnimator, IceAnimator,GrassAnimator;
    public ParticleSystem VoidParticles;
    public GameObject AffinityMenu;
    public GameObject FireAffinity;
    public GameObject IceAffinity;
    public GameObject JungleAffinity;
    public SpriteRenderer FireAffinityDeny;
    public SpriteRenderer IceAffinityDeny;
    public SpriteRenderer JungleAffinityDeny;
    public bool SelectingAffinity = false;
    // Update is called once per frame
    public void goToAffinitySelection(Astronaut.Element planetelement)
    {
        //
    }
    private float gamestartvalue = 0f;
    bool gamec = false;
    public enum EntranceType { StartingNewGame,ResumingGame,BeatIcePlanet,BeatFirePlanet,BeatJunglePlanet,BeatVoidPlanet,EscapedPlanet,FinishingGame
    }
    
    void Update () {
        
        FirePlanet.transform.Rotate(0f,0f,Time.deltaTime*-.8f,Space.World);
        GrassPlanet.transform.Rotate(0f, 0f, Time.deltaTime * 1f, Space.World);
        IcePlanet.transform.Rotate(0f, 0f, Time.deltaTime * .6f, Space.World);
        VoidPlanet.transform.Rotate(0f, 0f, Time.deltaTime * .2f, Space.World);
        
        if (StartingGame)
        {
            //Debug.Log(StageDestination);
            if (!gamec)
            {
                if (gamestartvalue >= 1f)
                {
                    gamec = true;
                    //Debug.Log("Go");

                    
                    switch (StageDestination)
                    {
                        case 0: {
                                GameManager.TheGameManager.ElementStage = Astronaut.Element.Ice;
                                SceneManager.LoadScene("IceWorldScene"); break; }
                        case 1: { GameManager.TheGameManager.ElementStage = Astronaut.Element.Fire;
                                
                                SceneManager.LoadScene("FireWorldScene"); break; }
                        case 2: { GameManager.TheGameManager.ElementStage = Astronaut.Element.Grass;
                                SceneManager.LoadScene("JungleWorldScene"); break; }
                        case 3: {
                                GameManager.TheGameManager.ElementStage = Astronaut.Element.Void;
                                SceneManager.LoadScene("VoidWorldScene"); break; }


                    }
                    
                } else
                {
                    MyCamera.transform.position = Vector3.Lerp(MyCamera.transform.position, new Vector3(FocusTarget.transform.position.x, 
                        FocusTarget.transform.position.y,MyCamera.transform.position.z),gamestartvalue/4f);
                    MyCamera.orthographicSize = Mathf.Lerp(OriginalWindow,.1f,gamestartvalue);
                    BlackFade.color = new Color(0f,0f,0f,gamestartvalue);
                    BlackFade.enabled = true;
                    gamestartvalue = Mathf.Min((gamestartvalue + (Time.deltaTime * .4f)));
                    IceDifficultyInfo.enabled = false;
                    FireDifficultyInfo.enabled = false;
                    JungleDifficultyInfo.enabled = false;
                    VoidDifficultyInfo.enabled = false;

                }

            }
            

        } else
        {
            float a = (BlackFade.color.a - Time.deltaTime);
            BlackFade.color = new Color(0f, 0f, 0f, a);
            if (a <= 0f) BlackFade.enabled = false;
            
            if (ChoosingAffinity)
            {
                if (Input.GetKeyDown(KeyCode.Escape) || false) //Check for controller inputs
                {
                    hideChooseAffinityMenu();
                    //GameManager.The
                }
                
            }
            if ((Input.GetKeyDown(KeyCode.Backspace)) && (!GameManager.TheGameManager.VoidWorldCheatEnabled))
            {
                GameManager.TheGameManager.VoidWorldCheatEnabled = true;
                GameManager.TheGameManager.FireVitaLevelAchieved = Mathf.Max(GameManager.TheGameManager.FireVitaLevelAchieved, 1);
                GameManager.TheGameManager.IceVitaLevelAchieved = Mathf.Max(GameManager.TheGameManager.IceVitaLevelAchieved, 1);
                GameManager.TheGameManager.JungleVitaLevelAchieved = Mathf.Max(GameManager.TheGameManager.JungleVitaLevelAchieved, 1);
                GameManager.TheGameManager.VoidVitaLevelAchieved = Mathf.Max(GameManager.TheGameManager.VoidVitaLevelAchieved, 1);
                GameManager.TheGameManager.FireWorldCompleted = true;
                GameManager.TheGameManager.IceWorldCompleted = true;
                GameManager.TheGameManager.JungleWorldCompleted = true;
                GameManager.TheGameManager.VoidWorldCompleted = true;

                startSelectingPlanets();
            }

            IceDifficultyInfo.color = Color.Lerp(IceDifficultyInfo.color, new Color(1f, 1f, 1f, IceAnimator.GetBool("Hover") ? 1f : (VoidAccess ? .0f : .5f)), Time.deltaTime * 3f);
            FireDifficultyInfo.color = Color.Lerp(FireDifficultyInfo.color, new Color(1f, 1f, 1f, FireAnimator.GetBool("Hover") ? 1f : (VoidAccess ? .0f : .5f)), Time.deltaTime * 3f);
            JungleDifficultyInfo.color = Color.Lerp(JungleDifficultyInfo.color, new Color(1f, 1f, 1f, GrassAnimator.GetBool("Hover") ? 1f : (VoidAccess ? .0f : .5f)), Time.deltaTime * 3f);
            VoidDifficultyInfo.color = Color.Lerp(VoidDifficultyInfo.color, new Color(1f, 1f, 1f, VoidAnimator.GetBool("Hover") ? 1f : 0f), Time.deltaTime * 3f);
        }

        bool beaten = (GameManager.TheGameManager.VoidWorldCompleted || GameManager.TheGameManager.FinishedGame);
        FirePlanet.color = Color.Lerp(FirePlanet.color,((!GameManager.TheGameManager.FireWorldCompleted||beaten) ? Color.white:Color.grey),Time.deltaTime); FireLock.enabled = ((!FireAccess) && (!GameManager.TheGameManager.FireWorldCompleted)); FireLock.transform.localScale = (Vector3.one * LockScale * (FirePlanet.transform.localScale.x / 8f));
        IcePlanet.color = Color.Lerp(IcePlanet.color, ((!GameManager.TheGameManager.IceWorldCompleted || beaten) ? Color.white : Color.grey), Time.deltaTime); IceLock.enabled = ((!IceAccess) && (!GameManager.TheGameManager.IceWorldCompleted)); IceLock.transform.localScale = (Vector3.one * LockScale * (IcePlanet.transform.localScale.x / 8f));
        GrassPlanet.color = Color.Lerp(GrassPlanet.color, ((!GameManager.TheGameManager.JungleWorldCompleted || beaten) ? Color.white : Color.grey), Time.deltaTime); GrassLock.enabled = ((!JungleAccess) && (!GameManager.TheGameManager.JungleWorldCompleted)); GrassLock.transform.localScale = (Vector3.one * LockScale * (GrassPlanet.transform.localScale.x / 8f));
        //VoidPlanet.color = Color.Lerp(VoidPlanet.color, ((!GameManager.TheGameManager.VoidWorldCompleted||beaten) ? Color.white : Color.grey), Time.deltaTime); VoidLock.enabled = ((!JungleAccess) && (!GameManager.TheGameManager.JungleWorldCompleted)); //GrassLock.transform.localScale = (Vector3.one * LockScale * (GrassPlanet.transform.localScale.x / 8f));

        if ((Time.time - startTime) >= 3f)
        {
            //GameManager.TheGameManager.showTutorialTip(TutorialSystem.TutorialTip.ChoosePlanet);
        }

        
        if (AudioManager.AM.CurrentMusic != null)
        {
            AudioManager.AM.CurrentMusic.volume = (1f - BlackFade.color.a);

        }

    }
    private float LockScale = 5.782167f;
    public Transform FocusTarget;
    public SpriteRenderer IceDifficultyInfo, FireDifficultyInfo, JungleDifficultyInfo,VoidDifficultyInfo;
    public void showChooseAffinityMenu()
    {
        ChoosingAffinity = true;
        
        if (plobjchosen.gameObject.Equals(IcePlanet.gameObject))
        {
            IceAffinityDeny.gameObject.SetActive(true);
            FireAffinityDeny.gameObject.SetActive(false);
            JungleAffinityDeny.gameObject.SetActive(false);
        } else if (plobjchosen.gameObject.Equals(FirePlanet.gameObject))
        {
            IceAffinityDeny.gameObject.SetActive(false);
            FireAffinityDeny.gameObject.SetActive(true);
            JungleAffinityDeny.gameObject.SetActive(false);
        }
        else if (plobjchosen.gameObject.Equals(GrassPlanet.gameObject))
        {
            IceAffinityDeny.gameObject.SetActive(false);
            FireAffinityDeny.gameObject.SetActive(false);
            JungleAffinityDeny.gameObject.SetActive(true);
        }
        //else if (plobjchosen.gameObject.Equals(VoidPlanet.gameObject))
        AffinityMenu.SetActive(true);

    }
    public void hideChooseAffinityMenu()
    {
        ChoosingAffinity = false;
        AffinityMenu.SetActive(false);

    }
    private GameObject plobjchosen;
    public void commenceSelection()
    {
        StartingGame = true;
        IceAnimator.SetBool("Done", true);
        FireAnimator.SetBool("Done", true);
        VoidAnimator.SetBool("Done", true);
        GrassAnimator.SetBool("Done", true);
        FocusTarget = plobjchosen.transform;
        

        if (plobjchosen.gameObject.Equals(IcePlanet.gameObject))
            StageDestination = 0;
        else if (plobjchosen.gameObject.Equals(FirePlanet.gameObject))
            StageDestination = 1;
        else if (plobjchosen.gameObject.Equals(GrassPlanet.gameObject))
            StageDestination = 2;
        else if (plobjchosen.gameObject.Equals(VoidPlanet.gameObject))
            StageDestination = 3;
    }
    public void confirmSelection(GameObject theplanet)
    {
        if (StartingGame) return;
        if (ChoosingAffinity) return;

        plobjchosen = theplanet.gameObject;
        bool chooseaffinity = false;
        if (plobjchosen.gameObject.Equals(IcePlanet.gameObject))
            chooseaffinity = ((GameManager.TheGameManager.FireVitaLevelAchieved <= 0) && (GameManager.TheGameManager.JungleVitaLevelAchieved <= 0));
        else if (plobjchosen.gameObject.Equals(FirePlanet.gameObject))
            chooseaffinity = ((GameManager.TheGameManager.IceVitaLevelAchieved <= 0) && (GameManager.TheGameManager.JungleVitaLevelAchieved <= 0));
        else if (plobjchosen.gameObject.Equals(GrassPlanet.gameObject))
            chooseaffinity = ((GameManager.TheGameManager.IceVitaLevelAchieved <= 0) && (GameManager.TheGameManager.FireVitaLevelAchieved <= 0));
        else if (plobjchosen.gameObject.Equals(VoidPlanet.gameObject))
            chooseaffinity = false;
        if (chooseaffinity)
        {

            showChooseAffinityMenu();
        }
        else
        {
            GameManager.TheGameManager.AffinityElement = Astronaut.Element.None;
            commenceSelection();
        }
        //AudioManager.AM.crossfade(AudioManager.AM.CurrentMusic,0f,2f);
    }

    public void confirmAffinity()
    {

    }

    public Camera MyCamera;
    public Image BlackFade;
    public Canvas canvasGroup;
    public Animator getPlanetAnimator(GameObject theplanet)
    {
        if (theplanet.gameObject.Equals(IcePlanet.gameObject))
            return IceAnimator;
        else if (theplanet.gameObject.Equals(FirePlanet.gameObject))
            return FireAnimator;
        else if (theplanet.gameObject.Equals(GrassPlanet.gameObject))
            return GrassAnimator;
        else if (theplanet.gameObject.Equals(VoidPlanet.gameObject))
            return VoidAnimator;
        return null;
    } 
    public void OnPlanetHover(GameObject theplanet)
    {
        //Debug.Log("Hover");
        Animator planetanimator=getPlanetAnimator(theplanet);
        planetanimator.SetBool("Hover", true);
        
    }
    public void OnPlanetHoverOff(GameObject theplanet)
    {
        //Debug.Log("HoverOff");
        Animator planetanimator = getPlanetAnimator(theplanet);
        planetanimator.SetBool("Hover", false);

    }
    private bool VoidPlanetUnlocked=false;
    public bool ChoosingAffinity = false;
    public GameObject ChoosePlanetText;
    public GameObject[] FirePlanetStars; //Length 4
    public GameObject[] IcePlanetStars; //Length 4
    public GameObject[] JunglePlanetStars; //Length 4

    public void onPlanetClicked(GameObject theplanet) 
    {
        //Debug.Log("Click");
        if (StartingGame) return;
        if (theplanet.Equals(VoidPlanet.gameObject) && (!VoidAccess)) return ;
        if (theplanet.Equals(FirePlanet.gameObject) && (!FireAccess)) return;
        if (theplanet.Equals(IcePlanet.gameObject) && (!IceAccess)) return;
        if (theplanet.Equals(GrassPlanet.gameObject) && (!JungleAccess)) return;
        confirmSelection(theplanet);

    }

    public void onAffinityClicked(GameObject theaffinity)
    {
        if (!ChoosingAffinity) return;
        Debug.Log(theaffinity);
        if (theaffinity.gameObject.Equals(IceAffinity.gameObject) && (!plobjchosen.gameObject.Equals(IcePlanet.gameObject)))
        {
            GameManager.TheGameManager.AffinityElement = Astronaut.Element.Ice;
            commenceSelection();
            hideChooseAffinityMenu();
        } else if (theaffinity.gameObject.Equals(FireAffinity.gameObject) && (!plobjchosen.gameObject.Equals(FirePlanet.gameObject)))
        {
            GameManager.TheGameManager.AffinityElement = Astronaut.Element.Fire;
            commenceSelection();
            hideChooseAffinityMenu();
        }
        else if (theaffinity.gameObject.Equals(JungleAffinity.gameObject) && (!plobjchosen.gameObject.Equals(GrassPlanet.gameObject)))
        {
            GameManager.TheGameManager.AffinityElement = Astronaut.Element.Grass;
            commenceSelection();
            hideChooseAffinityMenu();
        }

    }






    //Also show the Player's inventory of Weapons, Stim Packs, and Elemental Levels around the Planets
}
