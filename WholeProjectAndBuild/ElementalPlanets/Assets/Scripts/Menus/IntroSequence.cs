using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IntroSequence : MonoBehaviour {

    // Use this for initialization
    public enum State { Entering,Title,SelectingPlanet}
    public ParticleSystem Disruption1, Disruption2, Disruption3;
    public ParticleSystem FireElementDistantGlow, IceElementDistantGlow, JungleElementDistantGlow;
    public ParticleSystem FireFlare, IceFlare, JungleFlare;
    public SpriteRenderer EarthSprite, DecrepitEarthSprite;
    public bool SkipIntro = false;
    public PlanetSelector MyPlanetSelector;
    public Image BlackFade;
    public State IntroState = State.Entering;
    public Text TitleImage;
    private float StateTime;
    private float StateValue, StateDuration;
    public bool TitleScreenVisible = false;
    public bool AnimatingIntro = false;
    public Image TimeBorder;
    public Text TimePastText, TimePresentText;
    void Start () {
        IntroState = State.Entering;
        SkipIntro = false;
        ReturningToTitleScreen = false;
        AnimatingIntro = false;
    }
	
	
    public bool ReturningToTitleScreen = false;
    public Animator IntroAnimator;
    public void returnToTitleScreen()
    {
        if (TitleScreenVisible) { return; }
        
        TitleScreenVisible = true;
        

    }
	void Update () {

        bool confirmpress = Input.GetMouseButtonDown(0);
        bool backpress = Input.GetKeyDown(KeyCode.Escape);

    }

    public void initiateDisrutption()
    {

        Disruption1.Play(true);
        Disruption2.Play(true);
        Disruption3.Play(true);

    }
}
