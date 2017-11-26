using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Fire_Wizard : GenericEnemy {

    // Use this for initialization
    public enum State {None,Waiting,Attacking, FollowingThrough };
    public enum AttackType { Homing,Comb,Spread,Surround,BurstShot,Corkscrew,Rain,last} //Curse
    public State MyState = State.Waiting;
    public AttackType MyAttackType = AttackType.Spread;
    private float lastAttackChangeTime = -10f;
    private int attackUsages = 0;
	void Start () {
        StartPosition = this.transform.position;
        StateTime = Time.time;
        setState(State.Waiting, .5f + (1f * Random.value));

    }
    public void chooseAttack()
    {
        MyAttackType = ((AttackType)((int)(((int)AttackType.last)*Random.value)));
        //Debug.Log("Wizard Attack: "+MyAttackType.ToString());
        lastAttackChangeTime = Time.time;
        attackUsages = 0;
    }
    private Vector3 StartPosition;
    //private bool GoLeftGoRight;

    // Update is called once per frame
    public FireWizardProjectile ProjectilePrefab;
    
    //public ParticleSystem ShootWindUpGlow,ShootFlash;
    //public const float MoveSpeed = 5f;
    private float StateTime = -10f;
    private float StateDuration = 1f;
    public Animator MyAnimator;
    void Update()
    {

        if (Alive)
        {
            hitdirection = (hitdirection * (1f - Time.deltaTime));
            
        }
    }
    public const float ATTACKRATE = 1.0f;
    void FixedUpdate()
    {
        Astronaut plr = Astronaut.TheAstronaut;

        if (Alive && !isStunned())
        {
            bool stateexpired = (Time.time >= (StateTime + StateDuration));

            switch (MyState)
            {
                case State.None: { break; }
                case State.Waiting:
                    {

                        if (stateexpired)
                        {
                            bool ch = false;

                            if ((plr != null) && (plr.Alive))
                            {


                                Vector3 dif = (plr.transform.position - this.transform.position);
                                //Debug.Log("Mag: " + dif.magnitude);
                                if (dif.magnitude < 8f)
                                {
                                    RaycastHit2D rh = Physics2D.Linecast(this.transform.position, plr.transform.position, LayerMask.GetMask(new string[] { "Geometry" }));

                                    if (rh.distance <= 0f)
                                    {

                                        
                                            ch = true;
                                            setState(State.Attacking, ATTACKRATE * (1f - (.6f * Astronaut.AggressionLevelF)));

                                        //Debug.Log("Visible");
                                    }
                                    else
                                    {
                                        ch = true;
                                        if (dif.magnitude < 5f)
                                        {
                                            ch = true;
                                            setState(State.Attacking, ATTACKRATE*(1f-(.6f*Astronaut.AggressionLevelF)));

                                        }
                                    }

                                    if ((plr != null) && (plr.Alive))
                                    {
                                        //Vector3 dif = (plr.transform.position - this.transform.position);
                                        AimDirection = dif.normalized;

                                    }
                                    MySpriteRenderer.flipX = (Mathf.Sign(-AimDirection.x) < 0f);
                                }
                            } 
                            if (!ch)
                                setState(State.Waiting, .5f);
                        }
                            break;
                    }
                
                case State.Attacking:
                    {
                        if (!Attacked)
                        {
                            attackUsages++;
                            Attacked = true;
                            int pcount = (3+Astronaut.AggressionLevel);
                            switch (MyAttackType)
                            {
                                case AttackType.last: { break; }
                                case AttackType.BurstShot: {
                                        for (int i = 0; i < pcount; i++)
                                        {
                                            FireWizardProjectile p = GameObject.Instantiate(ProjectilePrefab, this.transform.position + AimDirection, new Quaternion()).GetComponent<FireWizardProjectile>();
                                            p.LaunchDirection = AimDirection;
                                            p.VelocityMultiplier = 2f*(0.5f+0.5f*(((float)(i + 1)) / (pcount)));
                                            p.VelocityMultiplier *= (1f+(1f*Astronaut.AggressionLevelF));
                                            p.Accelerating = false;
                                            p.Homing = false;
                                            p.CastPosition = this.transform.position;
                                            p.MyAttackType = this.MyAttackType;
                                        }

                                        break; }
                                case AttackType.Comb: {
                                        //Cross product
                                        Vector3 cross = Vector3.Cross(AimDirection.normalized,Vector3.forward);
                                        for (int i = 0; i < pcount; i++)
                                        {
                                            FireWizardProjectile p = GameObject.Instantiate(ProjectilePrefab, (this.transform.position - AimDirection)+(-cross) +(cross*2f*((float)i/3f)), new Quaternion()).GetComponent<FireWizardProjectile>();
                                            p.LaunchDirection = AimDirection;
                                            //p.VelocityMultiplier = (0.5f + 0.5f * (((float)(i + 1)) / 4f));
                                            p.VelocityMultiplier *= (1f + (1f * Astronaut.AggressionLevelF));
                                            p.Accelerating = false;
                                            p.Homing = false;
                                            p.CastPosition = this.transform.position;
                                            p.MyAttackType = this.MyAttackType;
                                        }
                                        break; }
                                case AttackType.Corkscrew:
                                    {
                                        //Vector3 cross = Vector3.Cross(AimDirection.normalized, Vector3.forward);
                                        for (int i = 0; i < pcount; i++)
                                        {
                                            float sd = ((float)i / pcount);

                                            Vector3 corkoffset = (new Vector3(Mathf.Cos(sd * Mathf.PI * 2f), Mathf.Sin(sd * Mathf.PI * 2f), 0f) * .5f);

                                            FireWizardProjectile p = GameObject.Instantiate(ProjectilePrefab, (this.transform.position + corkoffset), new Quaternion()).GetComponent<FireWizardProjectile>();
                                            p.LaunchDirection = AimDirection;
                                            p.CorkscrewVector = corkoffset;
                                            //p.VelocityMultiplier = (0.5f + 0.5f * (((float)(i + 1)) / 4f));
                                            p.VelocityMultiplier *= (1f + (1f * Astronaut.AggressionLevelF));
                                            p.Accelerating = false;
                                            p.CastPosition = this.transform.position;
                                            p.Homing = false;
                                            p.MyAttackType = this.MyAttackType;
                                        }
                                        break;
                                    }
                                case AttackType.Homing: {
                                        
                                            FireWizardProjectile p = GameObject.Instantiate(ProjectilePrefab, (this.transform.position + AimDirection), new Quaternion()).GetComponent<FireWizardProjectile>();
                                            p.LaunchDirection = AimDirection;

                                            p.VelocityMultiplier = .5f;//(0.5f + 0.5f * (((float)(i + 1)) / 4f));
                                        p.VelocityMultiplier *= (1f + (1f * Astronaut.AggressionLevelF));
                                        p.TurnRateMultiplier *= (1f + (3f * Astronaut.AggressionLevelF));
                                        p.Accelerating = false;
                                            p.Homing = true;
                                        p.CastPosition = this.transform.position;
                                        p.HomingTarget = plr.transform;
                                            p.MyAttackType = this.MyAttackType;
                                        
                                        break; }
                                case AttackType.Spread: {
                                        Vector3 cross = Vector3.Cross(AimDirection.normalized, Vector3.forward);
                                        for (int i = 0; i < (pcount+1); i++)
                                        {
                                            float sd = (((((float)i) / (pcount))*2f) -1f);
                                            float coneang = 30f;
                                            FireWizardProjectile p = GameObject.Instantiate(ProjectilePrefab, (this.transform.position + AimDirection), new Quaternion()).GetComponent<FireWizardProjectile>();
                                            p.LaunchDirection = ((AimDirection*Mathf.Cos(Mathf.PI*2f*((coneang*sd)/720f))) + (cross*Mathf.Sin(Mathf.PI * 2f * ((coneang * sd) / 720f))));

                                            //p.VelocityMultiplier = (0.5f + 0.5f * (((float)(i + 1)) / 4f));
                                            p.VelocityMultiplier *= (1f + (1f * Astronaut.AggressionLevelF));
                                            p.Accelerating = false;
                                            p.Homing = false;
                                            p.CastPosition = this.transform.position;
                                            p.MyAttackType = this.MyAttackType;
                                        }
                                        break; }
                                case AttackType.Surround: {
                                        for (int i = 0; i < pcount; i++)
                                        {
                                            float sd = ((float)i /pcount);

                                            Vector3 surrounddir = (new Vector3(Mathf.Cos(sd * Mathf.PI * 2f), Mathf.Sin(sd * Mathf.PI * 2f), 0f) * 1.5f);

                                            FireWizardProjectile p = GameObject.Instantiate(ProjectilePrefab, (plr.transform.position + (surrounddir*1f)), new Quaternion()).GetComponent<FireWizardProjectile>();
                                            p.LaunchDirection = -surrounddir;
                                            //p.VelocityMultiplier = (0.5f + 0.5f * (((float)(i + 1)) / 4f));
                                            p.CastPosition = this.transform.position;
                                            p.Accelerating = false;
                                            p.Homing = false;
                                            p.MyAttackType = this.MyAttackType;

                                        }
                                        break; }
                                case AttackType.Rain: {
                                        for (int i = 0; i < 4; i++)
                                        {
                                            float sd = ((((float)i / 3f)*2f) -1f);

                                            FireWizardProjectile p = GameObject.Instantiate(ProjectilePrefab, (plr.transform.position + (Vector3.up * 3f) + (-Vector3.right) + (Vector3.right * sd)), new Quaternion()).GetComponent<FireWizardProjectile>();
                                            p.LaunchDirection = Vector3.down;

                                            //p.VelocityMultiplier = (0.5f + 0.5f * (((float)(i + 1)) / 4f));
                                            p.Accelerating = false;
                                            p.Homing = false;
                                            p.CastPosition = this.transform.position;
                                            p.MyAttackType = this.MyAttackType;
                                        }
                                        break; }

                            }

                        }

                        if ((plr != null) && (plr.Alive))
                        {
                            Vector3 dif = (plr.transform.position - this.transform.position);
                            AimDirection = dif.normalized;

                        }
                        MySpriteRenderer.flipX = (Mathf.Sign(-AimDirection.x) < 0f);

                        if (stateexpired)
                        {
                            //ShootWindUpGlow.Play();

                            


                            //float sp = 10f;
                            //p.MyRigidbody.velocity = AimDirection*sp;

                            setState(State.Waiting,.5f);
                        }
                        else
                        {
                            //MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
                            MyRigidbody.velocity = new Vector2();
                        }

                        break;
                    }
                

            }

            DamageFlashStep();
            //MyRigidbody.velocity = new Vector2(movedir * movespeed, MyRigidbody.velocity.y);
        }
        else
        {
            setState(State.Waiting, .5f + (2f * Random.value));
        }
        freezeStep();
    }
    private Vector3 AimDirection = Vector3.left;
    private bool Attacked=false;
    public void setState(State st,float dur)
    {
        
        StateTime = Time.time;
        StateDuration = dur;
        if ((st == State.Attacking) &&(MyState != State.Attacking))
        {
            Attacked = false;
            MyAnimator.SetTrigger("CastAttack");
            
            if (((Time.time - lastAttackChangeTime) >= 5f) || (attackUsages >= 3))
            {
                
                chooseAttack();
            } 
            //MyRigidbody.velocity = new Vector2();
            //ShootWindUpGlow.Play();
        }

        MyState = st;
    }
    public override void Kill()
    {
        if (!Alive) return;
        Alive = false;
        //base.Kill();
        Astronaut.TheAstronaut.dropResistance(.4f / (1f + HitsDone), this.transform.position, Astronaut.Element.Fire);
        deathKnockback();
    }


    private float LastShotTime = -10f;
    private Vector3 TargetingDirection;

}
