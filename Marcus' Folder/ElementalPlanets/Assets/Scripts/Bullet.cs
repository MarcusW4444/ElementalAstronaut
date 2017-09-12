using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    // Use this for initialization
    public bool Live = true;
    public float Damage = 10f;
    public TrailRenderer MyTrailRenderer;
    public ParticleSystem GeometryImpact, DamageImpact;
    private float StartTime = -10f;
    public Rigidbody2D MyRigidbody;

	void Start () {
        StartTime = Time.time;
        lastposition = this.transform.position;

    }

    // Update is called once per frame
    private const float Lifetime = 3f;
	void Update () {
		
        if (Live)
        {
            if ((Time.time - StartTime) >= Lifetime)
            {
                Remove();
            }
        } else
        {

        }
	}

    private Vector3 lastposition = new Vector3();
    private int fr = 0;
    private void LateUpdate()
    {
        fr++;
        //Debug.Log(fr);
        lastposition = this.transform.position;
        //bool t = ((Live && (fr >= 3)));
        //this.MyRigidbody.simulated = t;
        //this.MyRigidbody.bodyType = ((t) ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic);
    }
    
    public void Remove()
    {
        Live = false;
        MyTrailRenderer.transform.SetParent(null);
        this.MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
        GameObject.Destroy(MyTrailRenderer.gameObject,1f);
        GameObject.Destroy(this.gameObject);
    }

    private void OnBulletHit(Collider2D col)
    {
        GenericEnemy en = col.GetComponent<GenericEnemy>();
        Vector2 vec = new Vector2((this.transform.position.x - lastposition.x), (this.transform.position.y - lastposition.y));
        if (en != null)
        {
            en.TakeDamage(Damage,vec*4f);//Damage
            ParticleSystem p = GameObject.Instantiate(DamageImpact, this.transform.position, Quaternion.LookRotation(-new Vector3(vec.x, vec.y, 0f)));
            GameObject.Destroy(p.gameObject, 1f);
            Remove();
            return;
        }

        if (col.gameObject.CompareTag("Water"))
        {
            //Splash
            MyRigidbody.velocity = (MyRigidbody.velocity * .5f);
            Damage *= .5f;
            return;
        }

        ParticleSystem ps = GameObject.Instantiate(GeometryImpact,this.transform.position,Quaternion.LookRotation(-new Vector3(vec.x,vec.y,0f)));
        GameObject.Destroy(ps.gameObject,1f);
        //Collision with geometry
        Remove();


    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnBulletHit(collision.collider);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        OnBulletHit(other);
    }



}
