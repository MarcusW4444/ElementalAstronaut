using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResistanceDrop : MonoBehaviour {

    // Use this for initialization
    public bool Live = true;
    public Rigidbody2D MyRigidbody;
    public Astronaut.Element ElementValue = Astronaut.Element.None;
    public float ResistanceValue = .1f;
    public SpriteRenderer MySpriteRenderer;
    public float SizeValue = 1f;
    private float StartTime = -10f;
    private const float HOMINGSPEED = 10f;
    public float ValueValue = 1f;
    public bool UsingDelay = false;
	void Start () {
        StartTime = Time.time;
        originalscale = this.transform.localScale;
    }
    public bool Removing = false;
    public ParticleSystem ParticleTrail;
    public void removeParticles()
    {
        if (ParticleTrail == null) return;
        ParticleTrail.Stop();
        ParticleTrail.transform.SetParent(null);
        GameObject.Destroy(ParticleTrail.gameObject, 5f);
        ParticleTrail = null;
    }
    public void Remove()
    {
        Live = false;
        Removing = true;
        MyRigidbody.velocity = new Vector2();
        MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
        removeParticles();


        ValueValue = 1f;
    }
    private Vector3 originalscale;
    // Update is called once per frame
    public bool Homing = false;
    private float HomingTime = -10f;
    void Update () {

        if (Removing)
        {
            this.transform.localScale = (this.transform.localScale * .8f);
            
            if (this.transform.localScale.x < .001f)
            {
                GameObject.Destroy(this.gameObject);
                return;
            }


        }
        else
        {
           
                RaycastHit2D rh = Physics2D.Linecast(this.transform.position, Astronaut.TheAstronaut.transform.position, LayerMask.GetMask(new string[] { "Geometry" }));

                bool cansee = (rh.distance <= 0f);
                
                    Vector3 dif = (Astronaut.TheAstronaut.transform.position - this.transform.position);


                float dist = dif.magnitude;
                float delt = (HOMINGSPEED * Time.deltaTime*((Time.time - HomingTime)*4f));
                if (Homing)
                {
                    MyRigidbody.velocity = new Vector2();
                    MyRigidbody.bodyType = RigidbodyType2D.Kinematic;

                if (!Astronaut.TheAstronaut.Alive)
                {
                    Homing = false;

                    foreach (Collider2D c in GetComponents<Collider2D>())
                    {
                        if (!c.isTrigger) c.enabled = true;
                    }
                }
                else
                {

                    if (dist < delt)
                    {
                        this.transform.position = Astronaut.TheAstronaut.transform.position;

                        Astronaut.TheAstronaut.absorbResistance(this);
                    }
                    else
                    {
                        float sp = delt;
                        this.transform.position = (this.transform.position + (dif.normalized * delt));
                    }
                }
                }
                else
                {
                    MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
                    
                    if (Astronaut.TheAstronaut.Alive)
                    if ((!UsingDelay) ||((Time.time - Astronaut.TheAstronaut.ReviveTime) >= 1.5f))
                    if (dist < 1.5f) 
                    {
                        Homing = true;
                        HomingTime = Time.time;
                        foreach (Collider2D c in GetComponents<Collider2D>())
                        {
                            if (!c.isTrigger) c.enabled = false;
                        }
                    }

                    

                    if ((ValueValue*ResistanceValue) < .0001f)//((Time.time - StartTime) >= 5f)
                    {
                        Remove();
                    }
                }
            
        }

        
	}
    public const float LIFETIMECONSTANT = 4f;
    public float Lifetime = 1f;
    void FixedUpdate()
    {
        if (!Removing)
        {
            ValueValue = Mathf.Max(0f, ValueValue - (Time.fixedDeltaTime / (LIFETIMECONSTANT*Lifetime)));

            this.transform.localScale = (originalscale*ValueValue);
            //this.ResistanceValue = (this.ResistanceValue*ValueValue);
        }
    }

}

