using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeltableIceWall : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        Health = MaxHealth;
        originalcolor = Color.white;

        MeshRenderer r = this.GetComponentInChildren<MeshRenderer>();
        if (r != null)
        {
            originalcolor = r.material.color;
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
    }
    public bool Alive = true;
    public float Health = 100f;
    public float MaxHealth = 100f;
    public Rigidbody2D MyRigidbody;
    public void Kill()
    {
        if (!Alive) return;
        Alive = false;
        Collider2D[] cols = this.GetComponentsInChildren<Collider2D>();
        foreach (Collider2D col in cols)
        {
            col.enabled = false;
        }
        Rigidbody2D rb = this.GetComponentInChildren<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.velocity = new Vector2(hitdirection.x, hitdirection.y);
        }

    }
    private Vector2 hitdirection = new Vector2();
    public void TakeDamage(float dmg, Vector2 dir)
    {
        if (!Alive) return;

        float hp = (Health - dmg);
        hitdirection += dir;
        MeshRenderer r = this.GetComponentInChildren<MeshRenderer>();
        if (r != null)
        {
            r.material.color = Color.Lerp(Color.black, originalcolor, (hp / MaxHealth));

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
