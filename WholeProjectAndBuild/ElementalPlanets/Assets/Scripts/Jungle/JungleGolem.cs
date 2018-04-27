using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JungleGolem : BossGolem {

    /*
     *
     * Boss:
        Flying type- You're at the top of the jungle stage

        Abilities:
        -Fly up, Shoot a beam of green laser as it fly down.
        -Fly straight into you to claw(melee)
        -Wind attack (Produce a tornado to deal damage over time?) or Wind slash
        -Enraged mode - turn red and double attack power and double the attack speed, when half hp
        Nov 16, 3:42 PM

        Marcus
 
        Good job!

        For the boss, come up with something else to replace Enraged Mode.
        All Enemies in the game are planned to have boosted attack speed while the player has a lot of Vita energy.
        No direct boosting of Damage; simply increasing damage per hit in any game is more punishing on hit and does not
        increase player interactivity/engagement as opposed to increasing the Attack speed/rate which has the opposite
        and more engaging effect of having the player accelerate their reflexes.
     */
    public enum State { Waiting, Introducing, StandingBy, ClawStriking,SporeShooting, TyphoonStarting, Defeated,Recoiling };
    // Use this for initialization
    public State MyState = State.Waiting;
    public ParticleSystem SwoopingParticles;
    public ParticleSystem[] FlightTrails;
    public Transform StartTransform;
    private bool SwoopedIn = false, Screeched = false;
    private float StateTime = -10f;
    private float StateDuration = 1f;
    public float StunTime = -10f;
    public Animator MyAnimator;
    public Collider2D ClawCollider;
    public ParticleSystem[] ClawTrails;
    public bool TyphoonWindsBlowing= false;
    private float TyphoonWindsBlowingStartTime = -10f;
    private float TyphoonWindsBlowingDuration = 5f;
    public ParticleSystem TyphoonGlow;
    void Start () {
        SwoopedIn = false;
        Screeched = false;
        MyCollider.enabled = false;
        MyWeakSpot.MyCollider.enabled = false;
        MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
        this.transform.position = StartTransform.position;
        PlayerHasBeenReleased = false;
        MyHiddenGroup.SetActive(false);
        ShadowSwoop.enabled = false;
        setState(State.Waiting, 0f);
    }

    public Transform IntroTargetPosition;
    public ParticleSystem ScreechParticles, DefeatSplosion;
    //public TyphoonForce TyphoonForcePrefab;
    public Transform MouthTransform;
    private float FlySpeed = 2f;
    private Vector2 MoveDirection = new Vector2();
    private Vector2 MoveTargetPosition;
    private bool MovingToTargetPosition;
    private int LookDirection = 0;
    private int TrudgeTowards = 0;
    private Vector3 IntroStartPosition;
    private float introvalue = 0f;
    private float ScreechTime = -10f;
    // Update is called once per frame
    void Update () {
		
	}
    public bool Introduced = false;
    private Vector3 SwoopStartPos, SwoopEndPos;
    public GameObject MyHiddenGroup;
    public Transform LeftMost, RightMost;
    public override void introduceBoss()
    {
        if (Introduced) return;
        Introduced = false;
        //animate the starting animation
        Debug.Log("Introducing the Golem!");
        hasbeenintroduced = true;
        Astronaut.TheAstronaut.PlayerHasControl = false;
        
        ShadowSwooped = 0f;
        ShadowSwoop.enabled = true;
        SwoopedIn = false;
        Vector3 apos = Astronaut.TheAstronaut.transform.position;
        SwoopStartPos = new Vector3(apos.x, apos.y - 20f, -2f);
        SwoopEndPos = new Vector3(apos.x, apos.y + 20f, -2f);
        MyHiddenGroup.SetActive(false);
        
        ShadowSwoop.transform.position = SwoopStartPos;
        //RainingIceShards = false;
        //EruptingParticles.Play(true);
        introvalue = 0f;
        //
        IntroStartPosition = this.transform.position;
        setState(State.Introducing, 0f);
    }
    public bool isStunned()
    {
        return ((StunTime - Time.time) >= 0f);
    }

    private float HighPosY = 0f;
    public SpriteRenderer ShadowSwoop;
    private float ShadowSwooped = 0f;
    public BoxCollider2D BossBounds;
    //public Vector2 MoveDirection = new Vector2();
    void FixedUpdate()
    {


        Astronaut plr = Astronaut.TheAstronaut;
        bool lookindirection = false;


        bool stateexpired = (Time.time >= (StateTime + StateDuration));
        Vector2 move = new Vector2();
        switch (MyState)
        {
            case State.Waiting: { break; }
            case State.Introducing:
                {
                    //Introduced = false;
                    //Rise up from the ground in a menacing fashion
                    this.Vulnerable = false;
                    //TrudgeTowards = 0;
                    Astronaut.TheAstronaut.MyRigidbody.velocity = new Vector2(0f, Astronaut.TheAstronaut.MyRigidbody.velocity.y); //Hold Still, please.


                    if (ShadowSwooped < 1f)
                    {
                        float sv = Mathf.Clamp01(ShadowSwooped + (Time.fixedDeltaTime * 2f));
                        if ((ShadowSwooped < .5f) && (sv >= .5f))
                        {
                            //play a swoop sound
                        }
                        ShadowSwooped = sv;
                        ShadowSwoop.transform.position = Vector3.Lerp(SwoopStartPos,SwoopEndPos,sv);
                    } else if (!SwoopedIn)
                    {
                        Vector3 dif = (IntroTargetPosition.position - this.transform.position);
                        float speed = 1f;
                        float du = (IntroStartPosition - IntroTargetPosition.position).magnitude;
                        ShadowSwoop.enabled = false;
                        MyHiddenGroup.SetActive(true);
                        Astronaut.TheAstronaut.WatchingLocation = IntroTargetPosition;

                        float d = (speed * Time.deltaTime*2.5f);
                        if (introvalue >= 1f)
                        {
                            this.transform.position = IntroTargetPosition.position;
                            SwoopedIn = true;
                            //SwoopingParticles.Stop(true);
                            Screeched = false;
                            
                            //MySpriteRenderer.transform.localPosition = new Vector3(0.99f, .73f, 0f);

                        }
                        else
                        {
                            introvalue = Mathf.Clamp01(introvalue + (d));

                            Vector2 c = (Random.insideUnitCircle.normalized * .1f);
                            this.transform.position = (Vector3.Lerp(IntroStartPosition, IntroTargetPosition.position, introvalue) + new Vector3(c.x, c.y, 0f));
                            //MySpriteRenderer.transform.localPosition = new Vector3(.99f+c.x,.73f+c.y,0f);
                        }


                    }
                    else if (!Screeched)
                    {
                        ScreechParticles.Play();
                        plr.addCamShake(1f, 1f, 1f, 2f, .5f);
                        AudioManager.AM.crossfade(AudioManager.AM.CurrentMusic, 0f, .3f);
                        screechsound = AudioManager.AM.playGeneralSound(this.transform.position, AudioManager.AM.JungleBossRoar, AudioManager.AM.PlayerAudioMixer, 1f, 1f, false, 30f);
                        //MyAnimator.SetTrigger("Awaken");
                        MyAnimator.SetTrigger("Screech");
                        
                        Screeched = true;
                        
                        ScreechTime = Time.time;
                        //Pass some RRRRRRRAAAAAAAAWWWWWWWRRRRRRRR!!!! text.
                    }
                    else
                    {

                        if ((Time.time - ScreechTime) >= 1.75f)
                        {

                            Introduced = true;
                            HighPosY = (this.transform.position.y+1f);
                            
                            beginBossFight();
                            AudioManager.AM.crossfade(screechsound, 0f, 2f);
                            //setState(State.StandingBy,3f+Random.value*3f);
                            standBy();
                        }
                    }


                    break;
                }
            case State.StandingBy:
                {
                    //Move Towards the Player
                    setSlashEffect(false);
                    if (!Defeated)
                    {
                        this.Vulnerable = true;
                        if ((plr != null) && (plr.Alive))
                        {

                            Vector3 dif = (plr.transform.position - this.transform.position);
                            
                            int dir = (int)Mathf.Sign(dif.x);
                            
                            if (!MovingToTargetPosition)
                            {
                                TrudgeTowards = dir;
                                float mxl = Mathf.Max(BossBounds.bounds.min.x, this.transform.position.x-10f);
                                float mxr = Mathf.Min(BossBounds.bounds.max.x, this.transform.position.x + 10f);
                                float myb = Mathf.Max(BossBounds.bounds.min.y, this.transform.position.y - 4f);
                                float myt = Mathf.Min(BossBounds.bounds.max.y, this.transform.position.y + 4f);
                                MoveTargetPosition = new Vector3(mxl + (Random.value * (mxr-mxl)), myb + (Random.value * (myt - myb)), this.transform.position.z);
                                //Debug.Log("Choose Target Position");
                                MovingToTargetPosition = true;
                                
                            }
                            else
                            {
                                
                                //MoveDirection = TrudgeTowards;
                                Vector2 destdif = (MoveTargetPosition - MyRigidbody.position);
                                LookDirection = (int)Mathf.Sign(destdif.x);

                                if ((destdif.magnitude < .1f))
                                {
                                    MovingToTargetPosition = false;
                                }
                                else
                                {
                                    if (Astronaut.AggressionLevel >= 3)
                                    {
                                        //Move using a non linear approach
                                        move = (destdif * (1f) * Mathf.Clamp01(Time.fixedDeltaTime*5f));//((destdif.magnitude > sp) ? (destdif.normalized * sp) : destdif);
                                    }
                                    else
                                    {

                                        float sp = (FlySpeed * (2f + (2f * Astronaut.AggressionLevelF))) * Time.fixedDeltaTime;
                                        move = ((destdif.magnitude > sp) ? (destdif.normalized * sp) : destdif);

                                    }
                                }

                                
                                
                                //MyRigidbody.velocity = new Vector2(TrudgeTowards * FlySpeed * (1f + (2f * Astronaut.AggressionLevelF)), MyRigidbody.velocity.y);
                            }

                            Vector3 ldif = (plr.transform.position- transform.position);
                            LookDirection = (int)Mathf.Sign(ldif.x);

                            if (stateexpired)
                            {
                                //Choose an action
                                //MyRigidbody.velocity = new Vector2(0f, MyRigidbody.velocity.y);
                                float r = Random.value;
                                //standBy();
                                
                                if (r < .25f)
                                    startClawStrikeAttack();
                                
                                else if (r < .5f)
                                    startSporeShoot();
                                else if (r < .75f)
                                    startTyphoon();
                                else 
                                    standBy();
                                
                            }
                        }
                        //MyAnimator.SetBool("Flying", Mathf.Abs(MyRigidbody.velocity.x) > .1f);
                    }
                    else
                    {
                        //MyAnimator.SetBool("Flying", false);

                    }


                    break;
                }
            case State.ClawStriking:
                {

                    if (!Defeated)
                    {
                        setSlashEffect(true);
                        float tf = (Time.time - clawStartTime);
                        float pre = .3f;
                        //ClawStrikeDirection
                        float sf = (Time.fixedDeltaTime * CLAWSTRIKESPEED * ((tf - pre)/pre));

                        if ((tf - pre) > 0f)
                        {
                            //sf = sf;//(sf * sf);
                            RaycastHit2D rh = Physics2D.Raycast(new Vector2(ClawCollider.transform.position.x, ClawCollider.transform.position.y),new Vector2(ClawStrikeDirection.x,ClawStrikeDirection.y),sf,LayerMask.GetMask("Geometry"));
                            ClawCollider.enabled = true;
                            if (rh.collider != null)
                            {
                                move = new Vector2(rh.point.x-ClawCollider.transform.position.x, rh.point.y - ClawCollider.transform.position.y);
                                if (ClawImpact)
                                {
                                    ClawImpact.transform.position = new Vector3(rh.point.x,rh.point.y,0f);
                                    ClawImpact.Emit(50);
                                }
                                MyAnimator.SetTrigger("SwoopImpact");
                                MyAnimator.SetBool("Swooping",false);
                                clawStartTime = Time.time;
                                ClawCollider.enabled = false;
                                setState(State.Recoiling,1.5f);
                            } else
                            {
                                move = (new Vector2(ClawStrikeDirection.x, ClawStrikeDirection.y) * sf);
                            }
                            
                        } else {
                            ClawCollider.enabled = false;
                            move = (new Vector2(ClawStrikeDirection.x, ClawStrikeDirection.y) * sf);
                        }
                        





                    if (stateexpired)
                        {
                            //Choose an action
                            

                            standBy();

                        }
                    }
                    
                    break;
                }
            case State.Recoiling:
                {

                    if (!Defeated)
                    {
                        setSlashEffect(false);
                        float tf = (Time.time - clawStartTime);
                        
                        //ClawStrikeDirection
                        float sf = (Time.fixedDeltaTime * CLAWSTRIKESPEED*.25f * Mathf.Clamp01(1f-(tf / StateDuration)));
                        move = (new Vector2(ClawStrikeDirection.x, ClawStrikeDirection.y) * -sf);


                        if (stateexpired)
                        {
                            //Choose an action


                            standBy();

                        }

                    }
                    break;
                }
            case State.SporeShooting:
                {
                    if (!Defeated)
                    {
                        setSlashEffect(false);
                        //Choose an action

                        if (((Time.time - StateTime) >= .5f) && (!FiredSporeAttack))
                        {
                            Vector3 dif = (plr.transform.position - MouthTransform.position);
                            FiredSporeAttack = true;
                            if (Random.value < .6f)
                            {
                                //Shoot Spore projectile
                                JungleTreeProjectile p = GameObject.Instantiate(JungleProjectile, MouthTransform.position, new Quaternion());
                                float sp = 4f * (1f + (2f * Astronaut.AggressionLevelF));
                                p.transform.localScale = (p.transform.localScale * ((1f + (Astronaut.AggressionLevelF))));



                                p.MyRigidbody.velocity = (dif.normalized * sp);
                                p.DetonationTime = (dif.magnitude / Mathf.Max(p.MyRigidbody.velocity.magnitude, 1f));


                                p.MyRigidbody.velocity = new Vector2(-1f * Mathf.Sign(this.transform.localScale.x), 0f) * sp;
                            }
                            else
                            {
                                //Release Spores
                                //Create a Cloud of Spores that slows the astronaut
                                SporeExplosion.Play(true);
                                int subs = (8 + (Astronaut.AggressionLevel * 3));


                                for (int i = 0; i < (subs); i++)
                                {
                                    JungleTreeProjectile proj = GameObject.Instantiate(SporePrefab, this.transform.position, this.transform.rotation).GetComponent<JungleTreeProjectile>();
                                    // proj.transform.localScale = (this.transform.localScale * (.3f));
                                    proj.DamageRatio = (.25f * (1f / ((float)(subs))));
                                    float af = (((float)i + (1 * (Random.value))) / ((float)(subs)));
                                    float ang = (360f * af);//((Random.value<(2f/3f)) ? 45f + (90f * af) : (360f * af));
                                    proj.MyConstantForce.enabled = true;
                                    proj.transform.localScale = (proj.transform.localScale * 5f);
                                    proj.MyRigidbody.velocity = (new Vector2(dif.x, dif.y) * 2f) + ((new Vector2(Mathf.Cos((ang / 360f) * 2f * Mathf.PI), Mathf.Sin((ang / 360f) * 2f * Mathf.PI)) * 2.5f) * (((float)(i + 1)) / ((float)subs)) * (.5f + Random.value * 5f) * (1f + Astronaut.AggressionLevelF));


                                    proj.MyConstantForce.force = (Random.insideUnitCircle.normalized * JungleTreeProjectile.WINDFORCEFACTOR);
                                    //proj
                                }
                            }


                        }
                        if (stateexpired)
                        {

                            standBy();

                        }
                    }

                    break;
                }
            case State.TyphoonStarting:
                {

                    if (!Defeated)
                    {
                        //setSlashEffect(false);
                        //float tf = (Time.time - clawStartTime);

                        //ClawStrikeDirection
                        //float sf = (Time.fixedDeltaTime * CLAWSTRIKESPEED * .25f * Mathf.Clamp01(1f - (tf / StateDuration)));
                        //move = (new Vector2(ClawStrikeDirection.x, ClawStrikeDirection.y) * -sf);

                        if (((Time.time - StateTime) >= .5f) && (!TyphoonInitiated))
                        {
                            Vector3 dif = (plr.transform.position - this.transform.position);
                            TyphoonInitiated = true;
                            TyphoonGlow.Play(true);
                            Vector2 r = Random.insideUnitCircle;
                            Vector3 v = Vector3.Lerp(new Vector3(r.x,r.y,0f),dif.normalized, Astronaut.AggressionLevelF);
                            MyTyphoonField.startTyphoon(v*(5f*(1f+(4f*Astronaut.AggressionLevelF)))+new Vector3(0f,10f),3f+(3f* Astronaut.AggressionLevelF));
                        }
                            

                        if (stateexpired)
                        {
                            //Choose an action


                            standBy();

                        }

                    }

                    break;
                }

            case State.Defeated:
                {
                    float f = (Time.time - StateTime);
                    //MyIceBreath.BreathActive = false;
                    //RainingIceShards = false;
                    MyTyphoonField.TyphoonWindsActive = false;
                    //MyAnimator.SetBool("Glowing", false);
                    //MyAnimator.SetBool("IceBreathing", false);
                    MyCollider.enabled = false;
                    MyWeakSpot.MyCollider.enabled = false;
                    MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
                    MyRigidbody.velocity = new Vector2(0f, 0f);
                    if (f < 3f)
                    {
                        setSlashEffect(false);
                        /*
                        MySpriteRenderer.enabled = true;
                        MySpriteRenderer.color = new Color(MySpriteRenderer.color.r, MySpriteRenderer.color.g, MySpriteRenderer.color.b, 1f - (f / 3f));
                        MySpriteRenderer.transform.Rotate(0f, 0f, Time.fixedDeltaTime * 360f * 4f);
                        */
                        MyDroppedGoalElement.Collectable = false;
                        
                    }
                    else
                    {

                        if (!PlayerHasBeenReleased)
                        {

                            plr.PlayerHasControl = false;
                            MyDroppedGoalElement.HomingInOnAstronaut = true;
                            MyDroppedGoalElement.HomingTime = Time.time;
                            MyDroppedGoalElement.Collectable = true;
                            Astronaut.TheAstronaut.WatchingLocation = null;
                            PlayerHasBeenReleased = true;
                        }
                        MySpriteRenderer.enabled = false;
                    }


                    break;
                }
        }
        if (!Defeated)
        {
            if (move.magnitude > 0f)
            {
                MyRigidbody.MovePosition(MyRigidbody.position+move);
            }
        }

        if (LookDirection != 0)
        {
            if (ForwardFacing != 0)
                ForwardFacing = LookDirection;
        }
        else
        {
            if (MoveDirection.magnitude > 0)
                ForwardFacing = (int)Mathf.Sign(MoveDirection.x);
        }

        if (LookDirection != 0)
        this.transform.localScale = new Vector3(-1 * LookDirection * Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);

    }
    
    public AudioSource screechsound;
    public TyphoonField MyTyphoonField;
    private bool PlayerHasBeenReleased = false;
    public override void beginBossFight()
    {
        Astronaut.TheAstronaut.PlayerHasControl = true;
        //MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
        MyCollider.enabled = true;
        this.Vulnerable = true;
        AudioManager.AM.playMusic(AudioManager.AM.BossMusic, 1f, 1f, true);
        Astronaut.TheAstronaut.WatchingLocation = null;
        Astronaut.TheAstronaut.MyBossGolem = this;

    }
    public const float CLAWSTRIKESPEED = 20f;
    private void LateUpdate()
    {
        //if (MyRigidbody.bodyType == RigidbodyType2D.Dynamic)
            //MyRigidbody.velocity = new Vector2(MyRigidbody.velocity.x, Mathf.Min(0f, MyRigidbody.velocity.y));
    }
    private bool slasheffectactive = false;
    public ParticleSystem ClawImpact;
    public void setSlashEffect(bool ac)
    {
        if (slasheffectactive == ac) return;
        ClawCollider.enabled = ac;
        foreach (ParticleSystem ct in ClawTrails)
        {
            if (ac)
            {

                ct.Play();
                
            }
            else
            {
                ct.Stop();
                
            }
        }
        slasheffectactive = ac;
    }


    public JungleTreeProjectile JungleProjectile,SporePrefab;
    public ParticleSystem SporeExplosion;
    public void standBy()
    {
        //MyIceBreath.BreathActive = false;
        //TrudgeTowards = 0;
        //MyAnimator.SetBool("SporeShoot", false);
        MyAnimator.SetBool("Swooping", false);
        MovingToTargetPosition = false;
        setState(State.StandingBy, .5f + (Random.value * 2.5f * ((1f - Astronaut.AggressionLevelF))));
    }
    private int ForwardFacing = -1;
    
    public ParticleSystem SlashCue;
    public void setState(State st, float dur)
    {

        StateTime = Time.time;
        StateDuration = dur;
        

        MyState = st;
    }

    private float clawStartTime = -10f;
    private bool SlashPoised = false;

    private float SlashLerp = 0f;
    public Vector3 ClawStrikeDirection;
    
    public void startClawStrikeAttack()
    {
        setState(State.ClawStriking, 2f);
        //Debug.Log("SLASH!");
        if (SlashCue) SlashCue.Play();
        //SlashLerp = 0f;
        //SlashTransform.position = SlashAwayTransform.position;
        ClawCollider.enabled = false;
        //SlashPoised = false;
        clawStartTime = Time.time;
        MoveDirection = new Vector2();
        MyAnimator.SetBool("Swooping",true);
        
        Astronaut plr = Astronaut.TheAstronaut;
        Vector3 dif = (plr.transform.position - this.transform.position);
        ClawStrikeDirection = dif.normalized;
        ClawStrikeDirection = new Vector3(dif.x,Mathf.Min(dif.y,-.25f),0f).normalized;

        int dir = (int)Mathf.Sign(dif.x);
        if (dir == 0)
        {
            LookDirection = ForwardFacing;
        }
        else
        {
            LookDirection = dir;
        }


    }
    private bool FiredSporeAttack = true;
    public void startSporeShoot()
    {
    setState(State.SporeShooting, 2f);
        
        MovingToTargetPosition = false; //MoveDirection = 0;

        FiredSporeAttack = false;
        Debug.Log("Shoot Spores");
        MyAnimator.SetTrigger("Spreading");
    }
    private bool TyphoonInitiated = false;
    public void startTyphoon()
    {
        setState(State.TyphoonStarting, 1f);
        TyphoonGlow.Play();
        //MyAnimator.SetTrigger("Glowing");
        TyphoonInitiated = false;
        //MyTyphoonField.WindStartTime = Time.time;
        //MyTyphoonField.WindForce = (new Vector2()*20f);
        //RainingIceShards = true;
        //RainingIceShardsStartTime = Time.time;
        //RainingIceShardsDuration = 5f;
    }

    


    public SpriteRenderer MySpriteRenderer;

    public override void OnDefeated()
    {
        base.OnDefeated();

        setState(State.Defeated, 3f);
        //MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
        MyRigidbody.bodyType = RigidbodyType2D.Static;
        MyCollider.enabled = false;
        MyWeakSpot.MyCollider.enabled = false;
        MyAnimator.SetBool("Swooping", false);
        
        MyAnimator.SetBool("Destroyed", true);
        if (DefeatSplosion == null)
            DefeatSplosion = ScreechParticles;
        DefeatSplosion.Play();
        AudioManager.AM.crossfade(AudioManager.AM.CurrentMusic, 0f, 4f);
        if (AudioManager.AM.GolemDestroyed)
            AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.GolemDestroyed, AudioManager.AM.PlayerAudioMixer, volume: 2f, pitch: 1f, looped: false, destroyafter: 5f);

        MySpriteRenderer.enabled = false;
        MyCollider.enabled = false;
        MyWeakSpot.MyCollider.enabled = false;
        ClawCollider.enabled = false;
        MyDroppedGoalElement.Collectable = false;
        explodeIntoPieces();

        MyDroppedGoalElement.gameObject.SetActive(true);
        Astronaut.TheAstronaut.WatchingLocation = MyDroppedGoalElement.transform;
        SpriteRenderer[] rs = MyDroppedGoalElement.gameObject.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer s in rs)
        {
            s.enabled = true;
        }

        Astronaut.TheAstronaut.Invulnerable = true;
        Astronaut.TheAstronaut.PlayerHasControl = false;
        this.Vulnerable = false;
        MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
    }
    private bool exploded = false;
    public Rigidbody2D ExplodedPiecePrefab;
    public void explodeIntoPieces()
    {
        if (exploded) return;
        exploded = true;
        SpriteRenderer[] rs = this.gameObject.GetComponentsInChildren<SpriteRenderer>();
        MyAnimator.enabled = false;
        foreach (SpriteRenderer s in rs)
        {
            if (s.transform.IsChildOf(MyDroppedGoalElement.transform)) continue;

            if ((s.enabled) && (s.gameObject.activeSelf) && (s.gameObject.activeInHierarchy))
            {
                Rigidbody2D exp = GameObject.Instantiate(ExplodedPiecePrefab, s.transform.position, s.transform.rotation).GetComponent<Rigidbody2D>();
                Vector2 rv = Random.insideUnitCircle;
                s.enabled = false;
                exp.velocity = (new Vector2(rv.x, rv.y + 1f) * 4f);
                exp.angularVelocity = (((Random.value - .5f) / .5f) * 5f);

                SpriteRenderer dp = GameObject.Instantiate(s, exp.transform).GetComponent<SpriteRenderer>();
                dp.enabled = true;
                dp.color = new Color(dp.color.r * .25f, dp.color.g * .25f, dp.color.b * .25f, dp.color.a);
                dp.flipX = s.flipX;
                dp.flipY = s.flipY;
                foreach (Transform t in dp.GetComponentsInChildren<Transform>())
                {
                    if ((t.gameObject.Equals(exp.gameObject)) || (t.gameObject.Equals(dp.gameObject))) continue;
                    GameObject.Destroy(t.gameObject);
                }
                GameObject.Destroy(exp.gameObject, 10f);
            }
        }

    }
    private void OnCollisionStay2D(Collision2D collision)
    {

        Astronaut a = collision.gameObject.GetComponent<Astronaut>();
        if ((a != null) && (a.Alive) && (!this.Defeated))
        {
            if (collision.otherCollider.gameObject.Equals(this.gameObject))
            {
                //Collided with the body.
                Debug.Log("Collided with the Body");
                if (!isStunned())
                    if ((Time.time - a.lastDamageTakenTime) >= 2f)
                    {
                        Vector3 dif = (a.transform.position - this.transform.position);
                        a.TakeDamage(5f, dif.normalized * 10f + new Vector3(0f, a.JumpSpeed, 0f));
                    }
            }
            else if (collision.otherCollider.gameObject.Equals(ClawCollider))
            {


            }
            else
            {
                Debug.Log("Collided with?: " + collision.otherCollider.gameObject.name);
            }

        }
    }

    public void onSlashed(Collider2D collider)
    {
        Astronaut a = collider.gameObject.GetComponent<Astronaut>();
        if ((a != null) && (a.Alive) && (!this.Defeated))
        {
            //Debug.Log("Collided with?: " + collider.gameObject.name);

            if ((Time.time - a.lastDamageTakenTime) >= 2f)
            {
                Vector3 dif = (a.transform.position - this.transform.position);
                a.TakeDamage(50f, dif.normalized * 10f + new Vector3(0f, a.JumpSpeed, 0f));
            }

        }

    }
}
