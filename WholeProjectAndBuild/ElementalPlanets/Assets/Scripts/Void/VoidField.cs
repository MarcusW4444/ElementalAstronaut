using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidField : MonoBehaviour {

    // Use this for initialization
    public float StartTime = -10f;
    public float Duration = 2f;
    public float ForceFactor = 1f;
    public Collider2D MyCollider;
    public bool Live = true;
	void Start () {
        StartTime = Time.time;
    }

    // Update is called once per frame
    private bool removing = false;
	void Update () {
		if (removing)
        {
            bool readytogo = true;
            foreach (ParticleSystem ps in this.gameObject.GetComponentsInChildren<ParticleSystem>())
            {
                if (ps.isPlaying)
                {
                    ps.Stop();
                    readytogo = false;
                    
                } else
                {

                }
            }
            if (readytogo)
            {
                GameObject.Destroy(this.gameObject);
            }

        } else
        {
            if (((Time.time - StartTime) >= Duration) && (Duration > 0f))
            {
                remove();
            }

        }
	}

    public ParticleSystem FieldParticles, SuckRings,CentroidParticles;
    public bool Active = false;
    public void Activate()
    {
        if (Active) return;
        Active = true;
        FieldParticles.Play();
        SuckRings.Play();
        CentroidParticles.Play();
        MyCollider.enabled = true;
    }
    public void Deactivate()
    {
        if (!Active) return;
        Active = false;
        FieldParticles.Stop();
        SuckRings.Stop();
        CentroidParticles.Stop();
        MyCollider.enabled = false;
    }

    public void remove()
    {
        if (removing) return;
        removing = true;
        Live = false;
        MyCollider.enabled = false;
        Deactivate();
    }

    public float ContactDPS = 4f;
    public float ContactRatio = .25f;
    

    private void FixedUpdate()
    {
        if (Live && Active)
        {
            Astronaut a = Astronaut.TheAstronaut;
            if (a != null)
            {
                Vector2 dif = (new Vector2(this.transform.position.x, this.transform.position.y) - a.MyRigidbody.position);
                float r = Mathf.Clamp01(1f - (dif.magnitude / (MyCollider as CircleCollider2D).bounds.extents.x));
                if (r > 0f)
                {



                    float succdps = ContactDPS;
                    if (a.MyRigidbody.bodyType == RigidbodyType2D.Dynamic)
                        a.MyRigidbody.position = (a.MyRigidbody.position+(dif*(1f-r)*Time.fixedDeltaTime));
                    //a.MyRigidbody.velocity = (a.MyRigidbody.velocity + ((dif.normalized * (Time.fixedDeltaTime * 100f * ForceFactor))));//best
                    //AddForce//(getForce(a.transform.position) * .25f * (a.Airborne ? 1f : 5f));
                    if (a.Alive)
                        if (r > ContactRatio)
                        {

                            //a.TakeDamage(succdps * r * Time.fixedDeltaTime, new Vector3());
                        }
                }
            }
        }
    }

    public Vector3 getForce(Vector3 pos)
    {
        //SINUSOID SPAM PLS
        //Vector3 sinenoise = new Vector3(0f, Mathf.Cos(pos.x * CosVariator) * Mathf.Sin(pos.x * SineVariator), 0f);
        Vector3 dif = (this.transform.position - pos);

        return (dif.normalized * (Time.fixedDeltaTime * 4f * ForceFactor));//(new Vector3(-1f, 1f + Mathf.Sin(pos.x), 0f) * 20f);
    }

    public void OnTriggerTouch(Collider2D col)
    {
        if ((!Live) || (!Active)) return;
        //Detect for:
        //Astronaut
        //Bullets and Grenades
        //Spores
        Bullet b = col.gameObject.GetComponent<Bullet>();
        if ((b != null) && (b.Live))
        {
            //Debug.Log(col.gameObject.name);
            float r = (MyCollider as CircleCollider2D).bounds.extents.x;
            Vector3 off = (new Vector3(b.MyRigidbody.position.x, b.MyRigidbody.position.y,0f) - new Vector3(this.transform.position.x, this.transform.position.y,0f));
            float dot = Vector3.Dot(new Vector3(b.Velocity.x, b.Velocity.y,0f).normalized, off.normalized);
            //Vector3.Rot
            Vector3 crs = Vector3.Cross(off.normalized, Vector3.forward);
            Vector3 vc = crs * Mathf.Sign(Vector3.Dot(crs, b.Velocity.normalized)) * b.Velocity.magnitude;
            b.Velocity = (Vector2.Lerp(new Vector2(vc.x,vc.y),b.Velocity, Mathf.Abs(dot)).normalized *b.Velocity.magnitude);//Vector2.Reflect(b.Velocity,-off.normalized);
            //b.Velocity = Vector2.Lerp(b.Velocity,,Mathf.Abs(dot)).normalized*b.Velocity.magnitude; //b.Interference += getForce(b.transform.position) * .5f;
            return;
        }
        Grenade g = col.gameObject.GetComponent<Grenade>();
        if ((g != null) && (g.Live))
        {
            //Debug.Log(col.gameObject.name);
            //Vector3 f = getForce(g.transform.position);
            //g.MyRigidbody.AddForce(new Vector2(f.x, f.y) * .25f);
            float r = (MyCollider as CircleCollider2D).bounds.extents.x;
            Vector3 off = (new Vector3(g.MyRigidbody.position.x, g.MyRigidbody.position.y, 0f) - new Vector3(this.transform.position.x, this.transform.position.y, 0f));
            float dot = Vector3.Dot(new Vector3(g.MyRigidbody.velocity.x, g.MyRigidbody.velocity.y, 0f).normalized, off.normalized);
            //Vector3.Rot
            Vector3 crs = Vector3.Cross(off.normalized, Vector3.forward);
            Vector3 vc = crs * Mathf.Sign(Vector3.Dot(crs, g.MyRigidbody.velocity.normalized)) * g.MyRigidbody.velocity.magnitude;
            b.Velocity = (Vector2.Lerp(new Vector2(vc.x, vc.y), g.MyRigidbody.velocity, Mathf.Abs(dot)).normalized * g.MyRigidbody.velocity.magnitude);//Vector2.Reflect(b.Velocity,-off.normalized);
            //g.Interference += getForce(b.transform.position);
            return;
        }
        
        /*
        TyphoonWindStreak ws = col.gameObject.GetComponent<TyphoonWindStreak>();
        if (ws != null)
        {
            //ws.MyRigidbody.AddForce(getForce(ws.transform.position));
            Vector3 f = (getForce(b.transform.position));
            //ws.MyRigidbody.velocity += (new Vector2(f.x, f.y)*Time.fixedDeltaTime*1f);
            return;
        }
        */
        Astronaut a = col.gameObject.GetComponent<Astronaut>();
        if (a != null)
        {
            //Ignore this. the actual code is in FixedUpdate()
            return;
            Vector2 dif = (new Vector2(this.transform.position.x, this.transform.position.y) - a.MyRigidbody.position);
            float r = Mathf.Clamp01(1f-(dif.magnitude/(MyCollider as CircleCollider2D).radius));
            float succdps = 4f;

            a.MyRigidbody.velocity = (a.MyRigidbody.velocity+((dif.normalized*(Time.fixedDeltaTime*2f  *ForceFactor*(1f+(3f*Astronaut.AggressionLevelF))))));//AddForce//(getForce(a.transform.position) * .25f * (a.Airborne ? 1f : 5f));
            if (a.Alive)
            if (r > 0f) {
                    a.TakeDamage(succdps * r*Time.fixedDeltaTime,new Vector3());
                    }
            //Also deal damage over time
            
        }


    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        
        OnTriggerTouch(collision);
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerTouch(collision);
    }


}
