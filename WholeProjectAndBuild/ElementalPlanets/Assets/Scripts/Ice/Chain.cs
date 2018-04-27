using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : GenericEnemy {

    public Collider2D MyCollider;
    private void Update()
    {
        MySpriteRenderer.color = Color.Lerp(Color.black,Color.Lerp(Color.white,Color.blue,(MaxHealth-50f)/(150f-50f)),(Health/MaxHealth));
        AnchoredToVine = true;
    }




    public override void onIncinerated()
    {
        Alive = false;
        Health = 0f;
    }
    public override bool isStunned()
    {
        return false;
    }
    public override void freeze(float dur)
    {
        return;
    }


    public override void Kill()
    {
        if (!Alive) return;
        Alive = false;
        Astronaut.PlayKillSound(1);
        Health = 0f;
        


    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        Astronaut a = collision.gameObject.GetComponent<Astronaut>();
        if ((a != null) && (a.Alive))
        {
                if (Astronaut.AggressionLevel >= 2)
                if ((Time.time - a.lastDamageTakenTime) >= 1.25f)
                {
                    Vector3 dif = (a.transform.position - this.transform.position);
                    a.TakeDamage(10f, dif.normalized * 3f + new Vector3(0f, a.JumpSpeed, 0f));
                }
        }
    }
}
