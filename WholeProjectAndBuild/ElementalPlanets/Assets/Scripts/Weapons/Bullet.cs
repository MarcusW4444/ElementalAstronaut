using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    // Use this for initialization
    public bool Live = true;
    public float Damage = 10f;
    public TrailRenderer MyTrailRenderer;
    public ParticleSystem GeometryImpact, DamageImpact,RailPierceParticles,RailGlow,DustTrail;
    private float StartTime = -10f;
    public Rigidbody2D MyRigidbody;
    public Vector3 Interference = new Vector3();
    public bool RailSlug = false;
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
    public Vector2 Velocity = new Vector2();

    void FixedUpdate()
    {
        if (Live)
        {
            
            //Velocity = MyRigidbody.velocity;
            Vector2 vm = ((Velocity * Time.fixedDeltaTime)+ (new Vector2(Interference.x, Interference.y) *Time.fixedDeltaTime));
            if (vm.magnitude > 0)
            {
                
                //Debug.Log("Bullet");
                bool hitsolid = false;
                Ray2D r = new Ray2D(new Vector2(transform.position.x, transform.position.y),vm);
                RaycastHit2D rs = Physics2D.Raycast(r.origin, r.direction, vm.magnitude, LayerMask.GetMask(new string[] { "Geometry" }));
                float mdist = vm.magnitude;
                if (rs.collider != null)
                {

                    mdist = rs.distance;
                    hitsolid = true;
                }
                if (RailSlug)
                {

                    RaycastHit2D[] rayhits = Physics2D.RaycastAll(r.origin, r.direction, mdist, LayerMask.GetMask(new string[] { "Enemy", "Boss" }));

                    foreach (RaycastHit2D rh in rayhits)
                    {
                        if (rh.collider != null)
                        {
                            this.transform.position = (this.transform.position + (new Vector3(r.direction.x, r.direction.y, 0f) * rh.distance));
                            if (rh.collider.isTrigger)
                            {
                                Debug.Log("Trigger: " + rh.collider.gameObject.name);
                            }
                            ParticleSystem p = GameObject.Instantiate(RailPierceParticles, new Vector3(rh.point.x, rh.point.y,0f), Quaternion.LookRotation(-new Vector3(Velocity.x, Velocity.y, 0f).normalized));
                            GameObject.Destroy(p.gameObject, 1f);
                            OnBulletHit(rh.collider);
                            
                        }
                        else
                        {
                            
                        }
                    }

                    /*
                    if (rs.collider != null)
                    {
                        this.transform.position = (this.transform.position + (new Vector3(r.direction.x, r.direction.y, 0f) * rs.distance));
                        ParticleSystem p = GameObject.Instantiate(RailPierceParticles, new Vector3(rh.transform.position.x, rh.transform.position.y, 0f), Quaternion.LookRotation(-new Vector3(Velocity.x, Velocity.y, 0f).normalized));
                        GameObject.Destroy(p.gameObject, 1f);
                        OnBulletHit(rs.collider);
                    }
                    */
                    
                    

                    if (hitsolid)
                    {
                        Vector2 vec = new Vector2((this.transform.position.x - lastposition.x), (this.transform.position.y - lastposition.y));
                        this.transform.position = rs.point;
                        ParticleSystem ps = GameObject.Instantiate(GeometryImpact, this.transform.position, Quaternion.LookRotation(-new Vector3(vec.x, vec.y, 0f)));
                        GameObject.Destroy(ps.gameObject, 1f);
                        //Collision with geometry

                        Remove();
                    } else
                    {
                        this.transform.position = (this.transform.position + (new Vector3(r.direction.x, r.direction.y, 0f) * vm.magnitude));
                    }


                }
                else
                {
                    RaycastHit2D rh = Physics2D.Raycast(r.origin, r.direction, mdist, LayerMask.GetMask(new string[] { "Enemy", "Boss" }));

                    if (rh.collider != null)
                    {
                        this.transform.position = (this.transform.position + (new Vector3(r.direction.x, r.direction.y, 0f) * rh.distance));
                        if (rh.collider.isTrigger)
                        {
                            Debug.Log("Trigger: " + rh.collider.gameObject.name);
                        }
                        OnBulletHit(rh.collider);
                    }
                    else
                    {
                        if (rs.collider != null)
                        {
                            this.transform.position = (this.transform.position + (new Vector3(r.direction.x, r.direction.y, 0f) * rs.distance));
                            OnBulletHit(rs.collider);
                        }
                        else
                        {
                            this.transform.position = (this.transform.position + (new Vector3(r.direction.x, r.direction.y, 0f) * vm.magnitude));
                        }
                    }
                }
                

                
            }

        }

        Interference = new Vector3();
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
        GameObject.Destroy(MyTrailRenderer.gameObject, 1f);
        if (RailSlug)
        {
            DustTrail.Stop();
            DustTrail.transform.SetParent(null);
            GameObject.Destroy(DustTrail.gameObject,4f);
            RailGlow.Stop();
            RailGlow.transform.SetParent(null);
            GameObject.Destroy(RailGlow.gameObject, 4f);
        } 
        GameObject.Destroy(this.gameObject);

    }
    private float railhitfactor;
    private void OnBulletHit(Collider2D col)
    {
        if (col == null) return;
        GenericEnemy en = col.GetComponent<GenericEnemy>();
        Vector2 vec = new Vector2((this.transform.position.x - lastposition.x), (this.transform.position.y - lastposition.y));
        if (en != null)
        {
            //if (col.isTrigger) return;
            en.TakeDamage(Damage, vec * 1f);
            Astronaut.TheAstronaut.tickHitMarker(Damage, (en.Health/en.MaxHealth)*(en.Alive?1f:0f), !en.Alive);
            GameManager.TheGameManager.ignoreTutorialTip(TutorialSystem.TutorialTip.Shoot);
            if (RailSlug)
            {
                ParticleSystem p = GameObject.Instantiate(RailPierceParticles, this.transform.position, Quaternion.LookRotation(-new Vector3(vec.x, vec.y, 0f)));
                GameObject.Destroy(p.gameObject, 1f);
            }
            else
            {
                ParticleSystem p = GameObject.Instantiate(DamageImpact, this.transform.position, Quaternion.LookRotation(-new Vector3(vec.x, vec.y, 0f)));
                GameObject.Destroy(p.gameObject, 1f);
                Remove();
            }
            return;
        }
        IceBlock ib = col.GetComponentInParent<IceBlock>();
        if (ib != null)
        {
            //be.TakeDamage(Damage, vec * 1f);//Damage
            ParticleSystem p = GameObject.Instantiate(DamageImpact, this.transform.position, Quaternion.LookRotation(-new Vector3(vec.x, vec.y, 0f)));
            ib.Remove();
            if (RailSlug)
            {
                
            }
            else
            {
                GameObject.Destroy(p.gameObject, 1f);

                Remove();
            }
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
            float dmg = Damage;
            if (weakspot)
            {
                float di = (dmg*2f);
                if (di >= 1f)
                {
                    bo.CriticalHitEffect.Emit(1);
                    bo.CriticalHitEffectSub.Emit(1);
                    bo.CriticalSparks.Emit((int)(20*(dmg/40f)));
                    Astronaut.PlayBossCriticalHitSound(bo.Health / bo.MaxHealth);
                    Astronaut.TheAstronaut.tickHitMarker(dmg, (bo.Health / bo.MaxHealth) * (bo.Defeated ? 1f : 0f), bo.Defeated);
                } else
                {
                    bo.damagelayover = (bo.damagelayover + di);
                    if ((bo.damagelayover) >= 1f)
                    {
                        bo.damagelayover -= 1f;
                        bo.CriticalHitEffect.Emit(1);
                        bo.CriticalHitEffectSub.Emit(1);
                        bo.CriticalSparks.Emit(20);
                        Astronaut.PlayBossCriticalHitSound(bo.Health/bo.MaxHealth);
                        Astronaut.TheAstronaut.tickHitMarker(dmg, (bo.Health / bo.MaxHealth) * (bo.Defeated ? 1f : 0f), bo.Defeated);
                    }
                }
                bo.TakeDamage(di, vec * 1f);//Damage
            }
            else
            {
                bo.TakeDamage(dmg, vec * 1f);//Damage
            }
            //Astronaut.TheAstronaut.tickHitMarker(Damage, (bo.Health / bo.MaxHealth) * (bo.Defeated ? 0f : 1f), bo.Defeated);
            weakspot = false;
            if (RailSlug)
            {

            }
            else
            {
                ParticleSystem p = GameObject.Instantiate(DamageImpact, this.transform.position, Quaternion.LookRotation(-new Vector3(vec.x, vec.y, 0f)));
                GameObject.Destroy(p.gameObject, 1f);
                Remove();
            }
            return;
        }
        BreakableIceWall be = col.GetComponent<BreakableIceWall>();
        if (be != null)
        {
            be.TakeDamage(Damage, vec * 1f);//Damage
            Astronaut.TheAstronaut.tickHitMarker(Damage, (be.Health / be.MaxHealth) * (be.Alive ? 1f : 0f), !be.Alive);
            if (RailSlug)
            {

            }
            else
            {
                ParticleSystem p = GameObject.Instantiate(DamageImpact, this.transform.position, Quaternion.LookRotation(-new Vector3(vec.x, vec.y, 0f)));
                GameObject.Destroy(p.gameObject, 1f);
                Remove();
            }
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
    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnBulletHit(collision.collider);
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        OnBulletHit(other);
    }
    */



}
