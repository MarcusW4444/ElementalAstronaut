using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}

    public bool Permafrozen;
    public bool CollidesWhenFrozen;
    public SpriteRenderer PermafreezeSprite;
    public float ExpireShatter = -1f;
    private float PermafreezeTime = -10f;
    public ParticleSystem PermaFrostShatterParticles;
    private void LateUpdate()
    {

        if (((Time.time - PermafreezeTime) >= ExpireShatter) && (ExpireShatter >= 0f))
        {
            if (PermaFrostShatterParticles)
            {
                PermaFrostShatterParticles.Play();
                PermaFrostShatterParticles.transform.SetParent(null);
                GameObject.Destroy(PermaFrostShatterParticles.gameObject,10f);
            }

            GameObject.Destroy(this.gameObject);
        }
    }

    public virtual void permafreezeUnique()
    {
        //special cases 
    }
    public void permafreeze()
    {
        if (Permafrozen) return;
        Permafrozen = true;
        PermafreezeTime = Time.time;
        foreach (Animator anim in this.GetComponentsInChildren<Animator>())
        {
            anim.enabled = false; //May not apply to all animators
        }
        foreach (Collider2D col in this.GetComponentsInChildren<Collider2D>())
        {
            if (col.gameObject.Equals(PermafreezeSprite.gameObject)) continue;
            col.enabled = CollidesWhenFrozen;


        }
        PermafreezeSprite.gameObject.SetActive(true);
        


    }
}
