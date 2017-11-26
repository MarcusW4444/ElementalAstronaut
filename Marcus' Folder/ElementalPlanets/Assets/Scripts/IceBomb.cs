using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBomb : MonoBehaviour {

    // Use this for initialization
    public bool Live = true;
    public bool Launched = false;
    public float BombCharge = 0f; //min to max
    public ParticleSystem MaxEffects;
    public ParticleSystem GlowEffect;
    public ParticleSystem SnowTrail;
    public bool Permafrost = false;
    public bool Cryonova = false;
    public int IcePowerLevel=0;
    public bool CanFreezeEnemies,CanFreezeHazards,CanFreezeProjectiles,BlizzardField;    
    public bool FormsIcePillar;
    public GameObject IcePillarPrefab;
    public FrozenProjectile FrozenProjectilePrefab;
    public ParticleSystem IceExplosion,FreezePoof;
    public Collider2D MyCollider;
    public Collider2D MyFrostAreaCollider;
    public Rigidbody2D MyRigidbody;

    void Start () {
        BombCharge = 0f;
	}

    // Update is called once per frame
    public bool useGravityOnRelease;
    void Release(Vector3 vel)
    {
        if (Launched) return;
        {
            Launched = true;
            MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
            MyRigidbody.velocity = vel;
            MyCollider.enabled = true; //It can now collide with enemies and geometry
        }
    }
	void Update () {
		
	}

    private void FixedUpdate()
    {
        if (Launched && Live)
        {
            if (!removing)
            {
                
                if ((Time.time - launchTime) >= 3f)
                {

                    Remove();
                }
            }

        } else
        {
            //...


        }

        if (removing)
        {
            
            GameObject.Destroy(this.gameObject);
        } else
        {
            if (BlizzardField)
            {
                if (Launched)
                {
                    MyFreezeField.enabled = true;
                }
                if (!FreezeFieldEffect.isPlaying) FreezeFieldEffect.Play();
            } else
            {

                MyFreezeField.enabled = false;
                if (FreezeFieldEffect.isPlaying) FreezeFieldEffect.Stop();
            }

            if (CanFreezeProjectiles)
            {

                if (!IceGlow.isPlaying) IceGlow.Play();
            }

            if (CanFreezeHazards)
            {

                if (!IceFlare.isPlaying) IceFlare.Play();
            }
        }
    }
    public Collider2D MyFreezeField;
    public ParticleSystem FreezeFieldEffect, IceGlow, IceFlare;
    private bool removing = false;
    private float removetime = -10f;
    private float launchTime = -10f;
    public void Launch(Vector3 dir)
    {
        if (Launched) return;
        Launched = true;
        MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
        MyRigidbody.velocity = dir;
        launchTime = Time.time;


    }
    public void Remove()
    {
        if (removing) return;
        removing = true;
        Live = false;
        removetime = Time.time;
        foreach (ParticleSystem p in this.GetComponentsInChildren<ParticleSystem>())
        {
            if (!p.transform.Equals(this.transform))
            {
                p.transform.SetParent(null);
                if (p.main.loop)
                    p.Stop();
                

            }
            else
            {
                //p.enabled = false;
                p.Stop();
            }
            GameObject.Destroy(p.gameObject, p.main.duration);
        }
    }
    private bool Exploded = false;
    public void explode()
    {
        if (Exploded) return;

        IceExplosion.Play();
        foreach (GenericEnemy en in GameObject.FindObjectsOfType<GenericEnemy>())
        {
            float dif = ((this.transform.position - en.transform.position).magnitude/ Mathf.Max(MyFrostAreaCollider.bounds.extents.magnitude,0.0001f));

            if (dif <= 1f)
            {
                en.freeze(.5f+(2f*(1f-dif)));
            }

        }
    }

    public void freezeProjectile(EnemyProjectile proj)
    {

        proj.Live = false;
        proj.enabled = false;
        FrozenProjectile frp = GameObject.Instantiate(FrozenProjectilePrefab, proj.transform.position, proj.transform.rotation).GetComponent<FrozenProjectile>();
        FreezePoof.Play();
        foreach (ParticleSystem p in proj.gameObject.GetComponentsInChildren<ParticleSystem>())
        {
            if (!p.transform.Equals(this.transform))
            {
                p.transform.SetParent(null);
                if (p.main.loop)
                    p.Stop();


            }
            else
            {
                //p.enabled = false;
                p.Stop();
            }
            GameObject.Destroy(p.gameObject, p.main.duration);
        }
        frp.transform.SetParent(proj.transform);
        frp.transform.position = frp.transform.position + new Vector3(0f, 0f, -2f);
    }
    public void freezeHazard(Hazard haz)
    {
        haz.permafreeze();
        
        IceExplosion.Play();
    }
    private void OnTriggerEnter2D(Collider2D col)
    {

        //collision with a projectile
        //collision with an enemy
        //IceSkullProjectile
        //Enemy_Projectile p
        if (!Launched)
        {
            EnemyProjectile proj = col.gameObject.GetComponent<EnemyProjectile>();
            if ((proj != null) && (proj.enabled) && (proj.Live))
            {
                freezeProjectile(proj);
                GameObject.Destroy(proj.gameObject,10f);
                
                return;
            }
        }

        GenericEnemy en = col.gameObject.GetComponent<GenericEnemy>();
        if ((en != null) && (en.Alive) && (!col.isTrigger))
        {


            
            if (!Launched){
                if (BlizzardField)
                {
                    en.freeze(1.2f);
                    FreezePoof.Play();
                }

            }


            return;
        }

    }
    private void LateUpdate()
    {
        prevel = MyRigidbody.velocity;
    }
    private Vector2 prevel=new Vector2(0f,0f);
    private void OnCollisionEnter2D(Collision2D col)
    {
        
        if (removing) return;
        //collision with geometry
        //collision.contacts[0].normal

        GenericEnemy en = col.gameObject.GetComponent<GenericEnemy>();
        if ((en != null) && (en.Alive) && (!col.collider.isTrigger))
        {
            
            if (CanFreezeEnemies)
            if (Launched)
            {
                en.freeze(1.2f);
                FreezePoof.Play();
                Remove();
            } else
            {
                /*
                if (FreezeFieldWhileHeld)
                {
                    en.freeze(1.2f);
                    FreezePoof.Play();
                }
                */
            }

            
            return;
        }

        EnemyProjectile proj = col.gameObject.GetComponent<EnemyProjectile>();
        if ((proj != null)&&(proj.enabled) &&(proj.Live) &&(CanFreezeProjectiles))
        {

            freezeProjectile(proj);
            Remove();
            return;
        }

        Hazard haz = col.gameObject.GetComponent<Hazard>();

        if ((haz != null) && (haz.enabled) && (!haz.Permafrozen))
        {
            if (CanFreezeHazards)
            {
                freezeHazard(haz);
            }
            Remove();
            return;
        }
        //EnvironmentHazard

        if (col.gameObject.layer == LayerMask.NameToLayer("Geometry"))
        {
            //Hit a wall.
            //Create Ice on this wall
            if (FormsIcePillar)
            {
                Vector2 ndir = col.contacts[0].normal;
                IcePillar p = GameObject.Instantiate(IcePillarPrefab, col.contacts[0].point, Quaternion.LookRotation(ndir,-Vector3.forward)).GetComponent<IcePillar>();
                //p.transform.Rotate(90f,0f,0f,Space.World);
                p.Lifetime = 5f;
            }
            

            FreezePoof.Play();
            if (BlizzardField)explode();
            Remove();
            return;
        }



    }


}
