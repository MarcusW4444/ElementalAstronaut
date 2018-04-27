using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TyphoonField : MonoBehaviour {

    // Use this for initialization


    public bool TyphoonWindsActive = false; 
    public Vector2 WindForce = new Vector2();
    public TyphoonWindStreak WindStreakPrefab;
    public float WindDuration = 1f;
    public Collider2D MyCollider;
    public float WindStartTime = -10f;
    public GameObject DebrisPrefab1, DebrisPrefab2, DebrisPrefab3;
    public AudioSource TyphoonSound;
    void Start () {
        TyphoonSound = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.TyphoonSound, AudioManager.AM.EnvironmentAudioMixer, 0f, .5f,true);
    }

    // Update is called once per frame
    public JungleTreeProjectile ProjectilePrefab;
    public float lastProjectileSpawnTime;
    private float lastWindLineSpawnTime;
    bool windrunning = false;
    public bool CanSpawnProjectiles = false;
	void FixedUpdate () {
        
        if (TyphoonWindsActive)
        if ((Time.time - WindStartTime) >= WindDuration)
        {
                TyphoonWindsActive = false;
        }
        if (TyphoonWindsActive)
        {
            
            MyCollider.enabled = true;
            if ((Time.time - lastWindLineSpawnTime) >= .05f)
            {
                Vector3 wdir = new Vector3(WindForce.x, WindForce.y, 0f).normalized;
                Vector3 crs = Vector3.Cross(wdir,Vector3.forward);
                Vector3 spos = (Astronaut.TheAstronaut.transform.position)+((wdir*-10f)+(crs*7.5f*((Random.value*2f) -1f)));
                TyphoonWindStreak ws = GameObject.Instantiate<TyphoonWindStreak>(WindStreakPrefab, spos, new Quaternion());
                ws.MyRigidbody.velocity = (WindForce+(new Vector2(crs.x, crs.y) *WindForce.magnitude*.25f* ((Random.value * 2f) - 1f)));
                ws.MyTyphoonField = this;
                lastWindLineSpawnTime = Time.time;
            }
            if (CanSpawnProjectiles)
            if (Astronaut.AggressionLevel > 1)
            if ((Time.time - lastProjectileSpawnTime) >= (1.5f*(.25f+(.75f*(1f-Astronaut.AggressionLevelF)))))
            {
                Vector3 wdir = new Vector3(WindForce.x, WindForce.y, 0f).normalized;
                Vector3 crs = Vector3.Cross(wdir, Vector3.forward);
                Vector3 spos = (Astronaut.TheAstronaut.transform.position) + ((wdir * -10f) + (crs * 7.5f * ((Random.value * 2f) - 1f)));
                    //TyphoonWindStreak ws = GameObject.Instantiate<TyphoonWindStreak>(WindStreakPrefab, spos, new Quaternion());
                    JungleTreeProjectile jp = GameObject.Instantiate<JungleTreeProjectile>(ProjectilePrefab, spos, new Quaternion());
                    jp.MyRigidbody.velocity = ((WindForce*(.1f+(.5f*Astronaut.AggressionLevelF))) + (new Vector2(crs.x, crs.y) * WindForce.magnitude * .25f * ((Random.value * 2f) - 1f)));
                    jp.SporeFactor = 0f;
                    lastProjectileSpawnTime = Time.time;
            }

        } else
        {
            MyCollider.enabled = false;
            //...
        }

        if (TyphoonWindsActive != windrunning)
        {

            if (TyphoonWindsActive)
            {
                Debug.Log("Initiating Typhoon Sound");
                //start the sound up
                float wf = 2f;// (WindForce.magnitude / 20f);
                if (!TyphoonSound.isPlaying)
                    TyphoonSound.Play();
                AudioManager.AM.crossfade(TyphoonSound, .6f+(.4f*wf), 1f);
                AudioManager.AM.crosstune(TyphoonSound, .4f+(.6f*wf), 1f);

            } else
            {
                Debug.Log("Fading Typhoon Sound");//Lerp the audio using a coroutine
                AudioManager.AM.crossfade(TyphoonSound, 0f, 2f);
                AudioManager.AM.crosstune(TyphoonSound, TyphoonSound.pitch * .8f, 1f);
            }
            windrunning = TyphoonWindsActive;
        }
	}
    public int SineVariator = 0, CosVariator = 0;
    public void startTyphoon(Vector2 windforce,float duration)
    {
        
        TyphoonWindsActive = true;
        WindStartTime = Time.time;
        WindForce = windforce;
        WindDuration = duration;
        SineVariator = Random.Range(1, 10);
        CosVariator = Random.Range(1, 10);
        
    }

    public void stopTyphoon()
    {
        if (TyphoonWindsActive)
        {
            TyphoonWindsActive = false;
        }
    }

    public Vector3 getForce(Vector3 pos)
    {
        //SINUSOID SPAM PLS
        Vector3 sinenoise = new Vector3(0f,Mathf.Cos(pos.x*CosVariator)* Mathf.Sin(pos.x * SineVariator), 0f);

        return (new Vector3(WindForce.x, WindForce.y, 0f) +(sinenoise*WindForce.magnitude));//(new Vector3(-1f, 1f + Mathf.Sin(pos.x), 0f) * 20f);
    }
    public void OnTriggerTouch(Collider2D col)
    {
        //Detect for:
        //Astronaut
        //Bullets and Grenades
        //Spores
        Bullet b = col.gameObject.GetComponent<Bullet>();
        if ((b != null) && (b.Live))
        {
            //Debug.Log(col.gameObject.name);
            b.Interference += getForce(b.transform.position)*.5f;
            return;
        }
        Grenade g = col.gameObject.GetComponent<Grenade>();
        if ((g != null) && (g.Live))
        {
            //Debug.Log(col.gameObject.name);
            Vector3 f = getForce(g.transform.position);
            g.MyRigidbody.AddForce(new Vector2(f.x,f.y)*.25f);
            //g.Interference += getForce(b.transform.position);
            return;
        }
        JungleTreeProjectile Spore = col.gameObject.GetComponent<JungleTreeProjectile>();
        if (Spore != null)
        {
            Spore.MyRigidbody.AddForce(getForce(Spore.transform.position));
            return;
        }
        /*
        TyphoonWindStreak ws = col.gameObject.GetComponent<TyphoonWindStreak>();
        if (ws != null)
        {
            //ws.MyRigidbody.AddForce(getForce(ws.transform.position));
            Vector3 f = (getForce(b.transform.position));
            //ws.MyRigidbody.velocity += (new Vector2(f.x, f.y)*Time.fixedDeltaTime*1f);
            return;
        }
        */
        Astronaut a = col.gameObject.GetComponent<Astronaut>();
        if (a != null)
        {
            a.MyRigidbody.AddForce(getForce(a.transform.position)*.25f*(a.Airborne?1f:5f));
            return;
        }


    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        OnTriggerTouch(collision);
    }
    public void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerTouch(collision);
    }
}
