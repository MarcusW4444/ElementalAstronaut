using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeslaLightning : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
        TeslaGunStart = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.TeslaGunStart, AudioManager.AM.PlayerAudioMixer, 1f, 1f, false);
        TeslaGunNoTarget = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.TeslaGunNoTarget, AudioManager.AM.PlayerAudioMixer, 1f, 1f, true);
        TeslaGun1Target = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.TeslaGun1Target, AudioManager.AM.PlayerAudioMixer, 1f, 1f, true);
        TeslaGunMoreTargets = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.TeslaGunMoreTargets, AudioManager.AM.PlayerAudioMixer, 1f, 1f, true);
        TeslaGunChangeTargets = AudioManager.AM.createGeneralAudioSource(AudioManager.AM.TeslaGunChangeTargets, AudioManager.AM.PlayerAudioMixer, 1f, 1f, false);
    }

    // Update is called once per frame
    public bool LightningTick = false;
    public float LightningTickDelta = 0f;
    
    public float DamagePerSecond = 100f;
    public float FlashDamage = 50f;
    public bool UsingFlash = false;
    public float FlashTime = -10f;
    public LineRenderer LightningRenderer;
    public ParticleSystem GunGlow;
    public ParticleSystem ElectricSparks;
    public ParticleSystem SegmentGlows;
    public ParticleSystem HitElectricity;
    public ParticleSystem HitGlow;
    public ParticleSystem HitFlare;
    public Vector3 LightningDirection = new Vector3(1f,0f,0f);
    public const float INITIALSEGMENTRANGE = 9f;
    public const float SUBSEQUENTSEGMENTRANGERATIO = 1f;
    public const float CONEANGLE = 90f; //Half and half, not two times
    private List<GameObject> HitList = new List<GameObject>();
    private List<Vector3> ExitDirectionList = new List<Vector3>();
    public AudioSource TeslaGunStart, TeslaGunNoTarget, TeslaGun1Target, TeslaGunMoreTargets, TeslaGunChangeTargets;
    private GameObject[] OldHitList = new GameObject[0];
    bool hitlistChanged(GameObject[] newhitlist)
    {
        for (int i = 0; i < (int)Mathf.Max(newhitlist.Length, OldHitList.Length); i++) {
            if ((i >= newhitlist.Length) || (i >= OldHitList.Length)) return true;
            if ((OldHitList[i] != null) != (newhitlist[i] != null)) return true;
            if (!OldHitList[i].Equals(newhitlist))
            {
                return true;
                
            }
            
        }
        return false;
    }

    public class LightningSubdivision
    {
        public Vector3 PointPosition = new Vector3();
        public LightningSubdivision Previous = null, Next = null;
        public bool Altered = false;
        public LightningSubdivision(Vector3 point)
        {
            PointPosition = point;
            Altered = false;
        }
        public void Alter(float maxdist,Vector3 crs)
        {
            PointPosition = PointPosition + (crs.normalized * maxdist * ((Random.value - .5f) * 2f));
            Altered = true;
        }
        public int getCount()
        {
            if (Next != null)
                return (1 + getCount());
            else
                return 1;
        }
    }
    public void SubdivideLightning(LightningSubdivision startpoint,int index,int subdivisionsremaining)
    {
        
        for (int i = 0; i < index; i++)
        {
            startpoint = startpoint.Next;  
        }
        LightningSubdivision ln = new LightningSubdivision(((startpoint.PointPosition + startpoint.Next.PointPosition)/2f));
        startpoint.Next.Previous = ln;
        ln.Next = startpoint.Next;
        ln.Previous = startpoint;
        startpoint.Next = ln;
        //subdivisionsremaining--; //???
        //SubdivideLightning()
    }
    void FixedUpdate() {
        HitList.Clear();
        if (LightningTick)
        {

            //Perform a while loop that goes through the enemies in the game
            //Allow Lightning to travel through targets at a certain angle

            Collider2D[] potentialtargets = new Collider2D[0];
            Vector2 ltpos = new Vector2(this.transform.position.x, this.transform.position.y);
            Vector2 ltdir = new Vector2(LightningDirection.normalized.x, LightningDirection.normalized.y);

            ExitDirectionList.Clear();// = new List<Vector3>();
            int hitindex = 0;
            HitList.Add(this.gameObject);
            ExitDirectionList.Add(new Vector3(ltdir.x, ltdir.y, 0f) * 1f);//INITIALSEGMENTRANGE
            do
            {
                float le = ((hitindex == 0) ? INITIALSEGMENTRANGE : (INITIALSEGMENTRANGE * SUBSEQUENTSEGMENTRANGERATIO));
                float wi = ((le * Mathf.Sin(Mathf.PI * (CONEANGLE / 180f))));
                float ang = Vector2.Angle(new Vector2(1f, 0f), new Vector2(ltdir.normalized.x, ltdir.normalized.y));

                potentialtargets = Physics2D.OverlapBoxAll(ltpos + (ltdir.normalized * le * .5f), new Vector2(le, wi), ang, LayerMask.GetMask("Enemy", "Boss"));
                GameObject besttarget = null;
                float bestdir = 0f;
                float bestdist = 0f;
                float bestdot = 0f;
                Vector2 bestdif = new Vector2();

                for (int i = 0; i < potentialtargets.Length; i++)
                {
                    Collider2D colt = potentialtargets[i];
                    GenericEnemy t = colt.gameObject.GetComponent<GenericEnemy>();
                    BossGolem b = colt.gameObject.GetComponent<BossGolem>();
                    GameObject en = null;
                    if (t != null) {
                        en = t.gameObject;
                    } else if (b != null)
                    {
                        en = b.gameObject;
                    } else
                    {
                        continue;
                    }
                    if (HitList.Contains(en))
                    {
                        continue;
                    }

                    // else if (Breakable Ice Wall)
                    // else etc..

                    Vector2 enpos = new Vector2(en.transform.position.x, en.transform.position.y);
                    Vector2 dif = (enpos - ltpos);
                    float enang = (Vector2.Angle(ltdir.normalized, dif) / (CONEANGLE * .5f));
                    if ((enang <= 1f) && (dif.magnitude <= ((hitindex == 0) ? INITIALSEGMENTRANGE : (INITIALSEGMENTRANGE * SUBSEQUENTSEGMENTRANGERATIO))))
                    {
                        //Also check for LOS
                        if (Physics2D.Linecast(ltpos, enpos, LayerMask.GetMask("Geometry")).collider != null) continue;
                        float dot = Vector2.Dot(ltdir.normalized, dif.normalized);
                        float dv = (dif.magnitude / dot);
                        if ((besttarget == null) || (dv < (bestdist / bestdot)))
                        {
                            besttarget = en;
                            bestdir = enang;
                            bestdist = dif.magnitude;
                            bestdot = dot;
                            bestdif = dif;
                        }
                    }
                }

                if (besttarget != null)
                {
                    HitList.Add(besttarget);
                    ltpos = besttarget.transform.position;
                    Vector3 v = Vector3.Reflect(new Vector3(ltdir.x, ltdir.y, 0f), Vector3.Cross(new Vector3(bestdif.normalized.x, bestdif.normalized.y, 0f), Vector3.forward));
                    //* bestdif.magnitude
                    //Vector3.Slerp(new Vector3(ltdir.x,ltdir.y,0f), new Vector3(bestdif.normalized.x, bestdif.normalized.y, 0f), .5f);
                    ExitDirectionList.Add(new Vector2(v.x, v.y));
                    ltdir = new Vector2(v.x, v.y);
                    hitindex++;
                } else
                {
                    ExitDirectionList.Add(new Vector3(ltdir.x, ltdir.y, 0f));
                    break;
                }


            } while (true);


            if (HitList.Count > 1)
            {
                //For now, just draw a simple line chain to all of the enemies that you hit
                int lightningarcpoints = 4;//min = 1
                Vector3[] positions = new Vector3[((HitList.Count - 1) * (lightningarcpoints)) + 1];
                Vector3[] exitlist = ExitDirectionList.ToArray();
                GameObject[] hitlist = HitList.ToArray();
                int i = 0;
                while (i < positions.Length)
                {


                    //Lerp the arc.
                    int nf = ((i) % lightningarcpoints);
                    int u = ((i) / lightningarcpoints);
                    float f = (((float)nf) / ((float)lightningarcpoints));

                    if (nf == 0)
                    {
                        positions[i] = hitlist[u].transform.position;
                    } else
                    {
                        //Debug.Log(""+positions.Length+" "+i);
                        Vector2 sp = Random.insideUnitCircle;
                        positions[i] = Vector3.Lerp(
                            Vector3.Lerp(hitlist[u].transform.position,
                            hitlist[u].transform.position + (exitlist[u].normalized * 1f * (hitlist[u].transform.position - hitlist[u + 1].transform.position).magnitude), f),
                            hitlist[u + 1].transform.position, f) + (new Vector3(sp.x, sp.y, 0f) * 1f) * (1f - (Mathf.Abs(f - .5f) / .5f));
                    }






                    i++;
                }


                //LightningSubdivision lsub;
                LightningRenderer.positionCount = positions.Length;
                LightningRenderer.SetPositions(positions);
                LightningRenderer.useWorldSpace = true;




                float dmgu = (UsingFlash ? FlashDamage : (DamagePerSecond * LightningTickDelta));
                for (int h = 0; h < hitlist.Length; h++)
                {





                    GameObject col = hitlist[h];

                    if (col.gameObject.Equals(this.gameObject)) { continue; }

                    GenericEnemy en = col.GetComponent<GenericEnemy>();
                    Vector2 vec = ExitDirectionList[h];//new Vector2((this.transform.position.x - lastposition.x), (this.transform.position.y - lastposition.y));
                    if (en != null)
                    {
                        float dmg = (dmgu * (1f + (1f * (Mathf.Max(en.Health - en.MaxHealth, 0f) / 200f))));
                        //if (col.isTrigger) return;
                        en.TakeDamage(dmg, vec * 1f);
                        Astronaut.TheAstronaut.tickHitMarker(dmg, (en.Health / en.MaxHealth) * (en.Alive ? 1f : 0f), !en.Alive);
                        GameManager.TheGameManager.ignoreTutorialTip(TutorialSystem.TutorialTip.Shoot);
                        //ParticleSystem p = GameObject.Instantiate(DamageImpact, this.transform.position, Quaternion.LookRotation(-new Vector3(vec.x, vec.y, 0f)));
                        //GameObject.Destroy(p.gameObject, 1f);
                        //Remove();
                        HitGlow.transform.position = en.transform.position; HitGlow.Emit(1);
                        HitFlare.transform.position = en.transform.position; HitFlare.Emit(1);
                        HitElectricity.transform.position = en.transform.position; HitElectricity.Emit(1);
                        ElectricSparks.transform.position = en.transform.position; ElectricSparks.Emit(2);
                        //if (UsingFlash)

                        //return;
                    }

                    IceBlock ib = col.GetComponentInParent<IceBlock>();
                    if (ib != null)
                    {
                        //be.TakeDamage(dmg, vec * 1f);//Damage
                        //ParticleSystem p = GameObject.Instantiate(DamageImpact, this.transform.position, Quaternion.LookRotation(-new Vector3(vec.x, vec.y, 0f)));
                        ib.Remove();
                        //ib.Take
                        //GameObject.Destroy(p.gameObject, 1f);

                        //Remove();
                        HitGlow.transform.position = ib.transform.position; HitGlow.Emit(1);
                        HitFlare.transform.position = ib.transform.position; HitFlare.Emit(1);
                        HitElectricity.transform.position = ib.transform.position; HitElectricity.Emit(1);
                        ElectricSparks.transform.position = ib.transform.position; ElectricSparks.Emit(2);
                        //if (UsingFlash)
                        //return;
                    }
                    BossGolem bo = col.GetComponent<BossGolem>();
                    if (bo != null)
                    {
                        float dmg = (dmgu * (1f + (1f * (Mathf.Max(bo.Health - bo.MaxHealth, 0f) / 200f))));
                        bo.TakeDamage(dmg, vec * 1f);//Damage
                                                     //Astronaut.TheAstronaut.tickHitMarker(dmg, (bo.Health / bo.MaxHealth) * (bo.Defeated ? 0f : 1f), bo.Defeated);
                                                     //ParticleSystem p = GameObject.Instantiate(DamageImpact, this.transform.position, Quaternion.LookRotation(-new Vector3(vec.x, vec.y, 0f)));
                                                     //GameObject.Destroy(p.gameObject, 1f);
                                                     //Remove();
                        HitGlow.transform.position = bo.transform.position; HitGlow.Emit(1);
                        HitFlare.transform.position = bo.transform.position; HitFlare.Emit(1);
                        HitElectricity.transform.position = bo.transform.position; HitElectricity.Emit(1);
                        ElectricSparks.transform.position = bo.transform.position; ElectricSparks.Emit(2);
                        //if (UsingFlash)
                        //return;
                    }
                    BreakableIceWall be = col.GetComponent<BreakableIceWall>();
                    if (be != null)
                    {
                        float dmg = (dmgu * (1f + (1f * (Mathf.Max(be.Health - be.MaxHealth, 0f) / 200f))));
                        be.TakeDamage(dmg, vec * 1f);//Damage
                        Astronaut.TheAstronaut.tickHitMarker(dmg, (be.Health / be.MaxHealth) * (be.Alive ? 1f : 0f), !be.Alive);
                        //ParticleSystem p = GameObject.Instantiate(DamageImpact, this.transform.position, Quaternion.LookRotation(-new Vector3(vec.x, vec.y, 0f)));
                        //GameObject.Destroy(p.gameObject, 1f);
                        //Remove();

                        HitGlow.transform.position = be.transform.position; HitGlow.Emit(1);
                        HitFlare.transform.position = be.transform.position; HitFlare.Emit(1);
                        HitElectricity.transform.position = be.transform.position; HitElectricity.Emit(1);
                        ElectricSparks.transform.position = be.transform.position; ElectricSparks.Emit(2);

                        //if (UsingFlash)
                        //return;
                    }







                }

            } else
            {
                // if there were no successful hits, spam lightning in a general direction
                // also, perform basic lightning subdivision as a practice run.
                float anm = (((Random.value - .5f)) * ((Mathf.PI * 2f) * (CONEANGLE / 360f)));
                Vector3 crs = Vector3.Cross(ltdir, Vector3.forward);
                Vector3 misspos = this.transform.position + (((new Vector3(ltdir.x, ltdir.y, 0f) * Mathf.Cos(anm)) + (crs * Mathf.Sin(anm))) * (INITIALSEGMENTRANGE));

                RaycastHit2D rhm = Physics2D.Linecast(ltpos, misspos, LayerMask.GetMask("Geometry"));
                if (rhm.collider != null)
                {
                    misspos = rhm.point;
                }

                int additionalpoints = 5;
                List<Vector3> misslist = new List<Vector3>();
                misslist.Add(this.transform.position);
                for (int i = 0; i <= additionalpoints; i++)
                {
                    float f = ((float)i / (float)additionalpoints);
                    Vector2 sp = Random.insideUnitCircle;
                    misslist.Add(Vector3.Lerp(ltpos, misspos, f) + (new Vector3(sp.x, sp.y, 0f) * 1f) * (1f - (Mathf.Abs(f - .5f) / .5f)));
                }
                Vector3[] ar = misslist.ToArray();
                LightningRenderer.SetPositions(ar);
                LightningRenderer.positionCount = ar.Length;
                LightningRenderer.useWorldSpace = true;


            }
            //Electrical Glowing
            float maxrange = 0f;
            Vector3 lpos = new Vector3();
            for (int o = 0; o < LightningRenderer.positionCount; o++)
            {
                Vector3 p = LightningRenderer.GetPosition(o);
                if (o > 0)
                {
                    maxrange += (lpos - p).magnitude;
                }
                lpos = p;
            }
            if (maxrange > 0f)
                for (int e = 0; e < 8; e++)
                {
                    float r = Random.value;
                    int ri = 0;
                    lpos = new Vector3();
                    while (ri < LightningRenderer.positionCount)
                    {
                        Vector3 p = LightningRenderer.GetPosition(ri);
                        if (ri > 0)
                        {

                            float ra = (lpos - p).magnitude;
                            if (ra > 0f)
                            {
                                float re = (ra / maxrange);
                                if ((r < re))
                                {
                                    SegmentGlows.transform.position = Vector3.Lerp(lpos, p, r / re);
                                    SegmentGlows.Emit(1);
                                    break;
                                }
                                else
                                {
                                    r -= re;
                                }
                            }
                            else
                            {
                                //These two points don't count
                            }
                        }
                        lpos = p;
                        ri++;
                    }

                }
            if (UsingFlash)
            {
                FlashTime = Time.time;
                GunGlow.Emit(25);

            }
            else
            {
                GunGlow.Emit(1);
            }

            LightningRenderer.enabled = true;
            


            //Glow effects on the arc
            float ef = 0f;
            float elength = 0f;
            int eindex = 0;

            while (ef < elength)
            {


                break;
            }



        } else
        {
            LightningRenderer.enabled = false;
        }
        
            GameObject[] newlist = OldHitList;
        //if (LightningTick)
            newlist = HitList.ToArray();

        if (hitlistChanged(newlist))
        {
            
            //ZUNGIT


        }
        if (LightningTick)
        {
            if ((newlist.Length == 0) || (newlist.Length == 1))
            {
                if (!TeslaGunNoTarget.isPlaying)
                {
                    TeslaGunNoTarget.Play();
                    TeslaGunChangeTargets.Stop();
                    TeslaGunChangeTargets.volume = 1f;
                    TeslaGunChangeTargets.Play();
                    
                }
                TeslaGun1Target.Stop();
                TeslaGunMoreTargets.Stop();

            }
            else if (newlist.Length == 2)
            {
                TeslaGunNoTarget.Stop();
                if (!TeslaGun1Target.isPlaying)
                {
                    TeslaGun1Target.volume = 1f;
                    TeslaGun1Target.Play();
                    TeslaGunChangeTargets.Stop();
                    TeslaGunChangeTargets.volume = 1f;
                    TeslaGunChangeTargets.Play();
                }
                TeslaGunMoreTargets.Stop();
            }
            else if (newlist.Length >= 2)
            {
                TeslaGunNoTarget.Stop();
                TeslaGun1Target.Stop();
                if (!TeslaGunMoreTargets.isPlaying)
                {
                    TeslaGunMoreTargets.volume = 1f;
                    TeslaGunMoreTargets.Play();
                    TeslaGunChangeTargets.Stop();
                    TeslaGunChangeTargets.volume = 1f;
                    TeslaGunChangeTargets.Play();
                }
            }

        } else {

            
        }
        OldHitList = newlist;
        //if (((Time.time - lightningticktime) >= .1f)){
            if ((LightningTick && !lastlightningtick))
            {
                TeslaGunStart.Stop();
                TeslaGunStart.Play();
            } else
            {
                //TeslaGunNoTarget.Stop();
            }
        //}
    
        
            
        
        if (LightningTick)
        {
            
            lightningticktime = Time.time;
            if (!TeslaGunNoTarget.isPlaying) TeslaGunNoTarget.Play();
            TeslaGunNoTarget.volume = 1f;
            Am.am.M.crossfade(TeslaGunNoTarget, 0f, .2f);
        }
        if (lastlightningtick && !LightningTick)
        {

            //if (TeslaGunNoTarget.isPlaying) Am.am.M.crossfade(TeslaGunNoTarget, 0f, .2f);
            if (TeslaGun1Target.isPlaying) Am.am.M.crossfade(TeslaGun1Target, 0f, .2f);
            if (TeslaGunMoreTargets.isPlaying) Am.am.M.crossfade(TeslaGunMoreTargets, 0f, .2f);
            

        }
        lastlightningtick = LightningTick;
        LightningTick = false;
        
}
    private bool lastlightningtick = false;
    private float lightningticktime = -10f;
}
