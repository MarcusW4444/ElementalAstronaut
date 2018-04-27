using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceShard : MonoBehaviour {

    private float StartTime = -10f;


    public bool Falling = false;
    public float FallTime = -10f;
    public SpriteRenderer MySpriteRenderer;
    private float WobbleTime = -10f;
    private bool Wobbling = false;
    public bool Melting = false;
    public bool Reforming = false;
    public bool CanReformWhenDropped = true;
    void Start()
    {
        StartTime = Time.time;
        if (Falling)
        {
            FallTime = Time.time;
            
        } else
        {
            MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
        }
        wobbledif = (WobbleTransform.position - this.transform.position);
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
            foreach (ParticleSystem ps in this.GetComponentsInChildren<ParticleSystem>())
            {
                ParticleSystem.EmissionModule e = ps.emission;
                e.rateOverTimeMultiplier = (e.rateOverTimeMultiplier * ((!prevparticlelowlevel) ? (1f / 8f) : 8f));
                e.rateOverDistanceMultiplier = (e.rateOverDistanceMultiplier * ((!prevparticlelowlevel) ? (1f / 4f) : 4f));

            }

            prevparticlelowlevel = GameManager.TheGameManager.UsingLowParticleEffects;
        }

    }

    public bool Autosensing = false;

    // Update is called once per frame
    public float DelayTime = .5f;
    private bool Detected = false;
    private float DetectTime = -10f;
    public Transform WobbleTransform;
    private Vector3 wobbledif;
    public Vector3 wobblescale;
    public float reformvalue = 0f;
    public void reform()
    {
        IceShard reformedshard = GameObject.Instantiate(this.gameObject).GetComponent<IceShard>();
        reformedshard.reformvalue = 0f;
        reformedshard.Reforming = true;
        reformedshard.Falling = false;
        reformedshard.Live = true;
        reformedshard.wobblescale = reformedshard.WobbleTransform.localScale;
        reformedshard.WobbleTransform.localScale = new Vector3(0f,0f,0f);
    }
    void FixedUpdate()
    {
        if (Live)
        {

            if (Reforming)
            {
                float df = Mathf.Clamp01(reformvalue+(Time.fixedDeltaTime*.25f));
                if (df >= 1f)
                {
                    WobbleTransform.localScale = wobblescale;
                    Reforming = false;
                    reformvalue = 0f;
                } else
                {
                    WobbleTransform.localScale = (wobblescale *Mathf.Pow(reformvalue,1f/3f));
                    reformvalue = df;
                }
            } else if (Falling)
            {

                if ((Time.time - FallTime) >= 5f)
                {

                    explode();
                }
            } else
            {
                if (Autosensing)
                {
                    if (Detected)
                    {
                        if (((Time.time - DetectTime)/DelayTime) < .5f)
                        {
                            WobbleTransform.position = (this.transform.position + wobbledif + new Vector3(Random.value * .1f, 0f, 0f));
                        } else
                        {
                            Detected = false;
                            WobbleTransform.position = (this.transform.position + wobbledif);
                            reform();
                            Falling = true;
                            FallTime = Time.time;
                            MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
                            
                        }

                    } else
                    {
                        Astronaut plr = Astronaut.TheAstronaut;

                        if ((plr != null) && (plr.Alive) && (!Astronaut.TheAstronaut.Quelling))
                        {


                            Vector3 dif = (plr.transform.position - this.transform.position);
                            if ((dif.magnitude < 30f) && (Mathf.Abs(dif.x) < 10f) && ((dif.y) < 0f))
                            {
                                RaycastHit2D rh = Physics2D.Linecast(this.transform.position, plr.transform.position, LayerMask.GetMask(new string[] { "Geometry" }));
                                //Debug.Log(rh.distance);
                                
                                if ((rh.distance <= 0f)|| true)
                                {
                                    //Line of sight
                                    if (dif.y < 0f)
                                    {
                                        float projectedfalltime = Mathf.Sqrt((this.transform.position.y - plr.transform.position.y) / 9.81f);
                                        if (Mathf.Abs(plr.MyRigidbody.velocity.x) > 0f) {
                                            float approachtime = ((this.transform.position.x - plr.transform.position.x)/ plr.MyRigidbody.velocity.x);
                                            if (approachtime > 0f) {
                                                float timerequired = (projectedfalltime + DelayTime);
                                                float threshold = .1f;
                                                if (Mathf.Abs(timerequired - approachtime) <= threshold)
                                                {
                                                    DetectTime = Time.time;
                                                    Detected = true;
                                                    FragmentTelegraph.Play();
                                                }


                                            }
                                        }
                                        

                                    }
                                    
                                }
                                
                            }
                        }

                        
                    }

                }
            }

            
        } else
        {
            transform.localScale = (transform.localScale * .5f);

        }
    }
    public Rigidbody2D MyRigidbody;
    public ParticleSystem ParticleTrail, IcicleExplosion,FragmentTelegraph;
    public void Remove()
    {
        
        IcicleExplosion.transform.SetParent(null);
        //ParticleExplosion.Stop();
        GameObject.Destroy(IcicleExplosion.gameObject, 5f);

        ParticleTrail.transform.SetParent(null);
        ParticleTrail.Stop();
        GameObject.Destroy(ParticleTrail.gameObject, 5f);

        GameObject.Destroy(this.gameObject);
    }
    public void explode()
    {
        if (!Live) return;
        Live = false;

        Am.am.oneshot(Am.am.M.chooseSound(Am.am.M.IcicleBreak1, Am.am.M.IcicleBreak2));
        IcicleExplosion.Play(true);
        MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
        Remove();
    }
    public bool Live = true;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Falling) return;
        Astronaut plr = collision.GetComponent<Astronaut>();

        if ((plr != null) && (plr.Alive) && (!plr.Invulnerable) && (Live))
        {

            Vector3 dif = (plr.transform.position - this.transform.position);
            plr.TakeDamage(10f, dif.normalized * 2f + new Vector3(0f, plr.JumpSpeed / 10f, 0f));
            explode();
        }
        else if ((collision.gameObject.CompareTag("Geometry")) || (collision.gameObject.GetComponent<IceBlock>() != null))
        {

            explode();
        }
    }

}
