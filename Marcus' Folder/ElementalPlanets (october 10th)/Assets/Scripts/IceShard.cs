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
    }

    public bool Autosensing = false;

    // Update is called once per frame
    public float DelayTime = .5f;
    private bool Detected = false;
    private float DetectTime = -10f;
    public Transform WobbleTransform;
    private Vector3 wobbledif;
    void FixedUpdate()
    {
        if (Live)
        {


            if (Falling)
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
                            WobbleTransform.position = (this.transform.position + wobbledif);
                            Falling = true;
                            FallTime = Time.time;
                            MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
                        }

                    } else
                    {
                        Astronaut plr = Astronaut.TheAstronaut;

                        if ((plr != null) && (plr.Alive))
                        {


                            Vector3 dif = (plr.transform.position - this.transform.position);
                            if (dif.magnitude < 30f)
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
