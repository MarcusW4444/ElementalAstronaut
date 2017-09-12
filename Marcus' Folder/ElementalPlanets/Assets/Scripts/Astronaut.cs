using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Astronaut : MonoBehaviour {

    // Use this for initialization
    public enum Element{None,Water,Fire,Ice,Grass}
    public float Health = 100f;
    public float ElementalEnergy;
    public Element ElementMode = Element.None;
    public bool Alive = true;

    public enum SpecialWeapon {Pistol,Shotgun,Laser, Gatling};
    public SpecialWeapon CurrentSpecialWeapon = SpecialWeapon.Pistol;
    public static Astronaut TheAstronaut = null;
	void Start () {
        TheAstronaut = this;

    }

    // Update is called once per frame
    public Rigidbody2D MyRigidbody;
    public CapsuleCollider2D MyCollider;
    public SpriteRenderer MySpriteRenderer;
    public Bullet BulletPrefab;
    private int LateralControl=0;
    private int VerticalControl = 0;
    private bool JumpControl = false;
    public float WalkSpeed = 1f;
    public float JumpSpeed = 5f;
    public float JumpCancelRate = .6f;
    public bool Airborne = false;
    public bool UsedDoubleJump = false;
    public bool ElementalControl=false;
    private bool ElementalPress = false;
    private bool jumppress = false;
    private bool JumpNow = false;
    public bool ShootControl;
    private bool ShootPress;
    public int LookDirection = 1;
    public Vector3 AimPosition = new Vector3();
    public Transform ReticleTransform;
    public int PistolAmmo = 15;
    public int ShotgunAmmo=0;
    public float LaserAmmo=0f;
    public int GatlingAmmo = 0;

    void Update () {

        

        if (true)
        {
            MyHealthBar.fillAmount = (Health / 100f);
            MyEnergyBar.fillAmount = (ElementalEnergy / 100f);
            
            switch (ElementMode)
            {
                case Element.Fire:
                    { MyEnergyBar.color = new Color(1f, .2f, .2f, 1f);
                        MyElementalIcon.sprite = ElementIcon_Fire;
                        break; }
                case Element.Ice:
                    { MyEnergyBar.color = new Color(.2f, .2f, 1f, 1f);
                        MyElementalIcon.sprite = ElementIcon_Ice;
                        break; }
                case Element.Water:
                    {
                        MyElementalIcon.sprite = ElementIcon_Water;
                        MyEnergyBar.color = new Color(.2f, .8f, .8f, 1f); break; }
                case Element.Grass:
                    {
                        MyElementalIcon.sprite = ElementIcon_Grass;
                        MyEnergyBar.color = new Color(.2f, .8f, .2f, 1f); break;
                    }
                case Element.None:
                default:
                    {
                        MyElementalIcon.sprite = ElementIcon_None;
                        MyEnergyBar.color = new Color(.8f, .8f, .8f, 1f); break; }
            }

            string ammostring = "";
            switch (CurrentSpecialWeapon)
            {
                case SpecialWeapon.Pistol: { ammostring = "Pistol: " + PistolAmmo; break; }
                case SpecialWeapon.Shotgun: { ammostring = "Shotgun: " + ShotgunAmmo; break; }
                case SpecialWeapon.Gatling: { ammostring = "Gatling: " + GatlingAmmo; break; }
                case SpecialWeapon.Laser: { ammostring = "Laser: " + (int)LaserAmmo+"%"; break; }
            }
            MyWeaponAmmoText.text = ammostring;
        }
    }

    private float lastshottime = -10f;
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
                        float speed = 80f;

                        if (((Time.time - lastshottime) > (1f / firerate)) && (PistolAmmo > 0))
                        {
                            lastshottime = Time.time;


                            Bullet b = GameObject.Instantiate(BulletPrefab, this.transform.position, new Quaternion()).GetComponent<Bullet>();//output.transform.
                            b.Damage = damage;
                            b.MyRigidbody.velocity = (new Vector2(dir.x, dir.y) * speed);
                                
                            PistolAmmo--;
                        }

                        

                         
                    }
                    if ((Time.time - lastshottime) >= 2f)
                    {
                        PistolAmmo = 15;
                    }

                    break; }
                case SpecialWeapon.Shotgun: {
                        if (ShootControl)
                        {
                            //switch (weapon)
                            float firerate = 1.5f;
                            float damage = 100f;
                            float spreadangle = 10f;
                            float speed = 50f;

                            if (((Time.time - lastshottime) > (1f / firerate)) && (ShotgunAmmo > 0))
                            {
                                lastshottime = Time.time;


                                Bullet b = GameObject.Instantiate(BulletPrefab, this.transform.position, new Quaternion()).GetComponent<Bullet>();//output.transform.
                                b.Damage = damage;
                                int fragments = 7;
                                spreadangle = 15f;
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



                                    frags++;
                                }
                                ShotgunAmmo--;
                            if (ShotgunAmmo <= 0) setWeapon(SpecialWeapon.Pistol);
                        }

                        


                        }



                        break; }
                case SpecialWeapon.Gatling: {


                    if (ShootControl)
                    {
                        //switch (weapon)
                        float firerate = 20f;
                        float damage = 15f;
                        float spreadangle = 30f;
                        float speed = 50f;

                        if (((Time.time - lastshottime) > (1f / firerate)) && (GatlingAmmo > 0))
                        {
                            lastshottime = Time.time;


                            Bullet b = GameObject.Instantiate(BulletPrefab, this.transform.position, new Quaternion()).GetComponent<Bullet>();//output.transform.
                            b.Damage = damage;
                            int fragments = 3;
                            spreadangle = 15f;
                            int frags = 0;
                            b.Damage = (b.Damage / ((float)fragments));
                            Vector3 cross = Vector3.Cross(dir, Vector3.forward);
                            float prt = ((float)(((float)2) / ((float)(fragments))));
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



                                frags++;
                            }
                            GatlingAmmo--;
                            if (GatlingAmmo <= 0) setWeapon(SpecialWeapon.Pistol);
                        }



                    }

                    break; }
                case SpecialWeapon.Laser: {



                    if (LaserAmmo <= 0) setWeapon(SpecialWeapon.Pistol);break; }


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
                    //Inferno Bar
				if ((ElementalControl) && (ElementalEnergy > 0f)) {

					ElementalEnergy = (ElementalEnergy - (10f * Time.deltaTime));
					LastElementalTime = Time.time;

                        Ray2D r = new Ray2D(this.transform.position, dir.normalized);

                        float maxfiredist = 10f;
                        RaycastHit2D rh = Physics2D.Raycast(r.origin, r.direction, maxfiredist, LayerMask.GetMask(new string[] { "Geometry" }));
                        if (rh.collider != null)
                        {
                            MyFlameBar.transform.LookAt(rh.point,Vector3.up);
                            //MyFlameBar.transform.localScale = new Vector3(1f, 1f, rh.distance);
                            ParticleSystem[] ps = MyFlameBar.GetComponentsInChildren<ParticleSystem>();
                            foreach (ParticleSystem p in ps)
                            {
                                ParticleSystem.ShapeModule m = p.shape;
                                m.position = new Vector3(0f, 0f, rh.distance*.5f);
                                m.scale = new Vector3(0f, 0f, rh.distance * 1f);
                            }
                            MyFlameBar.MyCollider.transform.localScale = new Vector3(rh.distance, 1f,1f);
                            

                        }
                        else
                        {
                            MyFlameBar.transform.LookAt(this.transform.position+ dir, Vector3.up);
                            ParticleSystem[] ps = MyFlameBar.GetComponentsInChildren<ParticleSystem>();
                            foreach (ParticleSystem p in ps)
                            {
                                ParticleSystem.ShapeModule m = p.shape;
                                m.position = new Vector3(0f, 0f, maxfiredist*.5f);
                                m.scale = new Vector3(0f, 0f, maxfiredist * 1f);
                            }
                            MyFlameBar.MyCollider.transform.localScale = new Vector3(maxfiredist, 1f, 1f);
                        }


                        if (!MyFlameBar.MyParticles.isPlaying) {
						MyFlameBar.MyParticles.Play(true);
					    }



                        MyFlameBar.FlameActive = true;
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
					//Construct Ice Pillars

				if ((ElementalControl) && (ElementalEnergy>0f)) {
                        ElementalEnergy = (ElementalEnergy - (25f * Time.deltaTime));
                        LastElementalTime = Time.time;
                        //Ray Cast
                        Ray2D r = new Ray2D(this.transform.position, dir.normalized);
                        
                        float maxicedist = 15f;
                        RaycastHit2D rh = Physics2D.Raycast(r.origin, r.direction, maxicedist, LayerMask.GetMask(new string[] { "Geometry"}));
                        if (rh.collider != null)
                        {
                            IceBlock ice = rh.collider.gameObject.GetComponentInParent<IceBlock>();
                            if (ice != null)
                            {
                                ice.LastTickTime = Time.time;
                                ice.transform.localScale = new Vector3(ice.transform.localScale.x, ice.transform.localScale.y, ice.transform.localScale.z+(5f*Time.deltaTime));
                                
                            }  else
                            {
                                ice = GameObject.Instantiate(IceBlockPrefab,rh.point,Quaternion.LookRotation((this.transform.position - new Vector3(rh.point.x, rh.point.y, 0f)).normalized, Vector3.up)).GetComponent<IceBlock>();

                            }
                            ParticleSystem[] ps = ice.GetComponentsInChildren<ParticleSystem>();
                            foreach (ParticleSystem p in ps)
                            {
                                ParticleSystem.ShapeModule m = p.shape;
                                m.position = new Vector3(0f, 0f, ice.transform.localScale.z * .5f);
                                //m.scale = new Vector3(0f, 0f, ice.transform.localScale.z * 1f);
                            }
                            
                        } else
                        {
                            //Miss.
                            
                        }

                        if (FrostBlower)
                            if (!FrostBlower.isPlaying)
                                FrostBlower.Play(true);

                    } else
                    {
                        if (FrostBlower)
                            if (!FrostBlower.isPlaying)
                                FrostBlower.Stop(true);
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
                                ElementalEnergy = (ElementalEnergy - (10f * Time.deltaTime));
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
    }

    
	public FlameBar MyFlameBar,BurningPrefab;
    public Vines VinePrefab;
    private Vines MyVines;
    private Vines2 MyVines2;
    public Vines2 VinePrefab2;


    public ParticleSystem WaterPump;
    public ParticleSystem FrostBlower;
    public IceBlock IceBlockPrefab;
    private bool GrassSwinging = false;
    public Transform MyCameraRig;
	public Image MyHealthBar, MyEnergyBar,MyElementalIcon,MyWeaponImage;
    public Text MyWeaponAmmoText;
    public Sprite ElementIcon_Fire, ElementIcon_Ice, ElementIcon_Water, ElementIcon_Grass, ElementIcon_None;

    private float LastElementalTime = -10f;
    
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
        if (Alive)
        {
            lc = ((Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0));
            vc = ((Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0));
            jc = (Input.GetKey(KeyCode.Space));
            sp = Input.GetMouseButton(0);
            ec = Input.GetMouseButton(1);
            if (Input.mouseScrollDelta.y > 0)
            {
                if (ElementMode == Element.Fire)
                    ElementMode = Element.Ice;
                else if (ElementMode == Element.Ice)
                    ElementMode = Element.Grass;
                else if (ElementMode == Element.Grass)
                    ElementMode = Element.Fire;
            }
            else if (Input.mouseScrollDelta.y < 0)
            {
                if (ElementMode == Element.Fire)
                    ElementMode = Element.Grass;
                else if (ElementMode == Element.Grass)
                    ElementMode = Element.Ice;
                else if (ElementMode == Element.Ice)
                    ElementMode = Element.Fire;
            }

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                setWeapon(SpecialWeapon.Pistol);
            } else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                setWeapon(SpecialWeapon.Shotgun);
            } else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                setWeapon(SpecialWeapon.Gatling);
            } else if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                setWeapon(SpecialWeapon.Laser);
            }
            Ray pr = Camera.main.ScreenPointToRay(Input.mousePosition);

            AimPosition = (pr.origin - (pr.direction * (pr.origin.z / pr.direction.z)));
            ReticleTransform.position = AimPosition;
        }

        LateralControl = lc;
        VerticalControl = vc;
        JumpControl = jc;
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
            MySpriteRenderer.flipX = (LookDirection < 0);
        }




        if (Airborne)
        {
            if (!GrassSwinging)
                if (!stunned) MyRigidbody.velocity = new Vector2(((LateralControl != 0)? (LateralControl * WalkSpeed):MyRigidbody.velocity.x), MyRigidbody.velocity.y); //AddForce(new Vector2(0f,0f),ForceMode2D.);
        } else
        {
            if (!GrassSwinging)
                if (!stunned) MyRigidbody.velocity = new Vector2(LateralControl * WalkSpeed, MyRigidbody.velocity.y); //AddForce(new Vector2(0f,0f),ForceMode2D.);
        }
        if (JumpControl)
        {
            if (JumpNow)
            if (!Airborne || !UsedDoubleJump)
            {
                MyRigidbody.velocity = new Vector2(MyRigidbody.velocity.x, JumpSpeed);
                if (Airborne)
                    UsedDoubleJump = true;
                
            }
        } else
        {
            if (Airborne)
            {
                if (MyRigidbody.velocity.y > 0f)
                MyRigidbody.velocity = new Vector2(MyRigidbody.velocity.x, MyRigidbody.velocity.y*JumpCancelRate);
            }
        }

        
        
        if (Mathf.Abs(MyRigidbody.velocity.y) >= .1f)
        {
            Airborne = true;
            
        }


        Vector3 rigtargpos = new Vector3(transform.position.x, transform.position.y,MyCameraRig.transform.position.z);


        if (Alive)
        {
            float t = ((Time.time - lastDamageTakenTime) / 1.5f);
            if (t < 1f)
            {
                MySpriteRenderer.color = Color.Lerp(Color.white, Color.red, Mathf.Sin((t * 4) * Mathf.PI));
            } else
            {
                MySpriteRenderer.color = Color.white;
            }
            
        } else
        {
            MySpriteRenderer.color = Color.red;
        }

        MyCameraRig.transform.position = Vector3.Lerp(MyCameraRig.transform.position, rigtargpos,Time.fixedDeltaTime*3f);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {

        
            
            
            foreach (ContactPoint2D co in collision.contacts)
            {
                
                
            if (co.collider.gameObject.CompareTag("Geometry"))
            {
                Vector2 po = co.point;
                if ((po.y <= (this.transform.position.y - (MyCollider.size.y * .4f))) && (MyRigidbody.velocity.y <= 0f))
                {
                    //Debug.Log("Geo");
                    Airborne = false;
                    UsedDoubleJump = false;
                }

            
            }

            }
    }

    public void setWeapon(SpecialWeapon wep)
    {
        switch (wep)
        {
            case SpecialWeapon.Pistol: {
                    CurrentSpecialWeapon = SpecialWeapon.Pistol;
                    break; }
            case SpecialWeapon.Shotgun: {

                    if (ShotgunAmmo > 0)
                    {
                        CurrentSpecialWeapon = SpecialWeapon.Shotgun;
                    } else
                    {
                        //small buzzer
                    }
                    break; }
            case SpecialWeapon.Gatling: {
                    if (GatlingAmmo > 0)
                    {
                        CurrentSpecialWeapon = SpecialWeapon.Gatling;
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
                    }
                    else
                    {
                        //small buzzer
                    }

                    break; }

        }
        

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
                    LaserAmmo = Mathf.Min(LaserAmmo + 10f, 40f);
                    break;
                }
        }


    }


    public void revive()
    {

        Health = 100f;
        Alive = true;
        this.transform.rotation = new Quaternion();
        ShotgunAmmo = 0;
    }
    public void kill()
    {
        if (!Alive) return;
        Alive = false;
        Health = 0f;
        this.transform.Rotate(0f,0f,90f,Space.World);
    }
    public float lastDamageTakenTime = -10f, lastStunTime = -10f;
    public bool TakeDamage(float damage,Vector3 knockback)
    {
        if (!Alive) return false;

        float health = (Health - damage);
            lastDamageTakenTime = Time.time;
        //Play a DamageTaken sound.
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
       

}
