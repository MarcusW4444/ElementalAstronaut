using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vines2 : MonoBehaviour {

    // Use this for initialization
    public LineRenderer MyLineRenderer;
    public Astronaut MyAstronaut;
    public float TravelDistance = 0f;
    public float MaxTravelDistance = 10f;
    public float TravelSpeed = 30f;
    public float RetractionRate = 80f;
    private Vector3 startPosition=new Vector3();
    public Vector3 VinePosition;
    public Vector3 VineAnchorOffset;
    public Vector3 TravelDirection;
    public GameObject VineAttachedToSolid=null;
    public GenericEnemy VineAttachedToEnemy = null;
    public VoidGolem VineAttachedToGolem = null;
    public bool Sustaining = true;
    public bool Attached = false;
    public bool Retracting = false;
    public int VitaPowerLevel = 0;

    public AudioSource VineGrapplingSound, LeechLoop;
    void Start () {
        TravelDistance = 0f;
        startPosition = this.transform.position;
        VinePosition = this.transform.position;
        VineAttachedToSolid = null;
        VineAttachedToEnemy = null;
        VineAttachedToGolem = null;
        VineGrapplingSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.JungleElementPower, AudioManager.AM.PlayerAudioMixer, .8f, 1f, true);
        
        LeechLoop = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.LeechLoop, AudioManager.AM.PlayerAudioMixer, 0f, 1f, true);
        LeechLoop.Play();


    }

    // Update is called once per frame

    public bool GrappleOnWalls = false;
    public bool ControlPush = false;
    public bool ControlHold = false;
    public bool PushesEnemies = false;
    public bool ConstrictsEnemies = false;
    public bool EnergyDrain = false;
    public bool ClusterConstriction = false;
    public Vector3 HoldLocation = new Vector3();
    public Vines2 ClusterVineParent = null;
    public List<Vines2> MyClusterVineChildren = new List<Vines2>();
    private Vector3 lastvictimpressposition = new Vector3();
	void FixedUpdate () {


        if (Retracting)
        {


            TravelDistance = Mathf.Max(0f, TravelDistance- (RetractionRate * Time.deltaTime));

            if (TravelDistance <= 0f)
            {
                GameObject.Destroy(this.gameObject);
                return;
            }

            VinePosition = Vector3.Lerp(MyAstronaut.transform.position,VinePosition, (TravelDistance / MaxTravelDistance));
            Attached = false;
        } else
        {
            
            if (Attached)
            {
                //Retracting = true;

                if (VineAttachedToSolid != null)
                {
                    Vector3 dif = (VineAnchorOffset - MyAstronaut.transform.position);
                    //MyAstronaut.MyRigidbody.AddForce(dif * MyAstronaut.MyRigidbody.velocity.magnitude * MyAstronaut.MyRigidbody.mass * Vector3.Dot(MyAstronaut.MyRigidbody.velocity.normalized,-dif.normalized) *1f); 

                    float towards = Vector3.Dot(MyAstronaut.MyRigidbody.velocity.normalized, -dif.normalized);
                    //if (dif.y > 0f)
                    MyAstronaut.MyRigidbody.AddForce(dif.normalized * (50f) * (.25f + 1.75f * Astronaut.JunglePowerFactor) * MyAstronaut.MyRigidbody.mass);
                    //if (dif.y > 0f)
                    float dun = ((Vector3.Dot(dif.normalized, Vector3.up) + 1f) / 2f);
                    VineGrapplingSound.volume = .25f + (.75f * dun);
                    MyAstronaut.MyRigidbody.AddForce((dun) * Vector2.up * MyAstronaut.MyRigidbody.mass * 70f * (1f / (0.1f + Mathf.Max(Mathf.Abs(dif.x), 0.1f))) * (.25f + 1.75f * Astronaut.JunglePowerFactor));
                    float speed = MyAstronaut.MyRigidbody.velocity.magnitude;

                    //float closedist = 5f;

                    //Pull Up
                    //Vector3 cross = Vector3.Cross(dif.normalized,Vector3.forward);
                    //MyAstronaut.MyRigidbody.AddForce(MyAstronaut.MyRigidbody.mass * speed*10f*Vector3.Lerp(Vector3.up ,dif.normalized,Mathf.Clamp01((dif.magnitude)/closedist)));
                    VinePosition = VineAnchorOffset;

                    //How high in what sense? Units? 
                    //Maybe you could have high cliffs inside the ice caves?
                    //I think he should. Let me add that..

                } else if ((VineAttachedToEnemy != null)) {

                    if ((VineAttachedToEnemy.Alive) && (!VineAttachedToEnemy.AnchoredToVine))
                    {
                        Vector3 dif = (VineAttachedToEnemy.transform.position - MyAstronaut.transform.position);
                        VinePosition = (VineAttachedToEnemy.transform.position + VineAnchorOffset);
                        //?
                        //MyAstronaut.MyRigidbody.AddForce(dif * MyAstronaut.MyRigidbody.mass * 10f);//Pull yourself towards an enemy
                        float stopthresh = 2f;
                        float delt = (Time.fixedDeltaTime * 10f * (4f * Astronaut.JunglePowerFactor));
                        float pushdps = 100f;
                        VineGrapplingSound.volume = 1f;

                        if (ControlHold)
                        {
                            Vector3 difpress = (lastvictimpressposition - VineAttachedToEnemy.transform.position);
                            if (ControlPush)
                            {
                                float d = (difpress.magnitude / (5f * Time.fixedDeltaTime));
                                lastvictimpressposition = VineAttachedToEnemy.transform.position;
                                if (d < 1f)
                                {

                                    VineAttachedToEnemy.TakeDamage(Time.fixedDeltaTime * pushdps * (1f - d) * (Astronaut.JunglePowerFactor), new Vector2());
                                }
                            }
                            
                            dif = (VineAttachedToEnemy.transform.position - HoldLocation);
                            if (dif.magnitude >= stopthresh)
                                VineAttachedToEnemy.MyRigidbody.MovePosition(VineAttachedToEnemy.MyRigidbody.position + (-new Vector2(dif.x, dif.y).normalized * delt));
                            else
                                VineAttachedToEnemy.MyRigidbody.MovePosition(VineAttachedToEnemy.MyRigidbody.position + (-new Vector2(dif.x, dif.y).normalized * delt * (1f - (dif.magnitude / stopthresh))));

                        } else if (ControlPush)
                        {
                            VineAttachedToEnemy.MyRigidbody.AddForce(dif * VineAttachedToEnemy.MyRigidbody.mass * (2f * Astronaut.JunglePowerFactor) * 10f);//Pull the enemy towards you
                            Vector3 difpress = (lastvictimpressposition - VineAttachedToEnemy.transform.position);
                            float d = (difpress.magnitude / (5f * Time.fixedDeltaTime));
                            lastvictimpressposition = VineAttachedToEnemy.transform.position;
                            if (d < 1f)
                            {

                                VineAttachedToEnemy.TakeDamage(Time.fixedDeltaTime * pushdps * (1f - d), new Vector2());
                            }

                        }
                        else
                        {
                            if (dif.magnitude < (stopthresh + delt))
                            {
                                //VineAttachedToEnemy.MyRigidbody.AddForce(-VineAttachedToEnemy.MyRigidbody.velocity * VineAttachedToEnemy.MyRigidbody.mass * 10f);
                                if (dif.magnitude > stopthresh)
                                    VineAttachedToEnemy.MyRigidbody.MovePosition(VineAttachedToEnemy.MyRigidbody.position + (-new Vector2(dif.x, dif.y).normalized * (dif.magnitude - stopthresh)));
                            }
                            else
                            {
                                //VineAttachedToEnemy.MyRigidbody.AddForce(-dif * VineAttachedToEnemy.MyRigidbody.mass * 10f);//Pull the enemy towards you
                                VineAttachedToEnemy.MyRigidbody.MovePosition(VineAttachedToEnemy.MyRigidbody.position + (-new Vector2(dif.x, dif.y).normalized * delt));
                            }
                        }
                        if (Astronaut.JunglePowerFactor > 0f)
                            VineAttachedToEnemy.StunTime = Mathf.Max(VineAttachedToEnemy.StunTime, Time.time + (.4f * Astronaut.JunglePowerFactor));
                        if (Astronaut.JunglePowerFactor > 0f)
                            if (EnergyDrain)
                        {
                            if (MyAstronaut != null)
                                if (MyAstronaut.Alive)
                                {
                                    if (MyAstronaut.Health < 100f)
                                    {
                                        MyAstronaut.Health = (MyAstronaut.Health + ((100f - MyAstronaut.Health) / 100f) * (400f * Astronaut.JunglePowerFactor) * Time.fixedDeltaTime);
                                        LeechLoop.pitch = .75f + (.25f * (MyAstronaut.Health / 100f));
                                        Debug.Log("Leech");
                                        if (!LeechLoop.isPlaying)
                                            LeechLoop.Play();
                                        LeechLoop.volume = Mathf.Lerp(LeechLoop.volume, 1f , 0.75f);//* Astronaut.JunglePowerFactor
                                       if (MyLeechParticles != null)
                                        {// Change the color based on the amount of health between us
                                                ParticleSystem ml = VineAttachedToEnemy.mydrainparticles;

                                                if (ml == null)
                                                {
                                                    ml = GameObject.Instantiate<ParticleSystem>(MyLeechParticles,VineAttachedToEnemy.transform);
                                                    VineAttachedToEnemy.mydrainparticles = ml;
                                                }
                                                ParticleSystem.MainModule mm = ml.main;
                                                mm.startColor = Color.Lerp(new Color(1f,.5f,0f),Color.yellow, (VineAttachedToEnemy.Health/ VineAttachedToEnemy.MaxHealth));
                                            ml.transform.position = VineAttachedToEnemy.transform.position;
                                            ml.Emit(1);
                                            

                                        }
                                        if (MyAbsorbParticles != null)
                                        {
                                                ParticleSystem ma = MyAstronaut.myabsorbparticles;
                                                if (ma == null)
                                                {
                                                    ma = GameObject.Instantiate<ParticleSystem>(MyAbsorbParticles, MyAstronaut.transform);
                                                    MyAstronaut.myabsorbparticles = ma;
                                                }
                                                ParticleSystem.MainModule mm = ma.main;
                                                mm.startColor = Color.Lerp(Color.red, Color.green, (MyAstronaut.Health / 100f));
                                                ma.transform.position = MyAstronaut.transform.position;
                                            ma.Emit(1);
                                        }

                                        Am.am.M.crossfade(LeechLoop, 0f, .5f);

                                    }
                                    else
                                    {
                                        //LeechLoop.volume = Mathf.Lerp(LeechLoop.volume, 0f, 0.5f);

                                    }


                                }
                        }
                        VineAttachedToEnemy.VineStrangle = true;
                    } else
                    {
                        Retracting = true;
                        Sustaining = false;
                        Attached = false;
                    }
                } else if (VineAttachedToGolem != null) {
                    if ((VineAttachedToGolem.VoidElementType == VoidGolem.VoidElement.Fire) && (VineAttachedToGolem.Fire_HoldingShield))
                    {
                        float ve = (VineAttachedToGolem.ShieldGrip - (Time.fixedDeltaTime*4f*(1f-(.5f*Astronaut.AggressionLevelF))));
                        if (ve <= 0f) {
                            //Instantiate a shield
                            VineAttachedToGolem.Fire_HoldingShield = false;
                            Rigidbody2D rb = GameObject.Instantiate<Rigidbody2D>(VineAttachedToGolem.MyShieldObject,VineAttachedToGolem.transform.position,VineAttachedToGolem.MyShieldObject.transform.rotation);
                            Vector3 dif = (startPosition - VinePosition).normalized;
                            rb.velocity = (new Vector2(dif.x, dif.y).normalized * -5f);
                            rb.angularVelocity = Random.Range(-360f, 360f);
                            
                            VineAttachedToGolem.ShieldRemovedEffect.Play(true);
                            VineAttachedToGolem.ShieldPullingTime = Time.time;
                            VineAttachedToGolem.ShieldGrip = 0f;
                            //Am.am.oneshot
                        } else
                        {
                            VineAttachedToGolem.ShieldPullingTime = Time.time;
                            VineAttachedToGolem.ShieldGrip = ve;
                        }
                        VinePosition = (VineAttachedToGolem.transform.position + VineAnchorOffset);
                    } else
                    {
                        Retracting = false;
                        Attached = false;
                        VineAttachedToGolem = null;
                    }
                } else
                {
                    Retracting = false;

                }
                if (!Sustaining)
                {
                    MyAstronaut.UsedDoubleJump = false;
                    Retracting = true;
                    Attached = false;
                    
                    VineAttachedToSolid = null;
                    if (VineAttachedToEnemy != null) VineAttachedToEnemy.VineStrangle = false;
                    VineAttachedToEnemy = null;
                }

            }
            else
            {

                float delta = (TravelSpeed*(.5f+4f*Astronaut.JunglePowerFactor) * Time.deltaTime);
                float d = (TravelDistance + delta);
                bool ending = false;
                
                if (d >= (MaxTravelDistance*(.25f+ (1.75f * Astronaut.JunglePowerFactor))))
                {
                    d = (MaxTravelDistance - TravelDistance);
                    ending = true;
                }
                else
                {
                    d = delta;
                }
                TravelDistance = (TravelDistance + d);


                Ray2D r = new Ray2D(VinePosition, TravelDirection.normalized);

                
                RaycastHit2D rh = Physics2D.Raycast(r.origin, r.direction, d, LayerMask.GetMask(new string[] { "Geometry","Enemy","Boss"}));
                if (rh.collider != null)
                {
                    GenericEnemy en = rh.collider.gameObject.GetComponentInParent<GenericEnemy>();
                    VoidGolem bo = rh.collider.gameObject.GetComponentInParent<VoidGolem>();
                    if ((en != null) &&(en.Alive))
                    {
                        Attached = true;
                        VineAnchorOffset = (new Vector3(rh.point.x, rh.point.y, 0f) - en.transform.position);
                        VineAttachedToEnemy = en;
                        lastvictimpressposition = en.transform.position;
                        VineAttachedToSolid = null;
                    }
                    else if ((bo != null) && (!bo.Defeated))
                    {
                        Attached = true;
                        VineAnchorOffset = (new Vector3(rh.point.x, rh.point.y, 0f) - bo.transform.position);
                        VineAttachedToEnemy = null;
                        VineAttachedToGolem = bo;
                        //lastvictimpressposition = en.transform.position;
                        VineAttachedToSolid = null;
                    } else 
                    {
                        
                        Attached = true;
                        VineAnchorOffset = (new Vector3(rh.point.x, rh.point.y, 0f));

                        VineAttachedToSolid = rh.collider.gameObject;
                        VineAttachedToEnemy = null;
                    }


                }
                else
                {
                    //Miss.
                    if (!Sustaining)
                    {
                        Retracting = true;
                    }
                }

                if ((!Attached) && (ending))
                {
                    Retracting = true;
                    
                }

                
                
                VinePosition = startPosition + (TravelDirection * TravelDistance);
            }
        }
        
        
        
        if (MyAstronaut != null)
        {
            MyLineRenderer.SetPosition(1, MyAstronaut.transform.position);
        }
        MyLineRenderer.SetPosition(0, VinePosition);
        if ((Attached) &&(MyAstronaut != null) && (MyAstronaut.Alive))
        {
            if (!VineGrapplingSound.isPlaying)VineGrapplingSound.Play();
        } else
        {
            if (VineGrapplingSound.isPlaying) VineGrapplingSound.Stop();
        }
        lastattached = Attached;
    }
    public ParticleSystem MyAbsorbParticles, MyLeechParticles;
    bool lastattached = false;
}
