using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MossGiantAttack : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public Enemy_Jungle_MossGiant MyMossGiantEnemy;
    private void OnTriggerStay2D(Collider2D collision)
    {
        Astronaut a = collision.gameObject.GetComponent<Astronaut>();
        if ((a != null) && (a.Alive))
        {
            if (!MyMossGiantEnemy.isStunned())
                if ((Time.time - a.lastDamageTakenTime) >= .5f)
                {
                    Am.am.oneshot(Am.am.M.MossGiantClubHit); 
                    Vector3 dif = (a.transform.position - this.transform.position);
                    MyMossGiantEnemy.HitsDone += 1f;
                    if (a.TakeDamage(20f, new Vector3(dif.x,-Mathf.Abs(dif.y),0f).normalized * (1f+(1.25f*Astronaut.AggressionLevel)) + new Vector3(0f, a.JumpSpeed*.025f, 0f)))
                    {
                        MyMossGiantEnemy.HitsDone += 4f;
                    } else
                    {
                        if (Astronaut.AggressionLevel > 1)
                        {
                            RaycastHit2D rh = Physics2D.Raycast(a.transform.position, Vector3.down, 2f, LayerMask.GetMask(new string[] { "Geometry" }));
                            if (rh.collider != null)
                            {
                                a.pressIntoGround(new Vector3(rh.point.x, rh.point.y) +Vector3.up*0.01f,1f+ (.5f*Astronaut.AggressionLevel));
                            }
                        }
                    }
                    
                }

        }
    }
}
