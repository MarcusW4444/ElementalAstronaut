using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Ice_Heart : GenericEnemy {

    // Use this for initialization
    public enum State { None, FloatingBackAndForth, Invading, Chaining};
    public State MyState = State.FloatingBackAndForth;
    public Chain[] Chains;
	

    void Start()
    {
        StartPosition = this.transform.position;
        StateTime = Time.time;
        setState(State.FloatingBackAndForth, .5f + (2f * Random.value));
    }
    private Vector3 StartPosition;
    private bool GoLeftGoRight;

    // Update is called once per frame

    public ParticleSystem WindUpGlow;
    public const float MoveSpeed = 5f;
    private float StateTime = -10f;
    private float StateDuration = 1f;
    void Update()
    {

        if (Alive)
        {
            hitdirection = (hitdirection * (1f - Time.deltaTime));
            DamageFlashStep();
        }
    }
    public override void onIncinerated()
    {
        ChainsRetracting = true;

    }
    public override void onIncineratedAgain()
    {
        //base.onIncineratedAgain();
        if (MyRigidbody.bodyType == RigidbodyType2D.Dynamic)
        if (MyRigidbody.velocity.y < 1f)
        {
            MyRigidbody.velocity = new Vector2(((Random.value * 2f) - 1f) * 3f, 20f);
        }
    }


    void FixedUpdate()
    {
        Astronaut plr = Astronaut.TheAstronaut;

        if (Alive)
        {
            bool stateexpired = (Time.time >= (StateTime + StateDuration));

            switch (MyState)
            {
                case State.None: { break; }
                case State.FloatingBackAndForth:
                    {
                        MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
                        float sdif = (this.transform.position.x - StartPosition.x);
                        if (Mathf.Abs(sdif) > 2f)
                        {
                            float sig = Mathf.Sign(sdif);
                            GoLeftGoRight = (sig >= 0f);
                        }
                        MyRigidbody.AddForce(new Vector2(GoLeftGoRight ? -1f : 1f, 0f) * 3f);
                        ChainsRetracting = true;

                        //if (stateexpired)
                        //{
                        bool ch = false;

                        if ((plr != null) && (plr.Alive) && (!Astronaut.TheAstronaut.Quelling))
                        {


                            Vector3 dif = (plr.transform.position - this.transform.position);
                            if (dif.magnitude < 15f)
                            {
                                RaycastHit2D rh = Physics2D.Linecast(this.transform.position, plr.transform.position, LayerMask.GetMask(new string[] { "Geometry" }));

                                if (rh.distance <= 0f)
                                {
                                    //The player is present. Invade their space
                                    //Debug.Log("Approach");
                                    //ParticleSystem s = ShootWindUpGlow;
                                    setState(State.Invading, 2f);
                                    ch = true;
                                    //Debug.Log("Visible");
                                }
                                else
                                {
                                    //Debug.Log("hiding...");
                                }
                            }
                        }
                        //if (!ch)
                        // setState(State.FloatingBackAndForth, .5f + (2f * Random.value));
                        //}


                        //if (MyRigidbody.velocity.x != 0f)
                        //{
                        //MySpriteRenderer.flipX = (Mathf.Sign(MyRigidbody.velocity.x) < 0f);
                        //}

                        break;
                    }
                case State.Invading:
                    {
                        MyRigidbody.bodyType = RigidbodyType2D.Dynamic;
                        ChainsRetracting = true;
                        if ((plr != null) && (plr.Alive) && (!Astronaut.TheAstronaut.Quelling))
                        {


                            Vector3 dif = ((plr.transform.position + invadeoffset) - this.transform.position);
                            if (dif.magnitude < 10f)
                            {
                                RaycastHit2D rh = Physics2D.Linecast(this.transform.position, plr.transform.position, LayerMask.GetMask(new string[] { "Geometry" }));

                                if (rh.distance <= 0f)
                                {
                                    //
                                    //ParticleSystem s = ShootWindUpGlow;
                                    if (dif.magnitude < 2f) {
                                        setState(State.Chaining, 2f);
                                    } else
                                    {

                                        MyRigidbody.AddForce(new Vector2(dif.normalized.x, dif.normalized.y) * (5f * (1f + (Astronaut.AggressionLevelF * 4f))));
                                        MyRigidbody.drag = (.6f * (1f + (Astronaut.AggressionLevelF * 2f)));
                                        //setState();
                                    }


                                    //Debug.Log("Visible");
                                }
                                else
                                {

                                    setState(State.FloatingBackAndForth, .5f + (2f * Random.value));
                                }
                            } else
                            {

                                setState(State.FloatingBackAndForth, .5f + (2f * Random.value));
                            }
                        }

                        if (stateexpired)
                        {
                            Vector2 c = Random.insideUnitCircle;
                            invadeoffset = (new Vector3(c.x, c.y, 0f) * 2f);
                            StateTime = Time.time;
                        }


                        break;
                    }
                case State.Chaining:
                    {
                        MyRigidbody.bodyType = RigidbodyType2D.Static;
                        //MyRigidbody.velocity = new Vector2();
                        if ((plr != null) && (plr.Alive) && ((plr.transform.position - this.transform.position).magnitude < 4f) && (!Astronaut.TheAstronaut.Quelling) && (!isIncinerating()))
                        {
                            ChainsRetracting = false;
                            if ((Time.time - StateTime) >= 5f) ChainsRetracting = true;

                            if (ChainsRetracting)
                                if (isfullyretracted)
                                {
                                    setState(State.Invading, 2f);
                                }


                        } else
                        {
                            if (stateexpired) ChainsRetracting = true;
                            if (isfullyretracted)
                            {
                                setState(State.Invading, 2f);
                            }
                        }

                        //if (stateexpired)
                        //{
                        //  ChainsRetracting = true;
                        //} 




                        break;
                    }
            }



            if (isfullyretracted)
            {
                foreach (Chain c in Chains) { c.MaxHealth = (50f * (1f + (2f * Astronaut.AggressionLevelF))); c.Health = c.MaxHealth; c.Alive = true; }
            }
            //MyRigidbody.velocity = new Vector2(movedir * movespeed, MyRigidbody.velocity.y);


            if (Alive)
            {
                Collider2D cha;
                float maxdist = 12f;
                float expansionrate = 10f * Time.fixedDeltaTime + (1f + (3f * Astronaut.AggressionLevelF));
                bool fr = true;
                for (int i = 0; i < Chains.Length; i++)
                {
                    cha = Chains[i].MyCollider;
                    RaycastHit2D hit = Physics2D.Raycast(cha.transform.position, cha.transform.right, maxdist, LayerMask.GetMask(new string[] { "Geometry" }));
                    float mx = maxdist;
                    if (hit.distance <= 0)
                    {
                        //visible
                    }
                    else
                    {
                        //limited
                        mx = hit.distance;
                    }



                    float curr = cha.transform.localScale.x;
                    bool hitmax = false;
                    bool personalretracting = (!Chains[i].Alive);
                    float c = Mathf.Clamp(curr + ((ChainsRetracting || personalretracting) ? -expansionrate : expansionrate), 0f, maxdist);
                    if ((curr < mx) && (c >= mx) && (mx < maxdist))
                    {
                        hitmax = true;
                        curr = mx;
                    }
                    else
                    {
                        curr = c;
                    }

                    if (hitmax)
                    {
                        if (impactSparks)
                        {
                            Am.am.oneshot(Am.am.M.ChainAttach,.3f);
                            impactSparks.transform.position = hit.point;
                            impactSparks.Emit(50);
                        }
                    }
                    cha.enabled = (curr > 0f);
                    if (curr <= 0f)
                    {

                    }
                    cha.transform.localScale = new Vector3(Mathf.Min(curr, mx), cha.transform.localScale.y, cha.transform.localScale.z);
                    if (curr > 0f) fr = false;



                }

                isfullyretracted = fr;
            }    

        }
        else
        {
            setState(State.FloatingBackAndForth, .5f + (2f * Random.value));
        }
    }
    public ParticleSystem impactSparks;
    private bool isfullyretracted = true;
    private Vector3 AimDirection = Vector3.left;
    private bool ChainsRetracting = false;
    public void setState(State st, float dur)
    {

        StateTime = Time.time;
        StateDuration = dur;
        if ((st == State.Chaining) && (MyState != State.Chaining))
        {
            //MyRigidbody.velocity = new Vector2();
            MyRigidbody.bodyType = RigidbodyType2D.Static;
            AnchoredToVine = true;
            ChainsRetracting = false;
            Am.am.oneshot(Am.am.M.ChainSlink);
            //ShootWindUpGlow.Play();
        }

        if (st == State.Invading)
        {
            Vector2 c = Random.insideUnitCircle;
            invadeoffset = (new Vector3(c.x, c.y, 0f)*2f);
        }

        MyState = st;
    }
    private Vector3 invadeoffset=new Vector3();
    public override void Kill()
    {
        if (!Alive) return;
        Alive = false;
        //base.Kill();
        Astronaut.PlayKillSound(3);
        Astronaut.TheAstronaut.dropResistance(.4f / (1f + HitsDone), this.transform.position, Astronaut.Element.Ice);
        deathKnockback();
    }


    private float LastShotTime = -10f;
    private Vector3 TargetingDirection;



}
