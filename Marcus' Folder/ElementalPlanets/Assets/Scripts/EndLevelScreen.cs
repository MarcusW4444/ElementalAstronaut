using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndLevelScreen : MonoBehaviour {

    // Use this for initialization
    
    public bool DoneShowingResults = false;
    
    public Slider EndLevelVitaSlider;
    public Text EndLevelPowerBaseText;
    public Text EndLevelPower1Text;
    public Text EndLevelPower2Text;
    public Text EndLevelPower3Text;
    public Image EndLevelNotchBase;
    public Image EndLevelNotch1;
    public Image EndLevelNotch2;
    public Image EndLevelNotch3;
    public Image EndLevelSymbolBase;
    public Image EndLevelSymbol1;
    public Image EndLevelSymbol2;
    public Image EndLevelSymbol3;

    public Text HeadlineText;
    public Image HeadlineGlow;
    public Text AdditionalCapacityText;
    public Image FillBar;
    public Text CommendationText;
    public Image CommendationGlow;

    public Sprite ElementSprite;
    public Sprite GrassIcon, FireIcon, IceIcon, VoidIcon;
    public Color ElementColor;
    private Astronaut.Element Element;
    private string skill1 = "none";
    private string skill2 = "none";
    private string skill3 = "none";
    private string skill4 = "none";
    public void setElement(Astronaut.Element e)
    {
        
        Element = e;
        switch (e)
        {
            case Astronaut.Element.Fire: {
                    ElementSprite = FireIcon;
                    ElementColor = new Color(1f, .6f, .1f);
                    HeadlineText.text = "FIRE ELEMENT OBTAINED";
                    
                    break; }
            case Astronaut.Element.Ice: {
                    ElementSprite = IceIcon;
                    ElementColor = new Color(.3f, .3f, 1f);
                    HeadlineText.text = "ICE ELEMENT OBTAINED";
                    break; }
            case Astronaut.Element.Grass: {
                    ElementSprite = GrassIcon;
                    ElementColor = new Color(.2f, 1f, .1f);
                    HeadlineText.text = "GRASS ELEMENT OBTAINED";
                    break; }
            case Astronaut.Element.Void: {
                    ElementSprite = VoidIcon;
                    ElementColor = new Color(.8f, 0f, .8f);
                    HeadlineText.text = "VOID FORCE DEFEATED";
                    break; }
            default: {
                    ElementSprite = null;
                    ElementColor = Color.white;
                    break; }

        }

        HeadlineGlow.color = ElementColor;
        HeadlineText.GetComponent<Outline>().effectColor = Color.Lerp(ElementColor, Color.black, .5f);
        CommendationGlow.color = ElementColor;
        Color midcol = Color.Lerp(ElementColor, Color.black, .5f);
        CommendationText.GetComponent<Outline>().effectColor = midcol;
        skill1 = Astronaut.getElementPowerLevelName(e, 0);
        skill2 = Astronaut.getElementPowerLevelName(e, 1);
        skill3 = Astronaut.getElementPowerLevelName(e, 2);
        skill4 = Astronaut.getElementPowerLevelName(e, 3);
        EndLevelNotchBase.color = new Color(ElementColor.r,ElementColor.g,ElementColor.b,.5f);
        EndLevelNotch1.color = new Color(ElementColor.r, ElementColor.g, ElementColor.b, .5f);
        EndLevelNotch2.color = new Color(ElementColor.r, ElementColor.g, ElementColor.b, .5f);
        FillBar.color = new Color(ElementColor.r, ElementColor.g, ElementColor.b, FillBar.color.a);

        EndLevelNotch3.color = new Color(ElementColor.r, ElementColor.g, ElementColor.b, .5f);
        EndLevelPowerBaseText.text = skill1;
        //EndLevelPowerBaseText.color = new Color(ElementColor.r, ElementColor.g, ElementColor.b, .5f);
        EndLevelPower1Text.text = skill2;
        EndLevelPower2Text.text = skill3;
        EndLevelPower3Text.text = skill4;
        EndLevelSymbolBase.sprite = ElementSprite;
        EndLevelSymbol1.sprite = ElementSprite;
        EndLevelSymbol2.sprite = ElementSprite;
        EndLevelSymbol3.sprite = ElementSprite;

    }
    public float EndLevelPowerValue = 0f; //(0f-3f)
    public float EndLevelPowerTime = -10f;
    public const float EndLevelPowerDuration = 8f;
    public AnimationCurve EndLevelPowerDurationCurve;
    public Text DurationBonusText;
    public Transform EndGroup;

    void Start () {
        DoneShowingResults = false;
        HasFinishedResults = false;
        ShowingResults = false;
        originalScale = EndGroup.transform.localScale;
        oscale = EndGroup.transform.localScale;
        EndGroup.transform.localScale = new Vector3();
        EndGroup.gameObject.SetActive(true);
    }
    private Vector3 originalScale;

    // Update is called once per frame
    private bool opening = false,closing=false;
    private float transitiontime = -10f;
    public bool DoneLookingAtResults = false, LookingAtResults = false;
    public RectTransform VitaBar,VitaBarParent;
    public float MeasuredVitaValue,TargetVitaValue;
    


    public enum Anim {Idle,Expanding,MeasuringVita,LookingAtFinalResults,Shrinking };
    private Anim CurrentAnim;
    private float AnimTime = -10f;
    private Vector3 oscale;
    private int VitaLevel=0;
    
    public void setCommendationText(int vval)
    {
        string s = "Good!";
        switch (vval)
        {
            case 0: { s = "Good!"; break; }
            case 1: { s = "Great"; break; }
            case 2: { s = "Awesome"; break; }
            case 3: { s = "EXCELLENT"; break; }
        }
        CommendationText.text = s;
    }
    void Update () {

        
        if (ShowingResults)
        {

            float duration;
            float tween = 0f;
            float time = (Time.time - AnimTime);
            switch (CurrentAnim)
            {
                case Anim.Idle: {

                        EndGroup.transform.localScale = new Vector3();
                        EndGroup.gameObject.SetActive(false);
                        MeasuredVitaValue = 0f;
                        break; }
                case Anim.Expanding: {
                        LookingAtResults = false;
                        HasFinishedResults = false;
                        duration = .5f;

                        tween = (time / duration);

                        EndGroup.transform.localScale = oscale * (Mathf.Pow(tween, 1f / 2.5f));
                        EndGroup.gameObject.SetActive(true);
                        CommendationText.transform.localScale = (Vector3.one);
                        MeasuredVitaValue = 0f;
                        setCommendationText(0);
                        //set the notches to nonvisible and smaller
                        if (tween >= 1f)
                        {
                            EndGroup.transform.localScale = oscale;
                            VitaLevel = 0;
                            setAnim(Anim.MeasuringVita);
                        }


                        break; }
                case Anim.MeasuringVita: {

                        duration = EndLevelPowerDuration;
                        tween = Mathf.Clamp01(time / duration);
                        
                        MeasuredVitaValue = TargetVitaValue*EndLevelPowerDurationCurve.Evaluate(tween);
                        if (MeasuredVitaValue>=3f) {
                            MeasuredVitaValue = TargetVitaValue;
                            tween = 1f;
                        }
                        int vl = ((int)MeasuredVitaValue);
                        if (vl > VitaLevel)
                        {
                            //Increase the Scale of the corresponding title as it stabilizes backwards
                            CommendationText.transform.localScale = (Vector3.one*vl);
                            setCommendationText(vl);
                        } else
                        {
                            
                            
                        }
                        VitaLevel = vl;
                        if (VitaLevel > 3)
                        {
                            AdditionalCapacityText.text = "+" + ((VitaLevel - 3)*50) + "% Element Usage";
                            AdditionalCapacityText.color = Color.Lerp(AdditionalCapacityText.color, new Color(1f, 1f, 1f, 1f),Time.deltaTime*3f);
                            //AdditionalCapacityText.enabled = true;
                        } else
                        {
                            AdditionalCapacityText.text = "Vita Power Bonuses";
                            AdditionalCapacityText.color = new Color(1f,1f,1f,.5f);
                            //AdditionalCapacityText.enabled = false;
                        }

                        setVitaBarValue(MeasuredVitaValue / 3f);
                        //VitaBar.offsetMax = new Vector2(VitaBar.offsetMax.x,200*Mathf.Clamp01(MeasuredVitaValue/3f));

                        if (tween >= 1f)
                        {
                            
                            
                            setAnim(Anim.LookingAtFinalResults);
                        }

                        break; }
                case Anim.LookingAtFinalResults: {

                        LookingAtResults = true;


                        if (DoneLookingAtResults)
                        {
                            setAnim(Anim.Shrinking);
                        } 


                        break; }
                case Anim.Shrinking: {
                        EndGroup.transform.localScale = new Vector3();

                        LookingAtResults = true;

                        duration = .5f;
                        tween = Mathf.Clamp01(time / duration);
                        EndGroup.transform.localScale = oscale * (Mathf.Pow(1f-tween, 1f / 2.5f));

                        if (tween >= 1f)
                        {
                            EndGroup.gameObject.SetActive(false);
                            setAnim(Anim.Idle);
                            LookingAtResults = true;
                            HasFinishedResults = true;

                        } else
                        {
                            EndGroup.gameObject.SetActive(true);
                        }

                        break; }

            }
        }

        //Animate the glow effects seperately

        float s = CommendationText.transform.localScale.x;
        CommendationText.transform.localScale = Vector3.Lerp(CommendationText.transform.localScale, Vector3.one, Time.deltaTime * 3f);
        CommendationGlow.transform.Rotate(0f,0f,225f*Time.deltaTime);
        Color col = Color.Lerp(Color.white,ElementColor,Mathf.Sin(Mathf.PI*2f*(Time.time*.5f)));
        HeadlineGlow.color = new Color(col.r,col.g,col.b,HeadlineGlow.color.a);
    }
    public void setAnim(Anim a)
    {
        CurrentAnim = a;
        AnimTime = Time.time;
        
    }
    public void setVitaBarValue(float f)
    {
        VitaBar.offsetMax = new Vector2(-VitaBarParent.rect.width * (1f - Mathf.Clamp01(f)), VitaBar.offsetMax.y);
    }
    public bool ShowingResults = false;
    
    public void showResults()
    {
        if (ShowingResults) return;

        //EndLevelPowerTime = Time.time;
        Debug.Log("Showin Results");
        ShowingResults = true;
        setAnim(Anim.Expanding);
        setElement(Astronaut.TheAstronaut.StageElement);
        DoneShowingResults = false;
        TargetVitaValue = ((1f * Astronaut.TheAstronaut.VitaLevel) + Astronaut.TheAstronaut.ResistanceXP);
        transitiontime = Time.time;
        opening = true;
    }
    public bool HasFinishedResults = false;
        
    public void finishResults()
    {
        setAnim(Anim.Shrinking);
        transitiontime = Time.time;
        closing = true;
    }
}
