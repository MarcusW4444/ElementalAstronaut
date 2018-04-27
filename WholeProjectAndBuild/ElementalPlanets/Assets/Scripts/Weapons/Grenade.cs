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
        DirectHit = null;
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
        TrailParticles.Stop(true);
    }
    private const float BLASTRADIUS = 6f; 
    public float Damage = 225f;
    public const float DIRECTDAMAGE = 75f;
    public const float MINDAMAGERATIO = .25f;
    private Vector3 ExplosionPosition=new Vector3();
    public void explode()
    {
        if (hasExploded) return;
        hasExploded = true;
        Am.am.oneshot(Am.am.M.GrenadeExplosion);
        ExplosionPosition = this.transform.position;
        GrenadeExplosion.Play(true);
        Astronaut.TheAstronaut.addCamShake(1f*(1f/(1f+((Astronaut.TheAstronaut.transform.position - ExplosionPosition).magnitude/8f))), .5f, 1f, .5f, 1f);
        MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
        
        Collider2D[] cols = Physics2D.OverlapCircleAll(this.transform.position, BLASTRADIUS, LayerMask.GetMask(new string[] { "Enemy", "Boss" }));
        
        foreach (Collider2D col in cols)
        {
            GenericEnemy ge = col.gameObject.GetComponent<GenericEnemy>();
            if (ge != null)
            {

                OnGrenadeHit(col);
                continue;
            }
            BossGolem bo = col.gameObject.GetComponent<BossGolem>();

            if (bo != null)
            {
                OnGrenadeHit(col);
                continue;
            }
        }
        Live = false;
        Remove();
    }
    public ParticleSystem TrailParticles;
    public ParticleSystem GrenadeExplosion;
    public ParticleSystem GeometryImpact;
    private bool removing = false;
    public void Remove()
    {
        if (removing) return;
        Live = false;
        if ((Astronaut.TheAstronaut.MyLaunchedGrenade != null) && (Astronaut.TheAstronaut.MyLaunchedGrenade.Equals(this)))
        {
            Astronaut.TheAstronaut.MyLaunchedGrenade = null;
        }
        removing = true;
        //Abandon the particles.
        TrailParticles.Stop();
        foreach (ParticleSystem p in this.gameObject.GetComponentsInChildren<ParticleSystem>())
        {
            
            p.transform.SetParent(null);
            GameObject.Destroy(p.gameObject,5f);

        }
        GameObject.Destroy(this.gameObject);
    }

    private Vector3 lastposition = new Vector3();
    public ParticleSystem CartoonImpact;//Draw some corny start particles
    private void OnGrenadeHit(Collider2D col)
    {
        GenericEnemy en = col.GetComponent<GenericEnemy>();
        Vector2 vec = new Vector2((this.transform.position.x - lastposition.x), (this.transform.position.y - lastposition.y));
        float dmg = Damage;
        if ((DirectHit != null) && col.Equals(DirectHit))
        {
            dmg = (Damage + DIRECTDAMAGE);
        } else
        {
            float mag = (col.transform.position - ExplosionPosition).magnitude;
            dmg = (Mathf.Lerp(Damage, Damage*MINDAMAGERATIO, Mathf.Clamp01(mag / BLASTRADIUS)));
        }
        if (en != null)
        {
            //if (col.isTrigger) return;
            en.TakeDamage(dmg, vec * 1f);
            
            Astronaut.TheAstronaut.tickHitMarker(dmg, (en.Health / en.MaxHealth) * (en.Alive ? 1f : 0f), !en.Alive);
            explode();
            //ParticleSystem p = GameObject.Instantiate(CartoonImpact, this.transform.position, Quaternion.LookRotation(-new Vector3(vec.x, vec.y, 0f)));
            //GameObject.Destroy(p.gameObject, 1f);
            
            return;
        }
        IceBlock ib = col.GetComponentInParent<IceBlock>();
        if (ib != null)
        {
            //be.TakeDamage(Damage, vec * 1f);//Damage
            //ParticleSystem p = GameObject.Instantiate(CartoonImpact, this.transform.position, Quaternion.LookRotation(-new Vector3(vec.x, vec.y, 0f)));
            ib.Remove();
            explode();
            Remove();
            //GameObject.Destroy(p.gameObject, 1f);


            return;
        }
        BossGolem bo = col.GetComponent<BossGolem>();
        bool weakspot = false;
        BossWeakSpot bwsp = col.GetComponent<BossWeakSpot>();
        if (bwsp != null)
        {
            bo = bwsp.MyBossGolem;
            weakspot = true;
        }
        if (bo != null)
        {

            if (col.Equals(bo.MyWeakSpot))
                weakspot = true;
            float di = dmg;
            if (weakspot)
            {
                di = (di * 2f);
                if (di >= 1f)
                {
                    bo.CriticalHitEffect.Emit(1);
                    bo.CriticalHitEffectSub.Emit(1);

                }
                else
                {
                    bo.damagelayover = (bo.damagelayover + dmg);
                    if ((bo.damagelayover) >= 1f)
                    {
                        bo.damagelayover -= 1f;
                        bo.CriticalHitEffect.Emit(1);
                        bo.CriticalHitEffectSub.Emit(1);
                    }
                }
                bo.TakeDamage(di, vec * 1f);//Damage
            }
            else
            {
                bo.TakeDamage(dmg, vec * 1f);//Damage
            }
            
            explode();
            Remove();
            //Astronaut.TheAstronaut.tickHitMarker(Damage, (bo.Health / bo.MaxHealth) * (bo.Defeated ? 0f : 1f), bo.Defeated);
            //ParticleSystem p = GameObject.Instantiate(CartoonImpact, this.transform.position, Quaternion.LookRotation(-new Vector3(vec.x, vec.y, 0f)));
            //GameObject.Destroy(p.gameObject, 1f);

            return;
        }
        BreakableIceWall be = col.GetComponent<BreakableIceWall>();
        if (be != null)
        {
            be.TakeDamage(dmg, vec * 1f);//Damage
            Astronaut.TheAstronaut.tickHitMarker(dmg, (be.Health / be.MaxHealth) * (be.Alive ? 1f : 0f), !be.Alive);
            explode();
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


        explode();
        
        Remove();


    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!Live) return;
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Intangible"))) return;
        if (collision.gameObject.layer.Equals(LayerMask.NameToLayer("Typhoon"))) return;
        Vector2 vec = new Vector2((this.transform.position.x - lastposition.x), (this.transform.position.y - lastposition.y));
        ParticleSystem ps = GameObject.Instantiate(GeometryImpact, this.transform.position, Quaternion.LookRotation(-new Vector3(vec.x, vec.y, 0f)));
        ps.Play(true);
        GameObject.Destroy(ps.gameObject, 1f);
        //Collision with geometry
        DirectHit = collision.collider;
        OnGrenadeHit(collision.collider);
        //Remove();
    }
    public Collider2D DirectHit = null;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!Live) return;
        
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Intangible"))) return;
        if (other.gameObject.layer.Equals(LayerMask.NameToLayer("Typhoon"))) return;
        
        Vector2 vec = new Vector2((this.transform.position.x - lastposition.x), (this.transform.position.y - lastposition.y));
        ParticleSystem ps = GameObject.Instantiate(GeometryImpact, this.transform.position, Quaternion.LookRotation(-new Vector3(vec.x, vec.y, 0f)));
        DirectHit = other;
        ps.Play(true);
        GameObject.Destroy(ps.gameObject, 1f);
        OnGrenadeHit(other);
        //Remove();
    }
}
