using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyphoonWindStreak : MonoBehaviour {

    // Use this for initialization
    private float StartTime = -10f;
    public float Duration = 3f;
	void Start () {
        StartTime = Time.time;
        ElapsedTime = 0f;
    }
    public float ElapsedTime = 0f;
    // Update is called once per frame
    public Rigidbody2D MyRigidbody;
    public ConstantForce2D MyConstantForce;
    public TyphoonField MyTyphoonField;
	void Update () {
        if (!removing)
        {
            ElapsedTime += Time.deltaTime;
            if (((Time.time - StartTime) >= Duration) || ((MyTyphoonField != null) &&(!MyTyphoonField.TyphoonWindsActive)))
            {
                

                Remove();
            } else
            {
                if ((MyTyphoonField != null)&& (MyTyphoonField.TyphoonWindsActive))
                {
                    //if (MyTyphoonField.MyCollider.OverlapPoint(MyRigidbody.position))
                    //{
                        Vector3 f = (MyTyphoonField.getForce(this.transform.position));
                        this.MyRigidbody.velocity += (new Vector2(f.x, f.y) * Time.fixedDeltaTime * 1f);
                    //}
                    //MyConstantForce.force = Vector2.Lerp(MyConstantForce.force, (Random.insideUnitCircle.normalized * (20f * .2f * (1f + (Astronaut.AggressionLevelF * 1f)))), .5f) + ((new Vector2(0f, -1f) * .25f) * (1f - Astronaut.AggressionLevelF));
                }
            }
            
        } else
        {
            if ((Time.time - removetime) >= 5f)
            {
                GameObject.Destroy(this.gameObject);
            }
        }
		
	}
    private bool removing = false;
    private float removetime = -10f;
    void Remove()
    {
        if (removing) return;
        removetime = Time.time;
        removing = true;
        MyRigidbody.velocity = new Vector2();
        MyRigidbody.bodyType = RigidbodyType2D.Kinematic;
        MyConstantForce.enabled = false;
        GameObject.Destroy(this.gameObject, 3f);
    }
}
