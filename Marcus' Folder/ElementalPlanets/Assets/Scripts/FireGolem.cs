using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireGolem : BossGolem {

    public enum State { Waiting, Introducing, StandingBy,Repositioning, FireBeaming, LavaThrowing, FlameMeteor,LavaPillar, Defeated };
    // Use this for initialization
    public State MyState = State.Waiting;
    public Transform StartTransform;
    void Start()
    {
        Risen = false;
        Roared = false;
        MyCollider.enabled = false;
        MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
        this.transform.position = StartTransform.position;
        collisionEvents = new List<ParticleCollisionEvent>();
        foreach (Collider2D c in LavaPlatforms)
        {
            Physics2D.IgnoreCollision(MyCollider,c);
        }
        PlayerHasBeenReleased = false;
        setState(State.Waiting, 0f);
    }

    public bool Introduced = false;
    public override void introduceBoss()
    {
        if (Introduced) return;
        Introduced = false;
        //animate the starting animation
        hasbeenintroduced = true;
        //AudioManager.AM.playMusic(AudioManager.AM.BossMusic, 1f, 1f, true);
        AudioManager.AM.StopMusic();
        Debug.Log("Introducing the Golem!");
        Astronaut.TheAstronaut.PlayerHasControl = false;
        Astronaut.TheAstronaut.WatchingBossLocation = IntroTargetPosition;
        RainingFlameMeteor = false;
        //
        setState(State.Introducing, 0f);
    }

    private float StateTime = -10f;
    private float StateDuration = 1f;
    public float StunTime = -10f;
    public Animator MyAnimator;
    
    public Transform ThrowTransform;
    public ParticleSystem LavaThrowTrail;// SlashTrail1Instance, SlashTrail2Instance, SlashTrail3Instance;
    public Transform[] MeteorSpawnPoint;
    public Collider2D[] LavaPlatforms;
    
    public FlameMeteor FlameMeteorPrefab; //FLAME METEOR
    public bool RainingFlameMeteor = false;
    public Transform LavaTransform;
    
    public ParticleSystem MeteorRainGlow;
    //ABILITY 1: Fire Laser
    //ABILITY 2: Lava Throw
    //ABILITY 3: Lava Pillar
    //ABILITY 4: Flame Meteor

    // Update is called once per frame
    void Update()
    {


    }

    public bool isStunned()
    {
        return ((StunTime - Time.time) >= 0f);
    }

    public Transform IntroTargetPosition;
    public ParticleSystem RoarParticles, DefeatSplosion;
    private bool Risen = false, Roared = false;
    private float RoarTime = -10f;


    
    public Transform BeamTransform;
    public FireBeam MyFireBeam;
    public bool FiringLaserBeam = false;
    
    private float WalkSpeed = 1f;
    private int MoveDirection = 0;
    private int LookDirection = 0;
    private int SwimTowards = 0;
    public ParticleSystem MagmaSplash;
    private bool HasRepositioned = false;
    private void FixedUpdate()
    {
        Astronaut plr = Astronaut.TheAstronaut;
        bool lookindirection = false;

        
        bool stateexpired = (Time.time >= (StateTime + StateDuration));
        /*
            if (Defeated)
        {
            MyRigidbody.bodyType = RigidbodyType2D.Static;
            MyCollider.enabled = false;
        }
            */
        switch (MyState)
        {
            case State.Waiting: { break; }
            case State.Introducing:
                {
                    //Introduced = false;
                    //Rise up from the ground in a menacing fashion
                    this.Vulnerable = false;
                    SwimTowards = 0;
                    
                    Astronaut.TheAstronaut.MyRigidbody.velocity = new Vector2(0f, Astronaut.TheAstronaut.MyRigidbody.velocity.y); //Hold Still, please.
                    if (!Risen)
                    {
                        
                        Vector3 dif = (IntroTargetPosition.position - this.transform.position);
                        float speed = 5f;
                        float d = (speed * Time.deltaTime);
                        if (!MagmaSplash.isPlaying) MagmaSplash.Play();
                        if (dif.magnitude < d)
                        {
                            this.transform.position = IntroTargetPosition.position;
                            Risen = true;
                            Roared = false;
                            MySpriteRenderer.transform.localPosition = new Vector3(0f, 0f, 0f);
                        }
                        else
                        {
                            this.transform.position = (this.transform.position + (dif.normalized * d));
                            Vector2 c = (Random.insideUnitCircle.normalized * .1f);

                            MySpriteRenderer.transform.localPosition = new Vector3(c.x, c.y, 0f);
                        }


                    }
                    else if (!Roared)
                    {
                        RoarParticles.Play();
                        AudioManager.AM.crossfade(AudioManager.AM.CurrentMusic,0f,.3f);
                        AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.FireBossRoar,AudioManager.AM.PlayerAudioMixer,1f,1f,false,10f);
                        plr.addCamShake(1f, 1f, 1f, 3f, .5f);
                        Roared = true;
                        RoarTime = Time.time;
                        MagmaSplash.Stop();
                        //Pass some RRRRRRRAAAAAAAAWWWWWWWRRRRRRRR!!!! text.
                    }
                    else
                    {

                        if ((Time.time - RoarTime) >= 3f)
                        {

                            Introduced = true;
                            beginBossFight();
                            //setState(State.StandingBy,3f+Random.value*3f);
                            standBy();
                        }
                    }


                    break;
                }
            case State.StandingBy:
                {
                    //Move Towards the Player
                    setThrowEffect(false);
                    if (!Defeated)
                    {
                        if (!MagmaSplash.isPlaying) MagmaSplash.Play();
                        this.Vulnerable = true;
                        if ((plr != null) && (plr.Alive))
                        {

                            Vector3 dif = (plr.transform.position - this.transform.position);
                            int dir = (int)Mathf.Sign(dif.x);
                            if (SwimTowards == 0)
                            {
                                SwimTowards = dir;
                            }
                            else
                            {
                                LookDirection = 0;
                                MoveDirection = SwimTowards;
                                MyRigidbody.velocity = new Vector2(SwimTowards * WalkSpeed*(1f+(1.5f*Astronaut.AggressionLevelF)), MyRigidbody.velocity.y);
                            }

                            if (stateexpired)
                            {
                                //Choose an action
                                MyRigidbody.velocity = new Vector2(0f, MyRigidbody.velocity.y);
                                float r = Random.value;
                                if (r < .25f)
                                startLavaPillars();
                                else if (r < .5f)
                                    startFireBeam();
                                else if (r < .75f)
                                startThrowAttack();//startRainingFlameMeteor();
                                else if ((Random.value < .5f) && (!HasRepositioned))
                                    startRepositioning();
                                else 
                                    standBy();

                            }
                        }
                    }


                    break;
                }
            case State.Repositioning:
                {
                    if (!Defeated)
                    {
                        MyCollider.enabled = false;
                        MyRigidbody.bodyType = RigidbodyType2D.Kinematic;

                        float st = ((Time.time - StateTime) / StateDuration);
                        if (st < .5f)
                        {
                            MagmaSplash.Emit(3);
                            this.transform.position = Vector3.Lerp(repositionstartposition,new Vector3(repositionstartposition.x,StartTransform.position.y, repositionstartposition.z),st/.5f);
                        }
                        else if (st < 1f)
                        {
                            
                            this.transform.position = Vector3.Lerp(new Vector3(repositiondestination.x, StartTransform.position.y, repositiondestination.z),repositiondestination, (st-.5f) / .5f);
                            MagmaSplash.Emit(3);
                            if (!MagmaSplash.isPlaying) MagmaSplash.Play();
                        } else {
                            if (MagmaSplash.isPlaying)MagmaSplash.Stop();
                            this.transform.position = repositiondestination;
                        }
                        if (!MagmaSplash.isPlaying) MagmaSplash.Play();
                    }

                    if (stateexpired)
                    {
                        if (!Defeated)
                        {
                            
                            this.transform.position = repositiondestination;
                            MyCollider.enabled = true;
                            MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
                            standBy();
                            HasRepositioned = true;
                        }
                        
                    }
                    break;
                }
            case State.FireBeaming:
                {
                    SwimTowards = 0;
                    MoveDirection = 0;
                    //MyFireBeam.BreathActive = true;
                    MyRigidbody.velocity = new Vector2(0f, MyRigidbody.velocity.y);
                    setThrowEffect(false);
                    if (!Defeated)
                    {
                        if ((plr != null) && (plr.Alive))
                        {
                            MyFireBeam.BeamActive = true;
                            Vector3 dif = (plr.transform.position - MyFireBeam.transform.position);
                            if (Mathf.Sign(dif.x) == LookDirection)
                            {

                                float ang = Vector3.SignedAngle(MyFireBeam.transform.forward, dif.normalized, Vector3.forward);
                                float maxdegsPS = 20f;
                                float delt = maxdegsPS * Time.fixedDeltaTime;
                                //MyIceBreath.transform.LookAt(MyIceBreath.transform.position + dif.normalized);
                                //if (false)

                                if (Mathf.Abs(ang) < delt)
                                {
                                    //MyFireBeam.transform.LookAt(MyFireBeam.transform.position + dif.normalized);
                                    MyFireBeam.transform.Rotate(0f, 0f, Mathf.Sign(ang), Space.World);
                                }
                                else
                                {
                                    MyFireBeam.transform.Rotate(0f, 0f, Mathf.Sign(ang) * delt, Space.World);
                                }
                            }
                            /*
                            if (MyFireBeam.MyCollider.OverlapPoint(new Vector2(plr.transform.position.x, plr.transform.position.y)))
                            {
                                MyFireBeam.OnTouched(plr.MyCollider);
                            }
                            */
                            //Physics2D.OverlapBox(,LayerMask.GetMask(new string[]{"Player" }));
                        }
                        if (stateexpired)
                        {
                            MyFireBeam.BeamActive = false;
                            standBy();
                        }
                    }
                    break;
                }
            case State.LavaPillar:
                {
                    if (!Defeated)
                    {


                        if (stateexpired)
                        {
                            if (LavaPillarsRemaining > 0)
                            {
                                LavaPillarsRemaining--;
                                Vector3 lpos = Vector3.Lerp(LavaPillarTransform1.position, LavaPillarTransform2.position,Random.value);
                                LavaPillarTelegraphRing.Play();
                                if ((Random.value < .25f)&&((plr != null) &&(plr.Alive)))
                                {

                                    lpos = new Vector3(plr.transform.position.x,lpos.y,lpos.z);
                                }
                                lpos = new Vector3(lpos.x,lpos.y-2f,lpos.z);
                                LavaPillar lp = GameObject.Instantiate(LavaPillarPrefab, lpos,LavaPillarPrefab.transform.rotation);
                                lp.PillarDuration *= 1f;
                                StateTime = Time.time;
                                StateDuration = 1f;
                            } else
                            {
                                standBy();
                            }
                            
                        }
                    }
                    break;
                }
            case State.FlameMeteor:
                {
                    /*
                    if (!Defeated)
                        if (stateexpired)
                        {
                            standBy();
                        }

                    */
                    break;
                }
            case State.LavaThrowing:
                {
                    
                    float throwPoseDur = 2.2f;
                    float throwWaitDur = .8f;
                    float throwDuration = .4f;
                    float ThrowVelocity = 10f;
                    float v = (Time.time - StateTime);
                    if (!Defeated)
                        if (ThrowPoised)
                        {

                            if ((v / throwDuration) >= 1f)
                            {

                                //setThrowEffect(false);
                                MyRigidbody.velocity = new Vector2(0f, MyRigidbody.velocity.y);

                                foreach (Lavaball lb in BallsOfLava)
                                {
                                    if (lb != null)
                                    {
                                        Vector3 rs = (Random.insideUnitCircle * 1f);
                                        Vector3 dif = ((plr.transform.position + new Vector3(rs.x, rs.y, 0f)) - lb.transform.position);
                                        dif = dif.normalized;
                                        lb.Launch(dif * ThrowVelocity);
                                    }
                                }
                                BallsOfLava.Clear();
                                //ThrowTransform.position = SlashEndTransform.position;
                                if ((v / (throwDuration + throwWaitDur)) >= 1f)
                                {
                                    standBy();
                                    ThrowTransform.position = ThrowAwayTransform.position;
                                }
                            }
                            else
                            {
                                ThrowTransform.position = Vector3.Lerp(ThrowStartTransform.position, ThrowEndTransform.position, (v / throwDuration));
                                //setThrowEffect(true);

                            }



                        }
                        else
                        {
                            //setThrowEffect(true);

                            if ((v / throwPoseDur) >= 1f)
                            {
                                ThrowTransform.position = ThrowStartTransform.position;
                                if (v / (throwPoseDur + throwWaitDur) >= 1f)
                                {
                                    ThrowPoised = true;
                                    StateTime = Time.time;
                                }
                            }
                            else
                            {
                                ThrowTransform.position = Vector3.Lerp(ThrowAwayTransform.position, ThrowStartTransform.position, Mathf.Clamp01(v / throwPoseDur));

                                MyRigidbody.velocity = new Vector2(0f, MyRigidbody.velocity.y);
                                
                            }
                            foreach (Lavaball lb in BallsOfLava)
                            {
                                if (lb != null)
                                {
                                    lb.transform.position = (lb.StartPosition + (ThrowTransform.position - ThrowAwayTransform.position));
                                }
                            }
                        }

                    if (Defeated)
                    {
                        foreach (Lavaball lb in BallsOfLava)
                        {
                            if (lb.gameObject.activeInHierarchy)
                            {
                                GameObject.Destroy(lb.gameObject);
                            }
                        }
                        BallsOfLava.Clear();
                    }
                    
                    if (!Defeated)
                        MyCollider.enabled = true;
                    break;
                }
            case State.Defeated:
                {
                    float f = (Time.time - StateTime);
                    MyFireBeam.BeamActive = false;
                    RainingFlameMeteor = false;
                    MyCollider.enabled = false;
                    MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
                    MyRigidbody.velocity = new Vector2(0f, 0f);
                    LavaTransform.transform.position = LavaTransform.transform.position - (Vector3.up * Time.deltaTime * 2f); //Make the lava subside when defeated
                    if (f < 3f)
                    {
                        setThrowEffect(false);
                        MySpriteRenderer.enabled = true;
                        MySpriteRenderer.color = new Color(MySpriteRenderer.color.r, MySpriteRenderer.color.g, MySpriteRenderer.color.b, 1f - (f / 3f));
                        MySpriteRenderer.transform.Rotate(0f, 0f, Time.fixedDeltaTime * 360f * 4f);
                    }
                    else
                    {
                        if (!PlayerHasBeenReleased)
                        {
                            plr.PlayerHasControl = true;
                            PlayerHasBeenReleased = true;
                        }
                        MySpriteRenderer.enabled = false;
                    }


                    break;
                }

        }

        if (LookDirection != 0)
        {
            if (ForwardFacing != 0)
                ForwardFacing = LookDirection;
        }
        else
        {
            if (MoveDirection != 0)
                ForwardFacing = MoveDirection;
        }

        if (RainingFlameMeteor)
        {
            RainingFlameMeteor = false;
            dropMeteor();
            /*
            if ((Time.time - RainingIceShardsStartTime) >= RainingIceShardsDuration)
            {
                RainingFlameMeteor = false;
            }

            if ((Time.time - LastShardDroppedTime) >= IceShardDropInterval)
            {
                dropIcicle();
            }
            */
        }



        updateForwardFace();
        //MyFireBeam.FrostParticles.transform.localScale = new Vector3(-1 * ForwardFacing * Mathf.Abs(MyFireBeam.FrostParticles.transform.localScale.x),
        //MyFireBeam.FrostParticles.transform.localScale.y, MyFireBeam.FrostParticles.transform.localScale.z);
        

    }

    public void updateForwardFace()
    {
        this.transform.localScale = new Vector3(-1 * ForwardFacing * Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
    }

    public void dropMeteor()
    {
        /*
        IceShard s = GameObject.Instantiate(FlameMeteorPrefab, Vector3.Lerp(MeteorSpawnPoint.position, IceShardSpawnPoint2.position, Random.value), FlameMeteorPrefab.transform.rotation);
        s.Falling = true;
        s.Autosensing = false;
        s.FallTime = Time.time;
        */
        //LastShardDroppedTime = Time.time;


    }
    private bool PlayerHasBeenReleased = false;
    private void LateUpdate()
    {
        if (MyRigidbody.bodyType == RigidbodyType2D.Dynamic)
            MyRigidbody.velocity = new Vector2(MyRigidbody.velocity.x, Mathf.Min(0f, MyRigidbody.velocity.y));
        
    }
    private bool lavathrowingeffectactive = false;
    public void setThrowEffect(bool ac)
    {
        if (lavathrowingeffectactive == ac) return;
        //SlashCollider.enabled = ac;
        if (ac)
        {

            LavaThrowTrail.Play();
            
        }
        else
        {
            LavaThrowTrail.Stop();
            
        }
        lavathrowingeffectactive = ac;
    }
    public void standBy()
    {
        MyFireBeam.BeamActive = false;
        HasRepositioned = false;
        SwimTowards = 0;
        setState(State.StandingBy, 1f + Random.value * 1.5f);
    }
    private int ForwardFacing = -1;
    public Transform ThrowStartTransform, ThrowEndTransform, ThrowAwayTransform;
    public ParticleSystem SlashCue;
    public void setState(State st, float dur)
    {

        StateTime = Time.time;
        StateDuration = dur;
        //if ((st == State.Firing) && (MyState != State.Firing))
        //{
        //MyRigidbody.velocity = new Vector2();
        //ShootWindUpGlow.Play();
        //}

        MyState = st;
    }

    private float slashStartTime = -10f;
    private bool ThrowPoised = false;

    private float ThrowLerpLerp = 0f;
    public Transform RepositionTransform1, RepositionTransform2;
    private Vector3 repositiondestination,repositionstartposition;
    public void startRepositioning()
    {
        setState(State.Repositioning, 2f);
        //Disable the collider momentarily
        repositionstartposition = this.transform.position;
        repositiondestination = Vector3.Lerp(RepositionTransform1.position,RepositionTransform2.position,Random.value);
        repositiondestination = new Vector3(repositiondestination.x,this.transform.position.y, this.transform.position.z);

        MyCollider.enabled = false;
        MyRigidbody.bodyType = RigidbodyType2D.Kinematic;

    }
    public List<Lavaball> BallsOfLava = new List<Lavaball>();
    public void startThrowAttack()
    {
        setState(State.LavaThrowing, 3f);
        //Debug.Log("SLASH!");
        //if (SlashCue) SlashCue.Play();
        ThrowLerpLerp = 0f;
        ThrowTransform.position = ThrowAwayTransform.position;
        //SlashCollider.enabled = false;
        ThrowPoised = false;
        MoveDirection = 0;
        Astronaut plr = Astronaut.TheAstronaut;
        Vector3 dif = (plr.transform.position - this.transform.position);
        int dir = (int)Mathf.Sign(dif.x);
        MeteorRainGlow.Play();

        if (dir == 0)
        {
            LookDirection = ForwardFacing;
        }
        else
        {
            LookDirection = dir;


        }

        int lavaballstothrow = (int)(1 + (3 * Random.value));

        BallsOfLava.Clear();
        for (int i = 0; i < lavaballstothrow; i++)
        {
            Lavaball lb = GameObject.Instantiate(LavaballPrefab,new Vector3(this.transform.position.x+(Random.Range(-3f,3f)), ThrowAwayTransform.position.y+Random.Range(-2f,0f),this.transform.position.z),LavaballPrefab.transform.rotation).GetComponent<Lavaball>();
            BallsOfLava.Add(lb);

        }
    }

    public Lavaball LavaballPrefab;
    public void startFireBeam()
    {
        setState(State.FireBeaming, 3f);
        Astronaut plr = Astronaut.TheAstronaut;
        Vector3 dif = (plr.transform.position - MyFireBeam.transform.position);
        int dir = (int)Mathf.Sign(dif.x);
        MoveDirection = 0;

        if (dir == 0)
        {
            LookDirection = ForwardFacing;
        }
        else
        {
            LookDirection = dir;


        }

        updateForwardFace();
        
        MyFireBeam.BeamActive = true;

        MyFireBeam.transform.LookAt(MyFireBeam.transform.position + dif.normalized);

        

    }
    public int LavaPillarsRemaining;
    public ParticleSystem LavaPillarTelegraphRing;
    public LavaPillar LavaPillarPrefab;
    public Transform LavaPillarTransform1, LavaPillarTransform2;
    public void startLavaPillars()
    {
        setState(State.LavaPillar,1f);
        int spouts = 3; //Change this based on the aggression
        LavaPillarsRemaining = spouts;
        
        
        
    }


    public void startRainingFlameMeteor()
    {
        setState(State.FlameMeteor, 1f);
        MeteorRainGlow.Play();
        RainingFlameMeteor = true;
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

        foreach (Lavaball lb in BallsOfLava)
        {
            if (lb.gameObject.activeInHierarchy)
            {
                GameObject.Destroy(lb.gameObject);
            }
        }
        BallsOfLava.Clear();

        if (DefeatSplosion == null)
            DefeatSplosion = RoarParticles;
        DefeatSplosion.Play();
        MyDroppedGoalElement.gameObject.SetActive(true);
        MySpriteRenderer.enabled = false;
        MyCollider.enabled = false;
        //SlashCollider.enabled = false;

        Astronaut.TheAstronaut.Invulnerable = true;
        Astronaut.TheAstronaut.PlayerHasControl = false;
        this.Vulnerable = false;
        MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
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
            /* else if (collision.otherCollider.gameObject.Equals(SlashCollider))
            {


            }
            */
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


    
    public List<ParticleCollisionEvent> collisionEvents;

    

    void OnParticleCollision(GameObject other)
    {
        int numCollisionEvents = MagmaSplash.GetCollisionEvents(other, collisionEvents);

        Astronaut a = other.GetComponent<Astronaut>();
        int i = 0;

        while (i < numCollisionEvents)
        {
            if (a)
            {
                Vector3 pos = collisionEvents[i].intersection;
                Vector3 force = collisionEvents[i].velocity * 10;
                a.TakeDamage(10f*Time.fixedDeltaTime, collisionEvents[i].velocity);
            }
            i++;
        }
    }

}
