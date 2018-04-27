using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableIceWall : MonoBehaviour {

	// Use this for initialization 
	void Start () {
        Health = MaxHealth;
        originalcolor = Color.white;
        
        MeshRenderer r = this.GetComponentInChildren<MeshRenderer>();
        if (r != null)
        {
            originalcolor = r.material.color;
        }

        
        
    SpriteRenderer sp = this.GetComponentInChildren<SpriteRenderer>();
        if (sp != null) {
            originalcolor2 = sp.color;
        }
    
}
    private Color originalcolor = Color.white, originalcolor2=Color.white;
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
            rb.gravityScale = 4f;
            rb.velocity = new Vector2(hitdirection.x, hitdirection.y);
        }

        /*
        SpriteRenderer[] spr = this.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer s in spr)
        {
            s.color = Color.Lerp(s.color,Color.black,0.75f);
            s.transform.Rotate(new Vector3(0f,90f*Mathf.Sign(Random.value-.5f),0f));
        }
        */

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
            r.material.color = Color.Lerp(Color.black,originalcolor,(hp/MaxHealth));

        }
        SpriteRenderer sp = this.GetComponentInChildren<SpriteRenderer>();

        if (sp != null)
        {
            sp.color = Color.Lerp(Color.black, originalcolor2, (hp / MaxHealth));

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
