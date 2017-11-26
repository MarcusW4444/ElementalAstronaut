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
	void Start () {
        canvasGroup.gameObject.SetActive(true);
        startSelectingPlanets();
    } 

    public void startSelectingPlanets()
    {
        OriginalCameraPosition = MyCamera.transform.position;
        OriginalWindow = MyCamera.orthographicSize;
        AudioManager.AM.playMusic(AudioManager.AM.PlanetSelectMusic, 0f, 1f, true);
        AudioManager.AM.crossfade(AudioManager.AM.CurrentMusic, 1f, 10f);
        //AudioManager.AM.MusicAudioMixer.audioMixer.SetFloat();
        FireAnimator.SetBool("Identified",true);
        //VoidAnimator.SetBool("Identified", true); VoidParticles.Play();

        IceAnimator.SetBool("Identified", true);
        GrassAnimator.SetBool("Identified",true);
        SelectingPlanets = true;
        BlackFade.color = new Color(0f, 0f, 0f, 1f);
    }
    public bool StartingGame = false;
    public int StageDestination = 0;
    public GameObject ComingSoonTextGrass, ComingSoonTextVoid;
    public SpriteRenderer FirePlanet, GrassPlanet, IcePlanet, VoidPlanet;
    public Animator FireAnimator, VoidAnimator, IceAnimator,GrassAnimator;
    public ParticleSystem VoidParticles;
    // Update is called once per frame
    private float gamestartvalue = 0f;
    bool gamec = false;
    
    void Update () {
        
        FirePlanet.transform.Rotate(0f,0f,Time.deltaTime*-.8f,Space.World);
        GrassPlanet.transform.Rotate(0f, 0f, Time.deltaTime * 1f, Space.World);
        IcePlanet.transform.Rotate(0f, 0f, Time.deltaTime * .6f, Space.World);
        VoidPlanet.transform.Rotate(0f, 0f, Time.deltaTime * .2f, Space.World);
        
        if (StartingGame)
        {
            Debug.Log(StageDestination);
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
                                SceneManager.LoadScene(1); break; }
                        case 1: { GameManager.TheGameManager.ElementStage = Astronaut.Element.Fire;
                                
                                SceneManager.LoadScene(2); break; }
                        case 2: { GameManager.TheGameManager.ElementStage = Astronaut.Element.Grass;
                                SceneManager.LoadScene(3); break; }
                        case 3: {
                                GameManager.TheGameManager.ElementStage = Astronaut.Element.Void;
                                SceneManager.LoadScene(4); break; }


                    }

                } else
                {
                    MyCamera.transform.position = Vector3.Lerp(MyCamera.transform.position, new Vector3(FocusTarget.transform.position.x, 
                        FocusTarget.transform.position.y,MyCamera.transform.position.z),gamestartvalue/4f);
                    MyCamera.orthographicSize = Mathf.Lerp(OriginalWindow,.1f,gamestartvalue);
                    BlackFade.color = new Color(0f,0f,0f,gamestartvalue);
                    BlackFade.enabled = true;
                    gamestartvalue = Mathf.Min((gamestartvalue + (Time.deltaTime * .4f)));
                }

            }

        } else
        {
            float a = (BlackFade.color.a - Time.deltaTime);
            BlackFade.color = new Color(0f, 0f, 0f, a);
            if (a <= 0f) BlackFade.enabled = false;
            
        }

        
        if (AudioManager.AM.CurrentMusic != null)
        {
            AudioManager.AM.CurrentMusic.volume = (1f - BlackFade.color.a);

        }

    }
    public Transform FocusTarget;

    public void confirmSelection(GameObject theplanet)
    {
        if (StartingGame) return;

        StartingGame = true;
        IceAnimator.SetBool("Done",true);
        FireAnimator.SetBool("Done", true);
        VoidAnimator.SetBool("Done", true);
        GrassAnimator.SetBool("Done", true);
        FocusTarget = theplanet.transform;
        if (theplanet.gameObject.Equals(IcePlanet.gameObject))
            StageDestination = 0;
        else if (theplanet.gameObject.Equals(FirePlanet.gameObject))
            StageDestination = 1;
        else if (theplanet.gameObject.Equals(GrassPlanet.gameObject))
            StageDestination = 2;
        else if (theplanet.gameObject.Equals(VoidPlanet.gameObject))
            StageDestination = 3;

        //AudioManager.AM.crossfade(AudioManager.AM.CurrentMusic,0f,2f);
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
    public void onPlanetClicked(GameObject theplanet) 
    {
        //Debug.Log("Click");
        if (StartingGame) return;
        if (theplanet.Equals(VoidPlanet.gameObject)) return;
        
        confirmSelection(theplanet);

    }
}
