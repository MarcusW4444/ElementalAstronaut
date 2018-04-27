using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Void_Eye : GenericEnemy {

    //The Void Eye can appear and disappear around you to get from place to place or to avoid your attacks
    //It can also hide the rest of the environment
    //Or it can create a swirl of orbiting projectiles that it can throw at you


	// Use this for initialization

	
    public bool EyeOpen = false;
    public float Existence = 0f;
	// Update is called once per frame
	

    //public VoidProjectile[] 

    private void FixedUpdate()
    {


        Astronaut plr = Astronaut.TheAstronaut;

        if (Alive && !isStunned())
        {

            if ((plr != null) && (plr.Alive) && (!Astronaut.TheAstronaut.Quelling))
            {
                Vector3 dif = (plr.transform.position - this.transform.position);



                if (dif.magnitude < 10f)
                {
                    bool los = (Physics2D.Linecast(this.transform.position, plr.transform.position, LayerMask.GetMask(new string[] { "Geometry" })).collider == null);

                    if (Existence >= 1f)
                    {
                        //create some orbs over time
                        //MyVoidField.Activate
                        if (EyeOpen)
                        {
                            MoveDirection = (dif.normalized * .5f)*(1f+(2f*Astronaut.AggressionLevelF));
                            


                            if ((Time.time - AppearTime) >= 4f)
                            {
                                //Debug.Log("Want to fade");
                                fadeOut();
                            }
                        }
                    }
                    else
                    {
                        MoveDirection = new Vector3();
                        if (!los)
                        {

                            if (Existence > 0f)
                            {

                            }

                        }
                        else
                        {
                            if ((Time.time - DisappearTime) >= (2f * (1f - Astronaut.AggressionLevelF)))
                            {
                                if (Existence == 0f)
                                {
                                    stealthTeleport();
                                    fadeIn();
                                }
                            }
                        }
                    }

                    //LookDirection = (int)((dif.x != 0) ? Mathf.Sign(dif.x) : LookDirection);








                }
            }





        }
        else
        {
            MoveDirection = new Vector3();
        }

        if (Alive)
        {

            MyCollider.enabled = (Existence > 0f);
            MySpriteRenderer.sprite = ((EyeOpen && (Existence >= 1f)) ? EyeOpenSprite : EyeClosedSprite);
            if (EyeOpen || Fading)
            {
                MySpriteRenderer.color = new Color(MySpriteRenderer.color.r, MySpriteRenderer.color.g, MySpriteRenderer.color.b, Existence);
            }
            else
            {

            }
            if (Fading)
            {
                if (FadingIn)
                {

                    Existence = Mathf.Clamp01(Existence + (Time.fixedDeltaTime * 1f));
                    if (Existence >= 1f)
                    {
                        if (Astronaut.AggressionLevel > 0)
                        {
                            MyVoidField.Activate();
                        }
                        EyeOpen = true;
                        Fading = false;
                        AppearTime = Time.time;
                    }
                }
                else
                {
                    Existence = Mathf.Clamp01(Existence - (Time.fixedDeltaTime * 1f));
                    if (Existence <= 0f)
                    {
                        Fading = false;
                        DisappearTime = Time.time;
                        fadefinishtime = Time.time;

                    }
                    //Dispose of the Projectiles since we're fading out
                    for (int i = 0; i < MyOrbittingProjectiles.Length; i++)
                    {
                        if (MyOrbittingProjectiles[i] != null)
                        {
                            MyOrbittingProjectiles[i].Disposing = true;
                            MyOrbittingProjectiles[i] = null;
                        }
                    }

                }
            }

            if (Existence > .5f)
            {
                if (!NegativeParticles.isPlaying)
                    NegativeParticles.Play();
            }
            else
            {
                if (NegativeParticles.isPlaying)
                    NegativeParticles.Stop();

            }
            if (Existence <= 0f)
            {
                MySpriteRenderer.enabled = false;
                //Dispose of the Projectiles since we're fading out
                if (isIncinerating()) 
                    IncinerationTime = Time.time;
                for (int i = 0; i < MyOrbittingProjectiles.Length; i++)
                {
                    if (MyOrbittingProjectiles[i] != null)
                    {
                        MyOrbittingProjectiles[i].Disposing = true;
                        MyOrbittingProjectiles[i] = null;
                    }
                }
            }
            else
            {
                MySpriteRenderer.enabled = true;
                if (EyeOpen)
                {
                    float sr = (1f / (OrbitSpawnRate*(1f+(2f*Astronaut.AggressionLevelF)) * 1f));
                    
                        for (int i = 0; i < MyOrbittingProjectiles.Length; i++)
                        {
                        if (MyOrbittingProjectiles[i] != null)
                        {
                            MyOrbittingProjectiles[i].transform.position = getOrbitalPosition(i);
                        }
                        else
                        {
                            if (!isStunned())
                            {
                                if (Time.time - lastProjectileFormTime > sr)
                                {
                                    formProjectile(i);
                                }
                            }
                        }
                        

                    }
                } else
                {
                    for (int i = 0; i < MyOrbittingProjectiles.Length; i++)
                    {
                        if (MyOrbittingProjectiles[i] != null)
                        {
                            MyOrbittingProjectiles[i].Disposing = true;
                            MyOrbittingProjectiles[i] = null;
                        }
                    }
                }
            }

            if (Alive)
            {
                MyRigidbody.velocity = new Vector3(MoveDirection.x, MoveDirection.y, 0f);
                OrbitalTime +=Time.fixedDeltaTime;
            }
            else
            {
                if ((NegativeParticles) && (NegativeParticles.isPlaying))
                    NegativeParticles.Stop();
            }
        }
        float sf = (1f + 1.5f * Astronaut.AggressionLevelF);
        SpeedFactor = Mathf.Lerp(SpeedFactor,sf,.5f);
    }
    public override void onIncinerated()
    {
        //base.onIncinerated();
        IncinerationTime = Time.time;
        fadeOut();
    }
    public ParticleSystem OpenFlare;
    private float OrbitalTime = 0f;
    private bool FadingIn = false;
    private float OrbitSpawnRate = 2f; //If aggression is high enough, spawn with a few orbs already
    private VoidProjectile[] MyOrbittingProjectiles = new VoidProjectile[24];
    public VoidField MyVoidField;
    private float fadestarttime = -10f;
    private float fadefinishtime = -10f;
    public void fadeIn()
    {
        if (Fading) return;
        EyeOpen = false;
        Fading = true;
        FadingIn = true;
        fadestarttime = Time.time;
    }
    public void fadeOut()
    {
        if (Fading) return;
        Fading = true;
        FadingIn = false;
        fadestarttime = Time.time;
        EyeOpen = false;
        launchProjectiles();
        MyVoidField.Deactivate();
    }
    private float DisappearTime = -10f, AppearTime = -10f;
    public ParticleSystem NegativeParticles;
    public bool Fading = false;
    public Sprite EyeOpenSprite,EyeClosedSprite;
    public Collider2D MyCollider;
    public VoidProjectile OrbitalProjectilePrefab;
    private float SpeedFactor = 1f;
    public Vector3 getOrbitalPosition(int index)
    {
        float ts = ((((Time.time * SpeedFactor) % 3f)) / 3f);

        if (index < 6)
        {
            int si = index;
            float sf = (((float)si) / 6f);
            float rad = 2f;
            float su = (sf + ts);
            return this.transform.position + new Vector3(Mathf.Cos(su * Mathf.PI * 2f) * rad, Mathf.Sin(su * Mathf.PI * 2f) * rad, 0f);
        }
        else if (index < 12)
        {
            int si = index - 6;
            float sf = (((float)si) / 12f);
            float rad = 4f;
            float su = (sf - ts);
            return this.transform.position + new Vector3(Mathf.Cos(su * Mathf.PI * 2f) * rad, Mathf.Sin(su * Mathf.PI * 2f) * rad, 0f);
        }
        else 
        {
            int si = index - 18;
            float sf = (((float)si) / 6f);
            float rad = 1f;
            float su = (sf + ts);
            return this.transform.position + new Vector3(Mathf.Cos(su * Mathf.PI * 2f) * rad, Mathf.Sin(su * Mathf.PI * 2f) * rad, 0f);
        }


            return this.transform.position;
    }
    public void formProjectile(int index)
    {
        Vector3 pos = getOrbitalPosition(index);
        VoidProjectile orb = GameObject.Instantiate<VoidProjectile>(OrbitalProjectilePrefab,pos,OrbitalProjectilePrefab.transform.rotation);
        orb.Damage = 5f;
        MyOrbittingProjectiles[index] = orb;
        lastProjectileFormTime = Time.time;
        

    }
    public void launchProjectiles()
    {
        //Launch the orbitting projectiles
        Vector3 dir = (Astronaut.TheAstronaut.transform.position - this.transform.position).normalized;
        float vel = 3f*(1f+(2f*Astronaut.AggressionLevelF));
        for(int i = 0; i < MyOrbittingProjectiles.Length; i++)
        {
            if (MyOrbittingProjectiles[i] != null)
            {
                MyOrbittingProjectiles[i].MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
                MyOrbittingProjectiles[i].MyRigidbody.velocity = (new Vector2(dir.x,dir.y)*vel);
                MyOrbittingProjectiles[i].Launched = true;
                MyOrbittingProjectiles[i].ParticleTrail.gameObject.SetActive(true);
                MyOrbittingProjectiles[i].ParticleTrail.Play(true);
                MyOrbittingProjectiles[i].ManipulatedByVoidEye = false;
                MyOrbittingProjectiles[i].ManipulatedByVoidSkeleton = false;
                MyOrbittingProjectiles[i].StartTime = Time.time;
                MyOrbittingProjectiles[i] = null;
            }
        }
        
    }
    private float[] distances;
    private float[] distancefloats;
    private int teleindex = 0;
    private float randval = 0f;
    private float lastProjectileFormTime = -10f;
    public void stealthTeleport()
    {
        //Teleport using the same algorithm that the Ghost used
        

            int numberofdirectionchoices = 16;
            float maxdistance = 8f*(1f-(.75f*Astronaut.AggressionLevelF));
            distances = new float[numberofdirectionchoices];
            //look in several different directions
            float ang = (360f / ((float)numberofdirectionchoices));
            float tot = 0f;
        Vector2 originalpos = Astronaut.TheAstronaut.MyRigidbody.position;
            for (int i = 0; i < numberofdirectionchoices; i++)
            {
                float rang = (Mathf.PI * 2f * (((float)i) / ((float)numberofdirectionchoices)));

                RaycastHit2D rh = Physics2D.Raycast(originalpos, new Vector2(Mathf.Cos(rang), Mathf.Sin(rang)), maxdistance, LayerMask.GetMask("Geometry")); //Teleport around the player
                if (rh.collider != null)
                {
                    //Debug.Log("Tele yes");
                    distances[i] = rh.distance;
                }
                else
                {
                    //Debug.Log("Tele none");
                    distances[i] = maxdistance;
                }
                tot += distances[i];
            }






            if (tot > 0f)
            {

                distancefloats = new float[numberofdirectionchoices];
                for (int i = 0; i < numberofdirectionchoices; i++)
                {
                    distancefloats[i] = (distances[i] / tot);
                }
                float rv = Random.value;
                int ind = 0;
                float f = 0f;
                randval = rv;
                while ((ind < numberofdirectionchoices) && (f < rv))
                {
                    float a = distancefloats[ind];
                    if (rv > a)
                    {
                        rv -= a;
                        ind++;
                    }
                    else
                    {
                        float rang = (Mathf.PI * 2f * (((float)ind) / ((float)numberofdirectionchoices)));
                        teleindex = ind;
                        Vector2 teledir = new Vector2(Mathf.Cos(rang), Mathf.Sin(rang));
                        this.transform.position = new Vector3(originalpos.x, originalpos.y,this.transform.position.z) + (new Vector3(teledir.x, teledir.y, 0f) * Mathf.Max(0f, (distances[ind] - .5f)) * (.5f + (.5f * Random.value)));
                        break;
                    }
                }


            }


            //commit to the teleport
        
    }
    private Vector3 MoveDirection = new Vector3();

    public override void Kill()
    {
        if (!Alive) return;
        Alive = false;
        
        Astronaut.PlayKillSound(3);
        NegativeParticles.Stop();
        GameObject.Destroy(NegativeParticles.gameObject, 4f);
        deathKnockback();
        Astronaut.TheAstronaut.dropResistance(.6f / (1f + HitsDone), this.transform.position, Astronaut.Element.Void);
        launchProjectiles();
        MyVoidField.Deactivate();
        MyVoidField.transform.SetParent(null);
        GameObject.Destroy(MyVoidField.gameObject,4f);
    }
}
