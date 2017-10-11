using System.Collections;
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
	void Update () {
		
	}
    public virtual void introduceBoss() {
        Astronaut.TheAstronaut.PlayerHasControl = false;
        
    }

    public virtual void beginBossFight()
    {
        Astronaut.TheAstronaut.PlayerHasControl = true;
        MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
        MyCollider.enabled = true;
        this.Vulnerable = true;
        Astronaut.TheAstronaut.WatchingBossLocation = null;
        Astronaut.TheAstronaut.MyBossGolem = this;
    }

    public ElementGoal MyDroppedGoalElement;
    private float DefeatTime = -10f;

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


    public void TakeDamage(float dmg, Vector2 dir)
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

        }


    }
}
