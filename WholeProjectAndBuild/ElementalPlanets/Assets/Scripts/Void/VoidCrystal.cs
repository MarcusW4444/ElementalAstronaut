using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoidCrystal : MeltableIceWall {

    // Use this for initialization
    public Transform StandByTransform;
    private Vector3 StandByLocation,DestinationLocation;

	void Start () {
        StandByLocation = StandByTransform.position;
        DestinationLocation = this.transform.position;

    }
	
	// Update is called once per frame
	void FixedUpdate () {

        if (Alive)
        {

            if (Extending)
            {
                Extended = Mathf.Clamp01(Extended + (Time.fixedDeltaTime * 5f));
                
               
            }
            this.transform.position = Vector3.Lerp(StandByLocation, DestinationLocation, Extended);
            if (!Extending) {
                Vector3 dif = (Astronaut.TheAstronaut.transform.position - DestinationLocation);
                if (dif.magnitude < (FirstTimeCrystallized?3f:6f))
                {
                    popOut();
                }
            }
        }
		
        if ((Extended > 0f) && (Alive))
        {
            MyCollider.enabled = true;
        } else
        {
            MyCollider.enabled = false;
        }
	}
    public Collider2D MyCollider;
    private float Extended = 0f;
    private bool Extending = false;
    public bool Enchanted = false;
    public void popOut()
    {
        if (Extending) return;
        Extending = true;
        Am.am.oneshot(Am.am.M.MakeIcePillar);


    }
    

    
    public override void TakeDamage(float dmg, Vector2 dir)
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

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0f, .8f);

        if (MyCollider != null)
        {
            Vector3 bo = new Vector3(MyCollider.bounds.size.x, MyCollider.bounds.size.y, 1f);
            if (StandByTransform != null)
            {

                Gizmos.DrawWireCube(StandByTransform.position, bo);
                Gizmos.DrawWireCube(this.transform.position, bo);
                Gizmos.DrawLine(this.transform.position, StandByTransform.position);
            }
        }


    }
    public static bool FirstTimeCrystallized = true;
    private void OnTriggerStay2D(Collider2D collision)
    {
        Astronaut a = collision.gameObject.GetComponent<Astronaut>();
        if ((a != null) && (a.Alive) && (this.Alive))
        {
            FirstTimeCrystallized = false;
            a.MyFreezingCrystal = this;
            a.CrystalFreeze = true;
            a.freeze(1f);
            a.CrystalFreeze = false;






        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerStay2D(collision);
    }




}
