using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hazard : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
    public bool HazardActive = true;



    public bool Permafrozen;
    public bool CollidesWhenFrozen;
    public SpriteRenderer PermafreezeSprite;
    public float ExpireShatter = -1f;
    private float PermafreezeTime = -10f;
    public ParticleSystem PermaFrostShatterParticles;
    public float Freezability = 1f;
    public float FreezeFactor = 0f;
    public float FreezeDuration = 5f;
    public void slowFreeze(float v1)
    {
        if (Permafrozen)
        {
            PermafreezeTime += (v1 * Freezability);
        } else { 
            FreezeFactor = Mathf.Clamp01(FreezeFactor + (v1 * Freezability));
            if (FreezeFactor >= 1f)
            {

                permafreeze();
            }
        }
    }
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

        if (Permafrozen) {
            if (FreezeDuration > 0f)
            {
                if ((Time.time - PermafreezeTime) >= FreezeDuration)
                {
                    permaThaw();
                    Am.am.oneshot(Am.am.M.MeltSound);
                }
        }
        }
    }

    public Renderer[] FreezableRenderers;
    public virtual void permafreezeUnique()
    {
        //special cases 
    }
    private List<Collider2D> frozencolliders = new List<Collider2D>();
    private List<ParticleSystem> frozenparticlesystems = new List<ParticleSystem>();
    private bool[] frozencollidersenabled;
    private bool[] frozenparticlesplaying;
    public void permafreeze()
    {
        if (Permafrozen) return;
        Permafrozen = true;
        PermafreezeTime = Time.time;
        FreezeFactor = 0f;
        HazardActive = false;
        List<bool> collist = new List<bool>();
        List<bool> parplaylist = new List<bool>();
        Am.am.oneshot(Am.am.M.MakeIcePillar);
        foreach (Animator anim in this.GetComponentsInChildren<Animator>())
        {
            anim.enabled = false; //May not apply to all animators
        }
        frozencolliders.Clear();
        frozenparticlesystems.Clear();
        foreach (Collider2D col in this.GetComponentsInChildren<Collider2D>())
        {
            if (PermafreezeSprite != null)
            if (col.gameObject.Equals(PermafreezeSprite.gameObject)) continue;
            frozencolliders.Add(col);
            collist.Add(col.enabled);
            
            col.enabled = CollidesWhenFrozen;
            

        }
        foreach (ParticleSystem pr in this.GetComponentsInChildren<ParticleSystem>())
        {
            //if (pr.gameObject.Equals(PermafreezeSprite.gameObject)) continue;
            frozenparticlesystems.Add(pr);
            parplaylist.Add(pr.isPlaying);
            pr.Stop();
            //col.enabled = CollidesWhenFrozen;


        }
        
        foreach (Renderer r in FreezableRenderers)
        {
            
            //if (ti > 0f)
            //ti = Mathf.Pow(ti, 1f / 3f);
            
            r.material.SetFloat("_FrozenFactor", 1f);
            
        }
        frozencollidersenabled = collist.ToArray();
        frozenparticlesplaying = parplaylist.ToArray();
        if (FreezableRenderers.Length <= 0)
        {

            if (PermafreezeSprite) PermafreezeSprite.gameObject.SetActive(true); //Use the world space material instead.
        }
        


    }

    public void permaThaw()
    {
        if (Permafrozen)
        {
            Permafrozen = false;
            HazardActive = true;
            FreezeFactor = 0f;
            foreach (Animator anim in this.GetComponentsInChildren<Animator>())
            {
                anim.enabled = true; //May not apply to all animators
            }
            foreach (Collider2D col in this.GetComponentsInChildren<Collider2D>())
            {
                if (PermafreezeSprite != null)
                    if (col.gameObject.Equals(PermafreezeSprite.gameObject)) continue;
                

                if (frozencolliders.Contains(col))
                {
                        col.enabled = frozencollidersenabled[frozencolliders.IndexOf(col)];
                }
            }
            foreach (ParticleSystem pr in this.GetComponentsInChildren<ParticleSystem>())
            {
                if (frozenparticlesystems.Contains(pr))
                {
                    if (frozenparticlesplaying[frozenparticlesystems.IndexOf(pr)])pr.Play();
                }
            }
            foreach (Renderer r in FreezableRenderers)
            {

                //if (ti > 0f)
                //ti = Mathf.Pow(ti, 1f / 3f);

                r.material.SetFloat("_FrozenFactor", 0f);

            }

            
            if (FreezableRenderers.Length <= 0)
            {
                if (PermafreezeSprite != null)
                    PermafreezeSprite.gameObject.SetActive(true); //Use the world space material instead.
            }
            if (PermafreezeSprite) PermafreezeSprite.gameObject.SetActive(false); //Use the world space material instead.

        }
    }
}
