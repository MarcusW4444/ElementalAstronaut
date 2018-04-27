using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceGolem : BossGolem {

    public enum State{Waiting,Introducing,StandingBy,FrostBreathing,ShardInitiating, Slashing,Defeated };
    // Use this for initialization
    public State MyState = State.Waiting;
    public ParticleSystem EruptingParticles;
    public Transform StartTransform;
    void Start () {
        Risen = false;
        Roared = false;
        MyCollider.enabled = false;
        MyWeakSpot.MyCollider.enabled = false;
        MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
        this.transform.position = StartTransform.position;
        PlayerHasBeenReleased = false;
        setState(State.Waiting, 0f);

    }

    public bool Introduced = false;
    public override void introduceBoss()
    {
        if (Introduced) return;
        Introduced = false;
        //animate the starting animation
        Debug.Log("Introducing the Golem!");
        hasbeenintroduced = true;
        Astronaut.TheAstronaut.PlayerHasControl = false;
        Astronaut.TheAstronaut.WatchingLocation = IntroTargetPosition;
        RainingIceShards = false;
        EruptingParticles.Play(true);
        introvalue = 0f;
        //
        IntroStartPosition = this.transform.position;
        setState(State.Introducing, 0f);
    }

    private float StateTime = -10f;
    private float StateDuration = 1f;
    public float StunTime = -10f;
    public Animator MyAnimator;
    public Collider2D SlashCollider;
    public Transform SlashTransform;
    public ParticleSystem SlashTrail1, SlashTrail2, SlashTrail3;// SlashTrail1Instance, SlashTrail2Instance, SlashTrail3Instance;
    public Transform IceShardSpawnPoint1, IceShardSpawnPoint2;
    private float LastShardDroppedTime = -10f;
    public IceShard IceShardPrefab;
    public bool RainingIceShards=false;
    private float RainingIceShardsStartTime = -10f;
        private float RainingIceShardsDuration = 5f;
    private float IceShardDropInterval = .3f;
    public ParticleSystem IceShardGlow;
    //ABILITY 1: Slash Attack
    //ABILITY 2: Frost Breath
    //ABILITY 3: Ice Shards

    // Update is called once per frame
    void Update () {
		
        
	}

    public bool isStunned()
    {
        return ((StunTime - Time.time) >= 0f);
    }

    public Transform IntroTargetPosition;
    public ParticleSystem RoarParticles,DefeatSplosion;
    private bool Risen=false, Roared=false;
    private float RoarTime = -10f;

    
    public FrostParticle FrostPrefab;
    public Transform MouthTransform;
    public IceBreath MyIceBreath;
    public bool BreathingFrost = false;
    public void spewFrostParticle(Vector3 dir)
    {
        float sp = 15f;
        FrostParticle p = GameObject.Instantiate(FrostPrefab,MouthTransform.position,new Quaternion());
        p.MyRigidbody.velocity = dir * sp;

    }
    private float WalkSpeed = 2f;
    private int MoveDirection = 0;
    private int LookDirection = 0;
    private int TrudgeTowards = 0;
    private Vector3 IntroStartPosition;
    private float introvalue = 0f;
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
                        TrudgeTowards = 0;
                        Astronaut.TheAstronaut.MyRigidbody.velocity = new Vector2(0f,Astronaut.TheAstronaut.MyRigidbody.velocity.y); //Hold Still, please.
                        if (!Risen)
                        {
                            Vector3 dif = (IntroTargetPosition.position - this.transform.position);
                            float speed = 1f;
                            float du = (IntroStartPosition - IntroTargetPosition.position).magnitude;
                            
                            float d = (speed * Time.deltaTime);
                            if (introvalue >= 1f)
                            {
                                this.transform.position = IntroTargetPosition.position;
                                Risen = true;
                            EruptingParticles.Stop(true);
                            Roared = false;
                                MySpriteRenderer.transform.localPosition = new Vector3(0.99f, .73f, 0f);
                            } else
                            {
                            introvalue = Mathf.Clamp01(introvalue + (d/du));
                            
                                Vector2 c = (Random.insideUnitCircle.normalized*.1f);
                            this.transform.position = (Vector3.Lerp(IntroStartPosition, IntroTargetPosition.position, introvalue) + new Vector3(c.x, c.y,0f));
                            //MySpriteRenderer.transform.localPosition = new Vector3(.99f+c.x,.73f+c.y,0f);
                            }


                        } else if (!Roared)
                        {
                            RoarParticles.Play();
                            plr.addCamShake(1f, 1f, 1f, 2f, .5f);
                        AudioManager.AM.crossfade(AudioManager.AM.CurrentMusic, 0f, .3f);
                        roarsound = AudioManager.AM.playGeneralSound(this.transform.position,AudioManager.AM.IceBossRoar, AudioManager.AM.PlayerAudioMixer, 1f, 1f, false,30f);
                        MyAnimator.SetTrigger("Awaken");
                            Roared = true;
                            RoarTime = Time.time;
                            //Pass some RRRRRRRAAAAAAAAWWWWWWWRRRRRRRR!!!! text.
                        } else
                        {
                            
                            if ((Time.time - RoarTime) >= 3f)
                            {

                            Introduced = true;
                            beginBossFight();
                            AudioManager.AM.crossfade(roarsound, 0f, 2f);
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
                            if (TrudgeTowards == 0)
                            {
                                TrudgeTowards = dir;
                            }
                            else
                            {
                                LookDirection = 0;
                                MoveDirection = TrudgeTowards;
                                
                                MyRigidbody.velocity = new Vector2(TrudgeTowards * WalkSpeed* (1f + (2f * Astronaut.AggressionLevelF)), MyRigidbody.velocity.y);
                            }

                            if (stateexpired)
                            {
                                //Choose an action
                                MyRigidbody.velocity = new Vector2(0f, MyRigidbody.velocity.y);
                                float r = Random.value;
                                if (r < .25f)
                                    startSlashAttack();
                                else if (r < .5f)
                                    startIceBreath();
                                else if (r < .75f)
                                    startRainingIceShards();
                                else
                                    standBy();
                            }
                        }
                        MyAnimator.SetBool("Walking",Mathf.Abs(MyRigidbody.velocity.x) > .1f);
                    } else
                    {
                        MyAnimator.SetBool("Walking",false);

                    }
                        

                        break;
                    }
                case State.FrostBreathing:
                    {
                        TrudgeTowards = 0;
                        MoveDirection = 0;
                        MyIceBreath.BreathActive = true;
                        MyRigidbody.velocity = new Vector2(0f, MyRigidbody.velocity.y);
                    setSlashEffect(false);
                    if (!Defeated)
                        if ((plr != null) && (plr.Alive))
                        {

                            Vector3 dif = (plr.transform.position - MyIceBreath.transform.position);
                            if (Mathf.Sign(dif.x) == LookDirection)
                            {

                                float ang = Vector3.SignedAngle(MyIceBreath.transform.forward,dif.normalized,Vector3.forward);
                                float maxdegsPS = (30f * (1f +(2f* Astronaut.AggressionLevelF)));
                                float delt = maxdegsPS * Time.fixedDeltaTime;
                                //MyIceBreath.transform.LookAt(MyIceBreath.transform.position + dif.normalized);
                                //if (false)
                                
                                if (Mathf.Abs(ang) < delt)
                                {
                                    MyIceBreath.transform.LookAt(MyIceBreath.transform.position + dif.normalized);
                                } else
                                {
                                    MyIceBreath.transform.Rotate(0f,0f,Mathf.Sign(ang)*delt,Space.World);
                                }
                            }
                            if (MyIceBreath.MyCollider.OverlapPoint(new Vector2(plr.transform.position.x, plr.transform.position.y)))
                            {
                                MyIceBreath.OnTouched(plr.MyCollider);
                            }
                            //Physics2D.OverlapBox(,LayerMask.GetMask(new string[]{"Player" }));
                        }
                            if (stateexpired)
                        {
                            MyIceBreath.BreathActive = false;
                            MyAnimator.SetBool("IceBreathing", false);
                        standBy();
                        }
                        break;
                    }
                case State.ShardInitiating:
                    {
                    if (!Defeated)
                        if (stateexpired)
                        {
                            standBy();
                        }

                        break;
                    }
                case State.Slashing:
                    {
                        float slashPoseDur = (24f/60f);
                        float slashWaitDur = (16f/60f);
                        float slashDuration = (36f/60f);
                        float SlashVelocity = 10f+(15f*Astronaut.AggressionLevelF);
                        float v = (Time.time - StateTime);
                    if (!Defeated)
                        if (SlashPoised)
                        {

                            if ((v/slashDuration) >= 1f)
                            {
                                
                                setSlashEffect(false);
                                MyRigidbody.velocity = new Vector2(0f, MyRigidbody.velocity.y);
                                SlashTransform.position = SlashEndTransform.position;
                                if ((v/(slashDuration+slashWaitDur))>= 1f)    
                                {
                                    standBy();
                                    SlashTransform.position = SlashAwayTransform.position;
                                }
                            } else
                            {
                                SlashTransform.position = Vector3.Lerp(SlashStartTransform.position, SlashEndTransform.position, (v / slashDuration));
                                setSlashEffect(true);
                                MyRigidbody.velocity = new Vector2(SlashVelocity*LookDirection, MyRigidbody.velocity.y);
                            }

                            

                        } else
                        {
                            setSlashEffect(true);
                            
                            if ((v/slashPoseDur) >= 1f)
                            {
                                SlashTransform.position = SlashStartTransform.position;
                                if (v/(slashPoseDur+slashWaitDur)>= 1f)
                                {
                                    SlashPoised = true;
                                    StateTime = Time.time;
                                }
                            } else
                            {
                                SlashTransform.position = Vector3.Lerp(SlashAwayTransform.position, SlashStartTransform.position, Mathf.Clamp01(v / slashPoseDur));
                                
                            }
                            MyRigidbody.velocity = new Vector2(0f, MyRigidbody.velocity.y);
                        }
                    if (!Defeated)
                    {
                        MyCollider.enabled = true; MyWeakSpot.MyCollider.enabled = true;
                    }
                        break;
                    }
                case State.Defeated:
                    {
                        float f = (Time.time - StateTime);
                        MyIceBreath.BreathActive = false;
                        RainingIceShards = false;
                    MyAnimator.SetBool("Glowing", false);
                    MyAnimator.SetBool("IceBreathing", false);
                    MyCollider.enabled = false;
                    MyWeakSpot.MyCollider.enabled = false;
                    MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
                    MyRigidbody.velocity = new Vector2(0f, 0f);
                    if (f < 3f)
                        {
                            setSlashEffect(false);
                            MySpriteRenderer.enabled = true;
                            MySpriteRenderer.color = new Color(MySpriteRenderer.color.r, MySpriteRenderer.color.g, MySpriteRenderer.color.b,1f-(f/3f));
                            MySpriteRenderer.transform.Rotate(0f,0f,Time.fixedDeltaTime*360f*4f);
                        } else
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

            if (RainingIceShards) {
                if ((Time.time - RainingIceShardsStartTime) >= RainingIceShardsDuration)
                {
                    RainingIceShards = false;
                
            }

                if ((Time.time - LastShardDroppedTime) >= (IceShardDropInterval* (1f - (.6f * Astronaut.AggressionLevelF))))
                {
                    dropIcicle();
                }
            }

        /*
    if (Defeated)
    {
        MyRigidbody.bodyType = RigidbodyType2D.Static;
        MyCollider.enabled = false;
        MyWeakSpot.MyCollider.enabled = false;
    }
    */

        this.transform.localScale = new Vector3(-1*ForwardFacing*Mathf.Abs(this.transform.localScale.x), this.transform.localScale.y, this.transform.localScale.z);
        MyIceBreath.FrostParticles.transform.localScale = new Vector3(-1 * ForwardFacing * Mathf.Abs(MyIceBreath.FrostParticles.transform.localScale.x), MyIceBreath.FrostParticles.transform.localScale.y, MyIceBreath.FrostParticles.transform.localScale.z);
        //MyRigidbody.velocity = new Vector2(movedir * movespeed, MyRigidbody.velocity.y);

    }
    public AudioSource roarsound;

    public void dropIcicle()
    {
        IceShard s = GameObject.Instantiate(IceShardPrefab,Vector3.Lerp(IceShardSpawnPoint1.position, IceShardSpawnPoint2.position, Random.value),IceShardPrefab.transform.rotation);
        s.Falling = true;
        s.Autosensing = false;
        s.FallTime = Time.time;
        LastShardDroppedTime = Time.time;


    }
    private bool PlayerHasBeenReleased = false;
    private void LateUpdate()
    {
        if (MyRigidbody.bodyType == RigidbodyType2D.Dynamic)
        MyRigidbody.velocity = new Vector2(MyRigidbody.velocity.x, Mathf.Min(0f,MyRigidbody.velocity.y));
    }
    private bool slasheffectactive = false;
    public void setSlashEffect(bool ac)
    {
        if (slasheffectactive == ac) return;
        SlashCollider.enabled = ac;
        if (ac)
        {
            
            SlashTrail1.Play();
            SlashTrail2.Play();
            SlashTrail3.Play();
        } else
        {
            SlashTrail1.Stop();
            SlashTrail2.Stop();
            SlashTrail3.Stop();
        }
        slasheffectactive = ac;
    }
    public void standBy()
    {
        MyIceBreath.BreathActive = false;
        TrudgeTowards = 0;
        MyAnimator.SetBool("IceBreathing", false);

        setState(State.StandingBy, .5f + (Random.value * 2.5f*((1f - Astronaut.AggressionLevelF))));
    }
    private int ForwardFacing = -1;
    public Transform SlashStartTransform, SlashEndTransform, SlashAwayTransform;
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
    private bool SlashPoised = false;
    
    private float SlashLerp = 0f;
    public void startSlashAttack()
    {
        setState(State.Slashing,3f);
        //Debug.Log("SLASH!");
        if (SlashCue) SlashCue.Play();
        SlashLerp = 0f;
        SlashTransform.position = SlashAwayTransform.position;
        SlashCollider.enabled = false;
        SlashPoised = false;
        MoveDirection = 0;
        MyAnimator.SetTrigger("Slash");
        Astronaut plr = Astronaut.TheAstronaut;
        Vector3 dif = (plr.transform.position - this.transform.position);
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

    public void startIceBreath()
    {
        setState(State.FrostBreathing,3f);
        Astronaut plr = Astronaut.TheAstronaut;
        Vector3 dif = (plr.transform.position - this.transform.position);
        int dir = (int)Mathf.Sign(dif.x);
        MoveDirection = 0;
        MyAnimator.SetBool("IceBreathing",true);
        Debug.Log("Breathe Ice!");
        MyIceBreath.BreathActive = true;
        MyIceBreath.transform.LookAt(this.transform.position+dif.normalized);
        if (dir == 0)
        {
            LookDirection = ForwardFacing;
        }
        else
        {
            LookDirection = dir;


        }

    }

    public void startRainingIceShards()
    {
        setState(State.ShardInitiating, 1f);
        IceShardGlow.Play();
        MyAnimator.SetTrigger("Glowing");
        RainingIceShards = true;
        RainingIceShardsStartTime = Time.time;
        RainingIceShardsDuration = 5f;
    }

    public SpriteRenderer MySpriteRenderer;
    
    public override void OnDefeated()
    { 
        base.OnDefeated();
        
        setState(State.Defeated,3f);
        //MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
        MyRigidbody.bodyType = RigidbodyType2D.Static;
        MyCollider.enabled = false;
        MyWeakSpot.MyCollider.enabled = false;
        MyAnimator.SetBool("Glowing", false);
        MyAnimator.SetBool("IceBreathing", false);
        MyAnimator.SetBool("Destroyed", true);
        if (DefeatSplosion== null)
            DefeatSplosion = RoarParticles;
        DefeatSplosion.Play();
        AudioManager.AM.crossfade(AudioManager.AM.CurrentMusic, 0f, 4f);
        if (AudioManager.AM.GolemDestroyed)
        AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.GolemDestroyed, AudioManager.AM.PlayerAudioMixer, volume: 2f, pitch: 1f, looped: false, destroyafter: 5f);
        
        MySpriteRenderer.enabled = false;
        MyCollider.enabled = false;
        MyWeakSpot.MyCollider.enabled = false; 
        SlashCollider.enabled = false;
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
            if ((s.enabled) && (s.gameObject.activeSelf)&&(s.gameObject.activeInHierarchy))
            {
                Rigidbody2D exp = GameObject.Instantiate(ExplodedPiecePrefab,s.transform.position,s.transform.rotation).GetComponent<Rigidbody2D>();
                Vector2 rv = Random.insideUnitCircle;
                s.enabled = false;
                exp.velocity = (new Vector2(rv.x, rv.y + 1f)*4f);
                exp.angularVelocity = (((Random.value - .5f) / .5f) * 5f);

                SpriteRenderer dp = GameObject.Instantiate(s,exp.transform).GetComponent<SpriteRenderer>();
                dp.enabled = true;
                dp.color = new Color(dp.color.r * .25f, dp.color.g * .25f, dp.color.b*.25f, dp.color.a);
                dp.flipX = s.flipX;
                dp.flipY = s.flipY;
                foreach (Transform t in dp.GetComponentsInChildren<Transform>())
                {
                    if ((t.gameObject.Equals(exp.gameObject)) || (t.gameObject.Equals(dp.gameObject))) continue;
                    GameObject.Destroy(t.gameObject);
                }
                GameObject.Destroy(exp.gameObject,10f);
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
            } else if (collision.otherCollider.gameObject.Equals(SlashCollider))
            {
                

            } else {
                Debug.Log("Collided with?: " + collision.otherCollider.gameObject.name);
            }
            
        }
    }

    

}
