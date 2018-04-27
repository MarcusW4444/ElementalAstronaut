using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Am;


public class Astronaut : MonoBehaviour {

    // Use this for initialization
    public enum Element { Fire, Ice, Grass, All, Water, None, Void }
    public float Health = 100f;
    public int VitaLevel = 0;
    public static int AggressionLevel = 0;
    public static float AggressionLevelF;
    public static int AggressionLevelAbsolute = 0;
    public static float IcePowerFactor=0f;
    public static float FirePowerFactor = 0f;
    public static float JunglePowerFactor = 0f;
    public static float VoidPowerFactor = 0f;
    public static float IcePowerScale = 1f;
    public static float FirePowerScale = 1f;
    public static float JunglePowerScale = 1f;
    public static float VoidPowerScale = 1f;

    public static float IcePowerUsage = 1f;
    public static float FirePowerUsage = 1f;
    public static float JunglePowerUsage = 1f;
    public static float VoidPowerUsage = 1f;
    public int StimPacks = 0;

    

    public float DamageVulnerability = 1f;
    public float ResistanceXP = 0f;

    public int Deaths = 0;
    public float ElementalEnergy;
    public Element ElementMode = Element.None;
    public Element ElementAffinity = Element.None;
    public bool HasElementIce = false, HasElementFire = false, HasElementGrass = false, HasElementVoid = false,HasElementAll = false;
    public bool EtherealLock = false;
    public Element EtherealElement;
    public EtherealManager MyEtherealManager;
    public Transform MyEtherealCheckPoint = null;
    
    public bool UsingEtherealTutorial = false;
    public void engageEtherealTutorial(EtherealManager etherealManager)
    {
        EtherealLock = true;
        MyEtherealManager = etherealManager;
        MyEtherealCheckPoint = etherealManager.EtherealStartingCheckpoint;
        UsingEtherealTutorial = true;
        //CurrentSpecialWeapon = SpecialWeapon.None;
        setWeapon(SpecialWeapon.None);
        ElementMode = etherealManager.EtherealElement;
    }
    public void SetEtherealSkillLevel(int skillindex)
    {
        switch (ElementMode)
        {
            case Astronaut.Element.Fire:
                {
                    FireElementPowerLevel = skillindex;
                    break;
                }
            case Astronaut.Element.Ice:
                {
                    IceElementPowerLevel = skillindex;
                    break;
                }
            case Astronaut.Element.Grass:
                {
                    GrassElementPowerLevel = skillindex;
                    break;
                }
        }
        MyEtherealManager.EtherealCurrentLevel = Mathf.Max(MyEtherealManager.EtherealCurrentLevel, skillindex);
    }
    public void SendBackToEtherealCheckpoint()
    {
        this.transform.position = MyEtherealCheckPoint.transform.position;
        MyRigidbody.velocity = new Vector2();
        AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.EtherealRespawn, AudioManager.AM.PlayerAudioMixer, volume: 1f, pitch: 1f, looped: false, destroyafter: 7f);
    }
    private bool etherealTutorialFinished = false;
    private float etherealTutorialFinishTime = -10f;
    public void finishEtherealTutorial()
    {
        if (etherealTutorialFinished) return;
        etherealTutorialFinished = true;
        etherealTutorialFinishTime = Time.time;
        PlayerHasControl = false;
        
    }

    public int IceElementPowerLevel = 5, FireElementPowerLevel = 5, GrassElementPowerLevel = 5, VoidElementPowerLevel = 5;
    public bool Alive = true;
    public bool Invulnerable = false;
    public enum SpecialWeapon { Pistol, Shotgun, Laser, Gatling,GrenadeLauncher,TeslaGun,RailGun,None};
    public SpecialWeapon CurrentSpecialWeapon = SpecialWeapon.Pistol;
    public static Astronaut TheAstronaut = null;
    private bool Introduced = false;
    public ParticleSystem IntroParticlesIce, IntroParticlesFire, IntroParticlesGrass;
    public bool PlayerHasControl = false;
    public Transform WatchingLocation = null;

    
    
    void Start() {
        TheAstronaut = this;
        BackgroundStartX = MyCameraRig.position.x;
        PistolAmmo = MAXPISTOLAMMO;
        Deaths = 0;
        UsingEtherealTutorial = false;
        LaserBaseScale = LaserBeamRenderer.transform.localScale.x;
        LaserBaseDistance = (this.transform.position - LaserDamageParticles.transform.position).magnitude;
        
        if (SporeEffectPrefab != null)
        {
            MySporeEffect = GameObject.Instantiate<ParticleSystem>(SporeEffectPrefab,transform.position,SporeEffectPrefab.transform.rotation, MySpriteRenderer.transform);
            MySporeValue = 0f;
        }
        initiateAudio();
        

        MyBossGolem = null;
        Introduced = false;
        FadeCanvas.SetActive(true);
        rotateElement(true);
        MyEndLevelScreen.gameObject.SetActive(true);
        LastGroundedLocation = this.transform.position;
        originalspriterotation = MySpriteRenderer.transform.localRotation;
        RaycastHit2D rh = Physics2D.Raycast(this.transform.position, Vector3.down, 100f, LayerMask.GetMask(new string[] { "Geometry" }));
        resistanceglowinitialscale = ResistanceGlow.transform.localScale;
        
        VitaVignette.color = new Color(ResistanceColor.r, ResistanceColor.g, ResistanceColor.b, 0f);
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
        ProjectileCollisionFilter = new ContactFilter2D();
        ProjectileCollisionFilter.SetLayerMask(LayerMask.GetMask("EnemyProjectile"));
        ProjectileCollisionFilter.useDepth = false;
        ProjectileCollisionFilter.useOutsideNormalAngle = false;
        ProjectileCollisionFilter.useOutsideDepth = false;
        ProjectileCollisionFilter.useNormalAngle = false;
        ProjectileCollisionFilter.useTriggers = true;
        ProjectileCollisionFilter.useLayerMask = false;

        GeometryCollisionFilter = new ContactFilter2D();
        GeometryCollisionFilter.SetLayerMask(LayerMask.GetMask("Geometry"));
        GeometryCollisionFilter.useDepth = false;
        GeometryCollisionFilter.useOutsideNormalAngle = false;
        GeometryCollisionFilter.useOutsideDepth = false;
        GeometryCollisionFilter.useNormalAngle = false;
        GeometryCollisionFilter.useTriggers = true;
        GeometryCollisionFilter.useLayerMask = false;
        loadGameManagerInfo();
        rotateElement(true);
        setWeapon(SpecialWeapon.Pistol);
    }
    private ContactFilter2D ProjectileCollisionFilter = new ContactFilter2D();
    private ContactFilter2D GeometryCollisionFilter = new ContactFilter2D();
    private Vector3 resistanceglowinitialscale;
    public void initiateAudio()
    {
        DoubleJumpSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.DoubleJumpSound, AudioManager.AM.EnvironmentAudioMixer, 1f, 1f, false);
        //IceBeamSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.IceElementPower, AudioManager.AM.EnvironmentAudioMixer, 1f, 1f, false);
        FlameBarSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.FireElementPower, AudioManager.AM.EnvironmentAudioMixer, 1f, 1f, false);
        FlameBarPressureSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.FireElementPowerPressure, AudioManager.AM.EnvironmentAudioMixer, 1f, 1f, false);
        VineSustainSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.JungleElementPower, AudioManager.AM.EnvironmentAudioMixer, 1f, 1f, true);
        CriticalBackSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.CriticalBackSound, AudioManager.AM.PlayerAudioMixer, 1f, 1f, false);
        DamageTakenSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.DamageTakenSound, AudioManager.AM.PlayerAudioMixer, 1f, 1f, false);
        LowHealthHeartbeatSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.HeartbeatLowHP, AudioManager.AM.PlayerAudioMixer, 1f, 1f, false);

        RailGunChargeSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.RailGunCharge, AudioManager.AM.PlayerAudioMixer, 1f, 1f, false);
        RailGunOvercharge = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.RailGunOvercharge, AudioManager.AM.PlayerAudioMixer, 1f, 1f, false);

        IceReadySound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.IceReady, AudioManager.AM.PlayerAudioMixer, 1f, 1f, false);
        FireReadySound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.FireReady, AudioManager.AM.PlayerAudioMixer, 1f, 1f, false);
        JungleReadySound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.JungleReady, AudioManager.AM.PlayerAudioMixer, 1f, 1f, false);
        ElementSwapSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.ElementRotate, AudioManager.AM.PlayerAudioMixer, .6f, 1.5f, false);

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
            case Element.Void: { ResistanceColor = new Color(.8f, 0f, .8f); ResistanceIcon.sprite = VitaSpriteVoid; ResistanceIcon.enabled = true;
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
        GrenadeLauncherAmmo = GameManager.TheGameManager.GrenadeLauncherInventory;
        TeslaAmmo = GameManager.TheGameManager.TeslaInventory;

        StimPacks = GameManager.TheGameManager.StimPackInventory;
        ElementAffinity = GameManager.TheGameManager.AffinityElement;
        HasElementFire = (((GameManager.TheGameManager.FireVitaLevelAchieved > 0) || (GameManager.TheGameManager.AffinityElement == Element.Fire)) && (StageElement != Element.Fire));
        FireElementPowerLevel = Mathf.Max(1,GameManager.TheGameManager.FireVitaLevelAchieved);
        
        HasElementGrass = (((GameManager.TheGameManager.JungleVitaLevelAchieved > 0) || (GameManager.TheGameManager.AffinityElement == Element.Grass)) && (StageElement != Element.Grass));
        GrassElementPowerLevel = Mathf.Max(1, GameManager.TheGameManager.JungleVitaLevelAchieved);

        HasElementIce = (((GameManager.TheGameManager.IceVitaLevelAchieved > 0)||(GameManager.TheGameManager.AffinityElement == Element.Ice)) && (StageElement != Element.Ice));
        IceElementPowerLevel = Mathf.Max(1, GameManager.TheGameManager.IceVitaLevelAchieved);

        HasElementVoid = false;//(GameManager.TheGameManager.VoidVitaLevelAchieved > 0);
        VoidElementPowerLevel = Mathf.Max(1, GameManager.TheGameManager.VoidVitaLevelAchieved);


        if (HasElementIce)
            ElementMode = Element.Ice;
        else if (HasElementGrass)
            ElementMode = Element.Grass;
        else if (HasElementFire)
            ElementMode = Element.Fire;
        
    }

    // Update is called once per frame
    public Rigidbody2D MyRigidbody;
    public CapsuleCollider2D MyCollider;
    public SpriteRenderer MySpriteRenderer;
    public Animator MyAnimator;
    public Bullet BulletPrefab,RailSlugPrefab;
    public Grenade GrenadePrefab;
    public Grenade MyLaunchedGrenade = null;
    public TeslaLightning MyLightning = null;
    public TeslaLightning TeslaLightningPrefab;
    public LineRenderer LaserBeamRenderer;
    private float LaserBaseScale = 1f;
    private float IceLaserBaseScale = 1f;
    private float LaserBaseDistance;
    private float IceLaserBaseDistance;
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
    public bool Quelling = false;
    public bool QuellLocked = true;
    private bool jumppress = false;
    private bool JumpNow = false;
    public bool ShootControl;
    private bool ShootPress;
    public bool QuellControl;
    public int LookDirection = 1;
    public Vector3 AimPosition = new Vector3();
    public Transform ReticleTransform;
    public Image ReticleEnergyArc;

    public Image ReticleAmmoArc;
    
    public SpriteRenderer HitMarkerReticle;
    public const int MAXPISTOLAMMO = 30;
    public int PistolAmmo = 15;
    public int ShotgunAmmo = 0;
    public float LaserAmmo = 0f;
    public int GatlingAmmo = 0;
    public int GrenadeLauncherAmmo = 0;
    public float TeslaAmmo = 0f;
    public int RailGunAmmo = 0;
    public float RailGunCharge = 0f;
    private bool railcharging = false;
    public ParticleSystem RailGunChargingParticles,RailGunFullyChargedParticles, RailGunReadyParticles;
    public ParticleSystem StimParticles,QuellParticles;
    private AudioSource StimSound,DeathSound;
    public static float FirstTimeUsingAStimPack = -10f;
    public void applyStimPack()
    {
        if ((FirstTimeUsingAStimPack > 0f) && ((Time.time - FirstTimeUsingAStimPack) < 6f) ) { return; }
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
                    if (!StimSound) StimSound = am.sound(am.M.StimSound);
                    StimSound.PlayOneShot(StimSound.clip);
                    this.StimPacks--;
                if (FirstTimeUsingAStimPack < 0f)
                    FirstTimeUsingAStimPack = Time.time;
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
                            float epd = ((100f-ElementalEnergy) *.5f* Time.deltaTime);
                            float hu = Mathf.Min(StimHeals[i],hpd);
                            if (Health < 100f)
                            {
                                Health = Mathf.Min(Health + hu, 100f);
                                
                                StimParticles.Emit(1);
                            }
                            ElementalEnergy = Mathf.Min(ElementalEnergy + (epd), 100f);
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

    private bool lastrotelpress = false;
    private bool lastrotwppress = false;
    private int AntiJ = 0;
    public void oneshot(AudioClip c)
    {
        Am.am.oneshot(c);
    }
    private AudioSource BossCriticalHitSource = null;
    public static void PlayBossCriticalHitSound(float hpremaining)
    {
        AudioClip a = new AudioClip[]{ AudioManager.AM.BossCriticalHit1, AudioManager.AM.BossCriticalHit2, AudioManager.AM.BossCriticalHit3, AudioManager.AM.BossCriticalHit4, AudioManager.AM.BossCriticalHit5, AudioManager.AM.BossCriticalHit6, AudioManager.AM.BossCriticalHit7, AudioManager.AM.BossCriticalHit8}[Mathf.Clamp(((int)(8f * Random.value)),0,7)];
        float v = (1f - hpremaining);


        float p = 0.75f + (v * .5f);
        if (Astronaut.TheAstronaut.BossCriticalHitSource == null)
            Astronaut.TheAstronaut.BossCriticalHitSource = AudioManager.AM.createGeneralAudioSource(a, AudioManager.AM.EnvironmentAudioMixer,1f, 1f, false);
        Astronaut.TheAstronaut.BossCriticalHitSource.Stop();
        Astronaut.TheAstronaut.BossCriticalHitSource.clip = a;
        Astronaut.TheAstronaut.BossCriticalHitSource.Play();



    }
    public static void PlayKillSound(int size)
    {
        AudioClip[] group = AudioManager.AM.EnemyKillSoundsMedium;
        if (size == 1)
            group = AudioManager.AM.EnemyKillSoundsSmall;
        else if (size == 2)
                group = AudioManager.AM.EnemyKillSoundsMedium;
        else if (size == 3)
            group = AudioManager.AM.EnemyKillSoundsLarge;

        AudioClip a = group[Mathf.Clamp(((int)(((float)group.Length) * Random.value)), 0, (group.Length - 1))];

        Am.am.oneshot(a,2f);
    }
    private int JoystickWeaponSwapTapIndex = 0;
    private float JoystickWeaponSwapTapTime = -10f;
    void Update() {

        bool joybA = Input.GetButton("JoyButton 0"); //A
        bool joybB = Input.GetButton("JoyButton 1"); //B
        bool joyb2 = Input.GetButton("JoyButton 2"); //?
        bool joybX = Input.GetButton("JoyButton 3"); //X
        bool joybY = Input.GetButton("JoyButton 4"); //Y
        bool joyb5 = Input.GetButton("JoyButton 5"); //?
        bool joybL1 = Input.GetButton("JoyButton 6"); //L1
        bool joybR1 = Input.GetButton("JoyButton 7"); //R1
        bool joybL2 = Input.GetButton("JoyButton 8"); //L2
        bool joybR2 = Input.GetButton("JoyButton 9"); //R2
        bool joybSelect = Input.GetButton("JoyButton 10"); //Select
        bool joybStart = Input.GetButton("JoyButton 11"); //Start
        bool joyb12 = Input.GetButton("JoyButton 12"); //?
        bool joybL3 = Input.GetButton("JoyButton 13"); //L3 (LStick Click)
        bool joybR3 = Input.GetButton("JoyButton 14"); //R3 (RStick Click)
        bool joyb15 = Input.GetButton("JoyButton 15"); //?
        Vector2 joyaxy = new Vector2(Input.GetAxis("JoyAxis X"), Input.GetAxis("JoyAxis Y")); //Left Analog Stick
        Vector2 joya34 = new Vector2(Input.GetAxis("JoyAxis 3"), Input.GetAxis("JoyAxis 4")); //Right Analog Stick
        Vector2 joya56 = new Vector2(Input.GetAxis("JoyAxis 5"), Input.GetAxis("JoyAxis 6")); //D-Pad


        //Debug.Log("In: "+Input.GetButton("JoyButton 0") +", " + Input.GetButton("JoyButton 1") + ", " + Input.GetButton("JoyButton 2") + ", " + Input.GetButton("JoyButton 3") + ", " + Input.GetButton("JoyButton 4") + ", " + Input.GetButton("JoyButton 5") + ", " + Input.GetButton("JoyButton 6") + ", " + Input.GetButton("JoyButton 7") + ", " + Input.GetButton("JoyButton 8") + ", " + Input.GetButton("JoyButton 9") + ", " + Input.GetButton("JoyButton 10") + ", " + Input.GetButton("JoyButton 11") + ", " + Input.GetButton("JoyButton 12") + ", " + Input.GetButton("JoyButton 13") + ", " + Input.GetButton("JoyButton 14") + ", " + Input.GetButton("JoyButton 15") + ", "); 
        //Debug.Log(new Vector2(Input.GetAxis("JoyAxis X"), Input.GetAxis("JoyAxis Y")));
        //(JoyAxis X,JoyAxis Y) //Left Stick
        //(JoyAxis 3,JoyAxis 4) //Right Stick
        //(JoyAxis 5,JoyAxis 6) //D-Pad
        // (no analog for the trigger)

        if (true)
        {
            if (EtherealLock)
            {
                MyHealthBar.color = new Color(1f,1f,1f,.2f);
                MyHealthBar.fillAmount = 1f;
            } else
            {
                MyHealthBar.color = Color.white;
                MyHealthBar.fillAmount = (Health / 100f);
                
            }
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
                ResistanceText.text = "" + (VitaLevel + 1);
            }
            else if (VitaLevel >= 3)
            {
                int m = (VitaLevel - 3);
                ResistanceText.text = "MAX" + ((m > 0) ? ("+" + m) : "");
            }

            VitaPowerBaseText.GetComponent<Outline>().effectColor = Color.Lerp(ResistanceColor, Color.black, 0.5f);

            VitaPowerBaseText.text = getElementPowerLevelName(ResistanceElement, vitalevel);

            float frac = (ResistanceDisplayedValue - (Mathf.Floor(ResistanceDisplayedValue)));
            VitaPowerBaseText.text = "";//getElementPowerLevelName(StageElement, 0);
            VitaPower1Text.text = "";//"+" + getElementPowerLevelName(StageElement, 1);
            VitaPower2Text.text = "";//"+" + getElementPowerLevelName(StageElement, 2);
            VitaPower3Text.text = "";//"+" + getElementPowerLevelName(StageElement, 3);

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
                    VitaPowerBonusText.color = new Color(1f, 1f, 1f, .5f);
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
            ResistanceDisplayedValue = Mathf.Lerp(ResistanceDisplayedValue, rd, .1f);
            ResistanceGauge.value = (ResistanceDisplayedValue - Mathf.Floor(ResistanceDisplayedValue));//Mathf.Lerp(ResistanceGauge.value, ,.5f);

            ResistanceFill.color = Color.Lerp(ResistanceColor, Color.white, 0.5f + (0.5f * (Mathf.Clamp01(vitalevel / 3f)) * Mathf.Sin((Time.time / .8f) * 2f * Mathf.PI)));

            ResistanceFill.enabled = (ResistanceXP > 0f);

            float rv = (((float)vitalevel) / 3f);
            
            if (Quelling)
            {
                ResistanceGlow.transform.rotation = Quaternion.Euler(0f,0f,0f);

                ResistanceGlow.transform.localScale = (resistanceglowinitialscale * Mathf.Lerp(1f, 2f, ResistanceXP));
                ResistanceGlow.color = Color.Lerp(ResistanceGlow.color, new Color(ResistanceColor.r * 1f, ResistanceColor.g * 1f, ResistanceColor.b * 1f, 1f), .5f);
            } else
            {
                ResistanceGlow.transform.Rotate(0f, 0f, 135f * Time.deltaTime, Space.Self);
                ResistanceGlow.color = Color.Lerp(ResistanceGlow.color, new Color(ResistanceColor.r * rv, ResistanceColor.g * rv, ResistanceColor.b * rv, 1f), .5f);
            }
            ResistanceIcon.color = Color.Lerp(ResistanceIcon.color, ((vitalevel > 0) ? new Color(1f, 1f, 1f, 0.9f) : new Color(.5f, .5f, .5f, .5f)), .5f);

            if (StimPacks > 0) {
                StimPackIcon.color = Color.white;
                StimPackGlow.color = Color.Lerp(StimPackGlow.color, new Color(39f / 255f, 74f / 255f, 62f / 255f, 1f), .5f);
                StimCountText.text = "StimPacks x" + StimPacks;//"" + StimPacks+" StimPack"+((StimPacks==1)?"":"s");
                StimCountText.enabled = true;

            } else
            {
                StimPackGlow.color = Color.Lerp(StimPackGlow.color, Color.black, .5f);
                StimPackIcon.color = new Color(.3f, .3f, .3f, .4f);
                StimCountText.enabled = false;
            }
            bool stimpress = (Input.GetKeyDown(KeyCode.E)||joybY);
            bool rotelpress = joybL1;
            bool rotwppress = joybR1;

            if ((Alive) && (PlayerHasControl))
            {

                //lc = ((Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0));
                //vc = ((Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0));

                if (!Frozen)
                {
                    if (stimpress && !laststimpress)
                    {

                        if (StimPacks > 0)
                        {
                            applyStimPack();
                        }
                    }

                    if (joybR1)
                    {

                        usingmouseaiminput = false;

                        if (!SwappingWeapons)
                        {
                            if ((Time.time - JoystickWeaponSwapTapTime) >= .75f)
                            {
                                JoystickWeaponSwapTapIndex = 0;
                            } else
                            {
                                Debug.Log("Swap Index: "+ JoystickWeaponSwapTapIndex);
                                int cu = JoystickWeaponSwapTapIndex;
                                for (int ci = 1; ci < 7; ci++)
                                {
                                    int ve = ((cu + ci) % 7);
                                    if (hasWeaponAmmo(ve))
                                    {
                                        JoystickWeaponSwapTapIndex = ve;
                                        break;
                                    }

                                     //7 for the railgun
                                }
                                
                            }
                            JoystickWeaponSwapTapTime = Time.time;

                        }
                        //Debug.Log("Choose new weapon " + Time.time);
                        int bestindex = -1;
                        float bestdt = -1f;
                        for (int sa = 0; sa <= 6; sa++)
                        {
                            Vector2 dir = new Vector2(Mathf.Cos(Mathf.PI * 2f * ((float)sa / 6)), Mathf.Sin(Mathf.PI * 2f * ((float)sa / 6)));
                            Vector2 dif = new Vector2(joyaxy.x, joyaxy.y);
                            if ((sa < 6) && (dif.magnitude >= .4f))
                            {
                                float dt = Vector2.Dot(dir, dif);
                                if (dt > bestdt)
                                {
                                    bestdt = dt;
                                    bestindex = sa;
                                }

                            }
                            else
                            {
                                //if (dif.magnitude < .4f)
                                    //bestindex = sa;

                            }

                        }
                        if (bestindex == -1)
                        {
                            setWeapon((SpecialWeapon)(JoystickWeaponSwapTapIndex%7));
                            
                            

                        } else if (bestindex == 0)
                        {
                            setWeapon(SpecialWeapon.Shotgun);
                        }
                        else if (bestindex == 1)
                        {
                            setWeapon(SpecialWeapon.Gatling);
                        }
                        else if (bestindex == 2)
                        {
                            setWeapon(SpecialWeapon.Laser);
                        }
                        else if (bestindex == 3)
                        {
                            setWeapon(SpecialWeapon.GrenadeLauncher);
                        }
                        else if (bestindex == 4)
                        {
                            setWeapon(SpecialWeapon.TeslaGun);
                        }
                        else if (bestindex == 5)
                        {
                            setWeapon(SpecialWeapon.RailGun); //Railgun
                        }
                        else if (bestindex == 6)
                        {
                            setWeapon(SpecialWeapon.Pistol);
                        }
                        
                        SwappingWeapons = true;
                        SwapWeaponViewTime = Time.time;

                    }
                    else
                    {

                        if (!usingmouseaiminput)
                        SwappingWeapons = false;
                        //SwapWeaponViewTime = Time.time;
                    }


                    
                    //if (false)
                        if (rotwppress && !lastrotwppress)
                        {
                            rotateWeapon(true);
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
                    else if (Input.GetKeyDown(KeyCode.Alpha5))
                    {
                        setWeapon(SpecialWeapon.GrenadeLauncher);
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha6))
                    {
                        setWeapon(SpecialWeapon.TeslaGun);
                    }
                    else if (Input.GetKeyDown(KeyCode.Alpha7))
                    {
                        setWeapon(SpecialWeapon.RailGun);
                    }



                    if (joybR3) //Accompany Weapon swapping with a Circular Display, please.
                    {
                        if (false)
                            if (joya56.magnitude > 0.1f)
                            {
                                float gatlingdot = Vector2.Dot(joya56, Vector2.right);
                                float shotgundot = Vector2.Dot(joya56, Vector2.down);
                                float laserdot = Vector2.Dot(joya56, Vector2.up);
                                float pistoldot = Vector2.Dot(joya56, Vector2.left);
                                //tesla
                                //grenade launcher
                                float h = Mathf.Max(gatlingdot, shotgundot, laserdot, pistoldot);

                                if (h == gatlingdot)
                                {
                                    setWeapon(SpecialWeapon.Gatling);
                                }
                                else if (h == shotgundot)
                                {
                                    setWeapon(SpecialWeapon.Shotgun);
                                }
                                else if (h == laserdot)
                                {
                                    setWeapon(SpecialWeapon.Laser);
                                }
                                else if (h == pistoldot)
                                {
                                    setWeapon(SpecialWeapon.Pistol);
                                }
                                else
                                {
                                    setWeapon(SpecialWeapon.Pistol);
                                }

                            }
                            else
                            {
                                setWeapon(SpecialWeapon.Pistol);
                            }
                    }
                }
                if (true||(!Frozen) || (CrystalFrozen))
                {
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
                    if (rotelpress && !lastrotelpress)
                    {
                        rotateElement(true);
                    }
                }


            }
            lastrotelpress = rotelpress;
            lastrotwppress = rotwppress;
            
            laststimpress = stimpress;
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

            if (Invulnerable)
            {
                if (!Alive)
                {
                    revive();
                }
            }
            switch (ElementMode)
            {
                case Element.Fire:
                    {
                        MyEnergyBar.sprite = FireBarSprite; //MyEnergyBar.color = new Color(1f, .2f, .2f, 1f);
                        MyElementalIcon.sprite = ElementIcon_Fire;

                        if (FireElementPowerLevel >= 4)
                        {
                            MyElementLevelDots.sprite = ElementLevelDot_Max;
                            //MyEnergyBar.color = new Color(1f, 1f, 1f, 1f);
                        }
                        else if (FireElementPowerLevel >= 3)
                        {
                            MyElementLevelDots.sprite = ElementLevelDot_3;
                            //MyEnergyBar.color = new Color(1f, 1f, 1f, 1f);
                        }
                        else if (FireElementPowerLevel >= 2)
                        {
                            MyElementLevelDots.sprite = ElementLevelDot_2;
                            //MyEnergyBar.color = new Color(1f, 1f, 1f, 1f);
                        }
                        else if (FireElementPowerLevel >= 1)
                        {
                            MyElementLevelDots.sprite = ElementLevelDot_1;
                            //MyEnergyBar.color = new Color(1f, 1f, 1f, 1f);
                        }
                        else
                        {
                            MyElementLevelDots.sprite = ElementLevelDot_Affinity;
                            //MyEnergyBar.color = new Color(1f,1f,1f,.25f);
                        }
                            


                        MyElementText.text = "   FIRE";
                        MyEnergyBar.enabled = true;

                        break;
                    }
                case Element.Ice:
                    {
                        MyEnergyBar.sprite = IceBarSprite; //MyEnergyBar.color = new Color(.2f, .2f, 1f, 1f);
                        MyElementalIcon.sprite = ElementIcon_Ice;

                        if (IceElementPowerLevel >= 4)
                        {
                            MyElementLevelDots.sprite = ElementLevelDot_Max;
                            //MyEnergyBar.color = new Color(1f, 1f, 1f, 1f);
                        }
                        else if (IceElementPowerLevel >= 3)
                        {
                            MyElementLevelDots.sprite = ElementLevelDot_3;
                            //MyEnergyBar.color = new Color(1f, 1f, 1f, 1f);
                        }
                        else if (IceElementPowerLevel >= 2)
                        {
                            MyElementLevelDots.sprite = ElementLevelDot_2;
                            //MyEnergyBar.color = new Color(1f, 1f, 1f, 1f);
                        }
                        else if (IceElementPowerLevel >= 1)
                        {
                            MyElementLevelDots.sprite = ElementLevelDot_1;
                            //MyEnergyBar.color = new Color(1f, 1f, 1f, 1f);
                        }
                        else
                        {
                            MyElementLevelDots.sprite = ElementLevelDot_Affinity;
                            //MyEnergyBar.color = new Color(1f,1f,1f,.25f);
                        }

                        MyElementText.text = "   ICE";
                        MyEnergyBar.enabled = true;
                        break;
                    }
                case Element.Water:
                    {
                        MyElementalIcon.sprite = ElementIcon_Water;
                        MyEnergyBar.sprite = GrassBarSprite;//MyEnergyBar.color = new Color(.2f, .8f, .8f, 1f);
                        MyElementText.text = "   WATER";
                        MyEnergyBar.enabled = true;
                        break;
                    }
                case Element.Grass:
                    {
                        MyElementalIcon.sprite = ElementIcon_Grass;
                        MyElementText.text = "   JUNGLE";
                        if (GrassElementPowerLevel >= 4)
                        {
                            MyElementLevelDots.sprite = ElementLevelDot_Max;
                            //MyEnergyBar.color = new Color(1f, 1f, 1f, 1f);
                        }
                        else if (GrassElementPowerLevel >= 3)
                        {
                            MyElementLevelDots.sprite = ElementLevelDot_3;
                            //MyEnergyBar.color = new Color(1f, 1f, 1f, 1f);
                        }
                        else if (GrassElementPowerLevel >= 2)
                        {
                            MyElementLevelDots.sprite = ElementLevelDot_2;
                            //MyEnergyBar.color = new Color(1f, 1f, 1f, 1f);
                        }
                        else if (GrassElementPowerLevel >= 1)
                        {
                            MyElementLevelDots.sprite = ElementLevelDot_1;
                            //MyEnergyBar.color = new Color(1f, 1f, 1f, 1f);
                        }
                        else
                        {
                            MyElementLevelDots.sprite = ElementLevelDot_Affinity;
                            //MyEnergyBar.color = new Color(1f,1f,1f,.25f);
                        }

                        MyEnergyBar.sprite = GrassBarSprite;//MyEnergyBar.color = new Color(.2f, .8f, .2f, 1f); break;
                        MyEnergyBar.enabled = true;
                        break;
                    }
                case Element.None:
                default:
                    {
                        MyElementalIcon.sprite = ElementIcon_None;
                        MyElementText.text = "   --";
                        MyEnergyBar.sprite = GrassBarSprite;
                        MyEnergyBar.enabled = false;
                        MyEnergyBar.color = new Color(.2f, .8f, .2f, .5f); break;
                        //MyEnergyBar.color = new Color(.8f, .8f, .8f, 1f); break; }
                    }
            }

            if ((PlayerHasControl) && (!GameManager.TheGameManager.paused))
            {
                Cursor.visible = false;
            } else
            {
                Cursor.visible = true;
            }

            bool quelltrigger = false;
            if (QuellControl)
            {

                if (!lastquellcontrol)
                {
                    quelltrigger = true;
                    //On a button press
                }
            } else
            {
                if ((lastquellcontrol) && (Quelling &&(Time.time - LastQuellTime) >= 1f))
                {
                    quelltrigger = true;
                }
            }

            //if (false)
            if (quelltrigger )
            {
                if (!Quelling)
                {
                    if (((Time.time - LastQuellTime) >= (1f))&&(!QuellLocked))
                    {
                        LastQuellTime = Time.time;
                        Quelling = true;
                        QuellLocked = false;
                    }
                } else
                {
                    if (((Time.time - LastQuellTime) >= (1f)))
                    {
                        LastQuellTime = Time.time;
                        Quelling = false;
                        //QuellLocked = false;
                    }
                }
                

            }



            if (Quelling)
            {

                QuellValue = Mathf.Lerp(QuellValue,1f,.5f);
                QuellColor = Color.Lerp(ResistanceColor, ResistanceColor2, ResistanceXP);
                ParticleSystem.ColorOverLifetimeModule mem = QuellParticles.colorOverLifetime;
                mem.color.gradient.colorKeys = new GradientColorKey[2] { new GradientColorKey(Color.white, 0f), new GradientColorKey(ResistanceColor, 1f) };
                QuellParticles.Emit(2);// (int)(Time.deltaTime/Time.fixedDeltaTime));
                
                    //QuellingSoundFilter.enabled = true;
                    AudioManager.AM.CurrentMusic.pitch = .5f;
                
                float quellunitduration = 8f;
                if (VitaLevel >= 4)
                {
                    quellunitduration = 2f;
                } else if (VitaLevel >= 3)
                {
                    quellunitduration = 4f;
                } else if (VitaLevel >= 2)
                {
                    quellunitduration = 6f;
                }
                else if (VitaLevel >= 1)
                {
                    quellunitduration = 7f;
                }
                else
                {
                    quellunitduration = 8f;
                }
                float qdelta = (Time.deltaTime / quellunitduration);
                if (ResistanceXP <= qdelta)
                {
                    if (VitaLevel > 0) {
                        ResistanceXP = .999999f;
                        VitaLevel--;
                    }
                    else
                    {
                        ResistanceXP = 0f;
                        Quelling = false;
                        QuellLocked = false;

                    }
                } else
                {
                    ResistanceXP -= qdelta;
                }
            } else
            {
                //QuellingSoundFilter.enabled = false;
                if (AudioManager.AM && AudioManager.AM.CurrentMusic)
                    AudioManager.AM.CurrentMusic.pitch = 1f;
                QuellValue = Mathf.Clamp01(QuellValue - (Time.deltaTime*4f));
                if (((Time.time - LastQuellTime) >= 5f) && (VitaLevel > 0))
                {
                    //Play a Quell Ready Sound
                    QuellLocked = false;
                }
            }
                

                /*
                float siphdelta = (.4f*Time.deltaTime);
                float endelta = (40f * Time.deltaTime);
                float sf = 0f;


                //Play a Cascaded Quell Loop Sound
                if (ResistanceXP >= siphdelta)
                {
                    ResistanceXP -= siphdelta;
                    sf = 1f;
                } else if (VitaLevel > 0)
                {
                    VitaLevel--;
                    ResistanceXP = (1f - (siphdelta-ResistanceXP));
                    sf = 1f;
                } else if (ResistanceXP > 0f)
                {
                    sf = (ResistanceXP/siphdelta);
                        ResistanceXP = 0f;
                }
                if (sf > 0f)
                {
                    if (ElementalEnergy < 100f)
                    {
                        ElementalEnergy = Mathf.Min(100f, ElementalEnergy + (endelta * sf));
                    }

                    ParticleSystem.ColorOverLifetimeModule ms = QuellParticles.colorOverLifetime;
                    ParticleSystem.MinMaxGradient mg = ms.color;
                    Gradient gr = new Gradient();
                    gr.SetKeys(new GradientColorKey[] { new GradientColorKey(ResistanceColor, 0f), new GradientColorKey((ElementMode != Element.None) ? getElementColor(this.ElementMode) : Color.clear, 1f) }, new GradientAlphaKey[] { new GradientAlphaKey(0f, 0f), new GradientAlphaKey(1f, .25f), new GradientAlphaKey(1f, 1f) });
                    mg.gradient = gr;
                    if (!QuellParticles.isPlaying) QuellParticles.Play();
                } else
                {
                    if (QuellParticles.isPlaying) QuellParticles.Stop();
                }
                */
                
            //} else
            //{
                //if (QuellParticles.isPlaying) QuellParticles.Stop();
            //}
            lastquellcontrol = QuellControl;

            string ammostring = "";
            switch (CurrentSpecialWeapon)
            {
                case SpecialWeapon.Pistol: { ammostring = "Pistol: " + PistolAmmo; break; }
                case SpecialWeapon.Shotgun: { ammostring = "Shotgun: " + ShotgunAmmo; break; }
                case SpecialWeapon.Gatling: { ammostring = "Machinegun: " + GatlingAmmo; break; }
                case SpecialWeapon.Laser: { ammostring = "Laser: " + (int)LaserAmmo+"%"; break; }
                case SpecialWeapon.GrenadeLauncher: { ammostring = "Grenade Launcher: " + GrenadeLauncherAmmo; break; }
                case SpecialWeapon.TeslaGun: { ammostring = "Tesla Gun: " + (int)TeslaAmmo+"%"; break; }
                case SpecialWeapon.RailGun: { ammostring = "Rail Gun: " + RailGunAmmo; break; }
                case SpecialWeapon.None: { ammostring = ""; break; }
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
                if ((BlackFadeAlpha >= 1f) && FinishingVoidStage && !_FinishedStage)
                {
                    _FinishedStage = true;
                    GameManager.TheGameManager.completeStage(Element.Void);
                }
                BlackFadeAlpha = Mathf.Clamp01(BlackFadeAlpha + (Time.deltaTime / 1.5f));
                
            }
            if (FadingWhiteThenBlack)
            {
                float fa = (BlackFadeAlpha/.5f);
                if (fa < .5f)
                {
                    fa = (fa / .5f);
                    BlackFade.color = new Color(1f, 1f, 1f, fa);
                } else
                {
                    fa = Mathf.Clamp01(1f-((fa - .5f) / .5f));
                    BlackFade.color = new Color(fa, fa, fa, 1f);
                }
                
            }
            else
            {
                BlackFade.color = new Color(0f, 0f, 0f, BlackFadeAlpha);
            }

            

            
            
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
            if (Health >= 95f)
            {
                CriticalBackSound.volume = Mathf.Lerp(CriticalBackSound.volume, 0f,Mathf.Lerp(Time.fixedDeltaTime*.05f,.25f, Health));
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
            if (Introduced)
            {
                if ((Time.time - IntroLandTime) >= 10f)
                {
                    int co = 0;
                    if (HasElementFire) co++;
                    if (HasElementIce) co++;
                    if (HasElementGrass) co++;
                    if (co >= 1)
                    {
                        GameManager.TheGameManager.showTutorialTip(TutorialSystem.TutorialTip.UseElement);
                    }
                    if (co >= 2)
                    {
                        GameManager.TheGameManager.showTutorialTip(TutorialSystem.TutorialTip.ScrollElement);
                    }
                }
            }
            if (!Alive)
            {
                //if ((Time.time - DeathTime) >= 4f) //Back when dieing resulted in a game over
                //{
                //FadingInOut = false;
                //}
                drainResistanceWhileKnockedOut();
                if (PlayerHasControl)
                {
                    
                    if (Input.GetKeyDown(KeyCode.Space)|| Input.GetButtonDown("JoyButton 0"))
                    {
                        Debug.Log("TAP");
                        int antij = 0;
                        if (Input.GetKeyDown(KeyCode.Space))
                            
                        {
                            antij = 1;

                        } 
                        if (Input.GetButtonDown("JoyButton 0"))
                        {
                            antij = 2;
                        }
                        if ((antij != AntiJ) && (AntiJ != 0) && (antij != 0))
                        {
                            //ManualRevive = 0f;
                            //Don't bother alternating the A Button and Spacebar...cheater...
                            AntiJ = antij;
                        }
                        
                        attemptToRevive();
                    }
                }
                if (Time.time - DeathTime > 1.25f)
                {
                    GameManager.TheGameManager.showTutorialTip(TutorialSystem.TutorialTip.Revive);
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

                    bool bu = true;
                    if ((EtherealManager.TheEtherealManager != null) && (EtherealManager.TheEtherealManager.gameObject.activeInHierarchy) && (EtherealManager.TheEtherealManager.FinishedEtherealTutorial))
                    {
                        
                        if ((Time.time - EtherealManager.TheEtherealManager.EtherealTutorialFinishTime) >= 3f)
                        {
                            /*
                            if (!TeleportedOut)
                            {
                                TeleportOut();
                            }
                            else if ((Time.time - TeleportedTime) >= 1.25f)
                            {
                                FadingInOut = false;

                                AudioManager.AM.crossfade(AudioManager.AM.CurrentMusic, 0f, 1.4f);
                            }
                            */
                            bu = true;
                        } else
                        {
                            bu = false;
                        }
                    } else
                    {
                        bu = false;
                    }
                    if (!ElementGoal.ShowingTutorialZone)
                        bu = true;
                    //else
                    //{
                        if (bu)
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

                            }
                            else if (MyEndLevelScreen.LookingAtResults)
                            {

                                if (Input.GetMouseButton(0))
                                {
                                    MyEndLevelScreen.DoneLookingAtResults = true;

                                }
                            }

                        }
                    //}
                }
            }
            if (Alive)
            {
                AntiJ = 0;
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
        sporeStep();
        pressStep();
        stimStep();
        directiveStep();
        voidFlashStep();
        fallingAreaStep();
        
        int vl = VitaLevel;
        AggressionLevelAbsolute = VitaLevel;
        if (vl > 3) vl = 3;
        else if (vl < 0) vl = 0;
        AggressionLevel = vl;
        AggressionLevelF = (vl / 3f); 
        //0-1-2-3
        //1-2-3-MAX
        if (HasElementIce)
        {
            if (IceElementPowerLevel >= 0)
            {
                IcePowerFactor = ((ElementalEnergy > 0f) ? Mathf.Clamp01(((float)IceElementPowerLevel) / 3f):0f);
                IcePowerScale = Mathf.Clamp01(ElementalEnergy/(100f-(25f*Mathf.Min(IceElementPowerLevel,3))));
                
                
            }
            else
            {
                IcePowerFactor = 0f;
                IcePowerScale = 0f;   
            }
            IcePowerUsage = 1f + (.5f * (Mathf.Max(0, IceElementPowerLevel - 3)));
            if (ElementAffinity == Element.Ice)
                IcePowerUsage = .4f;
        } else
        {
            IcePowerUsage = 1f;
            IcePowerFactor = 0f;
        }

        if (HasElementFire)
        {
            FirePowerUsage = 1f + (.5f * (Mathf.Max(0, FireElementPowerLevel - 3)));
            if (FireElementPowerLevel >= 0)
            {
                FirePowerFactor = ((ElementalEnergy > 0f) ? Mathf.Clamp01(((float)FireElementPowerLevel) / 3f) : 0f);
                FirePowerScale = Mathf.Clamp01(ElementalEnergy / (100f - (25f * Mathf.Min(FireElementPowerLevel, 3))));
            }
            else
            {
                FirePowerFactor = 0f;
                FirePowerScale = 0f;
            }
            if (ElementAffinity == Element.Fire)
                FirePowerUsage = .4f;
        }
        else
        {
            FirePowerFactor = 0f;
            FirePowerUsage = 1f;
        }
        if (HasElementGrass)
        {
            JunglePowerUsage = 1f + (.5f * (Mathf.Max(0, GrassElementPowerLevel - 3)));
            if (GrassElementPowerLevel >= 0)
            {
                JunglePowerFactor = ((ElementalEnergy > 0f) ? Mathf.Clamp01(((float)GrassElementPowerLevel) / 3f) : 0f);
                JunglePowerScale = Mathf.Clamp01(ElementalEnergy / (100f - (25f * Mathf.Min(GrassElementPowerLevel, 3))));
            }
            else
                JunglePowerFactor = 0f;

            if (ElementAffinity == Element.Grass)
                JunglePowerUsage = .4f;
        }
        else
        {
            JunglePowerUsage = 1f;
            JunglePowerFactor = 0f;
        }

        if (HasElementVoid)
        {
            VoidPowerUsage = 1f + (.5f * (Mathf.Max(0, VoidElementPowerLevel - 3)));
            if (VoidElementPowerLevel >= 0)
                VoidPowerFactor = ((ElementalEnergy > 0f)? Mathf.Clamp01(((float)VoidElementPowerLevel) / 3f):0f);
            else
                VoidPowerFactor = 0f;
            if (ElementAffinity == Element.Void)
                VoidPowerUsage = .4f;
        }
        else
        {
            VoidPowerFactor = 0f;
        }
        FirePowerUsage = (1f/FirePowerUsage);
        IcePowerUsage = (1f / IcePowerUsage);
        JunglePowerUsage = (1f / JunglePowerUsage);
        VoidPowerUsage = (1f / VoidPowerUsage);



        updateOverlay();
        
    }
    public ParticleSystem IceReadyParticles, FireReadyParticles, JungleReadyParticles;
    
    private bool laststimpress = false;

    public static Color getElementColor(Element e)
    {
        Color c = Color.white;
        switch (e)
        {
            case Element.All:
                {
                    break;
                }
            case Element.Fire:
                {
                    c = new Color(1f, .6f, .1f);
                    break;
                }
            case Element.Ice:
                {
                    c = new Color(.3f, .3f, 1f);
                    break;
                }
            case Element.Grass:
                {
                    c = new Color(.2f, 1f, .1f);
                    
                    break;
                }
            case Element.Void:
                {
                    c = new Color(.8f, 0f, .8f);
                    break;
                }
            
            
        }

        return c;
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
            case Element.Void:
                { cl = AudioManager.AM.VoidPlanetMusic; break; }
        }
        AudioManager.AM.playMusic(cl, 1f, 1f, true);
    }

    private float BurningTime= -10f;
    public bool OnFire = false;
    public ParticleSystem Shockwave;
    public ParticleSystem JetPackParticles;
    public GameObject VoidFlare = null;
    public GameObject VoidFlarePrefab;
    private bool FinishingVoidStage = false;
    private bool VoidFlashMaxed = false;
    private float VoidFlashScale = 0f;
    public void voidFlashStep()
    {
        if (!FinishingVoidStage) return;
        float highscale = 150f;
        VoidFlashScale = Mathf.Clamp01(VoidFlashScale + (Time.deltaTime / 2f));
        VoidFlare.transform.localScale = (Vector3.one * (VoidFlashScale * highscale));
        if ((VoidFlashScale >= 1f))
        {
            if (!VoidFlashMaxed)
            {
                VoidFlashMaxed = true;
                FadingInOut = false;
                FadingWhiteThenBlack = true;
            }

            
        } else
        {

        }

       
    }
    public bool BeatenVoidIceBoss = false;
    public bool BeatenVoidFireBoss = false;
    public bool BeatenVoidJungleBoss = false;
    
    public void finishVoidStage(Vector3 defeatedvoidbosslocation)
    {
        if (FinishingVoidStage) return;
        FinishingVoidStage = true;
        VoidFlashScale = 0f;
        VoidFlashMaxed = false;
        VoidFlare = GameObject.Instantiate(VoidFlarePrefab,defeatedvoidbosslocation + new Vector3(0f, 0f, -4.5f), VoidFlarePrefab.transform.rotation);
        AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.ElementObtained, AudioManager.AM.PlayerAudioMixer, volume: 1f, pitch: 1f, looped: false, destroyafter: 7f);
        AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.ElementGoalCollect, AudioManager.AM.PlayerAudioMixer, volume: 1f, pitch: 1f, looped: false, destroyafter: 7f);

    }
    public void cheat_TeleportToBoss()
    {
        GameObject bot = GameObject.Find("BossTeleport");
        if (bot != null)
        {
            this.transform.position = bot.transform.position;
        }

    }
    private bool lastquellcontrol = false;
    private float LastQuellTime = -10f;
    public SpriteRenderer PistolSprite, ShotgunSprite, MachinegunSprite, LasergunSprite,GrenadeLauncherSprite,TeslaGunSprite, RailGunSprite;
    public float lastshottime = -10f, lastpistolshottime = -10f;
    public void gunShock(Vector3 dir)
    {
        CameraRigCurrentPosition = (CameraRigCurrentPosition - new Vector3(dir.x, dir.y, 0f));
        MyCameraRig.position = (MyCameraRig.position - new Vector3(dir.x, dir.y, 0f));
    }
    private AudioSource PistolShootSound, PistolReloadSound, PistolEmptySound, ShotgunSound,MachinegunLoop,BeamRifleLoop, BeamRiflePlusLoop, BeamRifleHitLoop, BeamRifleDamageLoop, BeamRifleStart, GrenadeLauncherSound, GrenadeLauncherDetonatorSound;
    private AudioSource RailGunChargeSound, RailGunShootDry, RailGunShootMedium, RailGunShootMax,RailGunOvercharge;
    private int pistolshotsfired = 0;
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
                        float damage = 40f;
                        float speed = 50f;
                        float maxfirerate = (firerate * 2f);
                        if ((((Time.time - lastshottime) >= (1f / firerate))||(ShootControl != lastShootControl)) )
                        {
                            if (PistolAmmo > 0)
                            {
                                float uf = Mathf.Clamp01(((Time.time - lastshottime) - (1f / maxfirerate)) / ((1f / firerate) - (1f / maxfirerate))); //1f if holding down normally, <1f if rapid firing
                                lastshottime = Time.time;
                                lastpistolshottime = Time.time;
                                float noise = (1f - uf);
                                
                                if (uf < 1f)
                                {
                                    //Debug.Log("Spam: "+uf);
                                    Vector3 crs = Vector3.Cross(dir.normalized, Vector3.forward);
                                    float deltaangle = ((Mathf.PI * 2f * (15f / 360f)) * (uf)) * (Random.Range(-1f, 1f));

                                    dir = ((dir * Mathf.Cos(deltaangle)) + (crs * Mathf.Sin(deltaangle)));
                                }
                                Bullet b = GameObject.Instantiate(BulletPrefab, this.transform.position, new Quaternion()).GetComponent<Bullet>();//output.transform.
                                b.Damage = damage;
                                //b.Velocity
                                b.Velocity = (new Vector2(dir.x, dir.y) * speed);

                                b.transform.Rotate(0f, 0f, Vector3.SignedAngle(Vector3.right, new Vector3(b.Velocity.normalized.x, b.Velocity.normalized.y, 0f), Vector3.forward));
                                PistolAmmo--;
                                gunShock(dir.normalized * .1f);
                                pistolshotsfired++;
                                if (pistolshotsfired >= 100) GameManager.TheGameManager.showTutorialTip(TutorialSystem.TutorialTip.PistolFastFire);
                                if (PistolShootSound == null)
                                    PistolShootSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.PistolShoot, AudioManager.AM.EnvironmentAudioMixer, .9f, 1f, false);
                                PistolShootSound.pitch = 1f + ((Random.Range(-1f, 1f) * .35f * noise));
                                PistolShootSound.Play();
                                if (Quelling)
                                {
                                    Quelling = false;
                                }
                            } else
                            {
                                if (ShootControl != lastShootControl)
                                {
                                    if (PistolEmptySound == null)
                                        PistolEmptySound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.PistolEmpty, AudioManager.AM.EnvironmentAudioMixer, .9f, 1f, false);
                                    


                                    PistolEmptySound.Play();
                                }
                            }
                            

                            if (PistolAmmo < 10)
                                LastGunHUDAlertTime = Time.time;
                            
                            
                        }

                        

                         
                    }
                    if ((Time.time - lastshottime) >= 2f)
                    {
                        if (PistolAmmo < MAXPISTOLAMMO)
                        {
                            PistolAmmo = MAXPISTOLAMMO;
                            //Play a pistol reload sound
                            if (PistolReloadSound == null)
                                PistolReloadSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.PistolReload, AudioManager.AM.EnvironmentAudioMixer, .9f, 1f, false);
                            PistolReloadSound.Play();
                        }
                    }

                    break; }
                case SpecialWeapon.Shotgun: {
                        if (ShootControl)
                        {
                            //switch (weapon)
                            float firerate = 1.5f;
                            float damage = 400f;
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
                            if (Quelling)
                            {
                                Quelling = false;
                            }
                            int frags = 0;
                                b.Damage = (b.Damage / ((float)fragments));
                                Vector3 cross = Vector3.Cross(dir, Vector3.forward);
                            float ef = ((float)(((float)2) / ((float)(fragments - 1))));
                                for (float f = -1f; f <= 1f; f += ef)
                                {


                                    float p = (((spreadangle * .5f * (f+((Random.value-.5f)*ef))) / 180f) * Mathf.PI);
                                    Vector3 projdir = ((cross * Mathf.Sin(p)) + (dir * Mathf.Cos(p)));
                                    Bullet bu;
                                    if ((frags == (fragments / 2)))
                                        bu = b;
                                    else
                                        bu = GameObject.Instantiate(b);
                                    //bu.Velocity
                                    bu.Velocity = (new Vector2(projdir.x, projdir.y) * speed);
                                    bu.transform.Rotate(0f, 0f, Vector3.SignedAngle(Vector3.right, new Vector3(bu.Velocity.x, bu.Velocity.y, 0f), Vector3.forward));



                                frags++;
                                }
                                ShotgunAmmo--;
                            gunShock(dir.normalized * .5f);
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
                        float damage = 40f;
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
                                bu.Velocity = (new Vector2(projdir.x, projdir.y) * speed*(.75f+(Random.value*.5f)));
                                bu.transform.Rotate(0f, 0f, Vector3.SignedAngle(Vector3.right, new Vector3(bu.Velocity.x, bu.Velocity.y, 0f), Vector3.forward));


                                frags++;
                            }
                            if (GatlingAmmo < 25)
                                LastGunHUDAlertTime = Time.time;
                            GatlingAmmo--;
                            if (Quelling)
                            {
                                Quelling = false;
                            }
                            gunShock(dir.normalized * .2f);
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
                        float damagepersecond = 300f;
                        float ammoconsumptionpersecond = 20f;
                        LaserAmmo = Mathf.Max(0f,LaserAmmo - (ammoconsumptionpersecond*Time.deltaTime));
                        if (Quelling)
                        {
                            Quelling = false;
                        }
                        gunShock(dir.normalized * .05f);
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
                                //if (rh.collider.isTrigger) continue;
                                pi = Mathf.Max(pierce, pi);
                                en.TakeDamage(dmg, r.direction * 1f);
                                Astronaut.TheAstronaut.tickHitMarker(dmg, (en.Health / en.MaxHealth) * (en.Alive ? 1f : 0f), !en.Alive);
                                lh = true;
                            }
                            else
                            {
                                BossGolem bo = rh.collider.gameObject.GetComponent<BossGolem>();

                                
                                bool weakspot = false;
                                BossWeakSpot bwsp = rh.collider.GetComponent<BossWeakSpot>();
                                if (bwsp != null)
                                {
                                    bo = bwsp.MyBossGolem;
                                    weakspot = true;
                                }
                                if ((bo != null) && (!bo.Defeated))
                                {

                                    if (rh.collider.Equals(bo.MyWeakSpot))
                                        weakspot = true;
                                    float dm = dmg;
                                    if (weakspot)
                                    {
                                        dm = (dm * 2f);
                                        if (dm >= 1f)
                                        {


                                            bo.CriticalHitEffect.Emit(1);
                                            bo.CriticalHitEffectSub.Emit(1);

                                        }
                                        else
                                        {
                                            bo.damagelayover = (bo.damagelayover + dm);
                                            if ((bo.damagelayover) >= 1f)
                                            {
                                                bo.damagelayover -= 1f;
                                                bo.CriticalHitEffect.Emit(1);
                                                bo.CriticalHitEffectSub.Emit(1);
                                            }
                                        }
                                        pi = Mathf.Max(pierce, pi);
                                        bo.TakeDamage(dm, r.direction * 1f);
                                    }
                                    else
                                    {
                                        pi = Mathf.Max(pierce, pi);
                                        bo.TakeDamage(dm, r.direction * 1f);
                                    }
                                
                                    
                                    
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


                    if ((GrenadeLauncherAmmo <= 0) && ((MyLaunchedGrenade == null)||(!MyLaunchedGrenade.Live))) setWeapon(SpecialWeapon.Pistol);
                    if (ShootControl)
                        {
                            //switch (weapon)
                            float firerate = 1.85f;
                            float damage = 50f;
                            float speed = 10f;

                            if (((Time.time - lastshottime) > (1f / firerate)) && (GrenadeLauncherAmmo > 0) && (!lastShootControl) &&(MyLaunchedGrenade == null))
                            {
                                lastshottime = Time.time;

                                
                                Grenade b = GameObject.Instantiate(GrenadePrefab, this.transform.position, new Quaternion()).GetComponent<Grenade>();//output.transform.
                            b.Damage = damage;
                            
                            //b.Velocity
                            b.Sustaining = true;
                            MyLaunchedGrenade = b;
                            b.MyRigidbody.velocity = (new Vector2(dir.x, dir.y) * speed);
                            
                                if (GrenadeLauncherSound == null)
                                GrenadeLauncherSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.GrenadeLaunch, AudioManager.AM.EnvironmentAudioMixer, 1f, 1f, false);
                                GrenadeLauncherSound.PlayOneShot(GrenadeLauncherSound.clip);
                                


                            GrenadeLauncherAmmo--;
                            if (Quelling)
                            {
                                Quelling = false;
                            }
                            gunShock(dir.normalized * .25f);
                            if (GrenadeLauncherAmmo < 5)
                                    LastGunHUDAlertTime = Time.time;
                                if (GrenadeLauncherAmmo <= 0) setWeapon(SpecialWeapon.Pistol);
                            }
                        else if (MyLaunchedGrenade != null)
                        {
                            if (!lastShootControl)
                            {
                                if (GrenadeLauncherDetonatorSound == null)
                                    GrenadeLauncherDetonatorSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.GrenadeDetonator, AudioManager.AM.EnvironmentAudioMixer, .5f, 1f, false);
                                if ((MyLaunchedGrenade != null)&&(MyLaunchedGrenade.Live)) {

                                    lastshottime = Time.time;
                                    GrenadeLauncherDetonatorSound.Play();
                                    MyLaunchedGrenade.explode();
                                    //MyLaunchedGrenade.Remove();
                                    MyLaunchedGrenade = null;
                                    if (Quelling)
                                    {
                                        Quelling = false;
                                    }
                                }
                                
                            }
                        }




                        }  else  {
                            if ((lastShootControl) && ((Time.time - lastshottime) >= .25f))
                            {
                                if (MyLaunchedGrenade != null) 
                                {
                                if (MyLaunchedGrenade.Sustaining)
                                {
                                    MyLaunchedGrenade.Sustaining = false;
                                    MyLaunchedGrenade.MyRigidbody.gravityScale = 3f;
                                }
                                }
                            }

                        }



                        break;
                    
                }
            case SpecialWeapon.TeslaGun: {

                    if (TeslaAmmo <= 0f) setWeapon(SpecialWeapon.Pistol);
                    if (ShootControl)
                    {
                        


                        float damagepersecond = 140f;
                        float ammoconsumptionpersecond = 8f;//10f
                        float flashammoconsumption = 20f;//10f
                        float flashdamage = 10f;
                        bool startup = false;
                        if (!lightningtick)
                        {
                            startup = true;
                           
                        }
                        if (TeslaAmmo > 0f)
                        {
                            if (MyLightning == null)
                            {
                                MyLightning = GameObject.Instantiate<TeslaLightning>(TeslaLightningPrefab, this.transform.position,TeslaLightningPrefab.transform.rotation,this.transform);
                                //MyLightning.transform.SetParent(this.transform);    
                            }

                            MyLightning.DamagePerSecond = damagepersecond;
                            MyLightning.FlashDamage = flashdamage;
                            
                            float flashinterval = .75f;
                            if (startup) {

                                if (((Time.time - MyLightning.FlashTime) >= flashinterval))
                                {
                                    MyLightning.LightningTick = true;
                                    MyLightning.UsingFlash = true;
                                    lightningtick = true;
                                } else
                                {
                                    lightningtick = false;
                                    MyLightning.UsingFlash = false;
                                    MyLightning.LightningTick = false;
                                }

                            } else
                            {
                                lightningtick = true;
                                MyLightning.LightningTick = true;
                                MyLightning.UsingFlash = false;
                            }
                            
                            
                            
                            MyLightning.LightningDirection = dir;
                            MyLightning.LightningTickDelta = Time.fixedDeltaTime;
                            if (lightningtick)
                            {

                                if (MyLightning.UsingFlash)
                                {
                                    TeslaAmmo = Mathf.Max(0f, TeslaAmmo - (flashammoconsumption * Time.fixedDeltaTime));
                                } else
                                {
                                    TeslaAmmo = Mathf.Max(0f, TeslaAmmo - (ammoconsumptionpersecond * Time.fixedDeltaTime));
                                }
                                if (Quelling)
                                {
                                    Quelling = false;
                                }
                                gunShock(dir.normalized * .05f);
                            }
                            

                            

                        }


                    }

                    break;
                }
            case SpecialWeapon.RailGun:
                {

                    if (RailGunAmmo <= 0) setWeapon(SpecialWeapon.Pistol);
                    //switch (weapon)
                    float firerate = 1.1f;
                        float damage = 150f;
                        float speed = 80f;
                        float maxspeed = 240f;
                        float chargetime = 2.5f;
                        float chargeholdtime = 2.5f;
                    bool fireshot = false;
                    
                    
                    if (ShootControl && ((Time.time - lastshottime) >= (1f / firerate)))
                    {
                        //Charge the shot
                        if (!railcharging)
                        {
                            railcharging = true;
                            RailGunCharge = 0f;
                            if (RailGunShootDry == null) //RailGunCharging
                                RailGunShootDry = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.RailGunShootDry, AudioManager.AM.EnvironmentAudioMixer, .9f, 1f, false);
                            RailGunChargingParticles.Play();
                            if (RailGunFullyChargedParticles.isPlaying)
                                RailGunFullyChargedParticles.Stop();
                            RailGunChargeSound.Play();
                            //RailGunOvercharge.Stop();
                        }
                        float rf = RailGunCharge + ((RailGunCharge < 1f)?(Time.fixedDeltaTime / chargetime): (Time.fixedDeltaTime / chargeholdtime));
                        
                        if ((RailGunCharge < 1f) && (rf >= 1f))
                        {
                            //Play the charge ready sound
                            if (RailGunShootDry == null) //RailGunMaxCharge
                                RailGunShootDry = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.RailGunShootDry, AudioManager.AM.EnvironmentAudioMixer, .9f, 1f, false);
                            if (RailGunShootDry == null) //RailGunReady
                                RailGunShootDry = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.RailGunShootDry, AudioManager.AM.EnvironmentAudioMixer, .9f, 1f, false);
                            RailGunChargingParticles.Stop();
                            RailGunFullyChargedParticles.Play();
                            RailGunReadyParticles.Play();
                            RailGunChargeSound.Stop();
                            RailGunOvercharge.Play();
                        }

                        if (rf >= 2f) {
                            fireshot = true;
                        }
                        RailGunCharge = rf;
                    }
                    else
                    {
                        if (railcharging)
                        {
                            fireshot = true;
                        }
                    }
                            if ((RailGunAmmo > 0) && (fireshot))
                            {
                                float uf = Mathf.Clamp01(((Time.time - lastshottime) - (1f / firerate)) / ((1f / firerate) - (1f / firerate))); //1f if holding down normally, <1f if rapid firing
                                lastshottime = Time.time;
                                lastpistolshottime = Time.time;



                                Bullet b = GameObject.Instantiate(RailSlugPrefab, this.transform.position, new Quaternion()).GetComponent<Bullet>();//output.transform.
                                float ch = Mathf.Clamp01(RailGunCharge);
                                b.Damage = (damage*Mathf.Lerp(1f,3f,ch));
                                //b.Velocity
                                b.Velocity = (new Vector2(dir.x, dir.y) * speed*Mathf.Lerp(1f,3f,ch));

                                b.transform.Rotate(0f, 0f, Vector3.SignedAngle(Vector3.right, new Vector3(b.Velocity.normalized.x, b.Velocity.normalized.y, 0f), Vector3.forward));
                                RailGunAmmo--;
                        RailGunChargeSound.Stop();
                        RailGunOvercharge.Stop();
                        if (Quelling)
                        {
                            Quelling = false;
                        }
                        gunShock(dir.normalized * (.25f+(.25f*Mathf.Clamp01(RailGunCharge))));

                        if (RailGunShootDry == null)
                                    RailGunShootDry = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.RailGunShootDry, AudioManager.AM.EnvironmentAudioMixer, .9f, 1f, false);
                                if (RailGunShootMedium == null)
                                    RailGunShootMedium = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.RailGunShootMedium, AudioManager.AM.EnvironmentAudioMixer, .9f, 1f, false);
                                if (RailGunShootMax == null)
                                    RailGunShootMax = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.RailGunShootMax, AudioManager.AM.EnvironmentAudioMixer, .9f, 1f, false);
                                if (RailGunCharge >= 1f)
                                    RailGunShootMax.Play();
                                else if (RailGunCharge >= .25f)
                                    RailGunShootMedium.Play();
                                else
                                    RailGunShootDry.Play();

                        RailGunChargingParticles.Stop();
                        RailGunFullyChargedParticles.Stop();
                        railcharging = false;
                RailGunCharge = 0f;
                            }
                            
                        

                            if (RailGunAmmo < 3)
                                LastGunHUDAlertTime = Time.time;
                            //gunShock(dir.normalized*.1f);

                        





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

        if (ElementalEnergy < 100f)
            if ((Time.time - LastElementalTime) >= 3f)
        {
            //Debug.Log("Regen");
			float rechargetime = 10f;
            ElementalEnergy = 100f; //(ElementalEnergy+((100f*Time.deltaTime)/rechargetime));
            Am.am.oneshot(am.M.EnergyRecharge);
                triggerElementReady(ElementMode);
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
                    //float powerlevel = ((float)FireElementPowerLevel / 10f);
                    //Inferno Bar
                    //if (MyFlameBar) MyFlameBar.FirePowerLevel = powerlevel;
                    if ((ElementalControl)) {
                        float selfmeltdist = .1f;
                        if (!Frozen)
                        {
                            ElementalEnergy = (ElementalEnergy - (40f*(FirePowerUsage)) * Time.deltaTime);
                            if (ElementalEnergy > 0f)
                                LastElementalTime = Time.time;
                            else
                                LastElementalTime = Mathf.Min(Time.time, (LastElementalTime + (Time.fixedDeltaTime * 2f)));

                            Ray2D r = new Ray2D(this.transform.position, dir.normalized);

                            float maxfiredist = (2f + (10f * FirePowerFactor)* FirePowerScale);

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
                                if (FirePowerFactor > .25f)
                                if ((Time.time - LastFirePatchPlaceTime)>= .15f){
                                    FlameBar fp = GameObject.Instantiate<FlameBar>(FlamePatchPrefab, new Vector3(rh.point.x, rh.point.y, 0f), FlamePatchPrefab.transform.rotation);
                                        fp.transform.localScale = (Vector3.one * (1f+(2f*FirePowerFactor*FirePowerScale)));
                                        LastFirePatchPlaceTime = Time.time;
                                        }
                                if (Airborne)
                                {
                                    MyRigidbody.AddForce(-new Vector2(dir.x, dir.y) * 100f*(FirePowerFactor) * (1f - (rh.distance / maxfiredist)), ForceMode2D.Force); //If the player is moving away from this direction
                                    FlameBarPressureSound.volume = 1f;
                                    Am.am.M.crossfade(FlameBarPressureSound,0f,.4f);
                                }
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
                            //ProjectileCollisionFilter = new ContactFilter2D();
                            Collider2D[] colarray = new Collider2D[32];
                            MyFlameBar.MyCollider.OverlapCollider(ProjectileCollisionFilter,colarray);
                            foreach (Collider2D col in colarray)
                            {
                                if (col == null) continue;
                                EnemyProjectile ep = col.gameObject.GetComponent<EnemyProjectile>();
                                if ((ep == null)||(!ep.Live)) continue;
                                Rigidbody2D rb = ep.GetComponent<Rigidbody2D>();
                                //rb.velocity = (dir.normalized * rb.velocity.magnitude);
                                rb.AddForce(50f*dir*FirePowerFactor);
                                Debug.Log("Collider: "+col.name);
                                

                            }

                            if (!MyFlameBar.FlameActive)
                            {
                                FlameBarSound.Stop();
                                FlameBarSound.Play();
                                FlameBarSound.volume = 0f;
                                FlameBarPressureSound.Stop();
                                FlameBarPressureSound.Play();
                                FlameBarPressureSound.volume = 0f;
                            }
                            FlameBarSound.volume = Mathf.Lerp(FlameBarSound.volume,1f,.75f);
                            MyFlameBar.FlameActive = true;

                            
                        } else
                        {
                            //THAW!
                            ElementalEnergy = (ElementalEnergy - (40f * (FirePowerUsage)) * Time.deltaTime);
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

                            if (MyFreezingCrystal != null)
                            {

                                MyFreezingCrystal.TakeDamage((80f + (400f * FirePowerFactor)) * Time.fixedDeltaTime, this.transform.forward * 0.001f);
                                Am.am.oneshot(Am.am.M.FireElementSinge);
                                if (!MyFreezingCrystal.Alive)
                                {
                                    Am.am.oneshot(Am.am.M.MeltSound);
                                    if (MeltParticles)
                                    {
                                        MeltParticles.Emit(20);
                                    }
                                    unfreeze();
                                }
                                else
                                {
                                    if (MeltParticles)
                                    {
                                        MeltParticles.Emit(3);
                                    }
                                }
                            } else
                            {
                                FreezeTime -= (Time.deltaTime * 8f);
                                //Play the Melt sound

                                if (FreezeTime <= Time.time)
                                {
                                    Am.am.oneshot(Am.am.M.MeltSound);
                                    if (MeltParticles)
                                    {
                                        MeltParticles.Emit(20);
                                    }
                                }
                                else
                                {
                                    if (MeltParticles)
                                    {
                                        MeltParticles.Emit(3);
                                    }
                                }
                            }
                            
                            
                            

                            
                        }


                        if (!MyFlameBar.MyParticles.isPlaying) {
						MyFlameBar.MyParticles.Play(true);
					    }



                        
                    } else {
                        if (MyFlameBar.FlameActive)
                        {
                            AudioManager.AM.crossfade(FlameBarSound, 0f, 1f);
                            AudioManager.AM.crosstune(FlameBarSound, .9f, 1f);
                        }
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
                    float powerlevel = ((float)IceElementPowerLevel / 4f);

                    if (false) {
                        if ((ElementalControl))
                        {
                            ElementalEnergy = (ElementalEnergy - (20f * IcePowerUsage*Time.deltaTime));
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

                        if (MyIceBeam == null)
                        {
                            MyIceBeam = GameObject.Instantiate(IceBeamPrefab,this.transform.position, IceBeamPrefab.transform.rotation, this.transform);
                            IceLaserBaseScale = IceBeamPrefab.transform.localScale.x;
                            IceLaserBaseDistance = (this.transform.position - MyIceBeam.HitDamageParticles.transform.position).magnitude;
                        }
                        Vector3 startpoint = this.transform.position;
                        Vector3 endpoint = new Vector3();
                        bool frosthit = false;
                        bool beamhit = false;
                        if ((ElementalControl) && (!Frozen))
                        {
                            if (ElementalEnergy > 0f)
                                LastElementalTime = Time.time;
                            else
                                LastElementalTime = Mathf.Min(Time.time,  (LastElementalTime +(Time.fixedDeltaTime*2f)));

                            ElementalEnergy = (ElementalEnergy - (((40f * IcePowerUsage)) * Time.deltaTime));
                            
                            //Ray Cast
                            Ray2D r = new Ray2D(this.transform.position, dir);

                            float maxicedist = 2f+(12f*IcePowerFactor*IcePowerScale);
                            RaycastHit2D rh = Physics2D.Raycast(r.origin, r.direction, maxicedist, LayerMask.GetMask(new string[] { "Geometry","EnemyProjectile","Default","Enemy","Intangible","Hazard","Boss","VoidBush"}));
                            if (rh.collider != null)
                            {
                                beamhit = true;
                                IceBlock ice = rh.collider.gameObject.GetComponentInParent<IceBlock>();

                                if (ice != null)
                                {
                                    ice.LastTickTime = Time.time;
                                    if (IcePowerFactor > 0f)
                                        ice.transform.localScale = new Vector3(ice.transform.localScale.x, ice.transform.localScale.y, ice.transform.localScale.z + ((2f + (8f * IcePowerFactor)) * Time.deltaTime));

                                    ParticleSystem[] pe = ice.gameObject.GetComponentsInChildren<ParticleSystem>();
                                    foreach (ParticleSystem p in pe)
                                    {
                                        if (!p.isPlaying)
                                        {
                                            p.Play();
                                        }
                                    }
                                    endpoint = rh.point;

                                }
                                else
                                {
                                    Collider2D col = rh.collider;
                                    GenericEnemy en = col.GetComponent<GenericEnemy>();
                                    if ((en != null) && (en.Alive))
                                    {
                                        Astronaut.TheAstronaut.createFreezeParticlesForEnemy(en, col);
                                        //if (CanFreezeEnemies)
                                        //Debug.Log("IcePowerFactor "+IcePowerFactor);
                                        en.slowFreeze(Time.fixedDeltaTime * (.5f * (1f+(4f * IcePowerFactor * 1f))), 1.2f * (1f + (IcePowerFactor * 3f)));
                                        //en.freeze(1.2f);
                                        endpoint = rh.point;
                                        frosthit = true;
                                    } else {
                                        VoidGolem bo = col.GetComponent<VoidGolem>();
                                        if ((bo != null) && (bo.VoidElementType == VoidGolem.VoidElement.Jungle) && (!bo.Defeated))
                                        {
                                            float fps = Time.fixedDeltaTime * (.5f + (4f * IcePowerFactor * 1f));
                                            float mh = (bo.MossSkinHealth - (fps));
                                            if (mh <= 0f)
                                            {
                                                bo.MossSkinHealth = 0f;
                                                bo.freezeMossSkinOff();
                                            } else
                                            {
                                                bo.MossSkinHealth = mh;
                                            }

                                            endpoint = rh.point;
                                            frosthit = true;
                                        }
                                    else {
                                        EnemyProjectile proj = col.gameObject.GetComponent<EnemyProjectile>();
                                        FrozenProjectile froproj = col.gameObject.GetComponentInChildren<FrozenProjectile>();
                                        if ((proj != null) && ((proj.enabled) && (proj.Live)) && (froproj == null))
                                        {
                                            if ((proj.enabled) && (proj.Live)) {
                                                proj.slowFreeze(.25f * (1f + (12f * IcePowerFactor)) * (Time.fixedDeltaTime * 1f));
                                                if (proj.FreezeFactor >= 1f)
                                                    freezeProjectile(proj);


                                                GameObject.Destroy(proj.gameObject, 10f);
                                            }

                                            endpoint = rh.point;
                                            frosthit = true;
                                        }
                                        else
                                        {


                                            Hazard haz = rh.collider.gameObject.GetComponent<Hazard>();
                                            FrozenHazard froha = rh.collider.gameObject.GetComponent<FrozenHazard>();
                                            if (haz != null)
                                            {
                                                Debug.Log("Hazard");
                                                endpoint = rh.point;

                                                haz.slowFreeze(.25f * (Time.fixedDeltaTime * (8f * IcePowerFactor) * 1f));
                                                //freezeHazard(haz);
                                                frosthit = true;
                                            }
                                            else if (rh.collider.gameObject.layer != LayerMask.NameToLayer("Ignore") && (froproj == null) && (froha == null))
                                            {
                                                float l = Mathf.Sign((AimPosition - this.transform.position).x);
                                                if (l == 0f) l = 1f;
                                                //Debug.DrawLine(rh.point,rh.point+rh.normal*3f,Color.blue,3f);
                                                if ((!rh.normal.Equals(Vector2.zero)) && (Vector3.Dot(new Vector3(rh.normal.x, rh.normal.y, 0f), Vector3.forward) == 0f))
                                                {
                                                    //ice = GameObject.Instantiate(IceBlockPrefab, rh.point, Quaternion.LookRotation((this.transform.position - new Vector3(rh.point.x, rh.point.y, 0f)).normalized, Vector3.up)).GetComponent<IceBlock>();//Quaternion.LookRotation(Vector3.right * l, Vector3.up)
                                                    //Quaternion.LookRotation(rh.normal, Vector3.forward)
                                                    if (IcePowerFactor > 0f)
                                                    {
                                                        ice = GameObject.Instantiate(IceBlockPrefab, rh.point, IceBlockPrefab.transform.rotation).GetComponent<IceBlock>();

                                                        ice.transform.rotation = Quaternion.LookRotation(new Vector3(rh.normal.x, rh.normal.y), Vector3.forward);
                                                    }

                                                    //Vector3 relative = transform.InverseTransformPoint(rh.point+);
                                                    //float eangle = Mathf.Atan2(relative.x, relative.y) * Mathf.Rad2Deg;
                                                    //ice.transform.Rotate(0, 0, eangle,Space.World);
                                                }
                                                endpoint = rh.point;
                                                frosthit = false;
                                            } else
                                            {
                                                endpoint = rh.point;
                                            }
                                        }
                                    }
                                }

                                }
                                if (ice != null)
                                {
                                    ParticleSystem[] ps = ice.GetComponentsInChildren<ParticleSystem>();
                                    foreach (ParticleSystem p in ps)
                                    {
                                        ParticleSystem.ShapeModule m = p.shape;
                                        m.position = new Vector3(0f, 0f, ice.transform.localScale.z * .5f);
                                        //m.scale = new Vector3(0f, 0f, ice.transform.localScale.z * 1f);
                                    }
                                    endpoint = rh.point;
                                }


                                MyIceBeam.transform.LookAt(endpoint);
                                MyIceBeam.transform.Rotate(new Vector3(0f, -90f, 0f));
                                MyIceBeam.transform.localScale = new Vector3(IceLaserBaseScale * ((rh.distance) / IceLaserBaseDistance), MyIceBeam.transform.localScale.y, MyIceBeam.transform.localScale.z);
                            } else
                            {
                                endpoint = (this.transform.position + (dir * maxicedist));
                                MyIceBeam.transform.LookAt(endpoint);
                                MyIceBeam.transform.Rotate(new Vector3(0f, -90f, 0f));
                                MyIceBeam.transform.localScale = new Vector3(IceLaserBaseScale * (maxicedist / IceLaserBaseDistance), MyIceBeam.transform.localScale.y, MyIceBeam.transform.localScale.z);
                            }
                            Vector3[] posarray = new Vector3[] { startpoint, endpoint };
                            MyIceBeam.MyBeamRenderer.useWorldSpace = true;
                            MyIceBeam.MyBeamRenderer.positionCount = posarray.Length;
                            MyIceBeam.MyBeamRenderer.SetPositions(posarray);
                            
                            
                            MyIceBeam.MyBeamRenderer.enabled = true;
                            if (!MyIceBeam.FastParticles.isPlaying) MyIceBeam.FastParticles.Play();
                            if (!MyIceBeam.GlowParticles.isPlaying) MyIceBeam.GlowParticles.Play();
                            
                            
                            if (beamhit)
                            {
                                if (!MyIceBeam.HitDamageParticles.isPlaying)
                                MyIceBeam.HitDamageParticles.Play(true);
                                
                            } else
                            {
                                if (MyIceBeam.HitDamageParticles.isPlaying)
                                    MyIceBeam.HitDamageParticles.Stop(true);
                            }
                            if (frosthit)
                            {
                                if (!MyIceBeam.FreezingParticles.isPlaying)
                                    MyIceBeam.FreezingParticles.Play(true);
                            }
                            else
                            {
                                if (MyIceBeam.FreezingParticles.isPlaying)
                                    MyIceBeam.FreezingParticles.Stop(true);
                            }


                        } else
                        {
                            if (MyIceBeam)
                            {
                                MyIceBeam.MyBeamRenderer.enabled = false;
                                MyIceBeam.HitDamageParticles.Stop(true);
                                MyIceBeam.FreezingParticles.Stop(true);
                                if (MyIceBeam.FastParticles.isPlaying) MyIceBeam.FastParticles.Stop();
                                if (MyIceBeam.GlowParticles.isPlaying) MyIceBeam.GlowParticles.Stop();

                            }
                        }
                        }
                        else if (true) //Create an Ice Pillar
                        {

                            if ((ElementalControl) && (!Frozen)&& (ElementalEnergy > 0f))
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


                    if ((ElementalControl) && (!Frozen))
                    {

                        if (ElementalEnergy > 0f)
                            LastElementalTime = Time.time;
                        else
                            LastElementalTime = Mathf.Min(Time.time,(LastElementalTime + (Time.fixedDeltaTime * 2f)));
                        //LastElementalTime = Time.time;


                        //Grab an enemy if you can
                        GrassSwinging = true;

                        if ((MyVines2 == null) &&((Time.time - vineshoottime)>=.4f))
                        {
                            //Shoot a Ray
                            MyVines2 = GameObject.Instantiate(VinePrefab2, this.transform.position, VinePrefab2.transform.rotation).GetComponent<Vines2>();
                            MyVines2.TravelDirection = dir;
                            MyVines2.MyAstronaut = this;
                            vineshoottime = Time.time;
                            
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
                                bool burn = true;
                                
                                if (((MyVines2.VineAttachedToEnemy != null) && (MyVines2.VineAttachedToEnemy.Alive))||(MyVines2.VineAttachedToGolem != null))
                                {
                                    
                                    GrassSwinging = false;
                                }
                                else
                                {
                                    GrassSwinging = true;
                                }
                                if (MyVines2.VineAttachedToGolem != null)
                                    burn = false;
                                
                                ElementalEnergy = (ElementalEnergy - (((50f * (JunglePowerUsage))) * Time.deltaTime));
                            } else
                            {
                                GrassSwinging = false;
                            }
                            MyVines2.Sustaining = true;
                        }
                        //Debug.Log("Push? " + ((AimPosition - transform.position).magnitude >= 2f));
                        MyVines2.ControlPush = false;//((AimPosition - transform.position).magnitude >= 2f);
                        MyVines2.ControlHold = false; //(GrassElementPowerLevel >= 3);
                        MyVines2.EnergyDrain = true; //(GrassElementPowerLevel >= 2);// 3
                        MyVines2.HoldLocation = AimPosition;
                        MyVines2.MaxTravelDistance = (2f + (12f * JunglePowerFactor)* JunglePowerScale);



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

        if (ElementMode != Element.Fire)
        {
            if (MyFlameBar.FlameActive)
            {
                
                AudioManager.AM.crossfade(FlameBarSound, 0f, 1f);
                AudioManager.AM.crosstune(FlameBarSound, .9f, 1f);
                MyFlameBar.FlameActive = false;
            }
        }
        if (ElementMode != Element.Ice)
        {
            if (MyIceBeam)
            {
                if (MyIceBeam.FastParticles)
                    if (MyIceBeam.FastParticles.isPlaying) MyIceBeam.FastParticles.Stop();
                if (MyIceBeam.GlowParticles)
                    if (MyIceBeam.GlowParticles.isPlaying) MyIceBeam.GlowParticles.Stop();
                if (MyIceBeam.HitDamageParticles)
                    if (MyIceBeam.HitDamageParticles.isPlaying) MyIceBeam.HitDamageParticles.Stop(true);
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
    private float LastFirePatchPlaceTime = -10f;
    [HideInInspector]
    public ParticleSystem myabsorbparticles;
    public Material GlacierMaterial;
    private AudioSource FlameBarSound, FlameBarPressureSound, VineSustainSound;
    public ParticleSystem IncinerationParticlesPrefab;
    private bool lightningtick = false;
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
    private ParticleSystem MySporeEffect = null;
    public float MySporeValue = 0f;
    public ParticleSystem SporeEffectPrefab;
    public FrozenProjectile FrozenProjectilePrefab;
    public void freezeProjectile(EnemyProjectile proj)
    {

        proj.Live = false;
        proj.enabled = false;
        proj.Disabled = false;
        proj.gameObject.layer = LayerMask.NameToLayer("Intangible");
        FrozenProjectile frp = GameObject.Instantiate(FrozenProjectilePrefab, proj.transform.position, proj.transform.rotation).GetComponent<FrozenProjectile>();
        //FreezePoof.Play();
        //proj.gameObject.layer = LayerMask.NameToLayer("Ignore");
        ParticleSystem eff = GameObject.Instantiate(EnemyProjectileFreezePrefab, proj.transform.position, proj.transform.rotation).GetComponent<ParticleSystem>();
        GameObject.Destroy(eff.gameObject,5f);
        am.oneshot(am.M.chooseSound(am.M.FreezeSound3, am.M.FreezeSound4));
        foreach (ParticleSystem p in proj.gameObject.GetComponentsInChildren<ParticleSystem>())
        {
            if (!p.transform.Equals(this.transform))
            {
                p.transform.SetParent(null);
                if (p.main.loop)
                    p.Stop();

                p.Stop();
                
            }
            else
            {
                //p.enabled = false;
                p.Stop();
            }
            GameObject.Destroy(p.gameObject, p.main.duration);
        }

        foreach (SpriteRenderer sp in proj.GetComponentsInChildren<SpriteRenderer>())
        {
            sp.material = GlacierMaterial;
        }
        foreach (ConstantForce2D cf in proj.GetComponentsInChildren<ConstantForce2D>())
        {
            cf.enabled = false;
        }
        foreach (Collider2D co in proj.GetComponentsInChildren<Collider2D>())
        {
            Physics2D.IgnoreCollision(MyCollider,co);
            if ((co.enabled) && (co.bounds.extents.magnitude<5f)) {
                co.isTrigger = false; 
            }
        }
        Rigidbody2D rb = proj.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            //GameObject.Destroy(rb);
            rb.velocity = new Vector2();
            rb.gravityScale = 1f;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.freezeRotation = false;

            Collider2D col = proj.gameObject.GetComponent<Collider2D>();
            if (col != null)
            {
                Physics2D.IgnoreCollision(MyCollider, col,true);
            }
        }

        frp.transform.SetParent(proj.transform);
        frp.transform.position = frp.transform.position + new Vector3(0f, 0f, -2f);
    }
    public void freezeHazard(Hazard haz)
    {
        haz.permafreeze();

        //IceExplosion.Play();
    }
    private bool lastShootControl=false;
    public Image BossHealthBar;

    
	public FlameBar MyFlameBar,BurningPrefab;
    public FlameBar FlamePatchPrefab;
    public IceBomb IceBombPrefab;
    public IceBomb MyHeldIceBomb = null;
    
    public Vines VinePrefab;
    private Vines MyVines;
    private Vines2 MyVines2;
    private float vineshoottime = -10f; 
    public Vines2 VinePrefab2;


    public ParticleSystem WaterPump;
    public ParticleSystem FrostBlower;
    public IceBlock IceBlockPrefab;
    public IceBeam MyIceBeam;
    public IceBeam IceBeamPrefab;
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
    public Image MyElementLevelDots;
    public Text DeathCountText,MyElementText; 
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
    public Image StimVignette,PainVignette,ExtremeVignette,VitaVignette;
    public ParticleSystem DirectiveIceGlow, DirectiveFireGlow, DirectiveJungleGlow;
    private bool DirectiveGlowsActive = false;
    private float DirectiveActionTime = -10f;
    private float DirectiveStartTime = 0f;
    public void activateDirectiveGlows()
    {
        if (DirectiveGlowsActive) return;
        DirectiveGlowsActive = true;
        DirectiveStartTime = Time.time;
    }
    public bool HasDefeatedVoidIceGolem = false, HasDefeatedVoidFireGolem = false, HasDefeatedVoidJungleGolem = false;
    private float LastDirectiveGlowTime = -10f;
    public void directiveStep()
    {
        if (DirectiveGlowsActive)
        {
            if ((((Time.time - lastshottime)>= 5f) && ((Time.time - lastDamageTakenTime) >= 5f)) && (Alive) && (PlayerHasControl) && (MyBossGolem == null)&&((Time.time - LastDirectiveGlowTime)>= 8f))
            {
                LastDirectiveGlowTime = Time.time;
                Am.am.oneshot(Am.am.M.DirectiveDisplay);
                if (!HasDefeatedVoidIceGolem)
                    DirectiveIceGlow.Play();
                if (!HasDefeatedVoidFireGolem)
                    DirectiveFireGlow.Play();
                if (!HasDefeatedVoidJungleGolem)
                    DirectiveJungleGlow.Play();


            }
            Vector3 dif;
            Vector3 pos = new Vector3(MyCameraRig.position.x, MyCameraRig.position.y, 0f);
            if (!HasDefeatedVoidIceGolem)
            {
                dif = (VoidGolem.TheIceVoidGolem.transform.position - this.transform.position);
                if (dif.magnitude > 10f)
                {
                    DirectiveIceGlow.transform.position = (pos + (dif.normalized * 4f*(1f*Mathf.Pow(Mathf.Clamp01((Time.time - LastDirectiveGlowTime)/4f),2.5f)))) + (Vector3.forward * -4f);
                } else
                {
                    DirectiveIceGlow.Stop();
                }
            }
            if (!HasDefeatedVoidFireGolem)
            {
                dif = (VoidGolem.TheFireVoidGolem.transform.position - this.transform.position);
                if (dif.magnitude > 10f)
                {
                    DirectiveFireGlow.transform.position = (pos + (dif.normalized * 4f* (1f * Mathf.Pow(Mathf.Clamp01((Time.time - LastDirectiveGlowTime) / 4f),2.5f)))) + (Vector3.forward * -4f);
                }
                else
                {
                    DirectiveFireGlow.Stop();
                }
            }
            if (!HasDefeatedVoidJungleGolem)
            {
                dif = (VoidGolem.TheJungleVoidGolem.transform.position - this.transform.position);
                if (dif.magnitude > 10f)
                {
                    DirectiveJungleGlow.transform.position = (pos + (dif.normalized * 4f* (1f* Mathf.Pow(Mathf.Clamp01((Time.time - LastDirectiveGlowTime) / 4f),2.5f))))+(Vector3.forward*-4f);
                }
                else
                {
                    DirectiveJungleGlow.Stop();
                }
            }

        }
        

    }
    private float BlackFadeAlpha = 1f;
    public bool FadingInOut = true;
    [HideInInspector]public bool FadingWhiteThenBlack = false;
    public Text MyWeaponAmmoText;
    public Sprite ElementIcon_Fire, ElementIcon_Ice, ElementIcon_Water, ElementIcon_Grass, ElementIcon_None;
    public Sprite ElementLevelDot_Affinity, ElementLevelDot_1, ElementLevelDot_2, ElementLevelDot_3, ElementLevelDot_Max;
    private float LastElementalTime = -10f;

    private float LastElementRotateTime = -10f;
    public ParticleSystem TeleportationParticles;
    private bool TeleportedOut = false;
    private float TeleportedTime = -10f;
    public void TeleportOut()
    {
        TeleportationParticles.Play();
        MySpriteRenderer.gameObject.SetActive(false);
        Am.am.oneshot(Am.am.M.TeleportSound1);
        Am.am.oneshot(Am.am.M.TeleportSound2);
        PlayerHasControl = false;
        TeleportedOut = true;
        TeleportedTime = Time.time;
    }
    public bool ControllerInputDetected;
    public void rotateElement(bool nextprevious)
    {
        if (EtherealLock) return;
        if ((Time.time - LastElementRotateTime) <= .15f) return;
        Element em = ElementMode;
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

        if (ElementMode != em)
        {
            if (PlayerHasControl)
            {
                ElementSwapSound.Stop();
                ElementSwapSound.Play();
                triggerElementReady(ElementMode);
            }
            LastElementRotateTime = Time.time;
        }
    }
    public void triggerElementReady(Element em)
    {
        
        IceReadySound.Stop();
        JungleReadySound.Stop();
        FireReadySound.Stop();
        


        IceReadyParticles.Clear();
        JungleReadyParticles.Clear();
        FireReadyParticles.Clear();

        if (ElementMode == Element.Ice)
        {
            IceReadySound.Play();
            IceReadyParticles.Play();
        }
        else if (ElementMode == Element.Fire)
        {
            FireReadySound.Play();
            FireReadyParticles.Play();
        }
        else if (ElementMode == Element.Grass)
        {
            JungleReadySound.Play();
            JungleReadyParticles.Play();
        } //, FireReady, JungleReady, ElementRotate;
    }
    private AudioSource IceReadySound, FireReadySound, JungleReadySound,ElementSwapSound;
    public bool SwappingWeapons = false;
    public float SwapWeaponViewTime = -10f;


    public void rotateWeapon(bool nextprevious)
    {
        int rot = 0;
        SpecialWeapon wep = CurrentSpecialWeapon;
        SpecialWeapon[] weplist = new SpecialWeapon[] { SpecialWeapon.Pistol, SpecialWeapon.Gatling, SpecialWeapon.Shotgun, SpecialWeapon.Laser, SpecialWeapon.GrenadeLauncher, SpecialWeapon.TeslaGun, SpecialWeapon.RailGun };
        int ind = 0;
        for (int i = 0; i < weplist.Length; i++)
        {
            if (weplist[i] == wep)
            {
                ind = i;
            }
            
        }
        Debug.Log("Rotate Weapons");
        bool has = false;
        int e = (ind);
        do
        {
            if (nextprevious)rot++; else rot--;
            e = (ind + rot);
            has = false;
            if (e >= weplist.Length) e -= weplist.Length;
            else if (e < 0) e += weplist.Length;
            switch (weplist[e])
            {
                case SpecialWeapon.Pistol:
                    {
                        has = true;
                        break;
                    }
                case SpecialWeapon.Gatling:
                    {
                        has = (GatlingAmmo > 0);
                        break;
                    }
                case SpecialWeapon.Shotgun:
                    {
                        has = (ShotgunAmmo > 0);
                        break;
                    }
                case SpecialWeapon.Laser:
                    {
                        has = (LaserAmmo > 0f);
                        break;
                    }
                case SpecialWeapon.GrenadeLauncher:
                    {
                        has = (GrenadeLauncherAmmo > 0f);
                        break;
                    }
                case SpecialWeapon.TeslaGun:
                    {
                        has = (TeslaAmmo > 0f);
                        break;
                    }
                case SpecialWeapon.RailGun:
                    {
                        has = (RailGunAmmo > 0f);
                        break;
                    }


            }
            
        } while ((Mathf.Abs(rot) < weplist.Length)&&(!has));
        e = (ind + rot);
        if (e >= weplist.Length) e -= weplist.Length;
        else if (e < 0) e += weplist.Length;
        if (weplist[e] != CurrentSpecialWeapon)
        {
            setWeapon(weplist[e]);
        }

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
    private bool usingmouseaiminput = false;
    public bool UsedControllerInput = false;
    private Vector3 oldmouseposition;
    public float JungleMudTouchTime = -10f;
    public bool JungleMudTouching = false;
    public ParticleSystem MyMudParticles, MyMudSplashParticles;

    private void FixedUpdate()
    {
        //MyRigidbody.velocity = new Vector2(0f, 1f);
        //return;
        
        bool stunned = false;
        if ((Time.time - lastStunTime) <= .4f)
        {
            stunned = true;
        }

        bool joybA = Input.GetButton("JoyButton 0"); //A
        bool joybB = Input.GetButton("JoyButton 1"); //B
        bool joyb2 = Input.GetButton("JoyButton 2"); //?
        bool joybX = Input.GetButton("JoyButton 3"); //X
        bool joybY = Input.GetButton("JoyButton 4"); //Y
        bool joyb5 = Input.GetButton("JoyButton 5"); //?
        bool joybL1 = Input.GetButton("JoyButton 6"); //L1
        bool joybR1 = Input.GetButton("JoyButton 7"); //R1
        bool joybL2 = Input.GetButton("JoyButton 8"); //L2
        bool joybR2 = Input.GetButton("JoyButton 9"); //R2
        bool joybSelect = Input.GetButton("JoyButton 10"); //Select
        bool joybStart = Input.GetButton("JoyButton 11"); //Start
        bool joyb12 = Input.GetButton("JoyButton 12"); //?
        bool joybL3 = Input.GetButton("JoyButton 13"); //L3 (LStick Click)
        bool joybR3 = Input.GetButton("JoyButton 14"); //R3 (RStick Click)
        bool joyb15 = Input.GetButton("JoyButton 15"); //?
        
        Vector2 joyaxy = new Vector2(Input.GetAxis("JoyAxis X"), Input.GetAxis("JoyAxis Y")); //Left Analog Stick
        Vector2 joya34 = new Vector2(Input.GetAxis("JoyAxis 3"), Input.GetAxis("JoyAxis 4")); //Right Analog Stick
        Vector2 joya56 = new Vector2(Input.GetAxis("JoyAxis 5"), Input.GetAxis("JoyAxis 6")); //D-Pad

        /*
        if(PlayerHasControl)
        if (Input.GetKeyDown(KeyCode.PageDown))
            cheat_TeleportToBoss();
        */
        /*
        if ((joyaxy.magnitude > 0.1f) || (joya34.magnitude > 0.1f) || (joya56.magnitude > 0.1f))
        {
            
        }
        */
        int lc = 0;
        int vc = 0;
        bool jc = false;
        bool ec = false;
        bool sp = false;
        bool siphpress = false;
        jc = ((Input.GetKey(KeyCode.Space)) || Input.GetButton("JoyButton 0"));
        
        if ((Alive) && (PlayerHasControl))
        {
            //if ((!GrassSwinging) && (!Frozen))
            
                lc = ((Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0));
                vc = ((Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0));
                if ((joyaxy.magnitude > 0.1f) || (joya56.magnitude > 0.1f))
                {
                UsedControllerInput = true;
                if ((joya56.magnitude > 0.1f))
                    {
                    //Dpad
                    if (!false)
                    {
                        
                        lc = (int)Mathf.Sign(joya56.x);
                        vc = (int)Mathf.Sign(joya56.y);
                    }
                    } else
                    {

                    //Analog stick
                    if ((!joybL3) && (!joybL1) && (!SwappingWeapons))
                    {
                        if (Mathf.Abs(joyaxy.x) > .25f)
                            lc = (int)Mathf.Sign(joyaxy.x);
                        if (Mathf.Abs(joyaxy.y) > .25f)
                            vc = (int)Mathf.Sign(joyaxy.y);
                        //SwappingWeapons = false;
                    }
                }
                }

            


            //Key press of Weapons
            if (Input.GetKey(KeyCode.Alpha1))
            {
                //setWeapon(SpecialWeapon.Pistol);
                SwappingWeapons = true;
                SwapWeaponViewTime = Time.time;
                usingmouseaiminput = true;
            }
            else if (Input.GetKey(KeyCode.Alpha2))
            {
                //setWeapon(SpecialWeapon.Shotgun);
                SwappingWeapons = true;
                usingmouseaiminput = true;
                SwapWeaponViewTime = Time.time;
            }
            else if (Input.GetKey(KeyCode.Alpha3))
            {
                //setWeapon(SpecialWeapon.Gatling);
                SwappingWeapons = true;
                usingmouseaiminput = true;
                SwapWeaponViewTime = Time.time;
            }
            else if (Input.GetKey(KeyCode.Alpha4))
            {
                //setWeapon(SpecialWeapon.Laser);
                SwappingWeapons = true;
                usingmouseaiminput = true;
                SwapWeaponViewTime = Time.time;
            }
            else if (Input.GetKey(KeyCode.Alpha5))
            {
                //setWeapon(SpecialWeapon.GrenadeLauncher);
                SwappingWeapons = true;
                usingmouseaiminput = true;
                SwapWeaponViewTime = Time.time;
            }
            else if (Input.GetKey(KeyCode.Alpha6))
            {
                //setWeapon(SpecialWeapon.TeslaGun);
                SwappingWeapons = true;
                usingmouseaiminput = true;
                SwapWeaponViewTime = Time.time;
            }
            else if (Input.GetKey(KeyCode.Alpha7))
            {
                //setWeapon(SpecialWeapon.RailGun);
                SwappingWeapons = true;
                usingmouseaiminput = true;
                SwapWeaponViewTime = Time.time;
            }
            else
            {
                if (usingmouseaiminput)
                SwappingWeapons = false;
            }


            if (!Frozen)
                sp = (Input.GetMouseButton(0)||joybR2); //Input.GetButton("JoyButton 3") || joybR1
            siphpress = (Input.GetKey(KeyCode.LeftShift) || joybB);//||joyb?? //Oh you wanted B to be siphon from the beginning, huh. nice!
            
                ec = (Input.GetMouseButton(1) || joybL2);//Input.GetButton("JoyButton 1")||joybR2

            if (Input.mousePosition != oldmouseposition)
                usingmouseaiminput = true;

            oldmouseposition = Input.mousePosition;
            Vector2 controldir = joya34;
            //lockinfluence
            Vector3 controlAP = this.transform.position;
                if (joya34.magnitude > .1f)
                {
                    controlAP = this.transform.position + (new Vector3(joya34.x, joya34.y, 0f) * 3f); //Right-Analogue
                    usingmouseaiminput = false;
                } else if (joya56.magnitude > .1f)
                {
                controlAP = this.transform.position + (new Vector3(joya56.x, joya56.y, 0f)*3f); //D-Pad
                controldir = joya56;
                usingmouseaiminput = false;
                } else if (joyaxy.magnitude > 0.1f)
                {
                controlAP = this.transform.position + (new Vector3(joyaxy.x, joyaxy.y, 0f) * 3f); //Left Analogue
                controldir = joyaxy;
                usingmouseaiminput = false;
                } else
                {
                //Not actively aiming
                if (!usingmouseaiminput)
                {
                    controlAP = this.transform.position + (new Vector3((float)LookDirection, 0f, 0f) * 3f); //Left Analogue
                }
                }
            if (usingmouseaiminput)
            {
                Ray pr = Camera.main.ScreenPointToRay(Input.mousePosition);
                AimPosition = (pr.origin - (pr.direction * (pr.origin.z / pr.direction.z)));
                UsedControllerInput = false;
                ReticleTransform.position = AimPosition;
                lastcontroldir = new Vector2();
            } else
            {
                float cm = ((controldir - lastcontroldir).magnitude/1f);
                lockinfluence = Mathf.Clamp01(lockinfluence-cm);
                
                float lockrange = 10f;
                Vector3 dir = (controlAP - this.transform.position);
                Vector2 dir2 = new Vector2(dir.x,dir.y).normalized;
                Vector2 mypos = this.transform.position;
                bool includeprojectiles = ((Time.time - LastElementalTime) <= .5f);
                int laym = LayerMask.GetMask("Enemy", "Boss");
                if (includeprojectiles)
                {
                    laym = LayerMask.GetMask("Enemy", "Boss","EnemyProjectile");
                }

                Collider2D[] lockcandidates = Physics2D.OverlapCircleAll(mypos + (dir2*lockrange*.5f), (lockrange * .5f),laym);
                Transform lockontarget = null;
                float lockonscore = 0f;
                foreach (Collider2D cand in lockcandidates) {
                    GenericEnemy g = cand.gameObject.GetComponent<GenericEnemy>();
                    BossGolem b = cand.gameObject.GetComponent<BossGolem>();
                    if ((g == null) && (b == null)) { }

                    Vector3 dif = (cand.transform.position - this.transform.position);
                    RaycastHit2D rh = Physics2D.Linecast(this.transform.position, cand.transform.position, LayerMask.GetMask(new string[] { "Geometry" }));
                    if (rh.collider != null) continue;
                    NotLockable nl = cand.gameObject.GetComponent<NotLockable>();
                    if ((nl != null) && (!nl.Revealed)) continue;
                    float sc = (1f/(1f+(dif.magnitude/1f)))*(Mathf.Clamp01(Vector3.Dot(dir2,dif.normalized)));

                    if ((lockontarget == null)||(sc > lockonscore))
                    {
                        lockontarget = cand.transform;
                        lockonscore = sc;
                    }
                }
                //if (controldir.magnitude >.4f)
                    if (lockontarget != null)
                {
                    lockinfluence = (lockinfluence + (Time.fixedDeltaTime * 2f));
                    AimPosition = Vector3.Lerp(AimPosition, Vector3.Lerp(controlAP,new Vector3(lockontarget.position.x, lockontarget.position.y, AimPosition.z),lockinfluence), .25f);
                }
                else
                {
                    AimPosition = Vector3.Lerp(AimPosition, controlAP, .25f);
                }

                ReticleTransform.position = AimPosition;
                lastcontroldir = controldir;
            }





            
            ReticleTransform.gameObject.SetActive(true);
            
        } else
        {
            Quelling = false;
            ReticleTransform.gameObject.SetActive(false);
        }
        if ((!Frozen) && (PlayerHasControl))
            JumpControl = jc;

        HitMarkerStep();
        if ((Time.time - FellTime) >= FALLTIMEMOVEDELAY)
        {
            LateralControl = lc;
        } else
        {
            LateralControl = 0;
        }
        VerticalControl = vc;
        
        JumpNow = (JumpControl && !jumppress);
        
        ShootControl = sp;
        ElementalControl = ec;

        attackStep();

        ElementalPress = ec;
        ShootPress = ShootControl;
        QuellControl = siphpress;



        float sporefactor = 1f;
        if (MySporeValue > 0f)
        {
            sporefactor = (1f / (1f + (MySporeValue*2f)));
        }
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
            if ((!GrassSwinging) && (!Frozen)&&(!Slipping))
                if (!stunned) MyRigidbody.velocity = new Vector2(((LateralControl != 0)? (LateralControl * WalkSpeed*sporefactor):MyRigidbody.velocity.x), MyRigidbody.velocity.y); //AddForce(new Vector2(0f,0f),ForceMode2D.);
        } else
        {
            
            if ((!GrassSwinging) && (!Frozen))
                if (!stunned) MyRigidbody.velocity = new Vector2(LateralControl * WalkSpeed * sporefactor*(JungleMudTouching?.65f:1f), MyRigidbody.velocity.y); //AddForce(new Vector2(0f,0f),ForceMode2D.);
            
            RaycastHit2D rhdetect = Physics2D.Raycast(new Vector2(transform.position.x,transform.position.y),new Vector2(0f,-1f),2f,LayerMask.GetMask(new string[] { "Geometry"}));
            if ((rhdetect.collider != null) && (rhdetect.collider.gameObject.GetComponentInParent<Lava>() == null) && (rhdetect.collider.gameObject.GetComponentInParent<IceBlock>() == null)&&(!Airborne)&&(!Physics2D.GetIgnoreCollision(rhdetect.collider, MyCollider))) {
                LastGroundedLocation = this.transform.position;
                FallingIntoPit = false;
            }
        }
        if ((JumpControl) && Alive)
        {
            if (JumpNow)
            if (!Airborne || !UsedDoubleJump)
            {
                    
                MyRigidbody.velocity = new Vector2(MyRigidbody.velocity.x, JumpSpeed * sporefactor*((JungleMudTouching && !Airborne) ? .5f:1f));
                    if (JumpSound == null)
                        JumpSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.JumpSound, AudioManager.AM.EnvironmentAudioMixer, 1f, 1f, false);
                    if (JungleMudTouching && !UsedDoubleJump)
                    {
                        MyMudParticles.Emit(5);
                        Am.am.M.playGeneralSoundOneShot(Am.am.M.MudSquishSound, Am.am.M.PlayerAudioMixer, .5f, .5f, false, 3f);
                    }

                    if (Airborne)
                    {
                        UsedDoubleJump = true;
                        if (MyClimbingVine == null)
                        {
                            if ((Time.time - VineJumpTime) >= (Time.fixedDeltaTime * 2f))
                            {
                                JetPackParticles.Play(true);
                                DoubleJumpSound.Play();

                            }
                            
                        } else
                        {
                            JumpSound.Play();
                        }
                        
                    } else
                    {
                        
                        JumpSound.Play();
                        Airborne = true;
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


        
        if (MyRigidbody.velocity.y < -.2f)//Mathf.Abs(
        { 
            RaycastHit2D rh2d = Physics2D.CapsuleCast(this.transform.position, MyCollider.size, MyCollider.direction, 0f, new Vector2(0f, .2f));
            if ((rh2d.distance <= 0f))//no hit
            {

                Airborne = true;

            } 
        }
        MyAnimator.SetBool("Airborne", Airborne);

        Vector3 rigtargpos = new Vector3(transform.position.x, transform.position.y+.5f,MyCameraRig.transform.position.z)+(new Vector3(MyRigidbody.velocity.x*0f,MyRigidbody.velocity.y*0f,0f)*1f);

        if (!Introduced)
        {
            rigtargpos = new Vector3(transform.position.x, MyCameraRig.transform.position.y, MyCameraRig.transform.position.z);

        }

        if (WatchingLocation)
        {
            rigtargpos = WatchingLocation.position;
        }

        if (Alive)
        {
            float t = ((Time.time - lastDamageTakenTime) / 2f);
            Color mycol = Color.white;
            mycol = Color.Lerp(mycol,Color.Lerp(Color.yellow,Color.green,Mathf.Pow(Mathf.Sin(Time.time*.5f*Mathf.PI),2f)),1f-(1f/(1f+MySporeValue)));
            if (t < 1f)
            {
                
                MySpriteRenderer.color = Color.Lerp(mycol, Color.red, Mathf.Round((Mathf.Sin((t * (12*(Mathf.Lerp(8f,1f,Health/100f)))) * Mathf.PI)+1f)/2f));
                //MySpriteRenderer.gameObject.SetActive();

                
            } else
            {
                //MySpriteRenderer.gameObject.SetActive(true);
                //MySpriteRenderer.color = Color.white;
                MySpriteRenderer.color = mycol;
            }

            if ((Frozen) && ((Time.time - FreezeTime) >=0f))
            {
                if (!CrystalFrozen)
                {
                    unfreeze();
                }
            }
            
            
        } else
        {
            MySpriteRenderer.color = Color.red;
        }
        mudStep();

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
        

        if (MyRigidbody.bodyType == RigidbodyType2D.Dynamic)
        {
            if (!Pressed)
            {
                Collider2D[] arr = new Collider2D[5];
                MyCollider.OverlapCollider(ProjectileCollisionFilter,arr);
                bool coll = false;
                if (coll)
                {
                    pressedinsidewallforsomestupidreason = true;
                    if ((lastnotpressedinsidewalllocation - this.transform.position).magnitude < 8f)
                    {
                        if ((Time.time - lastnotpressedinwalltime) >= 1f)
                        {
                            this.transform.position = lastnotpressedinsidewalllocation;
                            Debug.Log("STUCK IN GEOMETRY");
                        }
                    }
                } else
                {
                    lastnotpressedinsidewalllocation = this.transform.position;
                    lastnotpressedinwalltime = Time.time;
                }
                
                
                
               }

        }
    }
    private bool pressedinsidewallforsomestupidreason = false;
    private Vector3 lastnotpressedinsidewalllocation = new Vector3();
    private float lastnotpressedinwalltime = -10f;

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
    public float FreezeTime = -10f;
    public SpriteRenderer FreezeSprite;
    public ParticleSystem FreezeFlashEffect;
    public ParticleSystem MeltParticles;
    public float ThawFactor = 0f;

    private float lockinfluence = 0f;
    private Vector2 lastcontroldir = new Vector2();
    public bool CrystalFreeze = false;
    private bool CrystalFrozen = false;
    public VoidCrystal MyFreezingCrystal;
    public void freeze(float dur)
    {
        
        ThawFactor = 0f;
        FreezeTime = Time.time + dur;
        if (Frozen) return;
            Frozen = true;
        MyAnimator.enabled = false;
        if (CrystalFreeze)
        {
            FreezeSprite.color = Color.magenta;
            CrystalFrozen = true;
        }
        else
        {
            FreezeSprite.color = Color.white;
            CrystalFrozen = false;
        }
        FreezeSprite.enabled = true;

        am.oneshot(am.M.chooseSound(am.M.FreezeSound1, am.M.FreezeSound2, am.M.FreezeSound3, am.M.FreezeSound4));
        

        this.MyRigidbody.velocity = new Vector2(0f,0f);
        if (CrystalFrozen)
        {
            this.MyRigidbody.simulated = false;//this.MyRigidbody.bodyType = RigidbodyType2D.Kinematic;

        }
        ThawFactor = 0f;
    }
    public void unfreeze()
    {
        MyAnimator.enabled = true;
        Frozen = false;
        FreezeSprite.enabled = false;
        ThawFactor = 0f;
        UnfreezeTime = Time.time;
        
        if (CrystalFrozen)
        {
            this.MyRigidbody.simulated = true;//bodyType = RigidbodyType2D.Dynamic;
            CrystalFrozen = false;
            MyFreezingCrystal = null;
        }

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

    public void mudStep()
    {

        if (JungleMudTouching)
        {
            if ((Time.time - JungleMudTouchTime) >= .5f)
            {
                JungleMudTouching = false;
            }
        } else
        {

        }
        
        

    }

    private float HitMarkerDamage = 0f;
    private float HitMarkerTime = -10f;
    private bool HitMarkerWasKill = false;
    private float HitMarkerHealthRemaining = 1f;
    public void HitMarkerStep()
    {
        HitMarkerDamage = Mathf.Max(0f, HitMarkerDamage - (200f * Time.deltaTime));
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

    
    private AudioSource HitMarkerSound, HitMarkerKillSound;
    public void tickHitMarker(float damage,float hpremaining,bool iskill)
    {
        
        HitMarkerDamage += damage;
        HitMarkerTime = Time.time;
        HitMarkerHealthRemaining = hpremaining;
        HitMarkerWasKill = iskill;
        float v = (1f - hpremaining);
        
        
        float p = .5f + (v * .5f);
        if (HitMarkerSound == null)
            HitMarkerSound =  AudioManager.AM.createGeneralAudioSource(AudioManager.AM.HitMarker, AudioManager.AM.EnvironmentAudioMixer, 1f,p, false);
        HitMarkerSound.Stop();
        HitMarkerSound.pitch = p;
        HitMarkerSound.Play();

        if (iskill)
        {
            if (HitMarkerKillSound == null)
                HitMarkerKillSound  = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.HitMarkerKill, AudioManager.AM.EnvironmentAudioMixer, 1f, 1f, false);
            HitMarkerKillSound.Play();
        }
        


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
                
                
            if (co.collider.gameObject.CompareTag("Geometry")|| co.collider.gameObject.layer == LayerMask.NameToLayer("Geometry"))
            {
                Vector2 po = co.point;
                //Debug.Log(co.collider.gameObject.name+","+ Mathf.Abs(po.x - this.transform.position.x)+","+(po.y - (this.transform.position.y - (MyCollider.size.y * .4f)))+","+ Mathf.Abs(MyRigidbody.velocity.y));
                if ((Mathf.Abs(po.x - this.transform.position.x) <1f) &&(po.y <= (this.transform.position.y - (MyCollider.size.y * .3f))) && (Mathf.Abs(MyRigidbody.velocity.y) <= .1))
                {
                    //Debug.Log("Geo");
                    

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

                    if (JumpLandSound == null)
                        JumpLandSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.JumpLand, AudioManager.AM.EnvironmentAudioMixer, 1f, 1f, false);
                    //if(Airborne) JumpLandSound.Play();
                    Airborne = false;
                    Slipping = false;
                    UsedDoubleJump = false;

                    
                     //Dead on the floor
                }
                if (!Alive)
                    MyRigidbody.velocity = new Vector2(0f, MyRigidbody.velocity.y);

            } 

            }
    }
    public bool hasWeaponAmmo(int wepindex)
    {
        switch (wepindex)
        {


        }
        return false;
    }
    public void setWeapon(SpecialWeapon wep)
    {
        SpecialWeapon w = CurrentSpecialWeapon;
        switch (wep)
        {
            case SpecialWeapon.None:
                {
                    CurrentSpecialWeapon = SpecialWeapon.None;
                    PistolSprite.enabled = false;
                    ShotgunSprite.enabled = false;
                    MachinegunSprite.enabled = false;
                    LasergunSprite.enabled = false;
                    GrenadeLauncherSprite.enabled = false;
                    TeslaGunSprite.enabled = false;
                    RailGunSprite.enabled = false;

                    break;
                }
            case SpecialWeapon.Pistol: {
                    CurrentSpecialWeapon = SpecialWeapon.Pistol;
                    PistolSprite.enabled = true;
                    ShotgunSprite.enabled = false;
                    MachinegunSprite.enabled = false;
                    LasergunSprite.enabled = false;
                    GrenadeLauncherSprite.enabled = false;
                    TeslaGunSprite.enabled = false;
                    RailGunSprite.enabled = false;
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
                        RailGunSprite.enabled = false;
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
                        RailGunSprite.enabled = false;
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
                        RailGunSprite.enabled = false;
                    }
                    else
                    {
                        //small buzzer
                    }

                    break; }
            case SpecialWeapon.GrenadeLauncher:
                {
                    if (GrenadeLauncherAmmo > 0)
                    {
                        CurrentSpecialWeapon = SpecialWeapon.GrenadeLauncher;
                        PistolSprite.enabled = false;
                        ShotgunSprite.enabled = false;
                        MachinegunSprite.enabled = false;
                        LasergunSprite.enabled = false;
                        GrenadeLauncherSprite.enabled = true;
                        TeslaGunSprite.enabled = false;
                        RailGunSprite.enabled = false;
                    }
                    else
                    {
                        //small buzzer
                    }

                    break;
                }
            case SpecialWeapon.TeslaGun:
                {
                    if (TeslaAmmo > 0)
                    {
                        CurrentSpecialWeapon = SpecialWeapon.TeslaGun;
                        PistolSprite.enabled = false;
                        ShotgunSprite.enabled = false;
                        MachinegunSprite.enabled = false;
                        LasergunSprite.enabled = false;
                        GrenadeLauncherSprite.enabled = false;
                        TeslaGunSprite.enabled = true;
                        RailGunSprite.enabled = false;
                    }
                    else
                    {
                        //small buzzer
                    }

                    break;
                }
            case SpecialWeapon.RailGun:
                {
                    if (RailGunAmmo > 0)
                    {
                        CurrentSpecialWeapon = SpecialWeapon.RailGun;
                        PistolSprite.enabled = false;
                        ShotgunSprite.enabled = false;
                        MachinegunSprite.enabled = false;
                        LasergunSprite.enabled = false;
                        GrenadeLauncherSprite.enabled = false;
                        TeslaGunSprite.enabled = false;
                        RailGunSprite.enabled = true;
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
            if (CurrentSpecialWeapon != SpecialWeapon.Pistol)
            {
                AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.WeaponPickup, AudioManager.AM.PlayerAudioMixer, volume: 1f, pitch: 1f, looped: false, destroyafter: 3f);

                
            } else
            {
                PistolReloadSound.Play();
            }
            

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
            if (w == SpecialWeapon.TeslaGun)
            {
                 //already handled in the TeslaLighning
            }
                if (w == SpecialWeapon.RailGun)
            {
                RailGunOvercharge.Stop();
                RailGunChargeSound.Stop();
                
            }
        }

    }

    //private AudioSource HealthPickupSound;
    public void pickupHealth(HealthPickup hp)
    {
        //this.Health = Mathf.Min(this.Health + hp.HealthValue, 100f);
        this.StimPacks++;
        AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.HealthPickup, AudioManager.AM.PlayerAudioMixer, volume: 1f, pitch: 1f, looped: false, destroyafter: 5f);

    }
    private AudioSource WeaponPickupSound;
    public const int SHOTGUNAMMOINCREMENT = 15;
    public const int MACHINEGUNAMMOINCREMENT = 100;
    public const float LASERGUNAMMOINCREMENT = 100f;
    public const int GRENADELAUNCHERAMMOINCREMENT = 10;
    public const float TESLAGUNAMMOINCREMENT = 100f;
    public const int RAILGUNAMMOINCREMENT = 8;
    public float NewWeaponAcquiredTime = -10f;
    public SpecialWeapon NewWeaponThatWasAcquired;
    public void pickUpWeapon(SpecialWeapon wep)
    {
        AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.WeaponPickup, AudioManager.AM.PlayerAudioMixer, volume: 1f, pitch: 1f, looped: false, destroyafter: 5f);
        GameManager.TheGameManager.showTutorialTip(TutorialSystem.TutorialTip.SwapPistol);
        GameManager.TheGameManager.showTutorialTip(TutorialSystem.TutorialTip.SwapWeapons);

        NewWeaponAcquiredTime = Time.time;
        NewWeaponThatWasAcquired = wep;
        switch (wep)
        {
            case SpecialWeapon.Pistol: { break; }
            case SpecialWeapon.Shotgun: {

                    ShotgunAmmo = Mathf.Min(ShotgunAmmo + SHOTGUNAMMOINCREMENT, SHOTGUNAMMOINCREMENT*4);
                    break; }
            case SpecialWeapon.Gatling:
                {

                    GatlingAmmo = Mathf.Min(GatlingAmmo+MACHINEGUNAMMOINCREMENT, MACHINEGUNAMMOINCREMENT*4);
                    break;
                }
            case SpecialWeapon.Laser:
                {
                    LaserAmmo = Mathf.Min(LaserAmmo + LASERGUNAMMOINCREMENT, LASERGUNAMMOINCREMENT*4f);
                    break;
                }
            case SpecialWeapon.GrenadeLauncher:
                {
                    GrenadeLauncherAmmo = Mathf.Min(GrenadeLauncherAmmo + GRENADELAUNCHERAMMOINCREMENT, GRENADELAUNCHERAMMOINCREMENT*4);
                    break;
                }
            case SpecialWeapon.TeslaGun:
                {
                    TeslaAmmo = Mathf.Min(TeslaAmmo + TESLAGUNAMMOINCREMENT, TESLAGUNAMMOINCREMENT*4f);
                    break;
                }
            case SpecialWeapon.RailGun:
                {
                    RailGunAmmo = Mathf.Min(RailGunAmmo+ RAILGUNAMMOINCREMENT, RAILGUNAMMOINCREMENT * 4);
                    break;
                }

        }


    }

    private Quaternion originalspriterotation;
    public float ReviveTime = -10f;
    private bool useresistancedelay = false;
    public void revive()
    {
        GameManager.TheGameManager.showTutorialTip(TutorialSystem.TutorialTip.OnRevived);
        Health = 100f;
        QuellLocked = true;
        Alive = true;

        //MySpriteRenderer.transform.localRotation = originalspriterotation;
        Am.am.oneshot(Am.am.M.ReviveSound);
        
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
        AreaToFallIn = null;
        fallcollider = null;
        ReviveTime = Time.time;
        this.transform.rotation = new Quaternion();
        //ShotgunAmmo = 0;
    }
    public const float FALLTIMEMOVEDELAY = .75f;
    private float FellTime = -10f;
    public MeshRenderer PainOverlay;
    private Color QuellColor;
    private float QuellValue = 0f;
    private float QuellBreakValue = 0f;
    //private float QuellTime
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
            //if (QuellValue)
            //PainOverlay.material.SetFloat("_Quell", Mathf.Lerp(PainOverlay.material.GetFloat("_Quell"), QuellValue,0f));
            //PainOverlay.material.SetFloat("_QuellBreak", Mathf.Lerp(PainOverlay.material.GetFloat("_QuellBreak"), 0f, 0f));
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
        PainOverlay.material.SetFloat("_QuellValue", QuellValue);
        PainOverlay.material.SetColor("_QuellColor", QuellColor);


        PainOverlay.material.SetFloat("_DeathTime", DeathTime);
        ExtremeVignette.color = new Color(ExtremeVignette.color.r, ExtremeVignette.color.g, ExtremeVignette.color.b, Mathf.Clamp01(ExtremeVignette.color.a - Time.deltaTime * 2f));


        int vv = Mathf.Min(VitaLevel,3);
        float vlf = 0f;

        //if ()
        float vvu = (((float)vv) / 3f);
        Color vrcol = Color.Lerp(Color.black,ResistanceColor, .75f+(.25f*(1f-vvu))+(vvu*.25f *Mathf.Sin(VitaOscil*Mathf.PI*2f)));
            VitaOscil = ((VitaOscil + ((Time.deltaTime*vvu)*1f))%1f);
        if (vv > 0)
            vlf = (.5f+(.5f*vvu));
        else
            vlf = 0f;
        VitaVignette.color = new Color(vrcol.r, vrcol.g, vrcol.b, Mathf.Lerp(VitaVignette.color.a, vlf,Time.deltaTime*3f));
        


    }
    private float VitaVignetteAlpha;
    private float VitaOscil = 0f;
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
        if (Astronaut.TheAstronaut.Quelling) return;
        float quantities = .05f;
        
        ResistanceDrop dr = IceResistDropPrefab;
        //Debug.Log("Vita Dropped!");
        switch (el)
        {
            case Element.Fire: { dr = FireResistDropPrefab; break; }
            case Element.Ice: { dr = IceResistDropPrefab; break; }
            case Element.Grass: { dr = GrassResistDropPrefab; break; }
            case Element.Void: { dr = VoidResistDropPrefab; break; }
        }
        if (imdying)
        {
            quantities = .1f;
            if (resquantity >= 3f)
            {
                quantities = ((resquantity / 3f) * quantities);
            }
        } else
        {
            float patronizationmultiplier = 1f;
            if (Deaths > 0)
                patronizationmultiplier = Mathf.Pow(1f - .2f, Deaths);
            resquantity *= patronizationmultiplier;
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
        if (tobranches)
        {
            AudioManager.AM.crossfade(AudioManager.AM.CurrentMusic, 0f, 1.4f);
        }
    }

    

    public void transitionVoidFade(bool tofirstboss)
    {

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
                VineJumpTime = Time.time;
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
    [HideInInspector]public Vector3 LastGroundedLocation;
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
    private FallingArea AreaToFallIn = null;
    private Collider2D fallcollider = null;
    public void fallingAreaStep()
    {

        if ((AreaToFallIn != null) &&((this.transform.position.x >= fallcollider.bounds.min.x) && (this.transform.position.x <= fallcollider.bounds.max.x)))
        {
            float v = ((1f * VitaLevel) + ResistanceXP);
            
            FallingIntoPit = true;
            
            if ((MyCameraRig.transform.position.y - this.transform.position.y) >= 5f)
            {
                //You fell into the pit
                MyRigidbody.velocity = new Vector2(0f, 0f);
                if (TakeDamage(20f, new Vector3()))
                {
                    HeldFallenResistanceValue = v;
                }
                else
                {
                    FallingIntoPit = false;
                    this.transform.position = LastGroundedLocation;
                    FellTime = Time.time;
                    AreaToFallIn = null;
                    fallcollider = null;
                }
            }
        } else
        {
            AreaToFallIn = null;
            fallcollider = null;
        }
    }
    
    public void fallCollision(FallingArea fa,Collider2D col) {
        if (col == null) return;
        //if (Alive)
        //{
            if (LastGroundedLocation.y > fa.transform.position.y)
            {
                AreaToFallIn = fa;
                fallcollider = col;
            }
            
        //}
    }
    private AudioSource ResistancePickUpSound0,ResistancePickUpSound1, ResistancePickUpSound2, ResistancePickUpSound3, ResistancePickUpSound4, ResistancePickUpSoundP;
    public void absorbResistance(ResistanceDrop res)
    {
        res.Live = false;
        res.removeParticles();
        res.transform.localScale = (Vector3.one * 10f);
        res.MySpriteRenderer.enabled = false;
        res.Remove();
        GameManager.TheGameManager.showTutorialTip(TutorialSystem.TutorialTip.VitaPower);
        ElementalEnergy = Mathf.Min(ElementalEnergy + ((res.ResistanceValue * 100f)*3f), 100f);
        float nv = ResistanceXP + (res.ResistanceValue*res.ValueValue);
        float k0 = 1f, k1 = .75f, k2 = .5f,k3 = 0.375f,k4 = .25f;
        float km = 1f;
        k0 *=km;
        k1 *= km;
        k2 *= km;
        k3 *= km;
        k4 *= km;
        float V = .6f;
        if (ResistancePickUpSound1 == null)
        {
            ResistancePickUpSound0 = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.ResistancePickUp, AudioManager.AM.EnvironmentAudioMixer,  V,k0, false);
            ResistancePickUpSound1 = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.ResistancePickUp, AudioManager.AM.EnvironmentAudioMixer,  V,k1, false);
            ResistancePickUpSound2 = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.ResistancePickUp, AudioManager.AM.EnvironmentAudioMixer,  V,k2, false);
            ResistancePickUpSound3 = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.ResistancePickUp, AudioManager.AM.EnvironmentAudioMixer,  V,k3, false);
            ResistancePickUpSound4 = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.ResistancePickUp, AudioManager.AM.EnvironmentAudioMixer,  V,k4, false);
            ResistancePickUpSoundP = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.ResistancePickUp, AudioManager.AM.EnvironmentAudioMixer,  V,k0, false);
        }
        float rk = (1f+(nv*1f));//(.75f+(nv*.25f)) 

        if (nv>=1f)
        {
            rk = 2f;

        }
        if (true)
        rk = 1f;
        
        if (VitaLevel > 3)
        {
            float kx = (1f + (((float)(VitaLevel - 3)) / 8f));
            ResistancePickUpSound0.Stop();
            ResistancePickUpSound0.pitch = k0 * kx;
            ResistancePickUpSound0.Play();
            ResistancePickUpSound1.Stop();
            ResistancePickUpSound1.pitch = k1 * kx;
            ResistancePickUpSound1.Play();
            ResistancePickUpSound2.Stop();
            ResistancePickUpSound2.pitch = k2 * kx;
            ResistancePickUpSound2.Play();
            ResistancePickUpSound3.Stop();
            ResistancePickUpSound3.pitch = k3 * kx;
            ResistancePickUpSound3.Play();
            ResistancePickUpSound4.Stop();
            ResistancePickUpSound4.pitch = k4*kx;
            ResistancePickUpSound4.Play();
            ResistancePickUpSoundP.Stop();
            ResistancePickUpSoundP.volume = V * ResistanceXP;
            ResistancePickUpSoundP.pitch = k4*kx*(.8f+(.2f*ResistanceXP));
            ResistancePickUpSoundP.Play();
        } else
        {
            float P = k0;
            ResistancePickUpSound0.Stop();
            ResistancePickUpSound0.pitch = k0*rk;
            ResistancePickUpSound0.Play();
            if (VitaLevel >= 1)
            {
                ResistancePickUpSound1.Stop();
                ResistancePickUpSound1.pitch = k1 * rk;
                ResistancePickUpSound1.Play();
                P = k1;
            }
            if (VitaLevel >= 2)
            {
                ResistancePickUpSound2.Stop();
                ResistancePickUpSound2.pitch = k2 * rk;
                ResistancePickUpSound2.Play();
                P = k2;
            }
            if (VitaLevel >= 3)
            {
                ResistancePickUpSound3.Stop();
                ResistancePickUpSound3.pitch = k3 * rk;
                ResistancePickUpSound3.Play();
                P = k3;
            }
            if ((nv >= 1f) && (VitaLevel == 3))
            {
                ResistancePickUpSound4.Stop();
                ResistancePickUpSound4.pitch = k4*rk;
                ResistancePickUpSound4.Play();
                P = k4;
            }
            ResistancePickUpSoundP.Stop();
            ResistancePickUpSoundP.volume = V * ResistanceXP;
            ResistancePickUpSoundP.pitch = P * (.8f + (.2f * ResistanceXP));
            ResistancePickUpSoundP.Play();
        }
        
        if (nv >= 1f)
        {
            VitaLevel++;
            //for (int i = 0; i < Mathf.Max(VitaLevel,7); i++)
                Am.am.oneshot(Am.am.M.VitaIncrease,1f+(.25f*(float)Mathf.Min(VitaLevel,4)));
            
            ParticleSystem.MainModule mm = VitaIncreaseParticles.main;
            ParticleSystem.EmissionModule em = VitaIncreaseShadeParticles.emission;
            mm.startColor = ResistanceColor;
            mm = VitaIncreaseShadeParticles.main;
            mm.startColor = new Color(ResistanceColor.r * .4f, ResistanceColor.g * .4f, ResistanceColor.b*.4f,1f);
            
            em.rateOverTimeMultiplier = 20f*Mathf.Min(VitaLevel,4f);
            mm = VitaIncreaseTrailParticles.main;
            mm.startColor = ResistanceColor;
            VitaIncreaseParticles.Play();
            VitaIncreaseTrailParticles.Play();
            VitaIncreaseShadeParticles.Play();
            

            GameManager.TheGameManager.showTutorialTip(TutorialSystem.TutorialTip.VitaDifficulty);
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
                    ResistanceColor2 = new Color(1f, .2f, .2f);
                    ResistanceIcon.enabled = true;
                    break; };
            case Element.Grass: { ResistanceColor = new Color(.2f,1f,.1f);
                    ResistanceColor2 = new Color(.5f, 1f, 0f);
                    ResistanceIcon.sprite = VitaSpriteGrass; ResistanceIcon.enabled = true; break; };
            case Element.Ice: { ResistanceColor = new Color(.3f,.3f,1f);
                    ResistanceColor2 = new Color(.1f, .5f, 1f);  ResistanceIcon.sprite = VitaSpriteIce; ResistanceIcon.enabled = true;  break; };
            case Element.Void: { ResistanceColor = new Color(.8f,.2f,.8f);
                    ResistanceColor2 = new Color(1f, .0f, 1f);
                    ResistanceIcon.sprite = VitaSpriteVoid; ResistanceIcon.enabled = true;  break; };
        }
        //GameObject.Destroy(res.gameObject,5f);
    }
    public ParticleSystem VitaIncreaseParticles, VitaIncreaseShadeParticles, VitaIncreaseTrailParticles;
    public Color ResistanceColor = Color.white;
    public Color ResistanceColor2 = Color.white;
    public ParticleSystem ResistanceAbsorbedEffect,ResistanceLevelUpEffect;
    private float ResistanceDisplayedValue = 0f;
    private Element ResistanceElement = Element.Fire;
    public Image ResistanceBack, ResistanceGlow,ResistanceIcon, ResistanceFill;
    public Slider ResistanceGauge;
    public Sprite VitaSpriteFire, VitaSpriteGrass, VitaSpriteIce, VitaSpriteVoid;
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
        if (false)
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
    public void affectSpore(float sp)
    {
        if (Alive && !Invulnerable)
        {
            MySporeValue += sp;
        }
    }
    

    public void sporeStep()
    {
        ParticleSystem.EmissionModule m = MySporeEffect.emission;
        if (MySporeValue > 0f)
        {
            
            m.rateOverTime = (20f *Mathf.Clamp01(MySporeValue));
            MySporeValue = Mathf.Max(0f,MySporeValue*(1f-(Time.fixedDeltaTime*.5f)));
            if (MySporeValue < .01f)
            {
                MySporeValue = 0f;
            }
        } else
        {
            m.rateOverTime = 0f;
        }
    }
    public void pressIntoGround(Vector3 presspos,float dur)
    {
        if (Pressed)
        {
            if ((Time.time - PressTime) < (dur))
            {
                PressTime = Time.time;
                PressDuration = dur;
            }

            MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
            
        }
        else
        {
            Pressed = true;
            Am.am.oneshot(Am.am.M.PressedSound);
            PressTime = Time.time;
            PressDuration = dur;
            PressPosition = presspos;
            UnpressedPosition = this.transform.position;
            this.transform.position = PressPosition;
            MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
            //Play a sound indicating that you've been pressed
        }
        if (PressParticles) PressParticles.Emit(20);
        


    }
    public Vector3 PressPosition;
    public Vector3 UnpressedPosition;
    public float PressTime = 0f;
    public float PressDuration = 2f;
    public bool Pressed = false;
    public ParticleSystem PressParticles;
    public void pressStep()
    {
        if (Pressed)
        {
            //Debug.Log("Pressed.");
            if (((Time.time - PressTime) >= PressDuration) || (!Alive) || (Invulnerable))
            {
                this.transform.position = UnpressedPosition;
                MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
                Am.am.oneshot(Am.am.M.ReviveSound);
                Pressed = false;
            } else
            {
                this.transform.position = PressPosition;
            }
        }
    }
    public ParticleSystem DrainVitaParticles;
    public void drainResistanceWhileKnockedOut()
    {
        if (Alive) return;
        float losspersecond = (.1f*(1f+(.25f*Deaths)));
        if (VitaLevel > 0)
        {
            losspersecond = (losspersecond*(Mathf.Lerp(1f,4f,Mathf.Min(VitaLevel,3)/3f)));
        }
        if ((ResistanceXP > 0f) || (VitaLevel > 0))
        {
            ParticleSystem.MainModule mm = DrainVitaParticles.main;
            mm.startColor = ResistanceColor;
            DrainVitaParticles.Emit(VitaLevel + 1);
        }
        float ldelta = (losspersecond* Time.deltaTime);
        if (ResistanceXP <= ldelta)
        {
            if (VitaLevel > 0)
            {
                ResistanceXP = .999999f;
                VitaLevel--;
            }
            else
            {
                ResistanceXP = 0f;
                

            }
        }
        else
        {
            ResistanceXP -= ldelta;
        }
        

    }
    //public AudioLowPassFilter QuellingSoundFilter;
    private AudioSource CriticalBackSound;
    public void kill()
    {
        if (!Alive) return;
        Alive = false;
        Health = 0f;
        MySporeValue = 0f;
        Deaths++;
        if (Frozen)
        {
            unfreeze();
        }
        DeathTime = Time.time;
        FellToDeath = false; //This will be set to true by another script that states that you have fallen
        if (!DeathSound) DeathSound = am.sound(am.M.DeathSound);
        DeathSound.PlayOneShot(DeathSound.clip);
        DeathSound.PlayOneShot(am.M.chooseSound(am.M.AstronautDeath1, am.M.AstronautDeath2, am.M.AstronautDeath3));
        VitaIncreaseParticles.Stop();
        VitaIncreaseParticles.Clear();
        VitaIncreaseShadeParticles.Stop();

        if (Deaths>=3)
            GameManager.TheGameManager.showTutorialTip(TutorialSystem.TutorialTip.TooManyDeaths);
        
        if ((VitaLevel > 0) ||(ResistanceXP > 0f))
        {
            imdying = true;
            //if (!FallingIntoPit)
            //{
                //dropResistance(((1f * VitaLevel) + ResistanceXP), this.transform.position, ResistanceElement);
            //}
            GameManager.TheGameManager.showTutorialTip(TutorialSystem.TutorialTip.VitaDropped);
            imdying = false;

            //VitaLevel = 0;
            //ResistanceXP = 0f;

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
    private AudioSource LowHealthAlertSound, VeryLowHealthAlertSound;
    private AudioSource DamageTakenSound, LowHealthHeartbeatSound;
    public bool TakeDamage(float damage,Vector3 knockback)
    {
        if (!Alive) return false;
        damage *= DamageVulnerability;
        if (USINGRESISTANCE)
        if (VitaLevel > 0) {
            damage = (damage /(1f+(1f*(((float)VitaLevel)/10f))));
        }
        float patronizationmultiplier = 1f;
        if (Deaths > 0)
            patronizationmultiplier = Mathf.Pow(1f-.2f, Deaths);

        int rs = numberOfStimsActive();
        
        if (rs > 0)
        {
            damage /= (1+(rs*STIMRESISTANCECONSTANT));
            knockback /= (1 + (rs * STIMRESISTANCECONSTANT));
        }
        damage *= patronizationmultiplier;
        knockback *= patronizationmultiplier;
        float health = (Health - damage);
            lastDamageTakenTime = Time.time;
        //Play a DamageTaken sound.
        float extremethreshold = 15f;
        if (Quelling)
        {
            //Quelling = false;
            //QuellLocked = true;
            
        }


        if (damage >= extremethreshold)
        {
            float offset = .5f;
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
            CriticalBackSound.Stop();
            kill();

            return true;
        } else
        {
            DamageTakenSound.Stop();
            DamageTakenSound.pitch = (1.25f-(.5f*(1f-(Health/100f))));
            DamageTakenSound.Play();
            float hthresh = 95f;
            if (Health < hthresh) {
                CriticalBackSound.volume = 0f;
                CriticalBackSound.Stop();
                CriticalBackSound.Play();
                
                AudioManager.AM.crossfade(CriticalBackSound, 1f, (4f*(Health/hthresh)));
                //Debug.Log("Critical");
            }
            if (knockback.magnitude > 0.1f)
            {
                MyRigidbody.velocity = new Vector2(knockback.x, ((Mathf.Abs(knockback.y)<(JumpSpeed*.5f))? JumpSpeed: knockback.y));
                lastStunTime = Time.time;
            }
            Health = health;
            if (Health <25f)
            {
                
                if (LowHealthAlertSound == null)
                    LowHealthAlertSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.LowHealthAlertSound, AudioManager.AM.PlayerAudioMixer, 1f, 1f, false);
                LowHealthAlertSound.Play();
            } else if (Health < 50f)
            {

                if (VeryLowHealthAlertSound == null)
                    VeryLowHealthAlertSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.VeryLowHealthAlertSound, AudioManager.AM.PlayerAudioMixer, 1f, 1f, false);
                VeryLowHealthAlertSound.Play();
            }

            if (Health < 70f)
            {
                GameManager.TheGameManager.showTutorialTip(TutorialSystem.TutorialTip.Injured);
            }

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
    public void createFreezeParticlesForEnemy(GenericEnemy en,Collider2D col)
    {
        //look at the bounds of the collider

        if (en.MyFreezeFlare == null)
        {
            en.MyFreezeFlare = GameObject.Instantiate<ParticleSystem>(EnemyFreezeFlarePrefab, col.bounds.center, EnemyFreezeFlarePrefab.transform.rotation, en.transform);
            en.MyFreezeFlare.transform.localScale = (Vector3.one*col.bounds.extents.magnitude);
            
        }
        if (en.MyFreezeSparkles == null)
        {
            en.MyFreezeSparkles = GameObject.Instantiate<ParticleSystem>(EnemyFreezeSparklesPrefab, col.bounds.center, EnemyFreezeSparklesPrefab.transform.rotation, en.transform);
            en.MyFreezeSparkles.transform.localScale = col.bounds.extents;
            ParticleSystem.ShapeModule sh = en.MyFreezeSparkles.shape;
            sh.position = new Vector3(col.offset.x, col.offset.y);
            sh.scale = col.bounds.extents;
        }
        if (en.MyFreezeDust == null)
        {
            en.MyFreezeDust = GameObject.Instantiate<ParticleSystem>(EnemyFreezeDustPrefab, col.bounds.center, EnemyFreezeDustPrefab.transform.rotation, en.transform);
            en.MyFreezeDust.transform.localScale = col.bounds.extents;
        }
    }

    public ParticleSystem EnemyFreezeFlarePrefab, EnemyFreezeSparklesPrefab, EnemyFreezeDustPrefab,EnemyProjectileFreezePrefab;

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
    public float LastGunHUDAlertTime= -10f; //set to Time.time to get the hud to appear close
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
                    case SpecialWeapon.GrenadeLauncher: { hudguntexture = HUDMachinegun; ammovalue = "" + GrenadeLauncherAmmo; break; }
                    case SpecialWeapon.TeslaGun: { hudguntexture = HUDMachinegun; ammovalue = "" + (int)LaserAmmo; break; }
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
