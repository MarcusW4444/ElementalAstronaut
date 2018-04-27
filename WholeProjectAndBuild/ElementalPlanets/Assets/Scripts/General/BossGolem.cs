﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGolem : MonoBehaviour
{

    // Use this for initialization
    
    public float Health = 3000f;
    public float MaxHealth = 3000f;
    public Rigidbody2D MyRigidbody;
    public Collider2D MyCollider;
    public bool Vulnerable = false;
    public bool Defeated = false;
    //public float StunTime = -10f;
    void Start () {

        Health = MaxHealth;
    }

    // Update is called once per frame
    public SpriteRenderer[] MyBossBodySpriteRenderers;
    public void DamageFlashStep()
    {

            if (this.Vulnerable)
            if (!Defeated)
            {
                float ti = Mathf.Clamp01(((Time.time - lastDamageTakenTime) / .25f));
                if (ti > 0f)
                    ti = Mathf.Clamp01(Mathf.Pow(ti, 1f / 3f));
                Color shade = Color.Lerp(Color.black, new Color(1f, .25f, .25f), (Health / MaxHealth));
                foreach (SpriteRenderer sp in MyBossBodySpriteRenderers) 
                sp.color = Color.Lerp(shade, Color.white, Mathf.Clamp01(ti));

            }
        
    }
    void Update () {
		if (!Defeated) DamageFlashStep();
    }
    public bool hasbeenintroduced = false;
    public virtual void introduceBoss() {
        if (hasbeenintroduced) return;
        hasbeenintroduced = true;
        Astronaut.TheAstronaut.PlayerHasControl = false;
        AudioManager.AM.StopMusic();
    }
    

    public virtual void beginBossFight()
    {
        Astronaut.TheAstronaut.PlayerHasControl = true;
        MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
        MyCollider.enabled = true;
        this.Vulnerable = true;
        AudioManager.AM.playMusic(AudioManager.AM.BossMusic, 1f, 1f, true);
        Astronaut.TheAstronaut.WatchingLocation = null;
        Astronaut.TheAstronaut.MyBossGolem = this;

    }

    public ElementGoal MyDroppedGoalElement;
    protected float DefeatTime = -10f;
    public float damagelayover = 0f;
     
    public virtual void OnDefeated() {
        Astronaut.TheAstronaut.PlayerHasControl = false;
        //Freeze the player and explode
        //Explode massively
        Astronaut.TheAstronaut.Invulnerable = true;
        Astronaut.TheAstronaut.PlayerHasControl = false;
        Defeated = true;
        DefeatTime = Time.time;
        MyDroppedGoalElement.gameObject.SetActive(true);
        MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
    }
    public virtual void Kill()
    {
        if (Defeated) return;
        Defeated = true;
        Astronaut.TheAstronaut.MyBossGolem = null;
        OnDefeated();


    }

    public BossWeakSpot MyWeakSpot;
    public ParticleSystem CriticalHitEffect, CriticalHitEffectSub,CriticalSparks;

    private float lastDamageTakenTime = -10f;
    public virtual void TakeDamage(float dmg, Vector2 dir)
    {
        if (Defeated) return;


        float hp = (Health - dmg);
        
        if (hp <= 0f)
        {
            Kill();
        }
        else
        {
            Health = hp;
            lastDamageTakenTime = Time.time;
        }


    }
    public bool Ice_FreezingSkinActive = false;
    public bool Fire_HoldingShield = false;
    public bool Jungle_MossSkinActive = false;
    public void onSlashed(Collider2D collider)
    {
        Astronaut a = collider.gameObject.GetComponent<Astronaut>();
        if ((a != null) && (a.Alive) && (!this.Defeated))
        {
            //Debug.Log("Collided with?: " + collider.gameObject.name);

            if ((Time.time - a.lastDamageTakenTime) >= 2f)
            {
                Vector3 dif = (a.transform.position - this.transform.position);
                a.TakeDamage(50f, dif.normalized * 10f + new Vector3(0f, a.JumpSpeed, 0f));
            }

        }

    }
}
