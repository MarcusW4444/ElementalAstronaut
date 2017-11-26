using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Astronaut : MonoBehaviour {

    // Use this for initialization
    public enum Element { Fire, Ice, Grass, All, Water, None, Void }
    public float Health = 100f;
    public int VitaLevel = 0;
    public static int AggressionLevel = 0;
    public static float AggressionLevelF;
    public int StimPacks = 0;

    

    public float DamageVulnerability = 1f;
    public float ResistanceXP = 0f;

    public int Deaths = 0;
    public float ElementalEnergy;
    public Element ElementMode = Element.None;
    public bool HasElementIce = false, HasElementFire = false, HasElementGrass = false, HasElementVoid = false,HasElementAll = false;
    public int IceElementPowerLevel = 5, FireElementPowerLevel = 5, GrassElementPowerLevel = 5, VoidElementPowerLevel = 5;
    public bool Alive = true;
    public bool Invulnerable = false;
    public enum SpecialWeapon { Pistol, Shotgun, Laser, Gatling,GrenadeLauncher,TeslaGun };
    public SpecialWeapon CurrentSpecialWeapon = SpecialWeapon.Pistol;
    public static Astronaut TheAstronaut = null;
    private bool Introduced = false;
    public ParticleSystem IntroParticlesIce, IntroParticlesFire, IntroParticlesGrass;
    public bool PlayerHasControl = false;
    public Transform WatchingBossLocation = null; 
    void Start() {
        TheAstronaut = this;
        BackgroundStartX = MyCameraRig.position.x;
        PistolAmmo = MAXPISTOLAMMO;
        Deaths = 0;
        LaserBaseScale = LaserBeamRenderer.transform.localScale.x;
        LaserBaseDistance = (this.transform.position - LaserDamageParticles.transform.position).magnitude;
        MyBossGolem = null;
        Introduced = false;
        FadeCanvas.SetActive(true);
        rotateElement(true);
        MyEndLevelScreen.gameObject.SetActive(true);
        LastGroundedLocation = this.transform.position;
        originalspriterotation = MySpriteRenderer.transform.localRotation;
        RaycastHit2D rh = Physics2D.Raycast(this.transform.position, Vector3.down, 100f, LayerMask.GetMask(new string[] { "Geometry" }));

        if (rh.distance <= 0f)
        {
            Introduced = true;
            PlayerHasControl = true;
            CameraRigCurrentPosition = MyCameraRig.transform.position;
        } else
        {
            Introduced = false;
            PlayerHasControl = false;

            
            HasIntroLanded = false;
            if (DeployDropSound==null)
                DeployDropSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.AstronautFalling, AudioManager.AM.PlayerAudioMixer, 1f, 1.5f, false);

            MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
            MyCameraRig.transform.position = new Vector3(rh.point.x - 5f, rh.point.y + 3f, MyCameraRig.transform.position.z);
            CameraRigCurrentPosition = MyCameraRig.transform.position;
        }
        
        loadGameManagerInfo();
        setWeapon(SpecialWeapon.Pistol);
    }

    public MeshRenderer IceBackgroundExterior, IceBackgroundInterior,FireBackgroundExterior, FireBackgroundInterior, JungleBackgroundExterior, JungleBackgroundInterior, VoidBackgroundExterior, VoidBackgroundInterior;
    public ParticleSystem Weather_Ice, Weather_Fire, Weather_JungleSwampRain, Weather_JungleLeaves, Weather_Void;
    public Element StageElement = Element.None;
    public void loadGameManagerInfo()
    {

        IceBackgroundInterior.enabled = false;
        IceBackgroundExterior.enabled = false;
        FireBackgroundInterior.enabled = false;
        FireBackgroundExterior.enabled = false;
        JungleBackgroundInterior.enabled = false;
        JungleBackgroundExterior.enabled = false;
        VoidBackgroundInterior.enabled = false;
        VoidBackgroundExterior.enabled = false;

        switch (GameManager.TheGameManager.ElementStage)
        {
            case Element.Ice:
                {
                    StageElement = Element.Ice;
                    InteriorBackground = IceBackgroundInterior;
                    ExteriorBackground = IceBackgroundExterior;
                    usingJungleBackgroundSystem = false;
                    Weather_Ice.Play();


                    break;
                }
            case Element.Fire:
                {
                    StageElement = Element.Fire;
                    InteriorBackground = FireBackgroundInterior;
                    ExteriorBackground = FireBackgroundExterior;
                    usingJungleBackgroundSystem = false;
                    Weather_Fire.Play();
                    break;
                }
            case Element.Grass:
                {
                    StageElement = Element.Grass;
                    usingJungleBackgroundSystem = true;
                    //SwampBackground
                    //InteriorBackground = JungleBackgroundInterior;
                    //ExteriorBackground = JungleBackgroundExterior;
                    Weather_JungleSwampRain.Play();
                    //Weather_JungleLeaves
                    break;
                }
            case Element.Void:
                {
                    StageElement = Element.Void;
                    InteriorBackground = VoidBackgroundInterior;
                    ExteriorBackground = VoidBackgroundExterior;
                    Weather_Void.Play();
                    break;
                }
        }

        switch (StageElement)
        {
            case Element.All: { ResistanceColor = Color.white; ResistanceIcon.enabled = false; break; };
            case Element.Fire:
                {
                    ResistanceColor = new Color(1f, .6f, .1f);
                    ResistanceIcon.enabled = true;
                    BossHealthBar.sprite = FireBarSprite;
                    break;
                };
            case Element.Grass: { ResistanceColor = new Color(.2f, 1f, .1f); ResistanceIcon.sprite = VitaSpriteGrass; ResistanceIcon.enabled = true;
                    BossHealthBar.sprite = GrassBarSprite;
                    break;};
            case Element.Ice: { ResistanceColor = new Color(.3f, .3f, 1f); ResistanceIcon.sprite = VitaSpriteIce; ResistanceIcon.enabled = true;
                    BossHealthBar.sprite = IceBarSprite; break; };
            case Element.Void: { ResistanceColor = new Color(.8f, 0f, .8f); ResistanceIcon.sprite = VitaSpriteFire; ResistanceIcon.enabled = true;
                    BossHealthBar.sprite = VoidBarSprite;
                    break; };
        }

        if (usingJungleBackgroundSystem)
        {
            JungleSwampBackground.enabled = true;
            JungleBranchesBackground.enabled = false;
            JungleTreeTopsBackground.enabled = false;

            FireBackgroundInterior.enabled = false;
            FireBackgroundExterior.enabled = false;
            IceBackgroundInterior.enabled = false;
            IceBackgroundExterior.enabled = false;
            VoidBackgroundInterior.enabled = false;
            VoidBackgroundExterior.enabled = false;
            setJungleBackground(0);
        }
        else
        {

            InteriorBackground.enabled = true;
            ExteriorBackground.enabled = true;
            JungleSwampBackground.enabled = false;
            JungleBranchesBackground.enabled = false;
            JungleTreeTopsBackground.enabled = false;
        }

        ShotgunAmmo = GameManager.TheGameManager.ShotgunInventory;
        GatlingAmmo = GameManager.TheGameManager.MachineGunInventory;
        LaserAmmo = GameManager.TheGameManager.LaserRifleInventory;
        StimPacks = GameManager.TheGameManager.StimPackInventory;

        HasElementFire = (GameManager.TheGameManager.FireVitaLevelAchieved >= 0);
        FireElementPowerLevel = GameManager.TheGameManager.FireVitaLevelAchieved;
        

        HasElementGrass = (GameManager.TheGameManager.JungleVitaLevelAchieved >= 0);
        GrassElementPowerLevel = GameManager.TheGameManager.JungleVitaLevelAchieved;

        HasElementIce = (GameManager.TheGameManager.IceVitaLevelAchieved >= 0);
        IceElementPowerLevel = GameManager.TheGameManager.IceVitaLevelAchieved;

        HasElementVoid = (GameManager.TheGameManager.VoidVitaLevelAchieved >= 0);
        VoidElementPowerLevel = GameManager.TheGameManager.VoidVitaLevelAchieved;
    }

    // Update is called once per frame
    public Rigidbody2D MyRigidbody;
    public CapsuleCollider2D MyCollider;
    public SpriteRenderer MySpriteRenderer;
    public Animator MyAnimator;
    public Bullet BulletPrefab;
    public LineRenderer LaserBeamRenderer;
    private float LaserBaseScale = 1f;
    private float LaserBaseDistance;
    public ParticleSystem LaserGlowFlowParticles, LaserMicroFlowParticles, LaserDamageParticles;
    private int LateralControl = 0;
    private int VerticalControl = 0;
    private bool JumpControl = false;
    public float WalkSpeed = 1f;
    public float JumpSpeed = 5f;
    public bool SwapControl = false;
    private float SwapXOrigin = 0f, SwapYOrigin = 0f;
    public float JumpCancelRate = .6f;
    public bool Airborne = false;
    public bool UsedDoubleJump = false;
    public bool Slipping = false;
    public bool ElementalControl = false;
    private bool ElementalPress = false;
    private bool jumppress = false;
    private bool JumpNow = false;
    public bool ShootControl;
    private bool ShootPress;
    public int LookDirection = 1;
    public Vector3 AimPosition = new Vector3();
    public Transform ReticleTransform;
    public SpriteRenderer HitMarkerReticle;
    public const int MAXPISTOLAMMO = 30;
    public int PistolAmmo = 15;
    public int ShotgunAmmo = 0;
    public float LaserAmmo = 0f;
    public int GatlingAmmo = 0;
    public int GrenadeLauncherAmmo = 0;
    public float TeslaAmmo = 0f;
    public ParticleSystem StimParticles;
    public void applyStimPack()
    {
        //if (Health < 100f)
        //{
            //StimParticles.Play();
            
                for (int i = 0; i < MAXSTIMS; i++)
            {
                if (StimDurations[i] <= 0f)
                {
                    StimDurations[i] = STIMDURATIONCONSTANT;
                    StimHeals[i] = 100f;
                    StimVignette.color = new Color(StimVignette.color.r, StimVignette.color.g, StimVignette.color.b, StimVignette.color.a + ((1f-StimVignette.color.a)/2f));
                    this.StimPacks--;
                    break;
                }
            }
                
        //}
    }
    public void stimStep()
    {

        if (Alive)
        {
            for (int i = 0; i < MAXSTIMS; i++)
            {
                //Debug.Log("Stimming");
                //StimParticles.Emit(1);
                if (StimDurations[i] > 0f)
                {
                    float t = (StimDurations[i] - Time.deltaTime);
                    //StimParticles.Emit(2);
                    if (t > 0f)
                    {
                        StimDurations[i] = t;
                        
                        if (StimHeals[i] > 0f)
                        {
                            
                            float hpd = (STIMHEALINGCONSTANT*Time.deltaTime);
                            float hu = Mathf.Min(StimHeals[i],hpd);
                            if (Health < 100f)
                            {
                                Health = Mathf.Min(Health + hu, 100f);
                                StimParticles.Emit(1);
                            }
                            
                            StimHeals[i] = (StimHeals[i] - hu);
                        }
                    } else
                    {
                        StimDurations[i] = -1f;
                    }
                }

            }
        }
    }
    public const int MAXSTIMS = 10;
    public const float STIMDURATIONCONSTANT = 4f;
    public const float STIMRESISTANCECONSTANT = (1f/4f);
    public const float STIMHEALINGCONSTANT = 25f;
    public float[] StimDurations = new float[MAXSTIMS];
    public float[] StimHeals = new float[MAXSTIMS];

    
    void Update() {



        if (true)
        {
            MyHealthBar.fillAmount = (Health / 100f);
            MyEnergyBar.fillAmount = (ElementalEnergy / 100f);
            MyEnergyBar.color = Color.white;

            //ResistanceLevel = Mathf.Max(0, ResistanceLevel / 2);
            //ResistanceXP = 0f;
            int vitalevel = ((int)(ResistanceDisplayedValue));
            float flasht = (1f - ((Time.time % .5f) / .5f));
            if (VitaLevel <= 0)
            {
                ResistanceText.text = "1";

                
                
            }
            else if (VitaLevel < 3)
            {
                ResistanceText.text = "" + (VitaLevel+1);
            }
            else if (VitaLevel >= 3)
            {
                int m = (VitaLevel - 3);
                ResistanceText.text = "MAX"+((m > 0)?("+"+m):"");
            }
            
                VitaPowerBaseText.GetComponent<Outline>().effectColor = Color.Lerp(ResistanceColor, Color.black, 0.5f);
            
            VitaPowerBaseText.text = getElementPowerLevelName(ResistanceElement, vitalevel);

            float frac = (ResistanceDisplayedValue - (Mathf.Floor(ResistanceDisplayedValue)));
            VitaPowerBaseText.text = getElementPowerLevelName(StageElement, 0);
            VitaPower1Text.text = "+" + getElementPowerLevelName(StageElement, 1);
            VitaPower2Text.text = "+" + getElementPowerLevelName(StageElement, 2);
            VitaPower3Text.text = "+" + getElementPowerLevelName(StageElement, 3);

            if (VitaLevel > 0)
            {
                VitaPowerBaseText.color = new Color(ResistanceColor.r, ResistanceColor.g, ResistanceColor.b, Mathf.Lerp(VitaPowerBaseText.color.a, .5f, Time.deltaTime));

                if (VitaLevel >= 1)
                    VitaPower1Text.color = new Color(ResistanceColor.r, ResistanceColor.g, ResistanceColor.b, Mathf.Lerp(VitaPower1Text.color.a, .5f, Time.deltaTime));
                else
                    VitaPower1Text.color = new Color(1f, 1f, 1f, .1f);
                if (VitaLevel >= 2)
                    VitaPower2Text.color = new Color(ResistanceColor.r, ResistanceColor.g, ResistanceColor.b, Mathf.Lerp(VitaPower2Text.color.a, .5f, Time.deltaTime));
                else
                    VitaPower2Text.color = new Color(1f, 1f, 1f, .1f);
                if (VitaLevel >= 3)
                    VitaPower3Text.color = new Color(ResistanceColor.r, ResistanceColor.g, ResistanceColor.b, Mathf.Lerp(VitaPower3Text.color.a, .5f, Time.deltaTime));
                else
                    VitaPower3Text.color = new Color(1f, 1f, 1f, .1f);

                
                if (VitaLevel > 3)
                    VitaPowerBonusText.color = new Color(1f,1f,1f, .5f);
                else
                    VitaPowerBonusText.color = new Color(1f, 1f, 1f, 0f);
                VitaPowerBonusText.text = ("+" + ((VitaLevel - 3) * 50) + "% Element Usage");



            } else
            {
                VitaPowerBaseText.color = new Color(ResistanceColor.r, ResistanceColor.g, ResistanceColor.b, Mathf.Lerp(VitaPowerBaseText.color.a, .3f, Time.deltaTime));
                VitaPower1Text.color = new Color(1f, 1f, 1f, .1f);
                VitaPower2Text.color = new Color(1f, 1f, 1f, .1f);
                VitaPower3Text.color = new Color(1f, 1f, 1f, .1f);
                VitaPowerBonusText.color = new Color(1f, 1f, 1f, 0f);

            }
            /*
            if ((frac > 0.5f) && ((VitaLevel < 3)))
            {

                
                VitaPowerBaseText.text = getElementPowerLevelName(StageElement, VitaLevel);
                VitaPowerBottomText.text = getElementPowerLevelName(StageElement, VitaLevel + 1);
                VitaPowerBottomText.color = Color.Lerp(Color.clear, Color.white, flasht);// * ((frac - .5f) / .5f)
                //VitaPowerTopText.GetComponent<Outline>().effectColor = Color.gray;
            }
            else
            {
                
                VitaPowerBottomText.color = Color.clear;
                VitaPowerBottomText.text = getElementPowerLevelName(StageElement, VitaLevel + 1);
                if (VitaLevel > 0)
                {
                    VitaPowerBaseText.color = VitaPowerBaseText.color = new Color(ResistanceColor.r, ResistanceColor.g, ResistanceColor.b, Mathf.Lerp(VitaPowerBaseText.color.a, 1f, Time.deltaTime * 2f));
                } else
                {

                    VitaPowerBaseText.color = new Color(ResistanceColor.r, ResistanceColor.g, ResistanceColor.b, .15f);
                }
                VitaPowerBaseText.text = getElementPowerLevelName(StageElement, VitaLevel);
            }
            */

            float rd = ((VitaLevel * 1f) + ResistanceXP);
            ResistanceDisplayedValue = Mathf.Lerp(ResistanceDisplayedValue,rd,.1f);
            ResistanceGauge.value = (ResistanceDisplayedValue - Mathf.Floor(ResistanceDisplayedValue));//Mathf.Lerp(ResistanceGauge.value, ,.5f);

            ResistanceFill.color = Color.Lerp(ResistanceColor,Color.white,0.5f+(0.5f*(Mathf.Clamp01(vitalevel / 3f))*Mathf.Sin((Time.time/.8f)*2f*Mathf.PI)));

            ResistanceFill.enabled = (ResistanceXP > 0f);
            
            float rv = (((float)vitalevel) / 3f);
            ResistanceGlow.color = Color.Lerp(ResistanceGlow.color, new Color(ResistanceColor.r * rv, ResistanceColor.g * rv, ResistanceColor.b*rv, 1f), .5f);
            ResistanceGlow.transform.Rotate(0f,0f,135f*Time.deltaTime,Space.Self);
            ResistanceIcon.color = Color.Lerp(ResistanceIcon.color,((vitalevel > 0) ? new Color(1f, 1f, 1f, 0.9f) : new Color(.5f, .5f, .5f, .5f)),.5f);
            
            if (StimPacks > 0) {
                StimPackIcon.color = Color.white;
                StimPackGlow.color = Color.Lerp(StimPackGlow.color,new Color(39f / 255f, 74f / 255f, 62f/255f,1f),.5f);
                StimCountText.text = "StimPacks x" + StimPacks;//"" + StimPacks+" StimPack"+((StimPacks==1)?"":"s");
                StimCountText.enabled = true;

            } else
            {
                StimPackGlow.color = Color.Lerp(StimPackGlow.color,Color.black,.5f);
                StimPackIcon.color = new Color(.3f,.3f,.3f,.4f);
                StimCountText.enabled = false;
            }
            if ((Alive) && (PlayerHasControl))
            {
                //lc = ((Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0));
                //vc = ((Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0));
                if (!Frozen)
                if (Input.GetKeyDown(KeyCode.E)){

                    if (StimPacks > 0)
                    {
                        applyStimPack();
                    }
                }
                //jc = (Input.GetKey(KeyCode.Space));
                //sp = Input.GetMouseButton(0);
                //ec = Input.GetMouseButton(1);
                if (Input.mouseScrollDelta.y > 0)
                {
                    /*
                    if (ElementMode == Element.Fire)
                        ElementMode = Element.Ice;
                    else if (ElementMode == Element.Ice)
                        ElementMode = Element.Grass;
                    else if (ElementMode == Element.Grass)
                        ElementMode = Element.Fire;
                    */
                    rotateElement(false);
                }
                else if (Input.mouseScrollDelta.y < 0)
                {
                    rotateElement(true);
                    /*
                    if (ElementMode == Element.Fire)
                        ElementMode = Element.Grass;
                    else if (ElementMode == Element.Grass)
                        ElementMode = Element.Ice;
                    else if (ElementMode == Element.Ice)
                        ElementMode = Element.Fire;
                    */
                }

                if (Input.GetKeyDown(KeyCode.Alpha1))
                {
                    setWeapon(SpecialWeapon.Pistol);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    setWeapon(SpecialWeapon.Shotgun);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha3))
                {
                    setWeapon(SpecialWeapon.Gatling);
                }
                else if (Input.GetKeyDown(KeyCode.Alpha4))
                {
                    setWeapon(SpecialWeapon.Laser);
                }
            }
            if ((MyBossGolem != null) && (!Invulnerable))
            {
                //Change the image of the boss health bar to match its element
                float thresh = .2f;
                float f = (MyBossGolem.Health / MyBossGolem.MaxHealth);
                if (f < thresh)
                {
                    f = f / thresh;
                    BossHealthBar.color = Color.Lerp(Color.white,Color.red,((Mathf.Sin((Time.time/ .25f)*Mathf.PI)+1f)/2f));
                } else
                {
                    BossHealthBar.color = Color.white;
                }
                BossHealthBar.fillAmount = f;
                BossHealthBar.gameObject.SetActive(!MyBossGolem.Defeated);
                BossHealthFrame.gameObject.SetActive(!MyBossGolem.Defeated);
            } else
            {
                BossHealthBar.gameObject.SetActive(false);
                BossHealthFrame.gameObject.SetActive(false);
            }
            switch (ElementMode)
            {
                case Element.Fire:
                    {
                        MyEnergyBar.sprite = FireBarSprite; //MyEnergyBar.color = new Color(1f, .2f, .2f, 1f);
                        MyElementalIcon.sprite = ElementIcon_Fire;
                        MyEnergyBar.enabled = true;

                        break;
                    }
                case Element.Ice:
                    {
                        MyEnergyBar.sprite = IceBarSprite; //MyEnergyBar.color = new Color(.2f, .2f, 1f, 1f);
                        MyElementalIcon.sprite = ElementIcon_Ice;
                        MyEnergyBar.enabled = true;
                        break;
                    }
                case Element.Water:
                    {
                        MyElementalIcon.sprite = ElementIcon_Water;
                        MyEnergyBar.sprite = GrassBarSprite;//MyEnergyBar.color = new Color(.2f, .8f, .8f, 1f);
                        MyEnergyBar.enabled = true;
                        break;
                    }
                case Element.Grass:
                    {
                        MyElementalIcon.sprite = ElementIcon_Grass;
                        MyEnergyBar.sprite = GrassBarSprite;//MyEnergyBar.color = new Color(.2f, .8f, .2f, 1f); break;
                        MyEnergyBar.enabled = true;
                        break;
                    }
                case Element.None:
                default:
                    {
                        MyElementalIcon.sprite = ElementIcon_None;
                        MyEnergyBar.sprite = GrassBarSprite;
                        MyEnergyBar.enabled = false;
                        MyEnergyBar.color = new Color(.2f, .8f, .2f, .5f); break;
                        //MyEnergyBar.color = new Color(.8f, .8f, .8f, 1f); break; }
                    }
            }

            string ammostring = "";
            switch (CurrentSpecialWeapon)
            {
                case SpecialWeapon.Pistol: { ammostring = "Pistol: " + PistolAmmo; break; }
                case SpecialWeapon.Shotgun: { ammostring = "Shotgun: " + ShotgunAmmo; break; }
                case SpecialWeapon.Gatling: { ammostring = "Machinegun: " + GatlingAmmo; break; }
                case SpecialWeapon.Laser: { ammostring = "Laser: " + (int)LaserAmmo+"%"; break; }
            }
            MyWeaponAmmoText.text = ammostring;
            if (FadingInOut)
            {
                BlackFadeAlpha = Mathf.Clamp01(BlackFadeAlpha-(Time.deltaTime/1.5f));
            } else
            {
                if ((BlackFadeAlpha >= 1f)&&(FinishingStage) && (!_FinishedStage))
                {

                    _FinishedStage = true;
                    GameManager.TheGameManager.completeStage(StageElement);
                }
                if (((BlackFadeAlpha >= 1f)) && (TransitionClimbing))
                {

                    if (ClimbingToBranches)
                    {
                        transportToBranchesArea();
                    }
                    else
                    {
                        transportToTreeTopsArea();
                    }
                    //Transform t = null;

                    //this.transform.position = t.transform.position;
                    //CameraRigCurrentPosition = new Vector3(t.transform.position.x, t.transform.position.y, CameraRigCurrentPosition.z);
                }
                BlackFadeAlpha = Mathf.Clamp01(BlackFadeAlpha + (Time.deltaTime / 1.5f));
                
            }
            BlackFade.color = new Color(0f, 0f, 0f, BlackFadeAlpha);

            

            
            
            if (!Alive)
            {
                ManualRevive = Mathf.Max(0f,ManualRevive-(Time.deltaTime*.25f));
            } else
            {
                if ((Time.time - lastDamageTakenTime) >= 3f) //Regeneration
                {
                    Health = Mathf.Min(100f, Health + (Time.deltaTime * 8f));
                }
            }
            if (!Introduced)
            {

                if (Mathf.Abs(MyCameraRig.transform.position.x - this.transform.position.x) < .5f)
                {
                    if (MyRigidbody.bodyType == RigidbodyType2D.Kinematic)
                    {
                        MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
                        DeployDropSound.Play();
                    }
                    
                }
                if (HasIntroLanded)
                {
                    if ((Time.time - IntroLandTime) >= .4f)
                    {
                        Introduced = true;
                        PlayerHasControl = true;
                        
                    }
                }
            }
            ReviveBar.fillAmount = ManualRevive;
            DeathCountText.text = ("" + Deaths + " Death" + ((Deaths != 1) ? "s" : ""));
            if (!Alive)
            {
                //if ((Time.time - DeathTime) >= 4f) //Back when dieing resulted in a game over
                //{
                    //FadingInOut = false;
                //}
                if (PlayerHasControl)
                {
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        attemptToRevive();
                    }
                }
                
                
                if (Time.time - DeathTime > .75f)
                {
                    ReviveGroup.SetActive(true);
                } else {
                    ReviveGroup.SetActive(false);
                }

                DeathsOverlay.SetActive(true);


            } else if (FinishingStage)
            {
                if ((Time.time - ElementCollectedTime) >= 5f)
                {

                    if (!MyEndLevelScreen.ShowingResults)
                    {
                        MyEndLevelScreen.showResults();
                    }
                    else
                    {

                        if (MyEndLevelScreen.HasFinishedResults)
                        {

                            if (!TeleportedOut)
                            {
                                TeleportOut();
                            }
                            else if ((Time.time - TeleportedTime) >= 1.25f)
                            {
                                FadingInOut = false;
                                AudioManager.AM.crossfade(AudioManager.AM.CurrentMusic, 0f, 1.4f);
                            }

                        } else if (MyEndLevelScreen.LookingAtResults)
                        {

                            if (Input.GetMouseButton(0))
                            {
                                MyEndLevelScreen.DoneLookingAtResults = true;

                            }
                        }
                        
                    }
                }
            }
            if (Alive)
            {
                DeathsOverlay.SetActive(false);
                ReviveGroup.SetActive(false);
            }
        }

        
        if (((Time.time - BurningTime) <= 0f) && (Alive))
        {
            OnFire = true;
        } else
        {
            OnFire = false;
        }
        stimStep();
        int vl = VitaLevel;
        if (vl > 3) vl = 3;
        else if (vl < 0) vl = 0;
        AggressionLevel = vl;
        AggressionLevelF = (vl / 3f);
        updateOverlay();
        
    }

    public void startLevelMusic()
    {
        AudioClip cl = AudioManager.AM.PlanetSelectMusic;
        switch (GameManager.TheGameManager.ElementStage)
        {
            case Element.Fire:
                { cl = AudioManager.AM.FirePlanetMusic; break; }
            case Element.Ice:
                { cl = AudioManager.AM.IcePlanetMusic; break; }
            case Element.Grass:
                { cl = AudioManager.AM.JunglePlanetMusic; break; }
        }
        AudioManager.AM.playMusic(cl, 1f, 1f, true);
    }

    private float BurningTime= -10f;
    public bool OnFire = false;
    public ParticleSystem Shockwave;
    public ParticleSystem JetPackParticles;

    public SpriteRenderer PistolSprite, ShotgunSprite, MachinegunSprite, LasergunSprite,GrenadeLauncherSprite,TeslaGunSprite;
    private float lastshottime = -10f;
    public void gunShock(Vector3 dir)
    {
        CameraRigCurrentPosition = (CameraRigCurrentPosition + new Vector3(dir.x, dir.y, 0f));
        MyCameraRig.position = (MyCameraRig.position + new Vector3(dir.x, dir.y, 0f));
    }
    private AudioSource PistolShootSound,ShotgunSound,MachinegunLoop,BeamRifleLoop, BeamRiflePlusLoop, BeamRifleHitLoop, BeamRifleDamageLoop, BeamRifleStart;
    public void attackStep()
    {


        //Fire Arms


        Vector3 dir = ((AimPosition - this.transform.position).normalized);
        
            switch (CurrentSpecialWeapon)
            {
                case SpecialWeapon.Pistol: {

                    if (ShootControl)
                    {
                        //switch (weapon)
                        float firerate = 5f;
                        float damage = 30f;
                        float speed = 50f;
                        float maxfirerate = (firerate * 2f);
                        if ((((Time.time - lastshottime) >= (1f / firerate))||(ShootControl != lastShootControl)) && (PistolAmmo > 0))
                        {
                            
                            float uf = Mathf.Clamp01(((Time.time - lastshottime) - (1f / maxfirerate))/((1f / firerate) - (1f / maxfirerate))); //1f if holding down normally, <1f if rapid firing
                            lastshottime = Time.time;
                            float noise = (1f-uf);
                            if (uf < 1f)
                            {
                                //Debug.Log("Spam: "+uf);
                                Vector3 crs = Vector3.Cross(dir.normalized,Vector3.forward);
                                float deltaangle = ((Mathf.PI*2f * (15f/360f))*(uf))*(Random.Range(-1f,1f));

                                dir = ((dir*Mathf.Cos(deltaangle)) + (crs*Mathf.Sin(deltaangle)));
                            }
                                Bullet b = GameObject.Instantiate(BulletPrefab, this.transform.position, new Quaternion()).GetComponent<Bullet>();//output.transform.
                            b.Damage = damage;
                            b.MyRigidbody.velocity = (new Vector2(dir.x, dir.y) * speed);
                            b.transform.Rotate(0f,0f,Vector3.SignedAngle(Vector3.right,new Vector3(b.MyRigidbody.velocity.normalized.x, b.MyRigidbody.velocity.normalized.y, 0f),Vector3.forward));
                            PistolAmmo--;
                            if (PistolShootSound==null)
                                PistolShootSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.PistolShoot,AudioManager.AM.EnvironmentAudioMixer,1f,1f,false);
                            PistolShootSound.pitch = 1f + ((Random.Range(-1f,1f) * .35f*noise));
                            PistolShootSound.Play();
                            

                            if (PistolAmmo < 10)
                                LastGunHUDAlertTime = Time.time;
                            //gunShock(dir.normalized*.1f);
                            
                        }

                        

                         
                    }
                    if ((Time.time - lastshottime) >= 3.5f)
                    {
                        PistolAmmo = MAXPISTOLAMMO;
                    }

                    break; }
                case SpecialWeapon.Shotgun: {
                        if (ShootControl)
                        {
                            //switch (weapon)
                            float firerate = 1.5f;
                            float damage = 200f;
                            float spreadangle = 45f;
                            float speed = 30f;

                            if (((Time.time - lastshottime) > (1f / firerate)) && (ShotgunAmmo > 0))
                            {
                                lastshottime = Time.time;


                                Bullet b = GameObject.Instantiate(BulletPrefab, this.transform.position, new Quaternion()).GetComponent<Bullet>();//output.transform.
                                b.Damage = damage;
                                int fragments = 7;

                            if (ShotgunSound == null)
                                ShotgunSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.ShotgunShoot, AudioManager.AM.EnvironmentAudioMixer, 1f, 1f, false);
                            ShotgunSound.Play();

                            int frags = 0;
                                b.Damage = (b.Damage / ((float)fragments));
                                Vector3 cross = Vector3.Cross(dir, Vector3.forward);
                                for (float f = -1f; f <= 1f; f += ((float)(((float)2) / ((float)(fragments - 1)))))
                                {


                                    float p = (((spreadangle * .5f * f) / 180f) * Mathf.PI);
                                    Vector3 projdir = ((cross * Mathf.Sin(p)) + (dir * Mathf.Cos(p)));
                                    Bullet bu;
                                    if ((frags == (fragments / 2)))
                                        bu = b;
                                    else
                                        bu = GameObject.Instantiate(b);
                                    bu.MyRigidbody.velocity = (new Vector2(projdir.x, projdir.y) * speed);
                                    bu.transform.Rotate(0f, 0f, Vector3.SignedAngle(Vector3.right, new Vector3(bu.MyRigidbody.velocity.x, bu.MyRigidbody.velocity.y, 0f), Vector3.forward));



                                frags++;
                                }
                                ShotgunAmmo--;
                            if (ShotgunAmmo < 5)
                                LastGunHUDAlertTime = Time.time;
                            if (ShotgunAmmo <= 0) setWeapon(SpecialWeapon.Pistol);
                        }

                        


                        }



                        break; }
                case SpecialWeapon.Gatling: {


                    if (ShootControl)
                    {
                        //switch (weapon)
                        float firerate = 10f;
                        float damage = 25f;
                        float spreadangle = 30f;
                        float speed = 50f;

                        if (((Time.time - lastshottime) > (1f / firerate)) && (GatlingAmmo > 0))
                        {
                            lastshottime = Time.time;


                            Bullet b = GameObject.Instantiate(BulletPrefab, this.transform.position, new Quaternion()).GetComponent<Bullet>();//output.transform.
                            b.Damage = damage;
                            int fragments = 1+ (((GatlingAmmo % 3) == 0)?1:0);
                            spreadangle = 15f;
                            int frags = 0;
                            b.Damage = (b.Damage / ((float)fragments));
                            Vector3 cross = Vector3.Cross(dir, Vector3.forward);
                            float prt = ((float)(((float)2) / ((float)(fragments))));
                            if (MachinegunLoop == null) 
                                MachinegunLoop = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.MachinegunShootLoop, AudioManager.AM.EnvironmentAudioMixer, 1f, 1f,true);
                            if (!MachinegunLoop.isPlaying)MachinegunLoop.Play();
                            MachinegunLoop.loop = true; 

                            for (float f = -1f; f < 1f; f += prt)
                            {

                                
                                //float p = (((spreadangle * .5f * f) / 180f) * Mathf.PI);
                                float p = (((spreadangle * .5f * (f + (prt * Random.value))) / 180f) * Mathf.PI);
                                Vector3 projdir = ((cross * Mathf.Sin(p)) + (dir * Mathf.Cos(p)));
                                Bullet bu;
                                if ((frags == (fragments / 2)))
                                    bu = b;
                                else
                                    bu = GameObject.Instantiate(b);
                                bu.MyRigidbody.velocity = (new Vector2(projdir.x, projdir.y) * speed*(.75f+(Random.value*.5f)));
                                bu.transform.Rotate(0f, 0f, Vector3.SignedAngle(Vector3.right, new Vector3(bu.MyRigidbody.velocity.x, bu.MyRigidbody.velocity.y, 0f), Vector3.forward));


                                frags++;
                            }
                            if (GatlingAmmo < 25)
                                LastGunHUDAlertTime = Time.time;
                            GatlingAmmo--;
                            if (GatlingAmmo <= 0) setWeapon(SpecialWeapon.Pistol);
                        } else
                        {
                            
                            
                        }



                    }
                    else
                    {
                        if (MachinegunLoop)
                            MachinegunLoop.loop = false;
                    }

                    break; }
                case SpecialWeapon.Laser: {



                    if (LaserAmmo <= 0) setWeapon(SpecialWeapon.Pistol);


                    if (ShootControl)
                    {
                        float deltatime = Time.deltaTime;
                        float damagepersecond = 200f;
                        float ammoconsumptionpersecond = 20f;
                        LaserAmmo = Mathf.Max(0f,LaserAmmo - (ammoconsumptionpersecond*Time.deltaTime));
                        if (LaserAmmo <= 30)
                            LastGunHUDAlertTime = Time.time;
                        LaserBeamRenderer.enabled = true;
                        if (!LaserMicroFlowParticles.isPlaying)LaserMicroFlowParticles.Play();
                        if (!LaserGlowFlowParticles.isPlaying) LaserGlowFlowParticles.Play();
                        Ray2D r = new Ray2D(this.transform.position, dir.normalized);
                        
                        float maxfiredist = 10f;
                        RaycastHit2D re = Physics2D.Raycast(r.origin, r.direction, maxfiredist, LayerMask.GetMask(new string[] { "Geometry"}));
                        //"Enemy","Boss"

                        


                        if (BeamRifleLoop == null)
                            BeamRifleLoop = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.BeamRifleLoop1, AudioManager.AM.EnvironmentAudioMixer, 1f, 1f, true);
                        if (BeamRiflePlusLoop == null)
                            BeamRiflePlusLoop = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.BeamRifleLoop2, AudioManager.AM.EnvironmentAudioMixer, 1f, 1f, true);
                        if (BeamRifleHitLoop == null)
                            BeamRifleHitLoop = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.BeamRifleHit, AudioManager.AM.EnvironmentAudioMixer, 1f, 1f, true);
                        if (BeamRifleDamageLoop == null)
                            BeamRifleDamageLoop = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.BeamRifleDamage, AudioManager.AM.EnvironmentAudioMixer, 1f, 1f, true);
                        if (BeamRifleStart == null)
                            BeamRifleStart = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.BeamRifleStart, AudioManager.AM.EnvironmentAudioMixer, 1f, 1f, false);
                        
                        

                        if (!lastShootControl)
                            BeamRifleStart.PlayOneShot(BeamRifleStart.clip);

                        if (!BeamRifleLoop.isPlaying) {  BeamRifleLoop.Play(); BeamRifleLoop.loop = true; }
                        BeamRifleLoop.pitch = 1f;
                        BeamRifleLoop.volume = 1f;

                        if (!BeamRiflePlusLoop.isPlaying) { BeamRiflePlusLoop.Play(); BeamRiflePlusLoop.loop = true; }
                        BeamRiflePlusLoop.pitch = 1f;
                        BeamRiflePlusLoop.volume = 1f;

                        if (!BeamRifleHitLoop.isPlaying) { BeamRifleHitLoop.Play(); BeamRifleHitLoop.loop = true; }
                        BeamRifleHitLoop.volume = 0f;

                        if (!BeamRifleDamageLoop.isPlaying) { BeamRifleDamageLoop.Play(); BeamRifleDamageLoop.loop = true; }
                        BeamRifleDamageLoop.volume = 0f;


                        if (re.collider != null)
                        {
                            //MyFlameBar.transform.LookAt(rh.point, Vector3.up);
                            LaserBeamRenderer.transform.LookAt(re.point);
                            LaserBeamRenderer.transform.Rotate(new Vector3(0f,-90f,0f));
                            LaserBeamRenderer.transform.localScale = new Vector3(LaserBaseScale * (re.distance / LaserBaseDistance), 1f, 1f);
                            maxfiredist = re.distance;
                           
                           
                            //MyFlameBar.transform.localScale = new Vector3(1f, 1f, rh.distance);
                            /*
                            ParticleSystem[] ps = MyFlameBar.GetComponentsInChildren<ParticleSystem>();
                            foreach (ParticleSystem p in ps)
                            {
                                ParticleSystem.ShapeModule m = p.shape;
                                m.position = new Vector3(0f, 0f, rh.distance * .5f);
                                m.scale = new Vector3(0f, 0f, rh.distance * 1f);
                            }
                            */
                            LaserDamageParticles.Play(true);
                            //MyFlameBar.MyCollider.transform.localScale = new Vector3(rh.distance, 1f, 1f);

                            BeamRifleDamageLoop.volume = 1f;
                        }
                        else
                        {
                            //No hits
                            LaserBeamRenderer.transform.LookAt(this.transform.position + dir, Vector3.up);
                            LaserBeamRenderer.transform.Rotate(new Vector3(0f, -90f, 0f));
                            LaserBeamRenderer.transform.localScale = new Vector3(LaserBaseScale*(maxfiredist/LaserBaseDistance),1f,1f);
                            LaserDamageParticles.Stop(true);
                            BeamRifleDamageLoop.volume = 0f;
                        }

                        RaycastHit2D[] rs = Physics2D.RaycastAll(r.origin, r.direction, maxfiredist, LayerMask.GetMask(new string[] { "Enemy", "Boss" }));
                        bool damagingtarget = false;
                        float pi = 0f;
                        foreach (RaycastHit2D rh in rs)
                        {
                            bool lh = false;
                            
                            GenericEnemy en = rh.collider.gameObject.GetComponent<GenericEnemy>();
                            float emag = rh.collider.bounds.extents.magnitude;
                            Vector2 difu = (new Vector2(rh.collider.transform.position.x, rh.collider.transform.position.y) - rh.point);
                            float pierce = ((Vector3.Project(new Vector3(difu.x, difu.y, 0f), r.direction) - new Vector3(difu.x, difu.y, 0f)).magnitude / emag);
                            float dmg = (damagepersecond * (.4f + (.6f * (1f - pierce))) * deltatime);
                            if ((en != null) && (en.Alive))
                            {
                                //PierceParticles
                                if (rh.collider.isTrigger) continue;
                                pi = Mathf.Max(pierce, pi);
                                en.TakeDamage(dmg, r.direction * 1f);
                                Astronaut.TheAstronaut.tickHitMarker(dmg, (en.Health / en.MaxHealth) * (en.Alive ? 1f : 0f), !en.Alive);
                                lh = true;
                            }
                            else
                            {
                                BossGolem bo = rh.collider.gameObject.GetComponent<BossGolem>();
                                if ((bo != null) && (!bo.Defeated))
                                {
                                    pi = Mathf.Max(pierce, pi);
                                    bo.TakeDamage(dmg, r.direction * 1f);
                                    //Astronaut.TheAstronaut.tickHitMarker(dmg, (bo.Health / bo.MaxHealth) * (!bo.Defeated ? 1f : 0f), bo.Defeated);
                                    lh = true;
                                }
                                else
                                {
                                    BreakableIceWall bi = rh.collider.gameObject.GetComponent<BreakableIceWall>();
                                    if ((bi != null) && (!bi.Alive))
                                    {
                                        pi = Mathf.Max(pierce, pi);
                                        bi.TakeDamage(dmg, r.direction * 1f);
                                        Astronaut.TheAstronaut.tickHitMarker(dmg, (bi.Health / bi.MaxHealth) * (bi.Alive ? 1f : 0f), !bi.Alive);
                                        lh = true;
                                    }
                                }
                            }
                            if (lh)
                            {
                                Vector3 opos = LaserDamageParticles.transform.position;
                                LaserDamageParticles.transform.position = rh.point;
                                foreach (ParticleSystem ps in LaserDamageParticles.GetComponentsInChildren<ParticleSystem>())
                                {
                                    ps.Emit(2);
                                }
                                BeamRifleHitLoop.volume = .3f;
                                BeamRifleHitLoop.pitch = (1f + (pi * .2f));
                                LaserDamageParticles.transform.position = opos;
                            }
                        }

                        
                        
                            BeamRifleDamageLoop.volume = ((damagingtarget) ? 1f:0f);
                        
                    } else
                    {
                        //Stop Firing
                        if (lastShootControl)
                        {
                            if (BeamRifleLoop)
                            {
                                AudioManager.AM.crossfade(BeamRifleLoop, 0f, 1f);
                                AudioManager.AM.crosstune(BeamRifleLoop, .9f, 1f);
                            }
                            if (BeamRifleHitLoop)
                            {
                                AudioManager.AM.crossfade(BeamRifleHitLoop, 0f, 1f);
                                BeamRifleHitLoop.volume = 0f;
                            }
                            
                            if (BeamRiflePlusLoop)
                            {
                                AudioManager.AM.crossfade(BeamRiflePlusLoop, 0f, .5f);
                            }

                            if (BeamRifleHitLoop)
                            {
                                BeamRifleHitLoop.volume = 0f;
                            }
                            if (BeamRifleDamageLoop)
                            {
                                BeamRifleDamageLoop.volume = 0f;
                            }

                        }
                        LaserBeamRenderer.enabled = false;
                        if (LaserMicroFlowParticles.isPlaying) LaserMicroFlowParticles.Stop();
                        if (LaserGlowFlowParticles.isPlaying) LaserGlowFlowParticles.Stop();
                        LaserDamageParticles.Stop(true);
                    }



                        break; }

            case SpecialWeapon.GrenadeLauncher:
                {
                    
                        if (ShootControl)
                        {
                            //switch (weapon)
                            float firerate = 1.5f;
                            float damage = 150f;
                            float speed = 30f;

                            if (((Time.time - lastshottime) > (1f / firerate)) && (GrenadeLauncherAmmo > 0))
                            {
                                lastshottime = Time.time;

                                /*
                                Grenade b = GameObject.Instantiate(GrenadePrefab, this.transform.position, new Quaternion()).GetComponent<Grenade>();//output.transform.
                                
                            
                                if (GrenadeLauncherSound == null)
                                GrenadeLauncherSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.GrenadeLauncherShoot, AudioManager.AM.EnvironmentAudioMixer, 1f, 1f, false);
                                GrenadeLauncherSound.Play();
                                */
                                
                                
                                GrenadeLauncherAmmo--;
                                if (GrenadeLauncherAmmo < 5)
                                    LastGunHUDAlertTime = Time.time;
                                if (GrenadeLauncherAmmo <= 0) setWeapon(SpecialWeapon.Pistol);
                            }




                        }



                        break;
                    
                }
            case SpecialWeapon.TeslaGun:
                {

                    if (ShootControl)
                    {

                    }

                    break;
                }

                    }

        if (CurrentSpecialWeapon != SpecialWeapon.Laser)
        {
            LaserBeamRenderer.enabled = false;
            if (LaserMicroFlowParticles.isPlaying) LaserMicroFlowParticles.Stop();
            if (LaserGlowFlowParticles.isPlaying) LaserGlowFlowParticles.Stop();
            LaserDamageParticles.Stop(true);
        }

            
		if ((Time.time - LastElementalTime) >= 3f) {
            //Debug.Log("Regen");
			float rechargetime = 10f;
			ElementalEnergy = (ElementalEnergy+((100f*Time.deltaTime)/rechargetime));
		}

        
		//Do we want more abilities per element?
        switch (ElementMode)
        {
            case Element.Fire:
                {

                    //Discrete Boosted Powers!
                    //0: Burn and Melt
                    //1: Expanding Inferno
                    //2: Terrain Scorching
                    //3: Hyper Convection
                    float powerlevel = ((float)FireElementPowerLevel / 10f);
                    //Inferno Bar
                    if (MyFlameBar) MyFlameBar.FirePowerLevel = powerlevel;
                    if ((ElementalControl) && (ElementalEnergy > 0f)) {
                        float selfmeltdist = .1f;
                        if (!Frozen)
                        {
                        ElementalEnergy = (ElementalEnergy - ((20f+(40f*(1f-powerlevel))) * Time.deltaTime));
                        LastElementalTime = Time.time;

                        Ray2D r = new Ray2D(this.transform.position, dir.normalized);
                        
                        float maxfiredist = (4f+(8f*powerlevel));
                        
                        RaycastHit2D rh = Physics2D.Raycast(r.origin, r.direction, maxfiredist, LayerMask.GetMask(new string[] { "Geometry" }));
                        if (rh.collider != null)
                        {
                            MyFlameBar.transform.LookAt(rh.point, Vector3.up);
                            //MyFlameBar.transform.localScale = new Vector3(1f, 1f, rh.distance);
                            ParticleSystem[] ps = MyFlameBar.GetComponentsInChildren<ParticleSystem>();
                            foreach (ParticleSystem p in ps)
                            {
                                ParticleSystem.ShapeModule m = p.shape;
                                m.position = new Vector3(0f, 0f, rh.distance * .5f);
                                m.scale = new Vector3(0f, 0f, rh.distance * 1f);
                            }
                            MyFlameBar.MyCollider.transform.localScale = new Vector3(rh.distance, 1f, 1f);


                        }
                        else
                        {
                            MyFlameBar.transform.LookAt(this.transform.position + dir, Vector3.up);
                            ParticleSystem[] ps = MyFlameBar.GetComponentsInChildren<ParticleSystem>();
                            foreach (ParticleSystem p in ps)
                            {
                                ParticleSystem.ShapeModule m = p.shape;
                                m.position = new Vector3(0f, 0f, maxfiredist * .5f);
                                m.scale = new Vector3(0f, 0f, maxfiredist * 1f);
                            }
                            MyFlameBar.MyCollider.transform.localScale = new Vector3(maxfiredist, 1f, 1f);
                        }

                            MyFlameBar.FlameActive = true;

                        } else
                        {
                            MyFlameBar.transform.LookAt(this.transform.position + dir, Vector3.up);
                            ParticleSystem[] ps = MyFlameBar.GetComponentsInChildren<ParticleSystem>();
                            foreach (ParticleSystem p in ps)
                            {
                                ParticleSystem.ShapeModule m = p.shape;
                                m.position = new Vector3(0f, 0f, selfmeltdist * .5f);
                                m.scale = new Vector3(0f, 0f, selfmeltdist * 1f);
                            }
                            MyFlameBar.MyCollider.transform.localScale = new Vector3(selfmeltdist, 1f, 1f);
                            MyFlameBar.FlameActive = false;
                            FreezeTime -= (Time.deltaTime*2f);
                            if (MeltParticles)
                            {
                                MeltParticles.Emit(3);
                            }
                        }


                        if (!MyFlameBar.MyParticles.isPlaying) {
						MyFlameBar.MyParticles.Play(true);
					    }



                        
                    } else {
                        MyFlameBar.FlameActive = false;
                        if (MyFlameBar.MyParticles.isPlaying)
                            MyFlameBar.MyParticles.Stop (true);
				}
                    if (WaterPump.isPlaying)
                    {
                        WaterPump.Stop();
                        //WaterPump.Clear();
                    }
                    if (FrostBlower)
                        if (FrostBlower.isPlaying)
                            FrostBlower.Stop(true);
                    if (MyVines2 != null)
                    {
                        GrassSwinging = true;
                        MyVines2.Sustaining = false; //GameObject.Destroy(MyVines2.gameObject);
                        MyVines2.ControlPush = ((AimPosition - transform.position).magnitude >= 2f);
                    }
                    else
                    {
                        GrassSwinging = false;
                    }
                    break;
                }
            case Element.Water:
                {
					//High Pressure Deluge

				if ((ElementalControl) && (ElementalEnergy > 0f)) { 
					ElementalEnergy = (ElementalEnergy - (10f * Time.deltaTime));
                        WaterPump.transform.LookAt(dir, Vector3.up);
                        LastElementalTime = Time.time;
                        if (!WaterPump.isPlaying)
                        {
                            WaterPump.Play();
                        }

				} else {
                        if (WaterPump.isPlaying)
                        {
                            WaterPump.Stop();
                            //WaterPump.Clear();
                        }
				}

                    if (MyFlameBar.MyParticles.isPlaying)
                        MyFlameBar.MyParticles.Stop(true);
                    if (FrostBlower)
                        if (FrostBlower.isPlaying)
                            FrostBlower.Stop(true);
                    if (MyVines2 != null)
                    {
                        GrassSwinging = true;
                        MyVines2.Sustaining = false; //GameObject.Destroy(MyVines2.gameObject);
                    }
                    else
                    {
                        GrassSwinging = false;
                    }
                    break;
                }
            case Element.Ice:
                {
                    //Discrete Boosted Powers!
                    //0: Freeze Enemies
                    //1: Permafrost Hazards
                    //2: Intercept and Fortify (Ice Casts can block projectiles Create Ice Pillars to block many shots)
                    //3: Blizzard Field (Freeze everything nearby)
                    //Ultimate: Cryo Nova
                    float powerlevel = ((float)IceElementPowerLevel / 10f);

                    if (true) {
                        if ((ElementalControl) && (ElementalEnergy > 0f))
                        {
                            ElementalEnergy = (ElementalEnergy - (10f * Time.deltaTime));
                            LastElementalTime = Time.time;

                            if (MyHeldIceBomb == null)
                            {
                                MyHeldIceBomb = GameObject.Instantiate(IceBombPrefab, this.transform.position, IceBombPrefab.transform.rotation).GetComponent<IceBomb>();
                                MyHeldIceBomb.IcePowerLevel = IceElementPowerLevel;
                                MyHeldIceBomb.CanFreezeEnemies = (IceElementPowerLevel >= 0);
                                MyHeldIceBomb.CanFreezeHazards = (IceElementPowerLevel >= 1);
                                MyHeldIceBomb.FormsIcePillar = (IceElementPowerLevel >= 2);
                                MyHeldIceBomb.BlizzardField = (IceElementPowerLevel >= 3);
                                
                            }
                            MyHeldIceBomb.MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
                            MyHeldIceBomb.transform.position = this.transform.position;
                            //Hold and throw an Ice Bomb

                        } else
                        {
                            if (!Frozen)
                            {

                                if (MyHeldIceBomb != null)
                                {
                                    ElementalEnergy = (ElementalEnergy - 20f);
                                    MyHeldIceBomb.Launch(dir * 10f);
                                    //MyHeldIceBomb.transform.position = this.transform.position;

                                    
                                    //MyHeldIceBomb.MyRigidbody.velocity = ;

                                }
                            } else
                            {
                                if (MyHeldIceBomb != null)
                                MyHeldIceBomb.Remove();
                                
                            }
                            MyHeldIceBomb = null;

                        }

                        } else if (true) //Use new Ice power or Construct old Ice Pillars
                        {

                        if ((ElementalControl) && (ElementalEnergy > 0f))
                        {

                            LastElementalTime = Time.time;
                            //Ray Cast
                            Ray2D r = new Ray2D(this.transform.position, Vector3.down);

                            float maxicedist = 5f;
                            RaycastHit2D rh = Physics2D.Raycast(r.origin, r.direction, maxicedist, LayerMask.GetMask(new string[] { "Geometry" }));
                            if (rh.collider != null)
                            {

                                IceBlock ice = rh.collider.gameObject.GetComponentInParent<IceBlock>();

                                if (ice != null)
                                {
                                    ice.LastTickTime = Time.time;
                                    ice.transform.localScale = new Vector3(ice.transform.localScale.x, ice.transform.localScale.y, ice.transform.localScale.z + ((5f+(10f*powerlevel)) * Time.deltaTime));
                                    ElementalEnergy = (ElementalEnergy - ((10f+(30f*(1f-powerlevel))) * Time.deltaTime));
                                    ParticleSystem[] pe = ice.gameObject.GetComponentsInChildren<ParticleSystem>();
                                    foreach (ParticleSystem p in pe)
                                    {
                                        if (!p.isPlaying)
                                        {
                                            p.Play();
                                        }
                                    }

                                }
                                else
                                {
                                    float l = Mathf.Sign((AimPosition - this.transform.position).x);
                                    if (l == 0f) l = 1f;
                                    ice = GameObject.Instantiate(IceBlockPrefab, rh.point - (Vector2.up * .4f), Quaternion.LookRotation(Vector3.right * l, Vector3.up)).GetComponent<IceBlock>();

                                }
                                ParticleSystem[] ps = ice.GetComponentsInChildren<ParticleSystem>();
                                foreach (ParticleSystem p in ps)
                                {
                                    ParticleSystem.ShapeModule m = p.shape;
                                    m.position = new Vector3(0f, 0f, ice.transform.localScale.z * .5f);
                                    //m.scale = new Vector3(0f, 0f, ice.transform.localScale.z * 1f);
                                }

                            }
                        } else
                        {

                        }
                        }
                        else if (true) //Create an Ice Pillar
                        {

                            if ((ElementalControl) && (ElementalEnergy > 0f))
                            {
                                ElementalEnergy = (ElementalEnergy - (25f * Time.deltaTime));
                                LastElementalTime = Time.time;
                                //Ray Cast
                                Ray2D r = new Ray2D(this.transform.position, dir.normalized);

                                float maxicedist = 15f;
                                RaycastHit2D rh = Physics2D.Raycast(r.origin, r.direction, maxicedist, LayerMask.GetMask(new string[] { "Geometry" }));
                                if (rh.collider != null)
                                {
                                    IceBlock ice = rh.collider.gameObject.GetComponentInParent<IceBlock>();
                                    if (ice != null)
                                    {
                                        ice.LastTickTime = Time.time;
                                        ice.transform.localScale = new Vector3(ice.transform.localScale.x, ice.transform.localScale.y, ice.transform.localScale.z + (5f * Time.deltaTime));

                                    }
                                    else
                                    {
                                        ice = GameObject.Instantiate(IceBlockPrefab, rh.point, Quaternion.LookRotation((this.transform.position - new Vector3(rh.point.x, rh.point.y, 0f)).normalized, Vector3.up)).GetComponent<IceBlock>();

                                    }
                                    ParticleSystem[] ps = ice.GetComponentsInChildren<ParticleSystem>();
                                    foreach (ParticleSystem p in ps)
                                    {
                                        ParticleSystem.ShapeModule m = p.shape;
                                        m.position = new Vector3(0f, 0f, ice.transform.localScale.z * .5f);
                                        //m.scale = new Vector3(0f, 0f, ice.transform.localScale.z * 1f);
                                    }

                                }
                                else
                                {
                                    //Miss.

                                }

                                if (FrostBlower)
                                    if (!FrostBlower.isPlaying)
                                        FrostBlower.Play(true);

                            }
                            else
                            {
                                if (FrostBlower)
                                    if (!FrostBlower.isPlaying)
                                        FrostBlower.Stop(true);
                            }

                        } else if (false)
                        {
                        //Charge and Hold an Ice Bomb


                        }
                    

                            if (MyFlameBar.MyParticles.isPlaying)
                        MyFlameBar.MyParticles.Stop(true);
                    if (WaterPump.isPlaying)
                    {
                        WaterPump.Stop();
                        //WaterPump.Clear();
                    }
                    if (MyVines2 != null)
                    {
                        GrassSwinging = true;
                        MyVines2.Sustaining = false; //GameObject.Destroy(MyVines2.gameObject);
                    }
                    else
                    {
                        GrassSwinging = false;
                    }
                    break;
                }

            case Element.Grass:
                {

                    //Discrete Boosted Powers!
                    //0: Grappling Vines
                    //1: Enemy Grabbing
                    //2: Absorbing
                    //3: Cluster Binding

                    float powerlevel = ((float)GrassElementPowerLevel / 10f);
                    //Use vines to swing or grab


                    if ((ElementalControl) && (ElementalEnergy > 0f))
                    {
                        
                        LastElementalTime = Time.time;


                        //Grab an enemy if you can
                        GrassSwinging = true;

                        if (MyVines2 == null)
                        {
                            //Shoot a Ray
                            MyVines2 = GameObject.Instantiate(VinePrefab2, this.transform.position, VinePrefab2.transform.rotation).GetComponent<Vines2>();
                            MyVines2.TravelDirection = dir;
                            MyVines2.MyAstronaut = this;
                            /*
                            Ray2D r = new Ray2D(this.transform.position, dir.normalized);

                            float maxvinedist = 15f;
                            RaycastHit2D rh = Physics2D.Raycast(r.origin, r.direction, maxvinedist, LayerMask.GetMask(new string[] { "Geometry" }));
                            if (rh.collider != null)
                            {

                            } else
                            {

                            }
                            */
                        } else
                        {
                            if (MyVines2.Attached)
                            {
                                ElementalEnergy = (ElementalEnergy - ((20f+(30f*((1f-powerlevel)))) * Time.deltaTime));
                                GrassSwinging = true;
                            } else
                            {
                                GrassSwinging = false;
                            }
                            MyVines2.Sustaining = true;
                        }



                    } else
                    {
                        
                        if (MyVines2 != null)
                        {
                            GrassSwinging = true;
                            MyVines2.Sustaining = false; //GameObject.Destroy(MyVines2.gameObject);
                        } else
                        {
                            GrassSwinging = false;
                        }
                    }


                        if (MyFlameBar.MyParticles.isPlaying)
                        MyFlameBar.MyParticles.Stop(true);
                    if (WaterPump.isPlaying)
                    {
                        WaterPump.Stop();
                        //WaterPump.Clear();
                    }
                    break;
                }
            
            default:
                {
                    if (MyFlameBar.MyParticles.isPlaying)
                        MyFlameBar.MyParticles.Stop(true);
                    if (WaterPump.isPlaying)
                    {
                        WaterPump.Stop();
                        //WaterPump.Clear();
                    }
                    if (FrostBlower)
                        if (FrostBlower.isPlaying)
                            FrostBlower.Stop(true);
                    if (MyVines2 != null)
                    {
                        GrassSwinging = true;
                        MyVines2.Sustaining = false; //GameObject.Destroy(MyVines2.gameObject);
                    }
                    else
                    {
                        GrassSwinging = false;
                    }
                    break;
                }

        }
		ElementalEnergy = Mathf.Clamp(ElementalEnergy,0f,100f);
        if ((MyVines2 != null) && (!MyVines2.gameObject.activeInHierarchy))
        {
            MyVines2 = null;
        }


        //Update the Background
        //BackgroundStartX = 0f;

        if (usingJungleBackgroundSystem)
        {

            JungleBranchesBackground.material.mainTextureOffset = new Vector2((MyCameraRig.position.x - BackgroundStartX) / BackgroundRepeatDistance, 0f);
            JungleSwampBackground.material.mainTextureOffset = new Vector2((MyCameraRig.position.x - BackgroundStartX) / BackgroundRepeatDistance, 0f);
            JungleTreeTopsBackground.material.mainTextureOffset = new Vector2((MyCameraRig.position.x - BackgroundStartX) / BackgroundRepeatDistance, 0f);

        }
        else
        {
            InteriorBackground.material.mainTextureOffset = new Vector2((MyCameraRig.position.x - BackgroundStartX) / BackgroundRepeatDistance, 0f);
            ExteriorBackground.material.mainTextureOffset = new Vector2((MyCameraRig.position.x - BackgroundStartX) / BackgroundRepeatDistance, 0f);

            //InteriorBackground.material.mainTextureOffset = new Vector2((MyCameraRig.position.x - BackgroundStartX) / BackgroundRepeatDistance, 0f);
            //ExteriorBackground.material.mainTextureOffset = new Vector2((MyCameraRig.position.x - BackgroundStartX) / BackgroundRepeatDistance, 0f);
            if (UsingInteriorBackground)
            {
                InteriorBackground.enabled = true;
                ExteriorBackground.enabled = false;

            }
            else
            {

                InteriorBackground.enabled = false;
                ExteriorBackground.enabled = true;
            }
        }
        
        
        lastShootControl = ShootControl;

    }
    public void setJungleBackground(int locationindex)
    {
        JungleSwampBackground.enabled = false;
        JungleBranchesBackground.enabled = false;
        JungleTreeTopsBackground.enabled = false;
        
        if (locationindex == 0)
        {
            //swamp
            JungleSwampBackground.enabled = true;
            
        } else if (locationindex == 1)
        {
            //branches
            JungleBranchesBackground.enabled = true;
            
            AudioManager.AM.playMusic(AudioManager.AM.JungleBranchesMusic,0f,1f,true);
            AudioManager.AM.crossfade(AudioManager.AM.CurrentMusic, 1f, 1f);
            Weather_JungleSwampRain.Stop();
            //Weather_JungleLeaves.Play();
        } else if (locationindex == 2)
        {
            //tree tops
            JungleTreeTopsBackground.enabled = true;

        }
    }
    private bool lastShootControl=false;
    public Image BossHealthBar;

    
	public FlameBar MyFlameBar,BurningPrefab;
    public IceBomb IceBombPrefab;
    public IceBomb MyHeldIceBomb = null;
    
    public Vines VinePrefab;
    private Vines MyVines;
    private Vines2 MyVines2;
    public Vines2 VinePrefab2;


    public ParticleSystem WaterPump;
    public ParticleSystem FrostBlower;
    public IceBlock IceBlockPrefab;
    private bool GrassSwinging = false;
    public Transform MyCameraRig;
    public float CamSize = 1f;
    public float CamShakeTime = -10f; //since the last cam shake call.
    public float CamShakeDropOffThreshold = 1f;//Time when the cam shake begins to drop off.
    public float CamShakeMagnitude = 0f; //How far should the cam shake?
    public float CamShakeXFactor = 1f; 
    public float CamShakeYFactor = 1f;
    private bool usingJungleBackgroundSystem = false;
    public MeshRenderer ExteriorBackground, InteriorBackground;
    public MeshRenderer JungleSwampBackground, JungleBranchesBackground, JungleTreeTopsBackground;
    private float BackgroundStartX = 0f;
      private float BackgroundRepeatDistance = 100f;
    public bool UsingInteriorBackground;

    public Image MyHealthBar, MyEnergyBar,MyElementalIcon,MyWeaponImage, BossHealthFrame;
    public Text DeathCountText; 
    public GameObject DeathsOverlay, ReviveGroup;
    public GameObject ElementAcquiredText, ElementAcquiredLevelText, ElementAcquiredPowerLevelGroup;
    public Text ElementAcquiredPowerLevelText;
    public Image ElementAcquiredPowerLevelOrbs;
    public Image AcquiredBarValue;
    private int AcquiredBarLevelIndex; //Its the animation

    public Image ReviveBar;
    public BossGolem MyBossGolem = null;
    public Sprite IceBarSprite, FireBarSprite, GrassBarSprite, VoidBarSprite;
    public Image BlackFade;
    public GameObject FadeCanvas;
    public Image StimVignette,PainVignette,ExtremeVignette;
    private float BlackFadeAlpha = 1f;
    public bool FadingInOut = true;
    public Text MyWeaponAmmoText;
    public Sprite ElementIcon_Fire, ElementIcon_Ice, ElementIcon_Water, ElementIcon_Grass, ElementIcon_None;

    private float LastElementalTime = -10f;

    private float LastElementRotateTime = -10f;
    public ParticleSystem TeleportationParticles;
    private bool TeleportedOut = false;
    private float TeleportedTime = -10f;
    public void TeleportOut()
    {
        TeleportationParticles.Play();
        MySpriteRenderer.gameObject.SetActive(false);
        TeleportedOut = true;
        TeleportedTime = Time.time;
    }
    public void rotateElement(bool nextprevious)
    {
        if ((Time.time - LastElementRotateTime) <= .25f) return;
        if (ElementMode == Element.None)
        {
            
            if (HasElementFire)
                ElementMode = Element.Fire;
            else if (HasElementIce)
                ElementMode = Element.Ice;
            else if (HasElementGrass)
                ElementMode = Element.Grass;
        }

        switch (ElementMode)
        {
            case Element.Fire: {

                    if (nextprevious)
                    {
                        if (hasElement(Element.Ice))
                        {
                            ElementMode = Element.Ice;
                        }
                        else if (hasElement(Element.Grass))
                        {
                            ElementMode = Element.Grass;
                        }
                    } else
                    {
                        if (hasElement(Element.Grass))
                        {
                            ElementMode = Element.Grass;
                        }
                        else if (hasElement(Element.Ice))
                        {
                            ElementMode = Element.Ice;
                        }
                    }
                    


                    break; }
            case Element.Ice: {
                    if (nextprevious)
                    {
                        if (hasElement(Element.Grass))
                        {
                            ElementMode = Element.Grass;
                        }
                        else if (hasElement(Element.Fire))
                        {
                            ElementMode = Element.Fire;
                        }
                    } else
                    {
                        if (hasElement(Element.Fire))
                        {
                            ElementMode = Element.Fire;
                        }
                        else if (hasElement(Element.Grass))
                        {
                            ElementMode = Element.Grass;
                        }
                    }
                    break; }
            case Element.Grass: {
                    if (nextprevious)
                    {
                        if (hasElement(Element.Fire))
                        {
                            ElementMode = Element.Fire;
                        }
                        else if (hasElement(Element.Ice))
                        {
                            ElementMode = Element.Ice;
                        }
                    } else
                    {
                        if (hasElement(Element.Ice))
                        {
                            ElementMode = Element.Ice;
                        }
                        else if (hasElement(Element.Fire))
                        {
                            ElementMode = Element.Fire;
                        }
                    }

                    break; }
        }

        LastElementRotateTime = Time.time;
    }

    public bool hasElement(Element e)
    {

        switch (e)
        {
            case Element.Fire:{ return HasElementFire; }
            case Element.Ice: { return HasElementIce; }
            case Element.Grass: { return HasElementGrass; }
            case Element.All: { return HasElementAll; }

        }

        return false;
    }
    
    private void FixedUpdate()
    {
        //MyRigidbody.velocity = new Vector2(0f, 1f);
        //return;

        bool stunned = false;
        if ((Time.time - lastStunTime) <= .4f)
        {
            stunned = true;
        }

        int lc = 0;
        int vc = 0;
        bool jc = false;
        bool ec = false;
        bool sp = false;
        if ((Alive) && (PlayerHasControl))
        {
            //if ((!GrassSwinging) && (!Frozen))
            
                lc = ((Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0));

                vc = ((Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0));

                jc = (Input.GetKey(KeyCode.Space));
                if (!Frozen)
                sp = Input.GetMouseButton(0);
            
                ec = Input.GetMouseButton(1);
            
           
            Ray pr = Camera.main.ScreenPointToRay(Input.mousePosition);

            AimPosition = (pr.origin - (pr.direction * (pr.origin.z / pr.direction.z)));
            ReticleTransform.position = AimPosition;
            ReticleTransform.gameObject.SetActive(true);
            if (!Frozen)
            JumpControl = jc;
        } else
        {
            ReticleTransform.gameObject.SetActive(false);
        }

        HitMarkerStep();
        
            LateralControl = lc;
        VerticalControl = vc;
        
        JumpNow = (JumpControl && !jumppress);
        
        ShootControl = sp;
        ElementalControl = ec;

        attackStep();

        ElementalPress = ec;
        ShootPress = ShootControl;





        jumppress = JumpControl;

        if (lc != 0)
        {
            if(!stunned)LookDirection = ((LateralControl < 0) ? -1 : 1);
            if ((!stunned) && (!Frozen))
            if (Mathf.Sign(MySpriteRenderer.transform.localScale.x) != LookDirection)
                MySpriteRenderer.transform.localScale = new Vector3(-MySpriteRenderer.transform.localScale.x, MySpriteRenderer.transform.localScale.y, MySpriteRenderer.transform.localScale.z);//(LookDirection < 0);
        }
        MyAnimator.SetInteger("LookDirection", lc);
        MyAnimator.SetBool("Dead", !Alive);




        if (Airborne)
        {
            if ((!GrassSwinging) && (!Frozen))
                if (!stunned) MyRigidbody.velocity = new Vector2(((LateralControl != 0)? (LateralControl * WalkSpeed):MyRigidbody.velocity.x), MyRigidbody.velocity.y); //AddForce(new Vector2(0f,0f),ForceMode2D.);
        } else
        {
            
            if ((!GrassSwinging) && (!Frozen))
                if (!stunned) MyRigidbody.velocity = new Vector2(LateralControl * WalkSpeed, MyRigidbody.velocity.y); //AddForce(new Vector2(0f,0f),ForceMode2D.);
            RaycastHit2D rhdetect = Physics2D.Raycast(new Vector2(transform.position.x,transform.position.y),new Vector2(0f,-1f),2f,LayerMask.GetMask(new string[] { "Geometry"}));
            if ((rhdetect.collider != null) &&(!Physics2D.GetIgnoreCollision(rhdetect.collider, MyCollider))) {
                LastGroundedLocation = this.transform.position;
            }
        }
        if (JumpControl)
        {
            if (JumpNow)
            if (!Airborne || !UsedDoubleJump)
            {
                MyRigidbody.velocity = new Vector2(MyRigidbody.velocity.x, JumpSpeed);

                    if (Airborne)
                    {
                        UsedDoubleJump = true;
                        if ((Time.time - VineJumpTime) >= (Time.fixedDeltaTime*2f))
                        {
                            JetPackParticles.Play(true);
                            if (DoubleJumpSound == null)
                                DoubleJumpSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.DoubleJumpSound, AudioManager.AM.EnvironmentAudioMixer, 1f, 1f, false);
                        }
                        DoubleJumpSound.Play();
                    } else
                    {
                        if (JumpSound == null)
                            JumpSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.JumpSound, AudioManager.AM.EnvironmentAudioMixer, 1f, 1f, false);
                        JumpSound.Play();
                    }
                
            }
        } else
        {
            if (Airborne)
            {
                if (MyRigidbody.velocity.y > 0f)
                MyRigidbody.velocity = new Vector2(MyRigidbody.velocity.x, MyRigidbody.velocity.y*JumpCancelRate);
            }
        }


        
        if (Mathf.Abs(MyRigidbody.velocity.y) > .2f) {
            RaycastHit2D rh2d = Physics2D.CapsuleCast(this.transform.position, MyCollider.size, MyCollider.direction, 0f, new Vector2(0f, .2f));
            if ((rh2d.distance <= 0f))//no hit
            {

                Airborne = true;

            } 
        }
        MyAnimator.SetBool("Airborne", Airborne);

        Vector3 rigtargpos = new Vector3(transform.position.x, transform.position.y+.5f,MyCameraRig.transform.position.z);

        if (!Introduced)
        {
            rigtargpos = new Vector3(transform.position.x, MyCameraRig.transform.position.y, MyCameraRig.transform.position.z);

        }

        if (WatchingBossLocation)
        {
            rigtargpos = WatchingBossLocation.position;
        }

        if (Alive)
        {
            float t = ((Time.time - lastDamageTakenTime) / 2f);
            if (t < 1f)
            {
                MySpriteRenderer.color = Color.Lerp(Color.white, Color.red, Mathf.Round((Mathf.Sin((t * 12) * Mathf.PI)+1f)/2f));
                //MySpriteRenderer.gameObject.SetActive();

                
            } else
            {
                //MySpriteRenderer.gameObject.SetActive(true);
                //MySpriteRenderer.color = Color.white;
                MySpriteRenderer.color = Color.white;
            }

            if ((Frozen) && ((Time.time - FreezeTime) >=0f))
            {
                unfreeze();
            }
            
            
        } else
        {
            MySpriteRenderer.color = Color.red;
        }

        vineStep();
        int acst = numberOfStimsActive();
        float stal = StimVignette.color.a;
        float sttg = (.2f*acst);
        if (acst > 0) sttg = 1.0f; else sttg = 0.0f;
        float ad = (sttg - stal);
        float dl = (Time.deltaTime / 3f);
        if (Mathf.Abs(ad) < dl)
        {

            StimVignette.color = new Color(StimVignette.color.r, StimVignette.color.g, StimVignette.color.b,sttg);
        } else
        {
            StimVignette.color = new Color(StimVignette.color.r, StimVignette.color.g, StimVignette.color.b, stal + (dl * Mathf.Sign(ad)));
        } 


        if (usingVerticalBackgroundThreshold)
        {
            UsingInteriorBackground = (MyCameraRig.transform.position.y < backgroundThreshold);
        } else if (usingHorizontalBackgroundThreshold)
        {
            UsingInteriorBackground = (MyCameraRig.transform.position.x > backgroundThreshold);
        } else if (usingVerticalBackgroundThresholdInverted)
        {
            UsingInteriorBackground = (MyCameraRig.transform.position.y > backgroundThreshold);
        }
        else if (usingHorizontalBackgroundThresholdInverted)
        {
            UsingInteriorBackground = (MyCameraRig.transform.position.x < backgroundThreshold);
        }
        if ((Time.time - backgroundTime) >= 5f) {
            usingVerticalBackgroundThreshold = false;
            usingVerticalBackgroundThresholdInverted = false;
            usingHorizontalBackgroundThreshold = false;
            usingHorizontalBackgroundThresholdInverted = false;
        }
        //MyAnimator.SetBool("Airborne", false);
        //MyAnimator.SetBool("LookDirection", false);

        if (FallingIntoPit)
        {
            rigtargpos = new Vector3(rigtargpos.x, MyCameraRig.transform.position.y,rigtargpos.z);
        }
        CameraRigCurrentPosition = Vector3.Lerp(CameraRigCurrentPosition, rigtargpos,Mathf.Clamp01(Time.fixedDeltaTime*(Slipping?20f:(Introduced ? 3f:1f))));
        
        MyCameraRig.transform.position = CameraRigCurrentPosition;
        float ct = CamShakeTime - Time.time;
        if (ct > 0)
        {
            float m = CamShakeMagnitude;
            m = (m * Mathf.Clamp01(ct / Mathf.Max(CamShakeDropOffThreshold,0.001f)));
            MyCameraRig.transform.position = MyCameraRig.transform.position + (new Vector3(CamShakeXFactor*Random.value, CamShakeYFactor * Random.value, 0f)*m);
        }

        MyCanvas.gameObject.SetActive(PlayerHasControl);


    }

    public Canvas MyCanvas;
    public void addCamShake(float magnitude,float Xfactor,float Yfactor,float duration,float dropoff)
    {
        float ct = CamShakeTime - Time.time;
        if (ct > 0)
        {
            float m = CamShakeMagnitude;
            m = (m * Mathf.Clamp01(ct / Mathf.Max(CamShakeDropOffThreshold, 0.001f)));
            if (((duration > ct) || (magnitude > m)))
            {
                return;
            }
        }
        CamShakeTime = Time.time+duration; //since the last cam shake call.
    CamShakeDropOffThreshold = dropoff;//Time when the cam shake begins to drop off.
    CamShakeMagnitude = magnitude; //How far should the cam shake?
    CamShakeXFactor = Xfactor;
    CamShakeYFactor = Yfactor;
    }
    private Vector3 CameraRigCurrentPosition;
    public bool Frozen = false;
    private float FreezeTime = -10f;
    public SpriteRenderer FreezeSprite;
    public ParticleSystem FreezeFlashEffect;
    public ParticleSystem MeltParticles;
    public float ThawFactor = 0f;
    public void freeze(float dur)
    {
        
        ThawFactor = 0f;
        FreezeTime = Time.time + dur;
        if (Frozen) return;
            Frozen = true;
        MyAnimator.enabled = false;
        FreezeSprite.enabled = true;
        
        this.MyRigidbody.velocity = new Vector2(0f,0f);
        ThawFactor = 0f;
    }
    public void unfreeze()
    {
        MyAnimator.enabled = true;
        Frozen = false;
        FreezeSprite.enabled = false;
        ThawFactor = 0f;
        UnfreezeTime = Time.time;
    }
    public float UnfreezeTime = -10f;
    public void thaw()
    {
        //Attempt to Unfreeze manually
        float t = (Time.time - FreezeTime);
        float numberoftapstothaw = (3f / .3f +Mathf.Max(.001f,t));
        float th = ThawFactor + (1f / numberoftapstothaw);
        if (th >= 1f)
        {
            unfreeze();
        } else
        {
            ThawFactor = th;
        }


    }

    private float HitMarkerDamage = 0f;
    private float HitMarkerTime = -10f;
    private bool HitMarkerWasKill = false;
    private float HitMarkerHealthRemaining = 1f;
    public void HitMarkerStep()
    {
        HitMarkerDamage = Mathf.Max(0f, HitMarkerDamage - (100f * Time.deltaTime));
        float t = (Time.time - HitMarkerTime);
        float fadetime = .25f;
        t = (t / fadetime);
        if (t < 1f) {
            Color col = (HitMarkerWasKill ? Color.red : Color.yellow);
            HitMarkerReticle.color = new Color(col.r, col.g, col.b, 1f);
            float sc = Mathf.Clamp01(HitMarkerDamage / 200f);
            HitMarkerReticle.transform.localScale = (Vector3.one * (Mathf.Lerp(1f, 12f, sc))*(1f-t));
            HitMarkerReticle.transform.localRotation = Quaternion.Euler(0f,0f,45f*HitMarkerHealthRemaining);

            HitMarkerReticle.enabled = true;
        } else
        {
            HitMarkerReticle.enabled = false;
        }
        
    }

    public void tickHitMarker(float damage,float hpremaining,bool iskill)
    {
        
        HitMarkerDamage += damage;
        HitMarkerTime = Time.time;
        HitMarkerHealthRemaining = hpremaining;
        HitMarkerWasKill = iskill;
    }


    private bool HasIntroLanded = false;
    private float IntroLandTime = -10f;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionStay2D(collision);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {

        
            
            
            foreach (ContactPoint2D co in collision.contacts)
            {
                
                
            if (co.collider.gameObject.CompareTag("Geometry"))
            {
                Vector2 po = co.point;
                if ((Mathf.Abs(po.x - this.transform.position.x) <1f) &&(po.y <= (this.transform.position.y - (MyCollider.size.y * .4f))) && (Mathf.Abs(MyRigidbody.velocity.y) <= .1))
                {
                    //Debug.Log("Geo");
                    Airborne = false;

                    MyAnimator.SetTrigger("OnLanded");
                    if (!Introduced)
                    {
                        HasIntroLanded = true;
                        PlayerHasControl = true;
                        IntroLandTime = Time.time;
                        Introduced = true;
                        //if (IntroParticlesIce) IntroParticlesIce.Play();
                        Shockwave.transform.position = new Vector3(co.point.x, co.point.y,-2f);
                        Shockwave.Play();
                        DeployDropSound.Stop();

                        //AudioManager.AM.playInstanceSoundOnObject(this.gameObject,AudioManager.AM.AstronautLanding1, 1f,1f, false, 5f);
                        //AudioManager.AM.playInstanceSoundOnObject(this.gameObject, AudioManager.AM.AstronautLanding2, 1f, 1f, false, 5f);
                        AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.AstronautLanding1, AudioManager.AM.PlayerAudioMixer, volume:2f, pitch:1f, looped:false, destroyafter: 5f);
                        AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.AstronautLanding2, AudioManager.AM.PlayerAudioMixer,volume:2f, pitch:1f, looped:false, destroyafter:5f);
                        addCamShake(.4f, 0f, 1f, 1f, 1f);
                        startLevelMusic();
                        //based on level

                    }

                    Slipping = false;
                    UsedDoubleJump = false;

                    if (JumpLandSound == null)
                        JumpLandSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.JumpLand, AudioManager.AM.EnvironmentAudioMixer, 1f, 1f, false);
                     //Dead on the floor
                }
                if (!Alive)
                    MyRigidbody.velocity = new Vector2(0f, MyRigidbody.velocity.y);

            } 

            }
    }

    public void setWeapon(SpecialWeapon wep)
    {
        SpecialWeapon w = CurrentSpecialWeapon;
        switch (wep)
        {
            case SpecialWeapon.Pistol: {
                    CurrentSpecialWeapon = SpecialWeapon.Pistol;
                    PistolSprite.enabled = true;
                    ShotgunSprite.enabled = false;
                    MachinegunSprite.enabled = false;
                    LasergunSprite.enabled = false;
                    GrenadeLauncherSprite.enabled = false;
                    TeslaGunSprite.enabled = false;
                    break; }
            case SpecialWeapon.Shotgun: {

                    if (ShotgunAmmo > 0)
                    {
                        CurrentSpecialWeapon = SpecialWeapon.Shotgun;
                        PistolSprite.enabled = false;
                        ShotgunSprite.enabled = true;
                        MachinegunSprite.enabled = false;
                        LasergunSprite.enabled = false;
                        GrenadeLauncherSprite.enabled = false;
                        TeslaGunSprite.enabled = false;
                    } else
                    {
                        //small buzzer
                    }
                    break; }
            case SpecialWeapon.Gatling: {
                    if (GatlingAmmo > 0)
                    {
                        CurrentSpecialWeapon = SpecialWeapon.Gatling;
                        PistolSprite.enabled = false;
                        ShotgunSprite.enabled = false;
                        MachinegunSprite.enabled = true;
                        LasergunSprite.enabled = false;
                        GrenadeLauncherSprite.enabled = false;
                        TeslaGunSprite.enabled = false;
                    }
                    else
                    {
                        //small buzzer
                    }
                    break; }
            case SpecialWeapon.Laser: {
                    if (LaserAmmo > 0)
                    {
                        CurrentSpecialWeapon = SpecialWeapon.Laser;
                        PistolSprite.enabled = false;
                        ShotgunSprite.enabled = false;
                        MachinegunSprite.enabled = false;
                        LasergunSprite.enabled = true;
                        GrenadeLauncherSprite.enabled = false;
                        TeslaGunSprite.enabled = false;
                    }
                    else
                    {
                        //small buzzer
                    }

                    break; }
            case SpecialWeapon.GrenadeLauncher:
                {
                    if (LaserAmmo > 0)
                    {
                        CurrentSpecialWeapon = SpecialWeapon.GrenadeLauncher;
                        PistolSprite.enabled = false;
                        ShotgunSprite.enabled = false;
                        MachinegunSprite.enabled = false;
                        LasergunSprite.enabled = false;
                        GrenadeLauncherSprite.enabled = true;
                        TeslaGunSprite.enabled = false;
                    }
                    else
                    {
                        //small buzzer
                    }

                    break;
                }
            case SpecialWeapon.TeslaGun:
                {
                    if (LaserAmmo > 0)
                    {
                        CurrentSpecialWeapon = SpecialWeapon.TeslaGun;
                        PistolSprite.enabled = false;
                        ShotgunSprite.enabled = false;
                        MachinegunSprite.enabled = false;
                        LasergunSprite.enabled = true;
                        GrenadeLauncherSprite.enabled = false;
                        TeslaGunSprite.enabled = true;
                    }
                    else
                    {
                        //small buzzer
                    }

                    break;
                }

        }
        
        if (w != CurrentSpecialWeapon)
        {
            LastGunHUDAlertTime = Time.time;
            if (w == SpecialWeapon.Laser)
            {
                if (BeamRifleLoop)
                {
                    AudioManager.AM.crossfade(BeamRifleLoop, 0f, 1f);
                    AudioManager.AM.crosstune(BeamRifleLoop, .9f, 1f);
                }
                if (BeamRifleHitLoop)
                {
                    AudioManager.AM.crossfade(BeamRifleHitLoop, 0f, 1f);
                    BeamRifleHitLoop.volume = 0f;
                }

                if (BeamRiflePlusLoop)
                {
                    AudioManager.AM.crossfade(BeamRiflePlusLoop, 0f, .5f);
                }

                if (BeamRifleHitLoop)
                {
                    BeamRifleHitLoop.volume = 0f;
                }
                if (BeamRifleDamageLoop)
                {
                    BeamRifleDamageLoop.volume = 0f;
                }
            }
            if (w == SpecialWeapon.Gatling)
            {
                if (MachinegunLoop)
                    MachinegunLoop.loop = false;
            }
        }

    }

    public void pickupHealth(HealthPickup hp)
    {
        //this.Health = Mathf.Min(this.Health + hp.HealthValue, 100f);
        this.StimPacks++;

    }

    public void pickUpWeapon(SpecialWeapon wep)
    {

        switch (wep)
        {
            case SpecialWeapon.Pistol: { break; }
            case SpecialWeapon.Shotgun: {

                    ShotgunAmmo = Mathf.Min(ShotgunAmmo + 15, 60);
                    break; }
            case SpecialWeapon.Gatling:
                {

                    GatlingAmmo = Mathf.Min(GatlingAmmo+100,400);
                    break;
                }
            case SpecialWeapon.Laser:
                {
                    LaserAmmo = Mathf.Min(LaserAmmo + 100f, 400f);
                    break;
                }
        }


    }

    private Quaternion originalspriterotation;
    public float ReviveTime = -10f;
    private bool useresistancedelay = false;
    public void revive()
    {

        Health = 100f;
        Alive = true;
        //MySpriteRenderer.transform.localRotation = originalspriterotation;
        if (FallingIntoPit)
        {
            this.transform.position = LastGroundedLocation;
            
            float t = ((Time.time - DeathTime) / (ResistanceDrop.LIFETIMECONSTANT * 1.5f));
            if (t < 1f)
            {
                useresistancedelay = true;
                float ru = (HeldFallenResistanceValue * (1f - t));
                dropResistance(ru, this.transform.position, ResistanceElement);
                useresistancedelay = false;
            }
            
        }
        FallingIntoPit = false;
        ReviveTime = Time.time;
        this.transform.rotation = new Quaternion();
        //ShotgunAmmo = 0;
    }
    public MeshRenderer PainOverlay;
    void updateOverlay()
    {
        
        if (Alive)
        {
            float thr = 20f;
            if (Health > thr)
            {
                PainVignette.color = new Color(.7f, 0f, 0f,1f-((Health-thr)/(100-thr)));
            } else
            {
                PainVignette.color = new Color(.7f*(Health/thr), 0f, 0f,1f);
            }

            

            //PainOverlay.material.SetFloat("_PainValue", Mathf.Lerp(PainOverlay.material.GetFloat("_PainValue"),1f-(Health/100f),Time.deltaTime*1f));

            PainOverlay.material.SetFloat("_Desaturation", Mathf.Lerp(PainOverlay.material.GetFloat("_Desaturation"), Mathf.Clamp01(Mathf.Max(0f,(((1f - (Health / 100f)) - .5f) / .5f))), Time.deltaTime * 1f));
            if (RespawnTime <= 0f)
            {
                PainOverlay.material.SetFloat("_LifeValue", 1f);
            }
            else
            {
                PainOverlay.material.SetFloat("_LifeValue", (Time.time - RespawnTime)/.8f);

            }
            PainOverlay.material.SetFloat("_StimValue", Mathf.Lerp(PainOverlay.material.GetFloat("_StimValue"), Mathf.Max(0f,(((StimTime+2f) - Time.time)/2f)), Time.deltaTime * 3f));
            PainOverlay.material.SetFloat("_BorderCurve", 1.2f+Mathf.Max(((((lastDamageTakenTime + .5f) - Time.time)/2f))*8f,0f));
        } else
        {
            PainOverlay.material.SetFloat("_Desaturation", 1f);
            //PainOverlay.material.SetFloat("_PainValue", Mathf.Lerp(PainOverlay.material.GetFloat("_PainValue"), 1f - (Health / 100f), Time.deltaTime * 1f));
            
                PainOverlay.material.SetFloat("_LifeValue", 0f);
            
        }

        PainOverlay.material.SetFloat("_DeathTime", DeathTime);
        ExtremeVignette.color = new Color(ExtremeVignette.color.r, ExtremeVignette.color.g, ExtremeVignette.color.b, Mathf.Clamp01(ExtremeVignette.color.a - Time.deltaTime * 2f));





    }
    public float ManualRevive = 0f;
    public void attemptToRevive()
    {
        float inc = 1f / ((float)5+((Mathf.Min(Deaths,5)-1)*1));
        float rm = ManualRevive + inc;
        if (rm >= 1f)
        {
            ManualRevive = 0f;
            lastDamageTakenTime = Time.time;
            revive();
        } else
        {
            ManualRevive = rm;
        }
    }
    public void dropResistance(float resquantity,Vector3 location,Element el)
    {
        
        float quantities = .05f;
        ResistanceDrop dr = IceResistDropPrefab;
        //Debug.Log("Vita Dropped!");
        switch (el)
        {
            case Element.Fire: { dr = FireResistDropPrefab; break; }
            case Element.Ice: { dr = IceResistDropPrefab; break; }
            case Element.Grass: { dr = GrassResistDropPrefab; break; }
        }
        if (imdying)
        {
            quantities = .1f;
            if (resquantity >= 3f)
            {
                quantities = ((resquantity / 3f) * quantities);
            }
        }
            while (resquantity > 0f)
        {
            //Randomly decide on the size
            resquantity -= quantities;
            ResistanceDrop drop = GameObject.Instantiate(dr,location,new Quaternion()).GetComponent<ResistanceDrop>();
            drop.UsingDelay = useresistancedelay;
            drop.ResistanceValue = Mathf.Min(quantities,resquantity);
            if (imdying)
            {
                drop.Lifetime *= 1.5f;
            }
            drop.MyRigidbody.velocity = (Random.insideUnitCircle + new Vector2(0f, 1f)) * 3f;
        }

    }
    public ClimbingVine MyClimbingVine = null,MyJumpingVine = null; //The Vine you just jumped off of. This will be cleared when you land or change directions.
    public bool TransitionClimbing = false;
    public bool ClimbingToBranches = false;
    private float VineJumpTime = -10f;
    public void transportToBranchesArea()
    {
        
        Transform t = GameObject.FindGameObjectWithTag("BranchesDestination").transform;
        setJungleBackground(1);
        this.transform.position = t.transform.position;
        CameraRigCurrentPosition = new Vector3(t.transform.position.x, t.transform.position.y, CameraRigCurrentPosition.z);
        TransitionClimbing = false;
        FadingInOut = true;
        PlayerHasControl = true;
        MyClimbingVine = null;
        MyJumpingVine = null;

    }
    public void transportToTreeTopsArea()
    {
        Transform t = GameObject.FindGameObjectWithTag("TreeTopsDestination").transform;
        setJungleBackground(2);
        this.transform.position = t.transform.position;
        CameraRigCurrentPosition = new Vector3(t.transform.position.x, t.transform.position.y, CameraRigCurrentPosition.z);
        TransitionClimbing = false;
        PlayerHasControl = true;
        FadingInOut = true;
        MyClimbingVine = null;
        MyJumpingVine = null;
    }
     
    public void transitionClimb(bool tobranches) {
        TransitionClimbing = true;
        PlayerHasControl = false;
        ClimbingToBranches = tobranches;
        AudioManager.AM.crossfade(AudioManager.AM.CurrentMusic, 0f, 1.4f);
    }
    public float VineYPosition;
    private int climbdir = 0;
    public void vineStep()
    {

        if (MyClimbingVine != null)
        {

            if (TransitionClimbing)
            {
                VineYPosition = (VineYPosition + (Time.fixedDeltaTime* 1f * CLIMBINGSPEED));
                this.transform.position = new Vector3(MyClimbingVine.transform.position.x, VineYPosition, this.transform.position.z);
                FadingInOut = false;
            } else {

            if ((JumpNow) || ((!vineTouch) && ((Time.time - vineTouchTime) >= (Time.fixedDeltaTime * 4f))))
            {
                MyJumpingVine = MyClimbingVine;
                MyClimbingVine = null;
                if ((VerticalControl > 0) || (JumpNow))
                {
                    MyRigidbody.velocity = new Vector2(0f, JumpSpeed);
                    //Play a Jump sound


                }
                else
                    MyRigidbody.velocity = new Vector2(0f, 0f);

                //MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
                Airborne = true;
                UsedDoubleJump = false;

                //perform a Y velocity jump
            } else
            {
                if (VerticalControl != 0)
                {
                    VineYPosition = (VineYPosition + (Time.fixedDeltaTime * VerticalControl * CLIMBINGSPEED));
                    

                }
                    this.transform.position = new Vector3(MyClimbingVine.transform.position.x, VineYPosition, this.transform.position.z);
                    MyRigidbody.velocity = new Vector2();
                //MyRigidbody.bodyType = RigidbodyType2D.Kinematic;



            }
            climbdir = LookDirection;
        }
        } else
        {
            if ((!Airborne) ||((LookDirection != climbdir)&&(LookDirection != 0)))
            {
                MyJumpingVine = null;

            }
        }
        if (vineTouch)
        {
            vineTouchTime = Time.time;
        }
        vineTouch = false;
    }
    private Vector3 LastGroundedLocation;
    private bool FellToDeath = false;
    private float vineTouchTime = -10f;
    private bool vineTouch = false;
    private float CLIMBINGSPEED = 4f;
    public void vineCollision(ClimbingVine climbingVine)
    {
        if (!Alive) return;
        vineTouch = true;
        if ((MyClimbingVine == null) && (!climbingVine.Equals(MyJumpingVine)))
        {
            

            MyClimbingVine = climbingVine;
            climbdir = LookDirection;
            MyRigidbody.velocity = new Vector2();
            //MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
            VineYPosition = this.transform.position.y;
            this.transform.position = new Vector3(climbingVine.transform.position.x, VineYPosition, this.transform.position.z);
            Airborne = false;
            UsedDoubleJump = false;
            if (MyClimbingVine.TransitionClimbingVine)
            {
                transitionClimb(MyClimbingVine.TransitionToBranches);
            }
        }
        
    }
    private bool FallingIntoPit = false;
    private float HeldFallenResistanceValue = 0f;
    public void fallCollision(FallingArea fa) {

        if (Alive)
        {
            float v = ((1f * VitaLevel) + ResistanceXP);
            FallingIntoPit = true;
            if ((MyCameraRig.transform.position.y - this.transform.position.y) >= 5f)
            {
                if (TakeDamage(20f, new Vector3()))
                {
                    HeldFallenResistanceValue = v;
                }
                else
                {
                    FallingIntoPit = false;
                    this.transform.position = LastGroundedLocation;
                }
            }
        }
    }

    public void absorbResistance(ResistanceDrop res)
    {
        res.Live = false;
        res.transform.localScale = (Vector3.one * 10f);
        res.MySpriteRenderer.enabled = false;
        res.Remove();

        
        float nv = ResistanceXP + (res.ResistanceValue*res.ValueValue);
        if (nv >= 1f)
        {
            VitaLevel++;
            ResistanceXP = 0f;
        } else
        {
            ResistanceXP = nv;
        }
        ResistanceElement = res.ElementValue;
        switch (res.ElementValue)
        {
            case Element.All: { ResistanceColor = Color.white; ResistanceIcon.enabled = false;  break; };
            case Element.Fire: { ResistanceColor = new Color(1f,.6f,.1f);
                    ResistanceIcon.enabled = true;
                    break; };
            case Element.Grass: { ResistanceColor = new Color(.2f,1f,.1f); ResistanceIcon.sprite = VitaSpriteGrass; ResistanceIcon.enabled = true; break; };
            case Element.Ice: { ResistanceColor = new Color(.3f,.3f,1f); ResistanceIcon.sprite = VitaSpriteIce; ResistanceIcon.enabled = true;  break; };
            case Element.Void: { ResistanceColor = new Color(.8f,0f,.8f); ResistanceIcon.sprite = VitaSpriteFire; ResistanceIcon.enabled = true;  break; };
        }
        //GameObject.Destroy(res.gameObject,5f);
    }
    public Color ResistanceColor = Color.white;
    public ParticleSystem ResistanceAbsorbedEffect,ResistanceLevelUpEffect;
    private float ResistanceDisplayedValue = 0f;
    private Element ResistanceElement = Element.Fire;
    public Image ResistanceBack, ResistanceGlow,ResistanceIcon, ResistanceFill;
    public Slider ResistanceGauge;
    public Sprite VitaSpriteFire, VitaSpriteGrass, VitaSpriteIce;
    public Image StimPackIcon, StimPackGlow;
    public Text ResistanceText,VitaPowerBaseText,VitaPower1Text, VitaPower2Text, VitaPower3Text, VitaPowerBonusText, StimCountText;

    private float StimTime = -10f;
    public ResistanceDrop IceResistDropPrefab, FireResistDropPrefab, GrassResistDropPrefab,NeutralResistDropPrefab,VoidResistDropPrefab;
    public EndLevelScreen MyEndLevelScreen;
    private bool DisplayingEndLevelScreen = false;
    private bool imdying = false;
    
    public static string getElementPowerLevelName(Element e,int vitaindex)
    {
        string s = "(none)";
        string p0 = "Power Level 1";
        string p1 = "Power Level 2";
        string p2 = "Power Level 3";
        string p3 = "Power Level Max";
        if (vitaindex > 3) vitaindex = 3;
        else if (vitaindex < 0) vitaindex = 0;

        switch (e)
        {
            case Element.Fire:
                {
                    p0 = "Flame Beam";
                    p1 = "Fire Patches";
                    p2 = "Ignition";
                    p3 = "Combustion Convection";
                    break;
                }
            case Element.Ice:
                {
                    p0 = "Freeze Cast";
                    p1 = "Hazard Permafrost";
                    p2 = "Ice Defense";
                    p3 = "Cryo Nova";
                    break;
                }
            case Element.Grass:
                {
                    p0 = "Vine Grapple";
                    p1 = "Constrictor";
                    p2 = "Energy Drain";
                    p3 = "Cluster Constrictor";
                    break;
                }
            case Element.Void:
                {
                    p0 = "Void 0";
                    p1 = "Void 1";
                    p2 = "Void 2";
                    p3 = "Void 3";
                    break;
                }
            case Element.All:
                {
                    break;
                }
        }

        
        if (vitaindex == 0)
            s = p0;
        else if (vitaindex == 1)
            s = p1;
        else if (vitaindex == 2)
            s = p2;
        else if (vitaindex == 3)
            s = p3;

        return s;
    }
    public void showEndLevelScreen()
    {
        
        if (DisplayingEndLevelScreen) return;
        
        MyEndLevelScreen.setElement(StageElement);
        
        if (!MyEndLevelScreen.DoneShowingResults)
        {
            
            MyEndLevelScreen.showResults();
        } 
        DisplayingEndLevelScreen = true;
        
        
        
        
        


    }
    
    public void kill()
    {
        if (!Alive) return;
        Alive = false;
        Health = 0f;
        Deaths++;
        DeathTime = Time.time;
        FellToDeath = false; //This will be set to true by another script that states that you have fallen

        if ((VitaLevel > 0) ||(ResistanceXP > 0f))
        {
            imdying = true;
            if (!FallingIntoPit)
            {
                dropResistance(((1f * VitaLevel) + ResistanceXP), this.transform.position, ResistanceElement);
            }
            imdying = false;

            VitaLevel = 0;
            ResistanceXP = 0f;

            /*
            if (VitaLevel >= 4)
            {
                float de = ((1f * (VitaLevel - 3)) + ResistanceXP);
                VitaLevel = 3;
                
            } else if (VitaLevel > 0)
            {
                
                dropResistance(1f,this.transform.position,ResistanceElement);
            } else
            {
                dropResistance(ResistanceXP, this.transform.position, ResistanceElement);
            }
            */

            
            
        }
        
        //Mathf.Max(0, ((int)(VitaLevel*(3f/4f))));
        
        for (int i = 0; i < MAXSTIMS; i++)
            StimDurations[i] = -1f;
        this.transform.Rotate(0f,0f,90f,Space.World);
    }
    public float DeathTime = -10f;
    private float RespawnTime = -10f;
    public float lastDamageTakenTime = -10f, lastStunTime = -10f;
    public const bool USINGRESISTANCE = false;
    public int numberOfStimsActive()
    {
        int n = 0;
        for (int i = 0; i < MAXSTIMS; i++)
        {
            if (StimDurations[i] > 0f)
                n++;
            

        }
        return n;
    }
    public bool TakeDamage(float damage,Vector3 knockback)
    {
        if (!Alive) return false;
        damage *= DamageVulnerability;
        if (USINGRESISTANCE)
        if (VitaLevel > 0) {
            damage = (damage /(1f+(1f*(((float)VitaLevel)/10f))));
        }

        int rs = numberOfStimsActive();
        
        if (rs > 0)
        {
            damage /= (1+(rs*STIMRESISTANCECONSTANT));
            knockback /= (1 + (rs * STIMRESISTANCECONSTANT));
        }
        float health = (Health - damage);
            lastDamageTakenTime = Time.time;
        //Play a DamageTaken sound.
        float extremethreshold = 25f;
        if (damage >= extremethreshold)
        {
            float offset = .25f;
            float alp = offset + ((1f - offset) * ((damage-extremethreshold)/(100f-extremethreshold)));
            ExtremeVignette.color = new Color(ExtremeVignette.color.r, ExtremeVignette.color.g, ExtremeVignette.color.b, Mathf.Max(ExtremeVignette.color.a,Mathf.Clamp01(alp)));
        }
        if (health <= 0f)
        {
            if (knockback.magnitude > 0.1f)
            {
                MyRigidbody.velocity = new Vector2(knockback.x, ((Mathf.Abs(knockback.y) < (JumpSpeed * .5f)) ? JumpSpeed : knockback.y));
                lastStunTime = Time.time;
            }
            kill();
            return true;
        } else
        {
            if (knockback.magnitude > 0.1f)
            {
                MyRigidbody.velocity = new Vector2(knockback.x, ((Mathf.Abs(knockback.y)<(JumpSpeed*.5f))? JumpSpeed: knockback.y));
                lastStunTime = Time.time;
            }
            Health = health;

        }



        
        return false;
    }

    bool usingVerticalBackgroundThreshold = false;
    bool usingHorizontalBackgroundThreshold = false;
    bool usingVerticalBackgroundThresholdInverted = false;
    bool usingHorizontalBackgroundThresholdInverted = false;
    float backgroundThreshold = 0f;
    private float backgroundTime = -10f;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("BackgroundSwitcher"))
        {
            usingVerticalBackgroundThreshold = true;
            usingHorizontalBackgroundThreshold = false;
            usingVerticalBackgroundThresholdInverted = false;
            usingHorizontalBackgroundThresholdInverted = false;
            backgroundThreshold = collision.gameObject.transform.position.y;
            backgroundTime = Time.time;


        }  else if (collision.gameObject.CompareTag("BackgroundSwitcherHorizontal"))
        {
            usingVerticalBackgroundThreshold = false;
            usingHorizontalBackgroundThreshold = true;
            usingVerticalBackgroundThresholdInverted = false;
            usingHorizontalBackgroundThresholdInverted = false;
            backgroundThreshold = collision.gameObject.transform.position.x;

            backgroundTime = Time.time;

        } else if (collision.gameObject.CompareTag("BackgroundSwitcherInverted"))
        {
            usingVerticalBackgroundThreshold = false;
            usingHorizontalBackgroundThreshold = false;
            usingVerticalBackgroundThresholdInverted = true;
            usingHorizontalBackgroundThresholdInverted = false;
            backgroundThreshold = collision.gameObject.transform.position.y;
            backgroundTime = Time.time;



        }
        else if (collision.gameObject.CompareTag("BackgroundSwitcherHorizontalInverted"))
        {
            usingVerticalBackgroundThreshold = false;
            usingHorizontalBackgroundThreshold = false;
            usingVerticalBackgroundThresholdInverted = false;
            usingHorizontalBackgroundThresholdInverted = true;
            backgroundThreshold = collision.gameObject.transform.position.x;
            backgroundTime = Time.time;



        }
        else if (collision.gameObject.CompareTag("SlickGeometry"))
        {

            
            if (MyRigidbody.velocity.y <= 0f)
            {

                //Debug.Log("Geo");
                /*
                Airborne = false;

                MyAnimator.SetTrigger("OnLanded");
                if (!Introduced)
                {
                    HasIntroLanded = true;
                    PlayerHasControl = true;
                    Introduced = true;
                    //based on level

                }
                */
                Slipping = true;
                UsedDoubleJump = true;
                

            }
            
        }
    }
    private AudioSource JumpSound,DoubleJumpSound, JumpLandSound, DeployDropSound;


    private float ElementCollectedTime;
    public Element ElementCollected = Element.None;
    public bool FinishingStage = false;
    private bool _FinishedStage = false;
    public void collectElement(Element e)
    {
        ElementCollected = e;
        Invulnerable = true;
        ElementCollectedTime = Time.time;
        PlayerHasControl = false;
        FinishingStage = true;

    }

    public void completeStage()
    {
        FinishingStage = false;
        TeleportedOut = false;
    }

    //Collect All-Element

    public Texture HUDCrosshair;
    public Texture HUDPistol, HUDShotgun, HUDMachinegun, HUDLaserBeam;
    public Texture HUDPistolBullet, HUDShotgunBullet, HUDMachinegunBullet;
    public Texture BlankHUDTexture;
    //public Image HUDPistol, HUDShotgun, HUDMachinegun, HUDLaserBeam;
    private float LastGunHUDAlertTime= -10f; //set to Time.time to get the hud to appear close
    public AnimationCurve WeaponAmmoLerpCurve;
    private void OnGUI()
    {
        
        float screenwidth = Screen.width;
        float screenheight = Screen.height;
        float X, Y;
        if (false)
        if (PlayerHasControl)
        {
            //Draw the Ammo 
            X = (screenwidth / 2);
            Y = (screenheight * .75f);
            float boxwidth = 200f, boxheight = 100f;
            float border = 10f;
            float CornerX = (screenwidth - (boxwidth + border + 25f)), CornerY = screenheight - (boxheight + border + 25f);
            float lf = Mathf.Clamp01(((Time.time - LastGunHUDAlertTime) - 2f) / 1.5f);
            lf = WeaponAmmoLerpCurve.Evaluate(lf);
            X = Mathf.Lerp(X, CornerX, lf);
            Y = Mathf.Lerp(Y, CornerY, lf);

            Texture hudguntexture = HUDPistol;
            string ammovalue = "--";
            Color ammocolor = new Color(1f, 1f, 1f, 1f);
            string reloadstring = "";

            if (PistolAmmo < MAXPISTOLAMMO)
            {
                int e = (int)((Time.time - lastshottime) / .8f);
                if (e > 3) e = 3;
                for (int i = 0; i < e; i++)
                    reloadstring += ".";

            }
            switch (CurrentSpecialWeapon)
            {
                case SpecialWeapon.Pistol: { hudguntexture = HUDPistol; ammovalue = "" + PistolAmmo + "" + (reloadstring); break; }
                case SpecialWeapon.Shotgun: { hudguntexture = HUDShotgun; ammovalue = "" + ShotgunAmmo; break; }
                case SpecialWeapon.Gatling: { hudguntexture = HUDMachinegun; ammovalue = "" + GatlingAmmo; break; }
                case SpecialWeapon.Laser: { hudguntexture = HUDLaserBeam; ammovalue = "" + (int)LaserAmmo; break; }
            }

            GUI.color = new Color(.25f, .25f, .25f, .1f);
            
            GUI.DrawTexture(new Rect(X - (boxwidth / 2), Y - (boxheight / 2), boxwidth + (border * 2), boxheight + (border * 2)), BlankHUDTexture); //Background
            GUI.color = Color.white;
            //GUI.skin.font.fontSize = 30f;
            GUI.DrawTexture(new Rect(X - (boxwidth / 2), Y - (boxheight / 2), boxwidth / 2, boxheight / 2), hudguntexture);
            
            GUI.Label(new Rect(X, Y, boxwidth / 2, boxheight / 2), ammovalue);
            //Draw the Ammo images in a special pattern
        }
        GUI.color = Color.white;
    }


    void OnParticleCollision(GameObject other)
    {
        Debug.Log(other.name);
        if (other.CompareTag("Player"))
        {
            //gameObject.SetActive(false);
            //balls += 1;
        }
    }
    
}
