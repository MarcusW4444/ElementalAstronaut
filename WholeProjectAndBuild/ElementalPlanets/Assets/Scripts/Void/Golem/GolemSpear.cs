using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemSpear : MonoBehaviour {

    // Use this for initialization
    private Vector3 lastposition = new Vector3();
    void Start () {
        startTime = Time.time;
        lastposition = this.transform.position;
        Am.am.oneshot(Am.am.M.VoidPhantom_SlashSwing);
    }
    public bool Live = true;
    public float startTime = -10f;
    public bool Dispersing = false;
    // Update is called once per frame
    public float ContactTime = -10f;
    public bool MadeContact = false;
    public IcePillar IcePillarPrefab;
    
    public List<IcePillar> MyIcePillars;
    private void Update()
    {
        if (Live)
        {
            
            if ((Time.time - startTime) >= 5f)
            {
                Remove();
            }
        }
        else
        {
            if (MadeContact)
            {
                if ((Time.time - ContactTime) >= 10f)
                {
                    float v = Mathf.Clamp01((Time.time - (ContactTime + 5f)) / 1f);

                    this.transform.localScale = (this.transform.localScale * (1f - v));
                    if (v >= 1f)
                    {
                        GameObject.Destroy(this.gameObject);
                    }
                }
            }
            else
            {
                if ((Time.time - startTime) >= 5f)
                {
                    Remove();
                }
            }
        }
    }
    public Vector2 Velocity = new Vector2();
    void FixedUpdate () {
        //Perform ray casts to create 

        if (Live){

            //Velocity = MyRigidbody.velocity;
            Vector2 vm = (Velocity * Time.fixedDeltaTime);
            if (vm.magnitude > 0)
            {

                //Debug.Log("Bullet");
                bool hitsolid = false;
                Ray2D r = new Ray2D(new Vector2(transform.position.x, transform.position.y), vm);
                RaycastHit2D rs = Physics2D.Raycast(r.origin, r.direction, vm.magnitude, LayerMask.GetMask(new string[] { "Geometry" }));
                float mdist = vm.magnitude;
                if (rs.collider != null)
                {

                    mdist = rs.distance;
                    hitsolid = true;
                }
                
                    RaycastHit2D rh = Physics2D.Raycast(r.origin, r.direction, mdist, LayerMask.GetMask(new string[] { "Player",}));

                    if (rh.collider != null)
                    {
                        this.transform.position = (this.transform.position + (new Vector3(r.direction.x, r.direction.y, 0f) * rh.distance));
                        if (rh.collider.isTrigger)
                        {
                            Debug.Log("Trigger: " + rh.collider.gameObject.name);
                        }
                        OnSpearHit(rh.collider);
                    }
                    else
                    {
                        if (rs.collider != null)
                        {
                            this.transform.position = (this.transform.position + (new Vector3(r.direction.x, r.direction.y, 0f) * rs.distance));
                            OnSpearHit(rs.collider);
                        }
                        else
                        {
                            this.transform.position = (this.transform.position + (new Vector3(r.direction.x, r.direction.y, 0f) * vm.magnitude));
                        }
                    }
                



            }

        }

    }

    public void contact(Vector3 pos)
    {
        if (MadeContact || !Live) return;

        MadeContact = true;
        Live = false;
        this.transform.position = pos;
        Dispersing = true;
        ContactTime = Time.time;

        Am.am.oneshot(Am.am.M.MakeIcePillar);
        ParticleSystem ps = GameObject.Instantiate(IceExplosion, pos, IceExplosion.transform.rotation);
        ps.Play(true);
        GameObject.Destroy(ps.gameObject, 4f);

        //Maybe form some Ice Pillars while you're at it


        Vector3 dif = (Astronaut.TheAstronaut.transform.position - pos);
        float mag = (dif.magnitude / 5f);
        if (mag < 1f)
        {
            Astronaut plr = Astronaut.TheAstronaut;
            plr.TakeDamage(10f, new Vector3());
            plr.MyRigidbody.velocity = new Vector2();
            if (plr.Alive)
            {
                if ((!plr.Frozen) && ((Time.time - plr.UnfreezeTime) >= .25f))
                    if (Astronaut.AggressionLevel > 0)
                        plr.freeze(5f*Astronaut.AggressionLevelF);
            }
            
        }

    }
    public ParticleSystem TrailParticles;
    void Remove()
    {
        NegativeParticles.Stop(true);
        NegativeParticles.transform.SetParent(null);
        GameObject.Destroy(NegativeParticles,4f);
        GameObject.Destroy(this.gameObject);
    }

    public ParticleSystem IceExplosion;
    public ParticleSystem PierceParticles;
    public ParticleSystem NegativeParticles;
    private void OnSpearHit(Collider2D col)
    {
        if (col == null) return;
        Astronaut en = col.GetComponent<Astronaut>();
        Vector2 vec = new Vector2((this.transform.position.x - lastposition.x), (this.transform.position.y - lastposition.y));
        if (en != null)
        {
            //if (col.isTrigger) return;
            en.TakeDamage(30f, vec * 1f);
            if (en.Alive)
            {
                if (Astronaut.AggressionLevel > 0)
                    en.freeze(5f * Astronaut.AggressionLevelF);
                Am.am.oneshot(Am.am.M.VoidPhantom_SlashHit);
                Am.am.oneshot(Am.am.M.VoidPhantom_SlashHitMelee);
            }
            

            ParticleSystem p = GameObject.Instantiate(PierceParticles, this.transform.position, Quaternion.LookRotation(-new Vector3(vec.x, vec.y, 0f)));
                GameObject.Destroy(p.gameObject, 1f);
            
            return;
        }
        IceBlock ib = col.GetComponentInParent<IceBlock>();
        if (ib != null)
        {
            //be.TakeDamage(Damage, vec * 1f);//Damage
            ParticleSystem p = GameObject.Instantiate(PierceParticles, this.transform.position, Quaternion.LookRotation(-new Vector3(vec.x, vec.y, 0f)));
            ib.Remove();
            
                
            
            return;
        }







        //Collision with geometry
        //Remove();
        contact(this.transform.position);


    }
}
