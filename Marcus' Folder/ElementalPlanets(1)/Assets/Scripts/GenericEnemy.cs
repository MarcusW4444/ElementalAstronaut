using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericEnemy : MonoBehaviour {

    // Use this for initialization
    public bool Alive = true;
    public float Health = 100f;
    public float MaxHealth = 100f;
    public Rigidbody2D MyRigidbody;
    public float StunTime = -10f;
    void Start () {
        Health = MaxHealth;

    }
	
	// Update is called once per frame
	void Update () {

        if (Alive) {
            hitdirection = (hitdirection*(1f-Time.deltaTime));
        }
    }

    public bool isStunned()
    {
        return ((StunTime - Time.time) >= 0f);
    }


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
            rb.velocity = new Vector2(hitdirection.x, hitdirection.y);
        }

    }
    private Vector2 hitdirection=new Vector2();
    public void TakeDamage(float dmg,Vector2 dir)
    {
        if (!Alive) return;

        float hp = (Health - dmg);
        hitdirection += dir;
        if (hp <= 0f)
        {
            Kill();
        } else
        {
            Health = hp;
            
        }


    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        Astronaut a = collision.gameObject.GetComponent<Astronaut>();
        if ((a != null) &&(a.Alive))
        {
            if (!isStunned())
            if ((Time.time - a.lastDamageTakenTime) >= 2f)
            {
                Vector3 dif = (a.transform.position - this.transform.position);
                a.TakeDamage(20f,dif.normalized*10f+new Vector3(0f,a.JumpSpeed,0f));
            }
        }
    }
}
