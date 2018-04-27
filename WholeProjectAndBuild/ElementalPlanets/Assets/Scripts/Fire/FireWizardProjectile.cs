using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWizardProjectile : EnemyProjectile {


    // Use this for initialization
    private float StartTime = -10f;
    public Enemy_Fire_Wizard.AttackType MyAttackType = Enemy_Fire_Wizard.AttackType.Comb;
    public const float FormulationTime = .5f;
    public Enemy_Fire_Wizard MyWizard = null;
    void Start()
    {
        Live = true;
        StartTime = Time.time;
        LaunchTime = -10f;
        Launched = false;
        origscale = MySpriteRenderer.transform.localScale;
        
        OnParticleEffectLevelChanged();
    }
    bool prevparticlelowlevel = false;
    public void OnParticleEffectLevelChanged()
    {
        if (prevparticlelowlevel != GameManager.TheGameManager.UsingLowParticleEffects)
        {
            //*((!prevparticlelowlevel)?(1f/2f):2f)
            //reduce or restore particle emission/duration/size
            //
            ParticleSystem.EmissionModule e = ParticleTrail.emission;
            e.rateOverTimeMultiplier = (e.rateOverTimeMultiplier * ((!prevparticlelowlevel) ? (1f / 4f) : 4f));
            e.rateOverDistanceMultiplier = (e.rateOverDistanceMultiplier * ((!prevparticlelowlevel) ? (1f / 4f) : 4f));

            e = ParticleExplosion.emission;
            e.rateOverTimeMultiplier = (e.rateOverTimeMultiplier * ((!prevparticlelowlevel) ? (1f / 4f) : 4f));
            

            prevparticlelowlevel = GameManager.TheGameManager.UsingLowParticleEffects;
        }

    }
    // Update is called once per frame
    private Vector3 origscale;
    public AnimationCurve FormulationCurve;
    public Vector3 CorkscrewVector;
    public Vector3 CastPosition;
    void Update()
    {
        
            if (!Launched)
            {

                if ((Time.time - StartTime) >= FormulationTime)
                {
                    if (!Accelerating)
                    {
                        MyRigidbody.velocity = (LaunchDirection.normalized * VELOCITYCONSTANT*VelocityMultiplier);
                    }
                if ((MyWizard != null) && (MyWizard.gameObject.activeInHierarchy))
                if (MyWizard.isStunned() || MyWizard.Frozen)
                {
                    Remove();
                }
                else
                {
                    Launched = true;
                    LaunchTime = Time.time;
                    MySpriteRenderer.color = Color.white;
                    MySpriteRenderer.transform.localScale = origscale;
                    ParticleTrail.Play();

                        Am.am.oneshot(Am.am.M.WizardFireBallLaunch);
                        
                    }
                CastLine.enabled = false;


                } else
                {
                    float sc = ((Time.time - StartTime) / FormulationTime);
                    sc = FormulationCurve.Evaluate(sc);
                    MySpriteRenderer.transform.localScale = Vector3.Lerp(origscale * 4f,Vector3.one,sc);
                    MySpriteRenderer.color = Color.Lerp(new Color(1f,1f,1f,0f),Color.white,sc);
                CastLine.SetPosition(0, Vector3.Lerp(CastPosition, this.transform.position,sc));
                CastLine.SetPosition(1, this.transform.position);

            }

            
            } else
        {
            if ((Time.time - LaunchTime) >= 5f)
            {

                explode();
            }
        }
    }
    
    public bool Launched = false;
    private float phasedestroying = 0f;
    private float LaunchTime = -10f;
    public Vector3 LaunchDirection = new Vector3();
    public const float VELOCITYCONSTANT = 7f;
    public float VelocityMultiplier = 1f;
    public float TurnRateMultiplier = 1f;
    public bool Accelerating = false,Homing = false,Corkscrewing = false;
    public Transform HomingTarget = null;
    public SpriteRenderer MySpriteRenderer;
    public const float CORKSCREWFREQUENCY = 1.5f;
    public const float HOMINGTURNRATE = (1f/3f);
    public LineRenderer CastLine;
    private void FixedUpdate()
    {
        if (Live)
        {
            
            phasecollision = false;
            if (Launched)
            {
                switch(MyAttackType)
                {
                    case Enemy_Fire_Wizard.AttackType.last: { break; }
                    case Enemy_Fire_Wizard.AttackType.BurstShot: { break; }
                    case Enemy_Fire_Wizard.AttackType.Comb: { break; }
                    case Enemy_Fire_Wizard.AttackType.Corkscrew: {
                            float ang = (Mathf.Atan2(CorkscrewVector.y,CorkscrewVector.x)+(Time.fixedDeltaTime*CORKSCREWFREQUENCY*Mathf.PI*2f));
                            Vector3 newcv = (new Vector3(Mathf.Cos(ang), Mathf.Sin(ang), 0f) * CorkscrewVector.magnitude);
                            Vector3 dif = (newcv-CorkscrewVector);
                            MyRigidbody.transform.position = (MyRigidbody.transform.position + dif);
                            CorkscrewVector = newcv;
                            break; }
                    case Enemy_Fire_Wizard.AttackType.Homing: {
                            Vector3 vel = new Vector3(MyRigidbody.velocity.x, MyRigidbody.velocity.y, 0f);
                            if (!Astronaut.TheAstronaut.Quelling) {
                                Vector3 dif = (HomingTarget.position - this.transform.position);
                                Vector3 newvel = Vector3.RotateTowards(vel.normalized * vel.magnitude, dif.normalized * vel.magnitude, (Mathf.PI * 2f * HOMINGTURNRATE * TurnRateMultiplier * Time.fixedDeltaTime), 0f);

                                MyRigidbody.velocity = new Vector2(newvel.x, newvel.y);
                            } else
                            {
                                MyRigidbody.velocity = new Vector2(vel.x, vel.y);
                            }

                            break; }
                    case Enemy_Fire_Wizard.AttackType.Spread: { break; }
                    case Enemy_Fire_Wizard.AttackType.Surround: { break; }

                }
                if (phasecollision)
                {
                    phasedestroying = (Time.fixedDeltaTime * 1.0f);
                    if (phasedestroying >= 1f)
                    {
                        explode();
                    }
                }
            } 
        } else
        {

        }

        
    }
    public Rigidbody2D MyRigidbody;
    public ParticleSystem ParticleTrail, ParticleExplosion, NegativeParticles;
    public void Remove()
    {

        ParticleExplosion.transform.SetParent(null);
        //ParticleExplosion.Stop();
        GameObject.Destroy(ParticleExplosion.gameObject, 5f);

        ParticleTrail.transform.SetParent(null);
        ParticleTrail.Stop();
        NegativeParticles.Stop();
        NegativeParticles.transform.SetParent(null);
        GameObject.Destroy(NegativeParticles,5f);
        GameObject.Destroy(ParticleTrail.gameObject, 5f);

        GameObject.Destroy(this.gameObject);
    }
    public void explode()
    {
        if (!Live) return;
        Live = false;

        ParticleExplosion.Play(true);
        Remove();
    }
    
    private bool phasecollision = false;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if ((!Live || !Launched)) return;
        Astronaut plr = collision.GetComponent<Astronaut>();

        if ((plr != null) && (plr.Alive) && (!plr.Invulnerable) && (Live))
        {

            Am.am.oneshot(Am.am.M.FireballHit);
            Am.am.oneshot(Am.am.M.LavaBurn);
            Vector3 dif = (plr.transform.position - this.transform.position);
            plr.TakeDamage(10f, Vector3.zero);//dif.normalized * .2f + new Vector3(0f, plr.JumpSpeed / 4f, 0f)
            if (plr.Alive)
            {
                //plr.burn(2f);

            }

            explode();
        }
        else if ((collision.gameObject.GetComponent<IceBlock>() != null)|| (collision.gameObject.GetComponent<IcePillar>() != null))
        {
            Am.am.oneshot(Am.am.M.PillarHit);
            
            explode();
        }
    }
}
