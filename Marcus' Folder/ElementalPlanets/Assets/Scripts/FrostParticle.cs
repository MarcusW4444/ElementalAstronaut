using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrostParticle : MonoBehaviour {

    // Use this for initialization
    private float StartTime = -10f;
    void Start()
    {
        StartTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Live)
        {
            if ((Time.time - StartTime) >= 2f)
            {
                Live = false;
                Remove();
            }
        }
    }
    public Rigidbody2D MyRigidbody;
    public ParticleSystem FrostEffect,FrostHitEffect;
    public void Remove()
    {

        FrostEffect.transform.SetParent(null);
        FrostEffect.Stop();
        GameObject.Destroy(FrostEffect.gameObject, 5f);

        FrostHitEffect.transform.SetParent(null);
        FrostHitEffect.Stop();
        GameObject.Destroy(FrostHitEffect.gameObject, 5f);

        GameObject.Destroy(this.gameObject);
    }
    
    public bool Live = true;
    private void OnTriggerStay2D(Collider2D collision)
    {
        Astronaut plr = collision.GetComponent<Astronaut>();

        if ((plr != null) && (plr.Alive) && (!plr.Invulnerable) && (Live))
        {

            Vector3 dif = (plr.transform.position - this.transform.position);
            float particledot = 5f;
            plr.TakeDamage(particledot*Time.fixedDeltaTime, MyRigidbody.velocity.normalized * 1f);
            FrostHitEffect.Play();
            //Apply a slow effect?
            //And possibly a freeze effect

        }
        else if ((collision.gameObject.CompareTag("Geometry")) || (collision.gameObject.GetComponent<IceBlock>() != null))
        {

            
        }
    }
}
