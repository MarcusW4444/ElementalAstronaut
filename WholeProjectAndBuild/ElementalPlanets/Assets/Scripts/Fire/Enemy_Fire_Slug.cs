using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Fire_Slug : GenericEnemy {

    // Use this for initialization
    public enum State {None,Waiting,Crawling};
    public State MyState = State.Waiting;
	void Start () {
        StartPosition = this.transform.position;
        StateTime = Time.time;
        setState(State.Waiting, .5f + (1f * Random.value));
    }
    private Vector3 StartPosition;
    private bool GoLeftGoRight;
    public float LeftOffset = 1f, RightOffset = 1f;
    public Collider2D MyCollider;
    // Update is called once per frame
    //public FirePatch FirePatchPrefab;

    //public ParticleSystem ShootWindUpGlow,ShootFlash;
    public const float MoveSpeed = 5f;
    private float StateTime = -10f;
    private float StateDuration = 1f;
    public FirePatch FirePatchPrefab;
    public bool IsInFirePatch = false;
    private float LastFirePatchDroppedTime = -10f;
    public Sprite[] CrawlFrames;
    public void spawnFirePatch()
    {
        FirePatch fp = GameObject.Instantiate(FirePatchPrefab,this.transform.position,FirePatchPrefab.transform.rotation);
        fp.MyFireSlug = this;
        fp.Lifetime *= (.5f + (1.5f * Astronaut.AggressionLevelF));
        //Physics2D.IgnoreCollision(fp.MyCollider,this.MyCollider,true);
        LastFirePatchDroppedTime = Time.time;
    }
    void Update()
    {

        if (Alive)
        {
            hitdirection = (hitdirection * (1f - Time.deltaTime));
            
        }
    }
    void FixedUpdate()
    {
        Astronaut plr = Astronaut.TheAstronaut;
        bool OnGround = false;
        bool cancrawl = false;
        foreach (Sprite c in CrawlFrames)
        {
            if (c.Equals(MySpriteRenderer.sprite))
            {
                cancrawl = true;
            }
        }
        if (Alive && !isStunned())
        {
            bool stateexpired = (Time.time >= (StateTime + StateDuration));

            switch (MyState)
            {
                case State.None: { break; }
                case State.Waiting: {


                        if (stateexpired)
                        {
                            setState(State.Crawling,.5f);
                            StateTime = Time.time;
                        }

                        break; }
                case State.Crawling: {
                        float sdif = (this.transform.position.x - StartPosition.x);
                        
                        if (GoLeftGoRight?(sdif < -LeftOffset): (sdif > -RightOffset))//(Mathf.Abs(sdif) > 3f)
                        {
                            float sig = Mathf.Sign(sdif);
                            GoLeftGoRight = (sig >= 0f);
                        }

                        RaycastHit2D rh = Physics2D.Linecast(this.transform.position, this.transform.position + Vector3.down * .6f, LayerMask.GetMask(new string[] { "Geometry" }));
                        if (rh.distance <= 0f)
                        {
                            //in the air.
                        }
                        else if (rh.distance <= .6f)
                        {
                            //Landed
                            if (!isStunned())
                                //if (Mathf.Abs(sdif) <= 3f)
                                    MyRigidbody.velocity = new Vector2((GoLeftGoRight ? -1f : 1f)*(cancrawl? 1f:0f) * (1f - FreezeFactor) * 2f *((.5f + (2f * Astronaut.AggressionLevelF*(Astronaut.TheAstronaut.Quelling?0f:1f)))),MyRigidbody.velocity.y);
                        }
                        //MyRigidbody.AddForce(new Vector2(GoLeftGoRight ? -1f : 1f, 0f) * 10f);

                        /*
                        if (stateexpired)
                        {
                            bool ch = false;

                            if ((plr != null) && (plr.Alive))
                            {


                                Vector3 dif = (plr.transform.position - this.transform.position);
                                if (dif.magnitude < 10f)
                                {
                                    RaycastHit2D rh = Physics2D.Linecast(this.transform.position, plr.transform.position, LayerMask.GetMask(new string[] { "Geometry" }));
                                    Debug.Log(rh.distance);
                                    if (rh.distance <= 0f)
                                    {
                                        Debug.Log("Take aim");
                                        ParticleSystem s = ShootWindUpGlow;
                                        setState(State.Firing, s.main.duration);
                                        ch = true;
                                        //Debug.Log("Visible");
                                    } else
                                    {
                                        Debug.Log("hiding...");
                                    }
                                }
                            }
                            if (!ch)
                            setState(State.FloatingBackAndForth, .5f + (2f * Random.value));
                        }

                        */
                        if (MyRigidbody.velocity.x != 0f)
                        {
                            MySpriteRenderer.flipX = (Mathf.Sign(MyRigidbody.velocity.x) > 0f);
                        }

                        
                        break; }
               
            }


            //MyRigidbody.velocity = new Vector2(movedir * movespeed, MyRigidbody.velocity.y);
        }
        else
        {
            setState(State.Crawling, .5f + (2f * Random.value));
        }

        if (Alive)
        {

            if (((!IsInFirePatch) && ((Time.time - LastFirePatchDroppedTime)>= .8f))&&(!isStunned())&&(!Astronaut.TheAstronaut.Quelling))
            {
                RaycastHit2D rh = Physics2D.Raycast(this.transform.position, Vector3.down, .3f, LayerMask.GetMask(new string[] { "Geometry" }));
                if (rh.collider != null)
                {
                    spawnFirePatch();
                }
            }

            IsInFirePatch = false;
            DamageFlashStep();
            freezeStep();

        }

    }
    private Vector3 AimDirection = Vector3.left;
    
    public void setState(State st,float dur)
    {
        
        StateTime = Time.time;
        StateDuration = dur;
        /*if ((st == State.Firing) &&(MyState != State.Firing))
        {
            MyRigidbody.velocity = new Vector2();
            ShootWindUpGlow.Play();
        }*/

        MyState = st;
    }
    public override void Kill()
    {
        if (!Alive) return;
        Alive = false;
        //base.Kill();
        Astronaut.PlayKillSound(1);
        Astronaut.TheAstronaut.dropResistance(0.4f / (1f + HitsDone), this.transform.position, Astronaut.Element.Fire);
        deathKnockback();
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f,.5f,0f);
        float leftx = (this.transform.position.x - LeftOffset);
        float rightx = (this.transform.position.x + RightOffset);
        Gizmos.DrawLine(new Vector3(leftx, this.transform.position.y+.5f, this.transform.position.z), new Vector3(leftx, this.transform.position.y - .5f, this.transform.position.z));
        Gizmos.DrawLine(new Vector3(rightx, this.transform.position.y + .5f, this.transform.position.z), new Vector3(rightx, this.transform.position.y - .5f, this.transform.position.z));
        Gizmos.DrawLine(new Vector3(leftx, this.transform.position.y, this.transform.position.z), new Vector3(rightx, this.transform.position.y, this.transform.position.z));
    }

    private float LastShotTime = -10f;
    private Vector3 TargetingDirection;

}
