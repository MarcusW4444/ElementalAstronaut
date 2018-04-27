using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Jungle_TreeFace : GenericEnemy {

	// Use this for initialization
	void Start () {

        spritestartrotation = MySpriteRenderer.transform.rotation;
        spritestartsize = MySpriteRenderer.transform.localScale;
        startposition = this.transform.position;
        TreeSuckSound = AudioManager.AM.createAudioSource(Am.am.M.TreeSuckSound, this.gameObject, new Vector3(0f, 0f, 0f), Am.am.M.EnvironmentAudioMixer, 1f, 1f, true);

    }

    // Update is called once per frame
    public bool Visible = false;
    public float Visibility = 0f;
    public JungleFaceProjectile JungleFaceProjectilePrefab;
    public bool Shooting = false;
    public bool Inhaling = false;
    public bool Attacked = false;
    public float AttackTime = -10f;
    public float VisibilityTime = -10f;
    public Sprite MouthClosedSprite, MouthOpenSprite;
    public ParticleSystem SuckParticles;
    public int SuckDirection = 0;
    private Quaternion spritestartrotation;
    private Vector3 spritestartsize;
    private Vector3 startposition;
    public AudioSource TreeSuckSound;
    void Update()
    {

        if (Alive)
        {
            hitdirection = (hitdirection * (1f - Time.deltaTime));
            if (MyRigidbody.velocity.x != 0f)
            {
                MySpriteRenderer.flipX = (Mathf.Sign(MyRigidbody.velocity.x) < 0f);
            }
            freezeStep();
            DamageFlashStep();
        }
    }
    public float AppearanceRangeUp, AppearanceRangeDown;
    float suckacceleration = 0f;
    void FixedUpdate () {

        if (Alive && !isStunned())
        {
            Astronaut plr = Astronaut.TheAstronaut;

            if (Visible)
            {
                MyCollider.enabled = true;

                if (Inhaling)
                {
                    AttackTime = Time.time;
                    Attacked = true;
                    suckacceleration = Mathf.Clamp01(suckacceleration+(Time.fixedDeltaTime/Mathf.Lerp(2f,1f,Astronaut.AggressionLevelF)));
                    //Suck the player in

                    if ((plr != null) && (plr.Alive))
                    {
                        //ch = true;

                        
                            Vector3 dif = (plr.transform.position - this.transform.position);
                            AimDirection = dif.normalized;
                            if ((dif.magnitude < 6f)) //(SuckDirection == ((int)Mathf.Sign(dif.x)))
                        {
                                //SUCK.
                                float suckspeed = ((8f*(1f+(1f*Astronaut.AggressionLevelF)))* suckacceleration);
                            if (suckacceleration > .5f)
                            {
                                plr.MyJumpingVine = plr.MyClimbingVine;
                                plr.MyClimbingVine = null;
                            }
                            plr.transform.position = (plr.transform.position - (new Vector3(dif.normalized.x, dif.normalized.y,0f) *suckspeed*Time.fixedDeltaTime));
                                
                                SuckParticles.transform.LookAt(plr.transform.position);
                            MySpriteRenderer.sprite = MouthOpenSprite;
                            if (dif.magnitude > 9f) {
                                    //Shoot();
                                    Inhaling = false;
                                suckacceleration = 0f;
                                SuckParticles.Stop();
                                TreeSuckSound.Stop();
                                MySpriteRenderer.sprite = MouthClosedSprite;
                            }


                                




                            } else
                            {
                                Inhaling = false;
                            suckacceleration = 0f;
                            Attacked = true;
                            }
                        
                    } else
                    {
                        Inhaling = false;
                        suckacceleration = 0f;
                    }

                }
                if (!Attacked)
                {
                    MySpriteRenderer.sprite = MouthClosedSprite;
                    if ((Time.time - VisibilityTime) >= (1.5f * (1f - .75f * Astronaut.AggressionLevelF)))
                    {

                        
                        
                        bool ch = false;
                        if ((plr != null) && (plr.Alive))
                        {
                            //ch = true;

                            if ((Time.time - VisibilityTime) >= (1.5f* (1f - .75f * Astronaut.AggressionLevelF)))
                            {
                                Vector3 dif = (plr.transform.position - this.transform.position);
                                AimDirection = dif.normalized;
                                if (dif.magnitude < 6f)
                                {
                                    //SUCK.
                                    Debug.Log("SUCKING");
                                    ch = true;
                                    suckacceleration = 0f;
                                    Inhaling = true;
                                    AttackTime = Time.time;
                                    Attacked = true;
                                    SuckDirection = (int)Mathf.Sign(dif.x);
                                    MySpriteRenderer.sprite = MouthOpenSprite;


                                }
                                else if (dif.magnitude < 9f)
                                {
                                    //Shoot a projectile.
                                    Debug.Log("Phtooie!");
                                    ch = true;
                                    Attacked = true;
                                    AttackTime = Time.time;
                                    Shoot();
                                    MySpriteRenderer.sprite = MouthOpenSprite;



                                } else
                                {
                                    //...
                                }
                            }


                        }
                        if (!ch)
                        if ((Time.time - VisibilityTime) >= 4.5f)
                        {
                            VisibilityTime = Time.time;
                            Visible = false;
                            Attacked = false;
                                //MySpriteRenderer.sprite = MouthClosedSprite;
                            }
                    }
                } else
                {
                    if ((Time.time - AttackTime) >= (.75f * (1f - .75f * Astronaut.AggressionLevelF)))
                    {
                        MySpriteRenderer.sprite = MouthClosedSprite;
                    }
                        

                    if ((Time.time - AttackTime) >= (2f*(1f-.75f*Astronaut.AggressionLevelF)))
                    {

                        VisibilityTime = Time.time;
                        Visible = false;
                        MySpriteRenderer.sprite = MouthClosedSprite;
                    }
                }
                



            } else
            {
                MyCollider.enabled = false;
                //...hiding...
                MySpriteRenderer.sprite = MouthClosedSprite;
                if ((plr != null) && (plr.Alive))
                {


                    Vector3 dif = (plr.transform.position - this.transform.position);
                    float ydif = Mathf.Abs(Mathf.Clamp((plr.transform.position.y + 2f), (startposition.y - AppearanceRangeDown), (startposition.y + AppearanceRangeUp)) - plr.transform.position.y);
                    if ((Mathf.Abs(dif.x) < 5f) && (ydif < 5f))
                    {

                        if ((Time.time - VisibilityTime) >= 1.5f)
                        {
                            VisibilityTime = Time.time;
                            Visible = true;
                            Physics2D.IgnoreCollision(MyCollider, plr.MyCollider, false);
                            this.transform.position = new Vector3(this.transform.position.x,Mathf.Clamp((plr.transform.position.y+2f), (startposition.y - AppearanceRangeDown), (startposition.y + AppearanceRangeUp)),this.transform.position.z);
                            Attacked = false;
                            Inhaling = false;
                        }

                    }

                    
                }


            }





        } else
        {
            Inhaling = false;
            MyCollider.enabled = true;
            Visible = true;
        }
        if (!Alive)
        {
            MyCollider.enabled = false;
        }

        if (Alive)
        {
            

            if (Visible)
            {
                Visibility = Mathf.Lerp(Visibility, 1f, .2f);
            }
            else
            {
                Visibility = Mathf.Lerp(Visibility, 0f, .2f);
            }
            MySpriteRenderer.transform.rotation = spritestartrotation;
            MySpriteRenderer.transform.Rotate(0f,0f,(270f*(1f-Visibility)),Space.World);
            MySpriteRenderer.transform.localScale = (spritestartsize*Visibility);
        }
        
        BaseColor = (Visible ? new Color(1f, 1f, 1f, 1f) : new Color(1f, 1f, 1f, .1f));

        if (Inhaling)
        {
            if (!SuckParticles.isPlaying)
            {
                SuckParticles.Play();
                TreeSuckSound.Play();
            }
        } else
        {
            if (SuckParticles.isPlaying)
            {
                SuckParticles.Stop();
                TreeSuckSound.Stop();
            }
        }
        
		
	}
    private Vector3 AimDirection = Vector3.left;
    public void Shoot()
    {
        JungleFaceProjectile p = GameObject.Instantiate(JungleFaceProjectilePrefab, this.transform.position, new Quaternion());
        float sp = 5f;
        p.MyRigidbody.velocity = AimDirection * sp;
    }
    public ParticleSystem BiteFlash;
    public Collider2D MyCollider;
    public override void Kill()
    {
        
        this.Alive = false;
        MyCollider.enabled = false;
        Astronaut.PlayKillSound(2);
        Astronaut.TheAstronaut.dropResistance(.4f / (1f + HitsDone), this.transform.position, Astronaut.Element.Grass);
        GameObject.Destroy(this.gameObject);
        //Emit
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Astronaut a = collision.gameObject.GetComponent<Astronaut>();
        if ((a != null) && (a.Alive))
        {
            if (!isStunned())
                if ((Time.time - a.lastDamageTakenTime) >= 2f)
                {
                    Vector3 dif = (a.transform.position - this.transform.position);
                    HitsDone += 1f;
                    if (a.TakeDamage(40f, dif.normalized * 10f + new Vector3(0f, a.JumpSpeed, 0f)))
                    {
                        HitsDone += 4f;
                    }
                    if (Inhaling)
                    {
                        Inhaling = false;
                    }
                }

        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollisionStay2D(collision);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerStay2D(collision);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        Astronaut a = collision.gameObject.GetComponent<Astronaut>();
        if ((a != null) && (a.Alive))
        {
            if (!isStunned())
                //if ((Time.time - a.lastDamageTakenTime) >= 2f)
                //{
                    if (Inhaling)
                    {
                        Vector3 dif = (a.transform.position - this.transform.position);
                    float suckspeed = ((8f * (1f + (1f * Astronaut.AggressionLevelF))) * suckacceleration);
                    if (dif.magnitude < (suckspeed*Time.fixedDeltaTime*3))
                    {
                        HitsDone += 1f;
                        if (a.TakeDamage(40f, new Vector3()))
                        {
                            HitsDone += 4f;
                        }
                        a.transform.position = this.transform.position;
                        a.MyRigidbody.velocity = new Vector2();
                        Physics2D.IgnoreCollision(MyCollider, a.MyCollider, true);
                        BiteFlash.transform.LookAt(BiteFlash.transform.position - dif);
                        BiteFlash.Play(true);
                        Inhaling = false;
                        MySpriteRenderer.sprite = MouthClosedSprite;
                    }
                    }
                //}

        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(.5f, 1f, 0f);
        float bottomy = (this.transform.position.y - Mathf.Max(AppearanceRangeDown,0f));
        float topy = (this.transform.position.y + Mathf.Max(AppearanceRangeUp, 0f));
        Vector3 bo = new Vector3(MyCollider.bounds.size.x, MyCollider.bounds.size.y, 1f);
        Gizmos.DrawWireCube(new Vector3(this.transform.position.x,topy,this.transform.position.z),bo);
        Gizmos.DrawWireCube(new Vector3(this.transform.position.x, bottomy, this.transform.position.z), bo);
        Gizmos.DrawLine(new Vector3(this.transform.position.x, topy, this.transform.position.z), new Vector3(this.transform.position.x, bottomy, this.transform.position.z));

        //Gizmos.DrawLine(new Vector3(this.transform.position.x + .5f, bottomy, this.transform.position.z), new Vector3(this.transform.position.x - .5f, bottomy, this.transform.position.z));
        //Gizmos.DrawLine(new Vector3(this.transform.position.x + .5f, topy, this.transform.position.z), new Vector3(this.transform.position.x - .5f, topy, this.transform.position.z));
        //Gizmos.DrawLine(new Vector3(this.transform.position.x, bottomy, this.transform.position.z), new Vector3(this.transform.position.x, topy, this.transform.position.z));
    }
}
