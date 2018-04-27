using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidGolem : BossGolem
{

    public enum VoidElement {Ice,Fire,Jungle,Final};
    // Use this for initialization
    public VoidElement VoidElementType;
    public ParticleSystem EruptingParticles;
    public Transform StartTransform;
    public static VoidGolem TheIceVoidGolem = null, TheFireVoidGolem = null, TheJungleVoidGolem = null;
    void Start () {

        Risen = false;
        MyCollider.enabled = false;
        if (MyWeakSpot)MyWeakSpot.MyCollider.enabled = false;
        MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
        this.transform.position = StartTransform.position;
        PlayerHasBeenReleased = false;
        Introducing = false;
        RainingSleet = 0f;
        if (VoidElementType == VoidElement.Ice)
            TheIceVoidGolem = this;
        if (VoidElementType == VoidElement.Fire)
            TheFireVoidGolem = this;
        if (VoidElementType == VoidElement.Jungle)
            TheJungleVoidGolem= this;
        //setState(State.Waiting, 0f);
        bossbodyoffset = (MyAnimator.gameObject.transform.position - this.transform.position);
    }

    private bool PlayerHasBeenReleased = false;
    private void LateUpdate()
    {
        if (MyRigidbody.bodyType == RigidbodyType2D.Dynamic)
            MyRigidbody.velocity = new Vector2(MyRigidbody.velocity.x, Mathf.Min(0f, MyRigidbody.velocity.y)); //Stop being propelled into the air.
    }

    public Animator MyAnimator;
    public bool Risen = false;
    public bool BossActive = false;
    public bool Introduced = false;
    public Transform IntroTargetPosition;
    public ParticleSystem DefeatSplosion;
    public float RainingSleet=0f;
    public override void introduceBoss()
    {
        if (Introduced) return;
        Introduced = false;
        //animate the starting animation
        Debug.Log("Introducing a Void Golem!");
        hasbeenintroduced = true;
        Astronaut.TheAstronaut.PlayerHasControl = false;
        Astronaut.TheAstronaut.WatchingLocation = IntroTargetPosition;
        
        Introducing = true;
        EruptingParticles.Play(true);
        introvalue = 0f;
        //
        IntroStartPosition = this.transform.position;
        //setState(State.Introducing, 0f);
    }
    // Update is called once per frame
    private Vector3 IntroStartPosition;
    private float introvalue = 0f;
    public ParticleSystem ReapplyIceSkinParticles, ReapplyIceSkinGlow;
    public ParticleSystem ReapplyMossSkinParticles, ReapplyMossSkinGlow;
    
    public bool Introducing = false;
    private float risentime = -10f;
    private bool ThrowingSpear = false;
    private bool ReapplyingIceSkin;
    private bool ReapplyingMossSkin = false;
    public GolemSpear SpearPrefab;
    private GolemSpear MyLaunchedSpear;
    public float IceSkinHealth = 400;
    public ParticleSystem SkinMeltingParticles, SkinShatteringParticles;
    public IcePillar IcePillarPrefab;
    public VoidBush BushPrefab;
    public bool PillarCasting = false;
    private float PillarCastSpacing;
    private int PillarCastCount = 0;
    private float PillarCastInterval;
    private float lastpillarcasttime = -10f;
    private float pillarcastlocation;
    private float PillarCastHeight;

    private bool SparkleCasting = false;
    private float SparkleCastDistance = 10f;
    //private int SparkleCastCount = (int)(8 * ((1f + 2f * Astronaut.AggressionLevelF)));
    private float SparkleCastInterval;// = (.25f * (1f - .75f * Astronaut.AggressionLevelF));
    private float SparkleCastSpacing;// = (2f * (1f - .75f * Astronaut.AggressionLevelF));
        
    private float lastsparklecasttime = -10f;
    private Vector3 sparklecastlocation;// = this.transform.position;
    private Vector3 sparklecastdirection;// = (Astronaut.TheAstronaut.transform.position - this.transform.position).normalized;
    public void meltSkinOff()
    {
        if (!Ice_FreezingSkinActive) return;
        Ice_FreezingSkinActive = false;
        //Play the melting sound and particles
        
        SkinMeltingParticles.Play();
        SkinLostTime = Time.time;
    }
    private float SkinLostTime = -10f;
    public void restoreIceSkin()
    {
        Ice_FreezingSkinActive = true;
        IceSkinHealth = 100f*(1f+(1*Astronaut.AggressionLevel));
        
        
    }
    public float MossSkinHealth = 0f;
    public void restoreMossSkin()
    {
        Jungle_MossSkinActive= true;
        MossSkinHealth = .5f * (1f + (1 * Astronaut.AggressionLevel));

        //And shed the weight as colliding particles

    }
    public void freezeMossSkinOff()
    {
        if (!Jungle_MossSkinActive) return;
        Jungle_MossSkinActive = false;
        //Play the melting sound and particles
        Am.am.oneshot(Am.am.M.IceShatter);
        SkinShatteringParticles.Play();
        SkinLostTime = Time.time;
    }
    public void pillarCast()
    {
        ActionTime = Time.time;
        PillarCasting = true;
        PillarCastCount = (int)(8*((1f+2f*Astronaut.AggressionLevelF)));
        PillarCastInterval = (.5f*(1f-.75f*Astronaut.AggressionLevelF));
        PillarCastSpacing = (3f * (1f - .5f * Astronaut.AggressionLevelF));
        PillarCastHeight = (1f * (1f+(2f* Astronaut.AggressionLevelF)));
        lastpillarcasttime = Time.time;
        pillarcastlocation = this.transform.position.x;
        pillarcastdirection = (int)Mathf.Sign(Astronaut.TheAstronaut.transform.position.x - this.transform.position.x);
    }
    public void sparkleCast()
    {
        ActionTime = Time.time;
        SparkleCasting = true;
        SparkleCastDistance = 10f;
        //SparkleCastCount = (int)(8 * ((1f + 2f * Astronaut.AggressionLevelF)));
        SparkleCastInterval = (1.5f * (1f - .75f * Astronaut.AggressionLevelF));
        SparkleCastSpacing = (4f * (1f - .75f * Astronaut.AggressionLevelF));
        
        lastsparklecasttime = Time.time;
        sparklecastlocation = this.transform.position;
        Vector3 dif = (Astronaut.TheAstronaut.transform.position - this.transform.position);
        dif = new Vector3(dif.x,Mathf.Max(dif.y,0f),0f);
        sparklecastdirection = dif.normalized;
    }
    private int pillarcastdirection = 0;
    public float ActionTime = -10f;
    private int TrudgeDirection = 0;
    public IceSkullProjectile SleetProjectile;
    void FixedUpdate () {
        Astronaut plr = Astronaut.TheAstronaut;
        if (!Defeated)
        {

            
            if (Introducing)
            {

                    this.Vulnerable = false;
                    
                    Astronaut.TheAstronaut.MyRigidbody.velocity = new Vector2(0f, Astronaut.TheAstronaut.MyRigidbody.velocity.y); //Hold Still, please.
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
                            risentime = Time.time;
                            plr.addCamShake(0f, .25f, 1f, .5f, .5f);
                            AudioManager.AM.crossfade(AudioManager.AM.CurrentMusic, 0f, .3f);
                            //MySpriteRenderer.transform.localPosition = new Vector3(0.99f, .73f, 0f);
                        }
                        else
                        {
                            introvalue = Mathf.Clamp01(introvalue + (d / du));
                            //Debug.Log(introvalue);
                            Vector2 c = (Random.insideUnitCircle.normalized * .1f);
                            this.transform.position = (Vector3.Lerp(IntroStartPosition, IntroTargetPosition.position, introvalue) + new Vector3(c.x, 0f, 0f));
                            //MySpriteRenderer.transform.localPosition = new Vector3(.99f+c.x,.73f+c.y,0f);
                        }


                    }
                    else
                    {

                        if ((Time.time - risentime) >= .5f)
                        {

                            Introduced = true;
                            Introducing = false;
                        
                        beginBossFight();
                        Astronaut.TheAstronaut.MyBossGolem = this;
                        BossActive = true;
                        ActionTime = Time.time;
                            //AudioManager.AM.crossfade(roarsound, 0f, 2f);
                            //setState(State.StandingBy,3f+Random.value*3f);
                            //standBy();
                        }
                    }




                }











































            if (BossActive){
                //Boss Behaviors

                
                switch (VoidElementType)
                {
                    case VoidElement.Ice:
                        {
                            //DEFENSE: Ice Skin-Freezes on Contact and Freezes Weapons
                            //REDEFENSE: Refreeze Skin
                            //ABILITY 1: Ice Pillar Casting
                            //ABILITY 2: Ice Spear Throw
                            //ABILITY 3: Drop Ice Projectiles
                            string ds = "";

                            float ts = 1.0f*(1f - .5f * Astronaut.AggressionLevelF);
                            if (ReapplyingIceSkin)
                            {
                                
                                ds += "A";
                                if (TimeThreshold(ActionTime, .1f*ts))
                                {
                                    
                                    
                                }
                                if (TimeThreshold(ActionTime, .5f*ts))
                                {
                                    restoreIceSkin();
                                }
                                if (TimeThresholdAbsolute(ActionTime,.6f*ts))
                                {
                                    ReapplyingIceSkin = false;
                                    ActionTime = Time.time;
                                    ds += "B";
                                }
                                TrudgeDirection = 0;
                            } else if (ThrowingSpear)
                            {
                                ds += "C";
                                if (TimeThreshold(ActionTime, .5f*ts))
                                {
                                    GolemSpear sp = GameObject.Instantiate<GolemSpear>(SpearPrefab,this.transform.position, SpearPrefab.transform.rotation);
                                    MyLaunchedSpear = sp;
                                    Vector3 dif = (plr.transform.position - this.transform.position);

                                    sp.Velocity = (new Vector2(dif.x, dif.y).normalized * (8f * (1f + 2f * Astronaut.AggressionLevelF)));
                                    sp.transform.Rotate(0f, 0f, Vector3.SignedAngle(Vector3.right, new Vector3(sp.Velocity.normalized.x, sp.Velocity.normalized.y, 0f), Vector3.forward));
                                    HasSpear = false;
                                    ds += "D";
                                }
                                
                                if (TimeThresholdAbsolute(ActionTime, 2.5f))
                                {
                                    HasSpear = true;
                                    ThrowingSpear = false;
                                    ActionTime = Time.time;
                                    ds += "E";
                                }
                                TrudgeDirection = 0;
                                ds += "F";
                            } else
                            {
                                ds += "G";
                                if (!Ice_FreezingSkinActive && ((Time.time - SkinLostTime) >= (20f*(1f-.75f*Astronaut.AggressionLevelF))))
                                {
                                    ActionTime = Time.time;
                                    ReapplyingIceSkin = true;
                                    ReapplyIceSkinParticles.Play();
                                    ReapplyIceSkinGlow.Play();
                                    Am.am.oneshot(Am.am.M.IceSkin);
                                    Am.am.oneshot(Am.am.M.IceSkinFrag);
                                    TrudgeDirection = 0;
                                    ds += "H";
                                } else if (TrudgeDirection == 0)
                                {
                                    if (IdleTime >= (4f * (1f-(0.75f*Astronaut.AggressionLevelF))))
                                    {
                                        TrudgeDirection = ((int)Mathf.Sign((plr.transform.position - this.transform.position).x));
                                        IdleTime = 0f;
                                    } else {
                                        IdleTime += Time.fixedDeltaTime;
                                        ds += "I";
                                    }
                                    
                                } else if ((Time.time - ActionTime)> (5f* (1f - .5f * Astronaut.AggressionLevelF)))
                                {
                                    ds += "J";
                                    float r = Random.value;
                                    //Choose to throw a spear
                                    if ((r < .25f) && (HasSpear))
                                    {
                                        ThrowingSpear = true;
                                        ActionTime = Time.time;
                                        IdleTime = 0f;
                                        ds += "K";
                                    } else if (r < .5f)
                                    {
                                        //Drop a bunch of Ice Projectiles
                                        TrudgeDirection = 0;
                                        pillarCast();
                                        ActionTime = Time.time;
                                        IdleTime = 0f;
                                        ds += "L";
                                    } else if (r < .75f)
                                    {
                                        //Drop a bunch of ice projectiles
                                        RainingSleet = 5f;
                                        LastSleetDropTime = Time.time;
                                        TrudgeDirection = 0;
                                        IdleTime = 0f;
                                        ActionTime = Time.time;
                                        dropSleet();
                                        ds += "M";
                                    } else
                                    {
                                        //Do nothing
                                        ActionTime = Time.time;
                                        TrudgeDirection = 0;
                                        ds += "N";
                                    }
                                    
                                }
                                
                                

                            }


                            if (PillarCasting)
                            {
                                ds += "O";
                                if ((Time.time - lastpillarcasttime) >= PillarCastInterval)
                                {
                                    ds += "P";
                                    RaycastHit2D rh = Physics2D.Linecast(new Vector2(pillarcastlocation,this.transform.position.y), new Vector2(pillarcastlocation, this.transform.position.y)- new Vector2(0f,5f), LayerMask.GetMask(new string[] { "Geometry" }));
                                    if (rh.distance <= 0f)
                                    {
                                        //No collision here. please do not walk over the edge
                                        PillarCasting = false;
                                    } else
                                    {
                                        IcePillar ip = GameObject.Instantiate(IcePillarPrefab,rh.point,IcePillarPrefab.transform.rotation);
                                        ip.FormulationHeight = PillarCastHeight;
                                        ip.MaxHealth = (10f * (Astronaut.AggressionLevel*4));
                                        ip.Health = ip.MaxHealth;
                                        lastpillarcasttime = Time.time;
                                        pillarcastlocation = pillarcastlocation + (PillarCastSpacing * pillarcastdirection);

                                        PillarCastCount--;
                                        if (PillarCastCount <= 0) PillarCasting = false;
                                    }
                                }

                            }

                            if (RainingSleet > 0f)
                            {
                                ds += "Q";
                                if ((Time.time - LastSleetDropTime) >= (2f*(1f-(.75f*Astronaut.AggressionLevelF))))
                                {
                                    ds += "R";
                                    LastSleetDropTime = Time.time;
                                    dropSleet();
                                }
                                RainingSleet = Mathf.Max(0f,RainingSleet - Time.fixedDeltaTime);
                            }



                            if (!Defeated)
                            {
                                if (HasSpear && Ice_FreezingSkinActive)
                                {
                                    SkinSpearGroup.SetActive(true);
                                    SkinGroup.SetActive(false);
                                    SpearGroup.SetActive(false);
                                    NoneGroup.SetActive(false);
                                }
                                else if (!HasSpear && Ice_FreezingSkinActive)
                                {
                                    SkinSpearGroup.SetActive(false);
                                    SkinGroup.SetActive(true);
                                    SpearGroup.SetActive(false);
                                    NoneGroup.SetActive(false);

                                }
                                else if (HasSpear && !Ice_FreezingSkinActive)
                                {
                                    SkinSpearGroup.SetActive(false);
                                    SkinGroup.SetActive(false);
                                    SpearGroup.SetActive(true);
                                    NoneGroup.SetActive(false);
                                }
                                else if (!HasSpear && !Ice_FreezingSkinActive)
                                {
                                    SkinSpearGroup.SetActive(false);
                                    SkinGroup.SetActive(false);
                                    SpearGroup.SetActive(false);
                                    NoneGroup.SetActive(true);
                                }
                            }


                            //Debug.Log(ds);
                            break;
                        }




















                    case VoidElement.Fire:
                        {
                            //DEFENSE: Shield + Fire Aura
                            //REDEFENSE: Sink into Acid Lava Pool (Make him do this more often, use your Ice Freezing power to freeze the lava and starve him of his shield)
                            //ABILITY 1: Fire Ball Management/Rockets (Shotgun, Rapid Fire Casting)
                            //ABILITY 2: Flaming Shield Bash
                            //ABILITY 3: Combustion Sparks Cast

                            bool ineedanewshield = ((Time.time - ShieldForgedTime) >= 10f-(5f*Astronaut.AggressionLevelF));

                            if (Fire_HoldingShield)
                            {
                                ShieldForgedTime = Time.time;

                            }
                            int movement = 0;

                            MyRigidbody.bodyType = RigidbodyType2D.Kinematic;

                            if (Leaping)
                            {

                                float leaprate = 1f;//(.75f + (.5f * Astronaut.AggressionLevelF));
                                float vl = Mathf.Clamp01(LeapTimeValue + (Time.fixedDeltaTime * leaprate));
                                LeapTimeValue = vl;
                                this.transform.position = (Vector3.Lerp(LeapStartPosition, LeapEndPosition, LeapTimeValue) + new Vector3(0f, 3f * (Mathf.Sin(vl * Mathf.PI)), 0f));
                                movement = (int)Mathf.Sign((int)(LeapEndPosition - LeapStartPosition).x);
                                if (vl == 1f)
                                {

                                    InsideLava = LeapIntoLava;
                                    CurrentLocation = LeapDestination;
                                    Leaping = false;
                                    ActionTime = Time.time;
                                }
                            } else if(ShieldBashing) {
                                float leaprate = (1f * (1f + (.5f*Astronaut.AggressionLevelF)));
                                float vl = Mathf.Clamp01(LeapTimeValue + (Time.fixedDeltaTime * leaprate));
                                LeapTimeValue = vl;
                                this.transform.position = Vector3.Lerp(LeapStartPosition, LeapEndPosition, LeapTimeValue);
                                
                                if (vl == 1f)
                                {
                                    movement = 0;
                                    BlazingShieldBashParticles.Stop(true);
                                    ShieldBashing = false;
                                    MyShieldBashCollider.enabled = false;
                                    CurrentLocation = LeapDestination;
                                    ActionTime = Time.time;
                                } else
                                {
                                    MyShieldBashCollider.enabled = true;
                                    movement = (int)Mathf.Sign((int)(LeapEndPosition-LeapStartPosition).x);
                                    if (!BlazingShieldBashParticles.isPlaying) BlazingShieldBashParticles.Play(true);
                                }
                            }
                            else if (ForgingShield)
                            {
                                float de = 3f;//(2f * (1f - .5f * Astronaut.AggressionLevelF));
                                float t = ((Time.time - ActionTime)/de);

                                if (t >= .25f)
                                    if (!forgedshield)
                                {
                                        forgedshield = true;
                                        Fire_HoldingShield = true;
                                        ShieldGrip = 1f;
                                        Debug.Log("Yeah! New Shield!");
                                    }
                                if (t < .5f)
                                {
                                    downshift = new Vector3(0f, -2f * (1f-Mathf.Abs((t - .25f) / .25f)), 0f);
                                } else
                                {
                                    downshift = new Vector3(0f,0f,0f);
                                }

                                if (t >= 1f)
                                {
                                    ForgingShield = false;
                                    Transform tr = chooseNearestLocation(false);
                                    LeapStartPosition = CurrentLocation.position;
                                    LeapEndPosition = tr.position;
                                    LeapDestination = tr;
                                    LeapTimeValue = 0f;
                                    LeapIntoLava = false;
                                    Leaping = true;
                                    IdleTime = 0f;
                                    ActionTime = Time.time;
                                    Debug.Log("Hopping out of the lava forge");
                                }
                                

                            }
                            else if (ineedanewshield)
                            {

                                if (InsideLava)
                                {
                                    Debug.Log("I'm in lava, and forging a shield");
                                    ForgingShield = true;
                                    forgedshield = false;
                                } else
                                {
                                    Debug.Log("Oh crap. I need a new shield.");
                                    Transform tr = chooseNearestLocation(true);
                                    LeapIntoLava = true;
                                    LeapStartPosition = CurrentLocation.position;
                                    LeapDestination = tr;
                                    LeapEndPosition = tr.position;
                                    LeapTimeValue = 0f;
                                    Leaping = true;

                                }

                            }
                            else if ((Time.time - ActionTime) > (4f * (1f - .5f * Astronaut.AggressionLevelF)))
                            {

                                Debug.Log("Decide...");

                                if (MyHeldLavaBalls.Count >= MaxLavaBalls)
                                {
                                    LavaBallAssault = true;
                                    LavaBallAssaultShotInterval = 2f - (1.5f * Astronaut.AggressionLevelF);
                                    LavaBallAssaultMultiple = 1 + ((int)(Astronaut.AggressionLevel * Random.value));
                                    //Launch 1 at a time at the lowest level
                                    //Launch 4 at a time at the highest level
                                }
                                float r = Random.value;
                                Transform shieldbashdest = chooseNearestLocation(false);
                                if ( (r < .2f) && (Fire_HoldingShield) &&(!InsideLava)&& Mathf.Sign((int)(shieldbashdest.position.x - this.transform.position.x)) == Mathf.Sign((int)(Astronaut.TheAstronaut.transform.position.x - this.transform.position.x)))
                                {
                                    Debug.Log("Shield Bash!");
                                    //Shield Bash
                                    LeapStartPosition = CurrentLocation.position;
                                    LeapEndPosition = shieldbashdest.position;
                                    //ShieldBashDestination = shieldbashdest.position;
                                    LeapDestination = shieldbashdest;
                                    LeapIntoLava = false;
                                    //CurrentLocation = LeapDestination;
                                    LeapTimeValue = 0f;
                                    BlazingShieldBashParticles.Play();
                                    Am.am.oneshot(Am.am.M.ShieldBashShift);
                                    ShieldBashing = true;
                                    ActionTime = Time.time;
                                    IdleTime = 0f;

                                }
                                else if ((r < .4f))
                                {
                                    Debug.Log("Sparkle Cast!");
                                    //Cast Sparkles
                                    //TrudgeDirection = 0;
                                    sparkleCast();
                                    ActionTime = Time.time;
                                    IdleTime = 0f;

                                }
                                else if ((r < .6f) && ((!(LavaballsToSummon > 0)) && (!LavaBallAssault)))
                                {
                                    //Perform a Fireball Management Action
                                    Debug.Log("Fire Ball Action");

                                    float re = Random.value;
                                    if (((re < .5f)&&(MyHeldLavaBalls.Count < MaxLavaBalls)) || (MyHeldLavaBalls.Count == 0))
                                    {
                                        ToSummonCount += ((int)((1 + Astronaut.AggressionLevel) * (1f + (1f * Random.value)))); //summonFireBallEgg();
                                    } else if(MyHeldLavaBalls.Count > 0)
                                    {
                                        LavaBallAssault = true;
                                        LavaBallAssaultShotInterval = 2f-(1.5f*Astronaut.AggressionLevelF);
                                        LavaBallAssaultMultiple = 1+((int)(Astronaut.AggressionLevel*Random.value));
                                        //Launch 1 at a time at the lowest level
                                        //Launch 4 at a time at the highest level
                                    }
                                    IdleTime = 0f;
                                    ActionTime = Time.time;
                                    //summonFireBallEgg();//..?//dropSleet();

                                }
                                else if ((r < .8f)||(Astronaut.AggressionLevelF > .5f))
                                {
                                    Debug.Log("Repositioning!");
                                    //Leap to a different location
                                    Transform tr = chooseNearestLocation(false);
                                    LeapStartPosition = CurrentLocation.position;
                                    LeapDestination = tr;
                                    LeapEndPosition = tr.position;
                                    LeapTimeValue = 0f;
                                    Leaping = true;
                                    IdleTime = 0f;
                                    ActionTime = Time.time;


                                    

                                }
                                else
                                {
                                    Debug.Log("Meh....");
                                    //Do nothing
                                    ActionTime = Time.time;
                                    //TrudgeDirection = 0;

                                }

                            }

                            if (movement != 0) {
                                this.transform.localScale = new Vector3(Mathf.Abs(this.transform.localScale.x) * -movement, this.transform.localScale.y, this.transform.localScale.z);
                                this.MyAnimator.SetBool("Leaping", true);
                            } else
                            {
                                this.MyAnimator.SetBool("Leaping", false);
                            }
                            MaxLavaBalls = (4*(1+Astronaut.AggressionLevel));
                            float lint = LavaBallAssaultShotInterval;
                            if (LavaBallAssault)
                            {
                                
                                if (MyHeldLavaBalls.Count > 0)
                                {
                                    if ((Time.time - LastLavaballLaunchTime) >= lint)
                                    {
                                        int m = LavaBallAssaultMultiple;
                                        while ((m > 0) && (MyHeldLavaBalls.Count > 0))
                                        {
                                            Lavaball b = MyHeldLavaBalls[0];
                                            Vector2 cr = Random.insideUnitCircle;
                                            Vector3 variance = (new Vector3(cr.x,cr.y,0f)*1f); //Fudge the accuracy so the shots don't go to the same spot
                                            b.Launch(((Astronaut.TheAstronaut.transform.position+(variance)) -  b.transform.position).normalized*10f*(1f+(1f*Astronaut.AggressionLevelF)));
                                            MyHeldLavaBalls.Remove(b);
                                            m--;
                                        }
                                        LastLavaballLaunchTime = Time.time;
                                    }
                                } else
                                {
                                    LavaBallAssault = false;
                                }



                            } else if (ToSummonCount > 0)
                            {
                                if ((Time.time - LastLavaballSummonTime) >= lint)
                                {
                                    ToSummonCount--;
                                    summonFireBallEgg();
                                }
                            }

                            

                            if (!Defeated)
                            {
                                if (Fire_HoldingShield)
                                {
                                    
                                    ShieldGroup.SetActive(true);
                                    NoneGroup.SetActive(false);
                                    if (!FlameBodyAura.isPlaying)
                                    {
                                        FlameBodyAura.Play();
                                    }
                                }
                                else if (!Fire_HoldingShield)
                                {
                                    ShieldGroup.SetActive(false);
                                    NoneGroup.SetActive(true);
                                    if (FlameBodyAura.isPlaying)
                                    {
                                        FlameBodyAura.Stop();
                                    }

                                }
                            }




                            //FireBall Management
                            Lavaball[] arr = MyHeldLavaBalls.ToArray();
                            for(int i = 0; i < arr.Length; i++)
                            {
                                Lavaball lavaball = arr[i];
                                if (lavaball == null) return;

                                Vector3 tpos = getOrbitalPosition(i);
                                float sp = (Time.fixedDeltaTime*.25f * (1f + (2f * Astronaut.AggressionLevelF)));
                                if (lavaball.AcidPullout < 1f)
                                {
                                    tpos = (lavaball.StartPosition + new Vector3(0f, 3f, 0f));
                                    lavaball.AcidPullout = Mathf.Clamp01(lavaball.AcidPullout+sp);
                                    lavaball.transform.position = Vector3.Lerp(lavaball.StartPosition,tpos,Mathf.Clamp01(lavaball.AcidPullout/.5f));
                                } else if (lavaball.Accumulated < 1f)
                                {
                                    lavaball.hatch();
                                    lavaball.Accumulated = Mathf.Clamp01(lavaball.Accumulated + sp);
                                    tpos = Vector3.Lerp(lavaball.transform.position,tpos,Mathf.Pow(((lavaball.Accumulated-.5f)/.5f),3f));
                                    lavaball.transform.position = tpos;
                                    
                                } else
                                {

                                    lavaball.transform.position = Vector3.Lerp(lavaball.transform.position, tpos,.5f);// tpos, );
                                }


                            }

                            if (SparkleCasting)
                            {
                                
                                if ((Time.time - lastsparklecasttime) >= SparkleCastInterval)
                                {
                                    
                                    RaycastHit2D rh = Physics2D.Linecast(new Vector2(this.transform.position.x, this.transform.position.y),new Vector2(sparklecastlocation.x, sparklecastlocation.y), LayerMask.GetMask(new string[] { "Geometry" }));
                                    if ((rh.distance > 0f) && (false))
                                    {
                                        //No collision here. please do not walk over the edge
                                        SparkleCasting = false;
                                    }
                                    else
                                    {
                                        
                                        CombustionField cf = GameObject.Instantiate(CombustionFieldPrefab, sparklecastlocation, CombustionFieldPrefab.transform.rotation);
                                        //cf.Delay = Random.value * .25f;
                                        cf.SparkleTime = (3f*(1f-.75f*(Astronaut.AggressionLevelF)));
                                        lastsparklecasttime = Time.time;
                                        sparklecastlocation = sparklecastlocation + (SparkleCastSpacing * sparklecastdirection);

                                        SparkleCastDistance-=SparkleCastSpacing;
                                        if (SparkleCastDistance <= 0) SparkleCasting = false;
                                    }
                                }

                            }

                            if (Fire_HoldingShield)
                            {
                                if ((Time.time - ShieldPullingTime) >= 1f)
                                {
                                    ShieldGrip = Mathf.Clamp01(ShieldGrip+Time.fixedDeltaTime);
                                    ShieldGripVibration = new Vector3();
                                } else
                                {
                                    if ((Time.time - ShieldPullingTime) < .25f)
                                    {
                                        Vector2 rv = Random.insideUnitCircle.normalized;
                                        ShieldGripVibration = (new Vector3(rv.x,rv.y,0f)*.5f*(1f-ShieldGrip));
                                        
                                    } else
                                    {
                                        ShieldGripVibration = new Vector3();
                                    }
                                    
                                }
                                
                            } else
                            {
                                ShieldGripVibration = new Vector3();
                            }
                            MyAnimator.transform.position = (this.transform.position + bossbodyoffset + downshift + ShieldGripVibration);

                            break;
                        }





















                    case VoidElement.Jungle:
                        {
                            //DEFENSE: Moss Skin (Freeze the Body and Break it off)
                            //REDEFENSE: Grow Skin (Regenerates Health while active)
                            //ABILITY 1: Homing Spores (no slow, dirty)
                            //ABILITY 2: Void Bush Growth (Freeze and Shatter)
                            //ABILITY 3: Bramble Pressure (Freeze and Shatter)



                            string ds = "";

                            float ts = 1.0f * (1f - .5f * Astronaut.AggressionLevelF);
                            if (ReapplyingMossSkin)
                            {

                                ds += "A";
                                if (TimeThreshold(ActionTime, .1f * ts))
                                {


                                }
                                if (TimeThreshold(ActionTime, .5f * ts))
                                {
                                    restoreMossSkin();
                                }
                                if (TimeThresholdAbsolute(ActionTime, .6f * ts))
                                {
                                    ReapplyingMossSkin = false;
                                    ActionTime = Time.time;
                                    ds += "B";
                                }
                                TrudgeDirection = 0;
                            }
                            else if (ThrowingSpear)
                            {
                                ds += "C";
                                if (TimeThreshold(ActionTime, .5f * ts))
                                {
                                    GolemSpear sp = GameObject.Instantiate<GolemSpear>(SpearPrefab, this.transform.position, SpearPrefab.transform.rotation);
                                    MyLaunchedSpear = sp;
                                    Vector3 dif = (plr.transform.position - this.transform.position);

                                    sp.Velocity = (new Vector2(dif.x, dif.y).normalized * (8f * (1f + 2f * Astronaut.AggressionLevelF)));
                                    sp.transform.Rotate(0f, 0f, Vector3.SignedAngle(Vector3.right, new Vector3(sp.Velocity.normalized.x, sp.Velocity.normalized.y, 0f), Vector3.forward));
                                    HasSpear = false;
                                    ds += "D";
                                }

                                if (TimeThresholdAbsolute(ActionTime, 2.5f))
                                {
                                    HasSpear = true;
                                    ThrowingSpear = false;
                                    ActionTime = Time.time;
                                    ds += "E";
                                }
                                TrudgeDirection = 0;
                                ds += "F";
                            }
                            else
                            {
                                ds += "G";
                                if (!Jungle_MossSkinActive && ((Time.time - SkinLostTime) >= (20f * (1f - .75f * Astronaut.AggressionLevelF))))
                                {
                                    ActionTime = Time.time;
                                    ReapplyingMossSkin = true;
                                    ReapplyMossSkinParticles.Play();
                                    ReapplyMossSkinGlow.Play();
                                    //Am.am.oneshot(Am.am.M.MossSkin);
                                    //Am.am.oneshot(Am.am.M.IceSkinFrag);
                                    TrudgeDirection = 0;
                                    ds += "H";
                                }
                                else if (TrudgeDirection == 0)
                                {
                                    if (IdleTime >= (4f * (1f - (0.75f * Astronaut.AggressionLevelF))))
                                    {
                                        TrudgeDirection = ((int)Mathf.Sign((plr.transform.position - this.transform.position).x));
                                        IdleTime = 0f;
                                    }
                                    else
                                    {
                                        IdleTime += Time.fixedDeltaTime;
                                        ds += "I";
                                    }

                                }
                                else if ((Time.time - ActionTime) > (5f * (1f - .5f * Astronaut.AggressionLevelF)))
                                {
                                    ds += "J";
                                    float r = Random.value;
                                    
                                    //Empower Brambles
                                    if ((r < .25f))
                                    {
                                        
                                        ActionTime = Time.time;
                                        foreach (VoidThorns vt in MyVoidThorns)
                                        {
                                            vt.StartTime = Time.time;
                                            vt.Lifetime = (3f * 1f + (4f * Astronaut.AggressionLevelF));
                                            vt.Extending = true;
                                            vt.Health = vt.MaxHealth;
                                        }
                                        IdleTime = 0f;
                                        ds += "K";
                                    }
                                    else if ((r < .5f) && ((Time.time - LastVoidBushTime) >= (60f*(1f-.5f*Astronaut.AggressionLevelF))))
                                    {
                                        //Create a bunch of bushes
                                        TrudgeDirection = 0;
                                        int numbushes = (3+Astronaut.AggressionLevel);
                                        float range = 6f;
                                        for (float f = -1f; f <= 1f; f += (2f / numbushes))
                                        {

                                            RaycastHit2D rh = Physics2D.Linecast(new Vector2(this.transform.position.x+(range*f), this.transform.position.y+2f), new Vector2(this.transform.position.x + (range * f), this.transform.position.y - 4f), LayerMask.GetMask(new string[] { "Geometry" }));
                                            if (rh.distance <= 0f)
                                            {
                                                //skip

                                            }
                                            else
                                            {
                                                VoidBush vb = GameObject.Instantiate<VoidBush>(VoidBushPrefab, rh.point, VoidBushPrefab.transform.rotation);
                                                vb.Lifetime = 15f*(1f+(1f*Astronaut.AggressionLevelF));

                                                //PillarCastCount--;
                                                //if (PillarCastCount <= 0) PillarCasting = false;
                                            }
                                        }
                                        LastVoidBushTime = Time.time;
                                        ActionTime = Time.time;
                                        IdleTime = 0f;
                                        ds += "L";
                                    }
                                    else if (r < .75f)
                                    {
                                        //Drop a bunch of spores
                                        
                                        
                                        TrudgeDirection = 0;
                                        IdleTime = 0f;
                                        ActionTime = Time.time;
                                        releaseSpores();
                                        ds += "M";
                                    }
                                    else
                                    {
                                        //Do nothing
                                        ActionTime = Time.time;
                                        TrudgeDirection = 0;
                                        ds += "N";
                                    }

                                }



                            }

                            

                            

                            if (false)
                            if (ReleasingSpores > 0f)
                            {
                                ds += "Q";
                                if ((Time.time - LastSporeReleaseTime) >= (4f * (1f - (.75f * Astronaut.AggressionLevelF))))
                                {
                                    ds += "R";
                                    LastSporeReleaseTime = Time.time;
                                    releaseSpores();
                                }
                                ReleasingSpores = Mathf.Max(0f, ReleasingSpores - Time.fixedDeltaTime);
                            }

                            










                            if (!Defeated)
                            {
                                if (Jungle_MossSkinActive)
                                {

                                    SkinGroup.SetActive(true);
                                    NoneGroup.SetActive(false);
                                    
                                    if (Health < MaxHealth)
                                    {
                                        //if (!MossAuraParticles.isPlaying)
                                        //{
                                        //  MossAuraParticles.Play();
                                        //}
                                        MossAuraParticles.Emit(1+Astronaut.AggressionLevel);
                                        float hps = ((25f * (1f + (3f * Astronaut.AggressionLevelF)))*Time.fixedDeltaTime);

                                        Health = Mathf.Min(MaxHealth,Health+hps);
                                    } else
                                    {
                                        //if (MossAuraParticles.isPlaying)
                                        //{
                                          //  MossAuraParticles.Stop();
                                        //}
                                    }
                                }
                                else if (!Jungle_MossSkinActive)
                                {
                                    SkinGroup.SetActive(false);
                                    NoneGroup.SetActive(true);

                                    //if (MossAuraParticles.isPlaying)
                                    //{
                                      //  MossAuraParticles.Stop();
                                    //}
                                }
                            }

                            


                                break;
                        }

                }

                if ((Astronaut.TheAstronaut.transform.position - this.transform.position).magnitude < 3f)
                {
                    OnTriggerStay2D(Astronaut.TheAstronaut.MyCollider);
                }

                if ((VoidElementType == VoidElement.Ice)|| (VoidElementType == VoidElement.Jungle))
                {
                    if (TrudgeDirection != 0)
                    {
                        MyRigidbody.velocity = new Vector2(TrudgeDirection * WalkSpeed * (1f + (2f * Astronaut.AggressionLevelF)), MyRigidbody.velocity.y);
                        this.transform.localScale = new Vector3(Mathf.Abs(this.transform.localScale.x) * -TrudgeDirection, this.transform.localScale.y, this.transform.localScale.z);
                        this.MyAnimator.SetBool("Walking", true);
                    }
                    else
                    {
                        MyRigidbody.velocity = new Vector2(0f, MyRigidbody.velocity.y);
                        this.MyAnimator.SetBool("Walking", false);
                    }
                } else if ((VoidElementType == VoidElement.Ice))
                {

                }


            }

        } else
        {
            bool defeatedallminibosses = (Astronaut.TheAstronaut.HasDefeatedVoidIceGolem && Astronaut.TheAstronaut.HasDefeatedVoidFireGolem && Astronaut.TheAstronaut.HasDefeatedVoidJungleGolem);
            BossActive = false;
            if (!defeatedallminibosses)
            if (!Relinquished)
            if ((Time.time - DefeatTime) >= 3f)
            {

                    if (defeatedallminibosses)
                    {
                        

                    }
                    else
                    {
                        plr.PlayerHasControl = true;
                        foreach (Collider2D col in CollidersToDisable)
                        {
                            col.enabled = false;
                        }
                            Astronaut.TheAstronaut.WatchingLocation = null;
                            Relinquished = true;
                        plr.Invulnerable = false;
                        AudioManager.AM.playMusic(AudioManager.AM.VoidPlanetMusic, 1f, 1f, true);
                    }
            }



        }

        float sf = (1f + 1.5f * Astronaut.AggressionLevelF);
        SpeedFactor = Mathf.Lerp(SpeedFactor, sf, .5f);
    }
    public float LastVoidBushTime = -10f;
    public VoidThorns[] MyVoidThorns;
    public ParticleSystem ShieldRemovedEffect;
    public Vector3 ShieldGripVibration = new Vector3();
    public float ShieldGrip = 1f;
        public float ShieldPullingTime = -10f;
    public CombustionField CombustionFieldPrefab;
    private float SpeedFactor = 1f;
    private int MaxLavaBalls = 6;
    private bool LavaBallAssault = false;
    private float LavaBallAssaultShotInterval = .5f;
    private int LavaBallAssaultMultiple = 1;
    private float LastLavaballLaunchTime = -10f;
    public int ToSummonCount = 0;
    public Vector3 getOrbitalPosition(int index)
    {
        float ts = ((((Time.time * SpeedFactor) % 3f)) / 3f);

        
            int si = index;
            float sf = (((float)si) / (float)MaxLavaBalls);
            float rad = 2f;
            float su = (sf + ts);
            return this.transform.position + new Vector3(Mathf.Cos(su * Mathf.PI * 2f) * rad, Mathf.Sin(su * Mathf.PI * 2f) * rad, 0f);
        
    }
    private bool forgedshield = false;
    private bool ForgingShield = false;
    public Transform CurrentLocation;
    
    public Rigidbody2D MyShieldObject;
    private float ShieldForgedTime = -10f;
    public ParticleSystem ShieldBashParticles;
    private bool ShieldBashing = false;
    private Vector3 ShieldBashDestination;
    private bool Leaping = false;
    private float LeapTimeValue;
    private Vector3 LeapStartPosition;
    private Vector3 LeapEndPosition;
    private Transform LeapDestination;
    private bool LeapIntoLava = false;
    private bool InsideLava = false;
    public List<Lavaball> MyHeldLavaBalls = new List<Lavaball>();
    public Transform[] FireZoneLandLocations;
    public Transform[] FireZoneLavaLocations;
    public Transform chooseNearestLocation(bool lavalocations)
    {
        Transform bestleft = null;
        float leftmag = 0f;
        Transform bestright = null;
        float rightmag = 0f;

        foreach (Transform t in (lavalocations ? FireZoneLavaLocations:FireZoneLandLocations))
        {
            if (t == null) continue;
            if (t.Equals(CurrentLocation)) { continue; }
            Vector3 dif = (t.position - this.transform.position);
            if (dif.x >= 0f)
            {
                //to my right
                if ((bestright == null) || (dif.magnitude < rightmag))
                {
                    bestright = t;
                    rightmag = dif.magnitude;
                }
            } else
            {
                //to my left
                if ((bestleft == null) || (dif.magnitude < leftmag))
                {
                    bestleft = t;
                    leftmag = dif.magnitude;
                }
            }

        }

        if ((bestleft != null) &&(bestright != null))
        {
            return ((Random.value<.5f)?bestleft:bestright);
        } else if (bestleft != null)
        {
            return bestleft;
        } else if (bestright != null)
        {
            return bestright;
        }
        return null;
    }
    
    public VoidBush VoidBushPrefab;
    public GameObject VoidBramblePrefab; //Lifetime > 0f
    public Collider2D[] CollidersToDisable;
    private float IdleTime = 0f;
    private float LastSleetDropTime = -10f;
    public void dropSleet()
    {
        Vector3 pos = new Vector3(Astronaut.TheAstronaut.MyCameraRig.position.x, this.transform.position.y,0f)+new Vector3(0f,8f,0f);
        float offy = 2f;
        int lookdirection = (int)Mathf.Sign(this.transform.localScale.x);
        for (float f = -1f; f <= 1f; f += (2f / (2f * (1f + (2f * Astronaut.AggressionLevelF)))))
        {
            IceSkullProjectile p = GameObject.Instantiate<IceSkullProjectile>(SleetProjectile,new Vector3(this.transform.position.x,this.transform.position.y + (offy * f),pos.z),SleetProjectile.transform.rotation);
            p.MyRigidbody.velocity = new Vector2(6f*(1f+(1f*Astronaut.AggressionLevelF))*-lookdirection,((Random.value-.5f)*2f)*(5f*Astronaut.AggressionLevelF));
            //p.MyRigidbody.gravityScale = 1f+(2f*Astronaut.AggressionLevelF);
            p.LifeTime = 6f;
            p.DamageMultiplier = (10f / 30f);
            p.FreezeFactor = 0.00001f;
            p.FreezeTimeMultiplier = 1f + (4f * Astronaut.AggressionLevelF);
        }
            
        
        LastSleetDropTime = Time.time;
    }

    private float LastSporeReleaseTime = -10f;
    private float ReleasingSpores = 0f;
    public JungleTreeProjectile SporeProjectile;
    public void releaseSpores()
    {


        Vector2 dif = (Astronaut.TheAstronaut.MyRigidbody.position - MyRigidbody.position);
        int numprojectiles = ((int)(8f*(1f+Astronaut.AggressionLevelF)));
        for (float f = 0f; f < 1f; f+=(1f/numprojectiles))
        {
            JungleTreeProjectile p = GameObject.Instantiate<JungleTreeProjectile>(SporeProjectile, this.transform.position, SporeProjectile.transform.rotation);
            p.MyRigidbody.velocity = (new Vector2(Mathf.Cos(f*Mathf.PI*2f),Mathf.Sin(f*Mathf.PI*2f))* 6f * (1f + (1f * Astronaut.AggressionLevelF)))+(dif*.5f* (1f * Astronaut.AggressionLevelF));//new Vector2( * -lookdirection, ((Random.value - .5f) * 2f) * (5f * Astronaut.AggressionLevelF));

            //p.MyRigidbody.gravityScale = 1f+(2f*Astronaut.AggressionLevelF);
            //p.Lifetime = 6f;
            
            p.DamageRatio = (.3f / numprojectiles);
            //p.FreezeFactor = 0.00001f;
            
        }


        LastSleetDropTime = Time.time;
    }

    public Lavaball FireBallEggPrefab;
    private float LastLavaballSummonTime = -10f;
    private int LavaballsToSummon = 0;
    public void summonFireBallEgg()
    {
        Transform l = MyAcidPools[(int)Mathf.Clamp((int)(Random.value * MyAcidPools.Length),0, MyAcidPools.Length-1)].transform;
        Debug.Log(l);
        Lavaball p = GameObject.Instantiate<Lavaball>(FireBallEggPrefab, new Vector3(l.position.x, l.position.y, l.position.z), FireBallEggPrefab.transform.rotation);
        p.StartPosition = new Vector3(l.position.x, l.position.y, l.position.z);
        MyHeldLavaBalls.Add(p);
        LastLavaballSummonTime = Time.time;
    }
    private float WalkSpeed = 2f;
    public bool HasSpear = false;
    public bool HasShield = false;
    public GameObject SkinSpearGroup, SkinGroup, SpearGroup, NoneGroup, ShieldGroup;
    public bool TimeThreshold(float starttime,float thresh)
    {
        return (((Time.time - starttime) < thresh) && (((Time.time + Time.fixedDeltaTime) - starttime) >= thresh)); //Threshold

    }
    public bool TimeThresholdAbsolute(float starttime, float thresh)
    {
        return ((Time.time - starttime) > thresh);
    }
    private int MoveDirection = 0;
    private bool Relinquished = false;
    public override void OnDefeated()
    {
        
        //Freeze the player and explode
        //Explode massively
        Astronaut.TheAstronaut.Invulnerable = true;
        Astronaut.TheAstronaut.PlayerHasControl = false;
        Defeated = true;
        DefeatTime = Time.time;
        DefeatSplosion.Play();
        
        //MyDroppedGoalElement.gameObject.SetActive(true);
        MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
        switch (VoidElementType)
        {
            case VoidElement.Ice:
                {
                    AudioManager.AM.crossfade(AudioManager.AM.CurrentMusic, 0f, 4f);
                    if (AudioManager.AM.GolemDestroyed)
                        AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.GolemDestroyed, AudioManager.AM.PlayerAudioMixer, volume: 2f, pitch: 1f, looped: false, destroyafter: 5f);
                    Relinquished = false;
                    MyAnimator.gameObject.SetActive(false);
                    Astronaut.TheAstronaut.HasDefeatedVoidIceGolem = true;
                    explodeIntoPieces();
                    break;
                }
            case VoidElement.Fire:
                {
                    AudioManager.AM.crossfade(AudioManager.AM.CurrentMusic, 0f, 4f);
                    if (AudioManager.AM.GolemDestroyed)
                        AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.GolemDestroyed, AudioManager.AM.PlayerAudioMixer, volume: 2f, pitch: 1f, looped: false, destroyafter: 5f);
                    Relinquished = false;
                    Astronaut.TheAstronaut.HasDefeatedVoidFireGolem = true;
                    MyAnimator.gameObject.SetActive(false);
                    if (FlameBodyAura.isPlaying)
                    {
                        FlameBodyAura.Stop();
                    }
                    MyShieldBashCollider.enabled = false;
                    Lavaball[] lblist = MyHeldLavaBalls.ToArray();
                    for (int i = 0; i < lblist.Length; i++)
                    {
                        if (lblist[i] == null) continue;
                        GameObject.Destroy(lblist[i].gameObject);
                    }
                    explodeIntoPieces();
                    break;
                }
            case VoidElement.Jungle:
                {
                    AudioManager.AM.crossfade(AudioManager.AM.CurrentMusic, 0f, 4f);
                    if (AudioManager.AM.GolemDestroyed)
                        AudioManager.AM.playGeneralSoundOneShot(AudioManager.AM.GolemDestroyed, AudioManager.AM.PlayerAudioMixer, volume: 2f, pitch: 1f, looped: false, destroyafter: 5f);
                    Astronaut.TheAstronaut.HasDefeatedVoidJungleGolem = true;
                    Relinquished = false;
                    MyAnimator.gameObject.SetActive(false);
                    explodeIntoPieces();
                    break;
                }
        }
        Astronaut.TheAstronaut.WatchingLocation = this.transform;
        bool defeatedall = (Astronaut.TheAstronaut.HasDefeatedVoidIceGolem && Astronaut.TheAstronaut.HasDefeatedVoidFireGolem && Astronaut.TheAstronaut.HasDefeatedVoidJungleGolem);
        if (defeatedall)
        {
            Astronaut.TheAstronaut.finishVoidStage(this.transform.position);
        }

    }
    public Collider2D MyShieldBashCollider;
    public ParticleSystem MossAuraParticles;
    private float mylastDamageTakenTime = -10f;
    private Vector3 bossbodyoffset, downshift;
    public override void TakeDamage(float dmg, Vector2 dir)
    {
        if (Defeated) return;
        if (Ice_FreezingSkinActive)
        {
            dmg = (dmg * .05f);
        }
        if (Fire_HoldingShield)
        {
            FlameBodyAura.Emit((int)(dmg/5));
            return;
        }
        if (Jungle_MossSkinActive)
        {
            dmg = (dmg * .5f);
            
        }


        float hp = (Health - dmg);

        if (hp <= 0f)
        {
            Kill();
        }
        else
        {
            Health = hp;
            mylastDamageTakenTime = Time.time;
        }


    }
    public override void Kill()
    {
        if (Defeated) return;
        Defeated = true;
        Astronaut.TheAstronaut.MyBossGolem = null;
        OnDefeated();


    }
    private bool exploded = false;
    public Rigidbody2D ExplodedPiecePrefab;
    public Rigidbody2D[] MyExplodedPieces;
    public void explodeIntoPieces()
    {
        if (exploded) return;
        exploded = true;
        SpriteRenderer[] rs = this.gameObject.GetComponentsInChildren<SpriteRenderer>();
        MyAnimator.enabled = false;
        foreach (Rigidbody2D exp in MyExplodedPieces)
        {
            
            
                //Rigidbody2D exp = GameObject.Instantiate(ExplodedPiecePrefab, s.transform.position, s.transform.rotation).GetComponent<Rigidbody2D>();
                

            //SpriteRenderer dp = GameObject.Instantiate(s, exp.transform).GetComponent<SpriteRenderer>();
            //dp.enabled = true;
            //dp.color = new Color(dp.color.r * .25f, dp.color.g * .25f, dp.color.b * .25f, dp.color.a);
            exp.transform.SetParent(null);
            exp.GetComponent<shrinkovertime>().StartTime = Time.time;
            exp.gameObject.SetActive(true);

            Vector2 rv = Random.insideUnitCircle;

            exp.velocity = (new Vector2(rv.x, rv.y + 1f) * 20f);
            exp.angularVelocity = (((Random.value - .5f) / .5f) * 20f);

            /*
            foreach (Transform t in dp.GetComponentsInChildren<Transform>())
                {
                    if ((t.gameObject.Equals(exp.gameObject)) || (t.gameObject.Equals(dp.gameObject))) continue;
                    GameObject.Destroy(t.gameObject);
                }
            */
            GameObject.Destroy(exp.gameObject, 10f);
            
        }

    }
    public ParticleSystem BlazingShieldBashParticles;
    public ParticleSystem FlameBodyAura;
    public Lava[] MyAcidPools;
    private float TouchTime = -10f;
    private void OnTriggerStay2D(Collider2D collision)
    {
        Astronaut a = collision.GetComponent<Astronaut>();
        if ((a != null) && ((Time.time - TouchTime) >= .5f))
        {
            TouchTime = Time.time;
            switch (VoidElementType)
            {
                case VoidElement.Ice:
                    {
                        if (Ice_FreezingSkinActive)
                        {
                            if ((a.Alive)&&(!a.Frozen)&& ((Time.time - a.UnfreezeTime) >= 1.5f))
                            {

                                if (!a.TakeDamage(10f,new Vector3(Mathf.Sign((a.transform.position-this.transform.position).normalized.x),a.JumpSpeed/8f,0f)))
                                {
                                    a.freeze(1.0f*(1f+3f*Astronaut.AggressionLevelF));
                                }
                                
                            }
                        }
                        break;
                    }
                case VoidElement.Fire:
                    {
                        if (Fire_HoldingShield)
                        {
                            if (ShieldBashing)
                            {
                                Debug.Log("BASH");
                                if (a.TakeDamage(25f, new Vector3(Mathf.Sign((LeapEndPosition - LeapStartPosition).normalized.x)*8f*(1f+(1f*Astronaut.AggressionLevelF)), a.JumpSpeed / 8f, 0f)))
                                {
                                    Am.am.oneshot(Am.am.M.LavaBurn);
                                }
                            }
                        }
                        break;
                    }
                case VoidElement.Jungle:
                    {
                        break;
                    }
            }
        }


    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerStay2D(collision);
    }




}
