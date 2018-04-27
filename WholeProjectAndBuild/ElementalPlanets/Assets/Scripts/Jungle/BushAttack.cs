using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushAttack : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public Enemy_Jungle_Bush MyBushEnemy;
    private void OnTriggerStay2D(Collider2D collision)
    {
        Astronaut a = collision.gameObject.GetComponent<Astronaut>();
        if ((a != null) && (a.Alive))
        {
            if (!MyBushEnemy.isStunned())
            {
                //if ((Time.time - a.lastDamageTakenTime) >= 2f)
                //{
                Vector3 dif = (a.transform.position - this.transform.position);
                MyBushEnemy.HitsDone += 1f;
                if (a.TakeDamage(20f, new Vector3(0f, a.JumpSpeed*.25f, 0f)))
                {
                    MyBushEnemy.HitsDone += 4f;
                }

                //}
            }

        }
    }
}
