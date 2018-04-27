using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndingSequenceScreen : MonoBehaviour {

    // Use this for initialization
    
    public int IceCount = 1;
    
    public int JungleCount = 1;
    
    public int FireCount = 1;
    
    public int VoidCount = 1;
    private int TotalCount = 0;
    public GameObject TotalGroup;
    private float StartTime;
    public bool WaitingForInput = false;
    public Image WeakEndingBackground, GreatEndingBackground, EpicEndingBackground;
    public Image WeakEndingFocusStatue, WeakEndingFocusPlant;
    public Transform TopLeftCorner, BottomRightCorner,TotalLeftSide, TotalRightSide;
    public Text TotalLabel, GoodEndingText, GreatEndingText, EpicEndingText;
    
    void Start () {
        StartTime = Time.time;
        CircleIndex = 0;
        Starting = true;
        if (GameManager.TheGameManager != null)
        {
            IceCount = Mathf.Min(GameManager.TheGameManager.IceVitaLevelAchieved, 4);
            FireCount = Mathf.Min(GameManager.TheGameManager.FireVitaLevelAchieved, 4);
            JungleCount = Mathf.Min(GameManager.TheGameManager.JungleVitaLevelAchieved, 4);
            VoidCount = Mathf.Min(GameManager.TheGameManager.VoidVitaLevelAchieved, 4);
        } else
        {
            IceCount = 1;
            FireCount = 1;
            JungleCount = 1;
            VoidCount = 1;
        }
        
        generateCircles();
        //IceGroup.SetActive(false);
        //JungleGroup.SetActive(false);
        //FireGroup.SetActive(false);
        //VoidGroup.SetActive(false);

    }
    public Image ElementImagePrefab;
    public Image ElementMissImagePrefab;
    public Image ElementStandardPrefab;
    public Sprite IceSprite,JungleSprite, FireSprite, VoidSprite;
    private Image[] ImageArray;
    public Transform Container;
    public void generateCircles()
    {
        ImageArray = new Image[16];
        for (int i = 0; i <= 15; i++)
        {
            Vector3 pos = Vector3.Lerp(TotalLeftSide.position,TotalRightSide.position,(((float)i)/15f));
            Image im = GameObject.Instantiate<Image>(ElementMissImagePrefab, pos,ElementMissImagePrefab.transform.rotation,Container);
            if ((i+1) >= EPICMARGIN)
            {
                Image eu = GameObject.Instantiate<Image>(ElementStandardPrefab, pos, ElementMissImagePrefab.transform.rotation, Container);
                eu.color = new Color((255f/255f), (221f / 255f), (0f / 255f), .95f); //GOLD
                eu.transform.localScale *= 1.5f; 
            } else if ((i+1) >= GREATMARGIN)
            {
                Image eu = GameObject.Instantiate<Image>(ElementStandardPrefab, pos, ElementMissImagePrefab.transform.rotation, Container);
                eu.color = new Color((209f / 255f), (229f / 255f), (240f/255f), .85f); //Silver
                eu.transform.localScale *= 1.25f;
            } else
            {
                Image eu = GameObject.Instantiate<Image>(ElementStandardPrefab, pos, ElementMissImagePrefab.transform.rotation, Container);
                eu.color = new Color((198f / 255f), (158f/ 255f), (131f / 255f), .5f); //Bronze
            }
            ImageArray[i] = im;

        }
    }
    public Vector3 getLocationAcrossCorners(float x,float y)
    {
        return new Vector3((TopLeftCorner.position.x + ((BottomRightCorner.position.x - TopLeftCorner.position.x) * x)), (TopLeftCorner.position.y + ((BottomRightCorner.position.y - TopLeftCorner.position.y) * y)), 0f);
    }
    private int CircleIndex = 0;
    private int CircleShow = 0;

    // Update is called once per frame
    private bool DoneWithResultGroups = false;
    private bool DoneWithTotalGroup = false;
    private float CountTime = -10f;
    private bool Starting = false;
    
    private float EventTime = -10f;
    public Text EndingText;
    
	void Update () {


        
		if (Starting)
        {
            if ((Time.time - StartTime)>=2f)
            {
                Starting = false;
                DoneWithResultGroups = false;
                TotalGroup.SetActive(true);
                Am.am.oneshot(Am.am.M.TipIn);
                EventTime = Time.time;
                CircleShow = 0;
                CircleIndex = 0;
            }
            
        } else if (!DoneWithResultGroups)
        {
            if ((Time.time - EventTime) >= ((CircleShow == 0) ?2f:.5f))
            {
                if (CircleShow < 4)
                {
                    int c = 1;
                    Sprite sp = IceSprite;
                    if (CircleShow == 0)
                    {
                        c = IceCount;
                        sp = IceSprite;
                    }
                    else if (CircleShow == 1)
                    {
                        c = JungleCount;
                        sp = JungleSprite;
                    }
                    else if (CircleShow == 2)
                    {
                        c = FireCount;
                        sp = FireSprite;
                    }
                    else if (CircleShow == 3)
                    {
                        c = VoidCount;
                        sp = VoidSprite;
                    }
                    TotalCount += c;
                    
                    while (CircleIndex < TotalCount)
                    {
                        Image img = ImageArray[CircleIndex];
                        Image im = GameObject.Instantiate<Image>(ElementImagePrefab, img.transform.position, ElementImagePrefab.transform.rotation, img.transform);
                        im.sprite = sp;
                        CircleIndex++;
                    }
                    playCountSound(c);

                    CircleShow++;
                    EventTime = Time.time;
                }
                else
                {
                    DoneWithResultGroups = true;
                    EventTime = Time.time;
                }
            }

        } else
        {

            if (!EndingRevealed)
            {
                if ((Time.time - EventTime) >= 2f)
                {
                    revealEnding();
                }
                
            }
            else
            {

                if (ShowingEpicEnding)
                {
                    EpicEndingBackground.transform.localScale = (Vector3.one * (1f+(1f/(.1f+((Time.time - RevealTime)/1f)))));
                    if ((Time.time - RevealTime) >= 6f)
                    {
                        WaitingForInput = true;
                    }
                } else if (ShowingGreatEnding)
                {
                    float ce = Mathf.Clamp01(GreatEndingBackground.color.r + (Time.deltaTime / 1.5f));
                    GreatEndingBackground.color = new Color(ce, ce, ce, 1f);
                    if ((Time.time - RevealTime) >= 6f)
                    {
                        WaitingForInput = true;
                    }
                } else if (ShowingWeakEnding)
                {
                    if ((Time.time - RevealTime) >= 1.5f)
                    {
                        float ce = Mathf.Clamp01(WeakEndingBackground.color.r + (Time.deltaTime / 4f));

                        WeakEndingBackground.color = new Color(ce,ce,ce,1f);
                        float c1 = Mathf.Clamp01(WeakEndingFocusStatue.color.a + ((((Time.time - RevealTime) >= 7.5f)?1f:0f)*(Time.deltaTime / 1.5f)));
                        WeakEndingFocusStatue.color = new Color(1f,1f,1f, c1);
                        float c2 = Mathf.Clamp01(WeakEndingFocusPlant.color.a + ((((Time.time - RevealTime) >= 10f) ? 1f : 0f) * (Time.deltaTime / 3f)));
                        WeakEndingFocusPlant.color = new Color(1f, 1f, 1f, c2);
                    }
                    if ((Time.time - RevealTime) >= 15f)
                    {
                        WaitingForInput = true;
                    }
                }



                
                
            }


        }
        bool pr = (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space));
        if (!WaitingForInput)
        {
            pr = false;
        }
        if (pr)
        {
            FadingInOut = false;
        }
            if (FadingInOut)
        {
            float a = Mathf.Clamp01(BlackFade.color.a - Time.deltaTime);
            BlackFade.color = new Color(0f, 0f, 0f, a);



            //
        }
        else
        {
            float a = Mathf.Clamp01(BlackFade.color.a + Time.deltaTime);

            BlackFade.color = new Color(0f, 0f, 0f, a);
            
                if (a >= 1f)
                {
                    if (!faded)
                    {
                        faded = true;
                    GameManager.TheGameManager.leaveEndingScene();
                    }
                }
            
        }
    }
    public Image BlackFade;
    private bool faded = false; //just call the exit function once; not every frame after then
    public bool FadingInOut = true;
    private bool ShowingEndingImage = false;


    public void playCountSound(int count)
    {
        float k0 = 1f, k1 = .75f, k2 = .5f, k3 = 0.375f, k4 = .25f;
        float cs = (1f + ((1f / 8f) * CircleShow));
        if (count == 1)
        {
            AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.ResistancePickUp, AudioManager.AM.PlayerAudioMixer, volume: 1f, pitch: k0 * cs, looped: false, destroyafter: 7f);
            
        } else if (count == 2)
        {
            AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.ResistancePickUp, AudioManager.AM.PlayerAudioMixer, volume: 1f, pitch: k0 * cs, looped: false, destroyafter: 7f);
            AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.ResistancePickUp, AudioManager.AM.PlayerAudioMixer, volume: 1f, pitch: k1 * cs, looped: false, destroyafter: 7f);
            AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.ResistancePickUp, AudioManager.AM.PlayerAudioMixer, volume: 1f, pitch: k2 * cs, looped: false, destroyafter: 7f);
            
            
        } else if (count == 3)
        {
            AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.ResistancePickUp, AudioManager.AM.PlayerAudioMixer, volume: 1f, pitch: k0*cs, looped: false, destroyafter: 7f);
            AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.ResistancePickUp, AudioManager.AM.PlayerAudioMixer, volume: 1f, pitch: k1*cs, looped: false, destroyafter: 7f);
            AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.ResistancePickUp, AudioManager.AM.PlayerAudioMixer, volume: 1f, pitch: k2*cs, looped: false, destroyafter: 7f);
            AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.ResistancePickUp, AudioManager.AM.PlayerAudioMixer, volume: 1f, pitch: k3*cs, looped: false, destroyafter: 7f);
            AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.ResistancePickUp, AudioManager.AM.PlayerAudioMixer, volume: 1f, pitch: k4 * cs, looped: false, destroyafter: 7f);

        } else if (count == 4)
        {
            Am.am.oneshot(Am.am.M.KLEECE);//KLEECE
        }

    }
    
    private bool EndingRevealed = false;
    private float RevealTime = -10f;
    public bool ShowingEpicEnding = false,ShowingGreatEnding=false,ShowingWeakEnding=false;
    public const int EPICMARGIN = 12;
    public const int GREATMARGIN = 8;
    public void revealEnding()
    {
        if (EndingRevealed) return;
        ShowingEpicEnding = false;
        ShowingGreatEnding = false;
        ShowingWeakEnding = false;
        EndingRevealed = true;
        RevealTime = Time.time;
        if (TotalCount >= EPICMARGIN)
        {
            //EPIC ENDING
            Am.am.oneshot(Am.am.M.EndingFullyEnergized);
            Am.am.oneshot(Am.am.M.KLEECEEXTENDED);
            AudioManager.AM.playMusic(AudioManager.AM.MaxLevelMusic, 1f, 1f, true);
            ShowingEpicEnding = true;
            EpicEndingBackground.transform.localScale = (Vector3.one*11f);
            EpicEndingBackground.gameObject.SetActive(true);
            GreatEndingBackground.gameObject.SetActive(false);
            WeakEndingBackground.gameObject.SetActive(false);
            WeakEndingFocusPlant.gameObject.SetActive(false);
            WeakEndingFocusStatue.gameObject.SetActive(false);
        } else if (TotalCount >= GREATMARGIN)
        {
            //Great Ending!
            ShowingGreatEnding = true;
            Am.am.oneshot(Am.am.M.ElementGoalCollect);
            AudioManager.AM.playMusic(AudioManager.AM.EndingSequenceGreatMusic, 1f, 1f, true);
            GreatEndingBackground.color = Color.black;
            GreatEndingBackground.gameObject.SetActive(true);

            EpicEndingBackground.gameObject.SetActive(false);
            WeakEndingBackground.gameObject.SetActive(false);
            WeakEndingFocusPlant.gameObject.SetActive(false);
            WeakEndingFocusStatue.gameObject.SetActive(false);

        } else
        {
            //Weak Ending
            ShowingWeakEnding = true;
            AudioManager.AM.playMusic(AudioManager.AM.EndingSequenceWeakMusic, 0f, 1f, true);
            Am.am.M.crossfade(Am.am.M.CurrentMusic,1f,5f);
            GreatEndingBackground.gameObject.SetActive(false);
            EpicEndingBackground.gameObject.SetActive(false);
            
            WeakEndingBackground.color = Color.black;
            WeakEndingBackground.gameObject.SetActive(true);
            WeakEndingFocusPlant.color = new Color(1f,1f,1f,0f);
            WeakEndingFocusPlant.gameObject.SetActive(true);
            WeakEndingFocusStatue.color = new Color(1f, 1f, 1f, 0f);
            WeakEndingFocusStatue.gameObject.SetActive(true);
        }
        RevealTime = Time.time;
    }
}
