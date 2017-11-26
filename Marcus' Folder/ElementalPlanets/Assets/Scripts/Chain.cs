using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chain : MonoBehaviour {

    private void OnCollisionStay2D(Collision2D collision)
    {
        Astronaut a = collision.gameObject.GetComponent<Astronaut>();
        if ((a != null) && (a.Alive))
        {
            
                if ((Time.time - a.lastDamageTakenTime) >= 2f)
                {
                    Vector3 dif = (a.transform.position - this.transform.position);
                    a.TakeDamage(10f, dif.normalized * 3f + new Vector3(0f, a.JumpSpeed, 0f));
                }
        }
    }
}
