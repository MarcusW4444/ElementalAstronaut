using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeltableIceWall : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        Health = MaxHealth;
        originalcolor = Color.white;

        SpriteRenderer r = this.GetComponentInChildren<SpriteRenderer>();
        if (r != null)
        {
            originalcolor = r.color;
        }
    }
    private Color originalcolor = Color.white;

    // Update is called once per frame

    void Update()
    {

        if (Alive)
        {
            hitdirection = (hitdirection * (1f - Time.deltaTime));
        }
        else
        {
            if (this.transform.localScale.x >= .0001f)
            {
                float e = (this.transform.localScale.x / Mathf.Max(0.00000000000001f, (this.transform.localScale.x + (2f*(Time.deltaTime+(Time.time - DeathTime))))));
                this.transform.localScale = (this.transform.localScale * e);
            }
        }
    }
    public bool Alive = true;
    public float Health = 100f;
    public float MaxHealth = 100f;
    public Rigidbody2D MyRigidbody;
    private float DeathTime = -10f;
    public void Kill()
    {
        if (!Alive) return;
        Alive = false;
        Collider2D[] cols = this.GetComponentsInChildren<Collider2D>();
        DeathTime = Time.time;
        Health = 0f;
        foreach (Collider2D col in cols)
        {
            col.enabled = false;
        }
        
        foreach (ParticleSystem ps in this.GetComponentsInChildren<ParticleSystem>())
        {
            ps.Stop();
            ps.transform.SetParent(null);
            GameObject.Destroy(ps.gameObject,5f);
        }
        Rigidbody2D rb = this.GetComponentInChildren<Rigidbody2D>();
        if (rb != null)
        {
            //rb.bodyType = RigidbodyType2D.Dynamic;
            //rb.velocity = new Vector2(hitdirection.x, hitdirection.y);
        }

    }
    public ParticleSystem MeltParticles;
    private Vector2 hitdirection = new Vector2();
    public void TakeDamage(float dmg, Vector2 dir)
    {
        if (!Alive) return;

        float hp = (Health - dmg);
        hitdirection += dir;
        SpriteRenderer r = this.GetComponentInChildren<SpriteRenderer>();
        if (r != null)
        {
            r.color = Color.Lerp(Color.black, originalcolor, (hp / MaxHealth));

        }
        if (hp <= 0f)
        {
            Kill();
        }
        else
        {
            Health = hp;

        }


    }
}
