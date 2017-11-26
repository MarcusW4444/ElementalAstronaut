using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour {

    // Use this for initialization
    public Rigidbody2D MyRigidbody;
    public bool Live = true;
    public bool Sustaining = true;
    void Start () {
        Live = true;
	}

    // Update is called once per frame
    private void FixedUpdate()
    {

    }
    private bool hasExploded = false;
    public void detonate()
    {

    }
    
    public void releaseSustain()
    {

    }
    private const float BLASTRADIUS = 2f;
    public float Damage = 225f;
    public const float DIRECTDAMAGE = 75f;

    public void explode()
    {
        Collider2D[] cols = Physics2D.OverlapCircleAll(this.transform.position, BLASTRADIUS, LayerMask.GetMask(new string[] { "Enemy", "Boss" }));
        foreach (Collider2D col in cols)
        {
            GenericEnemy ge = col.gameObject.GetComponent<GenericEnemy>();
            if (ge != null)
            {
                
                continue;
            }
            BossGolem bo = col.gameObject.GetComponent<BossGolem>();

            if (bo != null)
            {

                continue;
            }
        }
    }
    public ParticleSystem GeometryImpact;

    private void Remove()
    {
        //Abandon the particles.

        GameObject.Destroy(this.gameObject);
    }

    private Vector3 lastposition = new Vector3();
    public ParticleSystem CartoonImpact;//Draw some corny start particles
    private void OnGrenadeHit(Collider2D col)
    {
        GenericEnemy en = col.GetComponent<GenericEnemy>();
        Vector2 vec = new Vector2((this.transform.position.x - lastposition.x), (this.transform.position.y - lastposition.y));
        if (en != null)
        {
            if (col.isTrigger) return;
            en.TakeDamage(Damage, vec * 1f);
            Astronaut.TheAstronaut.tickHitMarker(Damage, (en.Health / en.MaxHealth) * (en.Alive ? 1f : 0f), !en.Alive);

            //ParticleSystem p = GameObject.Instantiate(CartoonImpact, this.transform.position, Quaternion.LookRotation(-new Vector3(vec.x, vec.y, 0f)));
            //GameObject.Destroy(p.gameObject, 1f);
            Remove();
            return;
        }
        IceBlock ib = col.GetComponentInParent<IceBlock>();
        if (ib != null)
        {
            //be.TakeDamage(Damage, vec * 1f);//Damage
            //ParticleSystem p = GameObject.Instantiate(CartoonImpact, this.transform.position, Quaternion.LookRotation(-new Vector3(vec.x, vec.y, 0f)));
            ib.Remove();
            //GameObject.Destroy(p.gameObject, 1f);

            Remove();
            return;
        }
        BossGolem bo = col.GetComponent<BossGolem>();
        if (bo != null)
        {
            bo.TakeDamage(Damage, vec * 1f);//Damage
            //Astronaut.TheAstronaut.tickHitMarker(Damage, (bo.Health / bo.MaxHealth) * (bo.Defeated ? 0f : 1f), bo.Defeated);
            //ParticleSystem p = GameObject.Instantiate(CartoonImpact, this.transform.position, Quaternion.LookRotation(-new Vector3(vec.x, vec.y, 0f)));
            //GameObject.Destroy(p.gameObject, 1f);
            Remove();
            return;
        }
        BreakableIceWall be = col.GetComponent<BreakableIceWall>();
        if (be != null)
        {
            be.TakeDamage(Damage, vec * 1f);//Damage
            Astronaut.TheAstronaut.tickHitMarker(Damage, (be.Health / be.MaxHealth) * (be.Alive ? 1f : 0f), !be.Alive);
            //ParticleSystem p = GameObject.Instantiate(CartoonImpact, this.transform.position, Quaternion.LookRotation(-new Vector3(vec.x, vec.y, 0f)));
            //GameObject.Destroy(p.gameObject, 1f);
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



        ParticleSystem ps = GameObject.Instantiate(GeometryImpact, this.transform.position, Quaternion.LookRotation(-new Vector3(vec.x, vec.y, 0f)));
        GameObject.Destroy(ps.gameObject, 1f);
        //Collision with geometry
        Remove();


    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnGrenadeHit(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        OnGrenadeHit(other);
    }
}
