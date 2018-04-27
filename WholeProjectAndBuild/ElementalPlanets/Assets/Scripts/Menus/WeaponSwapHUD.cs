using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponSwapHUD : MonoBehaviour {

    // Use this for initialization


    public Image CurrentWeaponImage;
    public Image CurrentWeapon2StockImage, CurrentWeapon3StockImage, CurrentWeapon4StockImage;
    public Image CurrentWeaponEmptySpace;
    

    public Text AmmoLabelText, WeaponNameText;
    public string AmmoLabelString;
    public string WeaponName;
    public Image[] SwapWeaponImages;
    public Image[] SwapWeaponMainArcs;
    public Image[] SwapWeaponSubArcs;
    public Image[] SwapWeaponNewWepArc;
    public Transform SelectedCircle;
    public Sprite[] WeaponFlatSprites;
    public Sprite PistolFlatSprite, ShotgunFlatSprite,MachineGunFlatSprite,LaserBeamFlatSprite,GrenadeLauncherFlatSprite,TeslaCannonFlatSprite,RailGunFlatSprite;
    
    
    void Start () {
		
	}
    
     
    // Update is called once per frame
    public GameObject TotalGroup;
    public GameObject AmmoGroup;
    public GameObject SwapGroup;
    public float SwapGroupScale = 0f;
    public float BaseSwapGroupScale = .75f;
    public GameObject RailGunChargeBarGroup;
    public Image RailGunChargeBar;


    void Update () { 
        Astronaut plr = Astronaut.TheAstronaut;
        float thr = .25f;
        if (((plr != null) && (plr.Alive))) {
            //Draw the weapon swap hud
            //            SwapGroup.SetActive(false); //not now//YES NOW. I took care of it
            Color swpcol = Color.white;
            float re = 1f+(2f*(1f-(((Time.time - plr.NewWeaponAcquiredTime) % .5f) / .5f)));
            Vector3 selpos = SelectedCircle.transform.position;
            for (int i = 0; i < 7; i++)
            {
                float arcammo = 0f;

                if (i == 0)
                {
                    //Pistol
                    //SwapWeaponImages
                    arcammo = (float)plr.PistolAmmo / Astronaut.MAXPISTOLAMMO;
                    if (plr.CurrentSpecialWeapon == Astronaut.SpecialWeapon.Pistol)
                        selpos = SwapWeaponMainArcs[i].transform.position;
                    

                }
                else if (i == 1)
                {
                    //Shotgun
                    arcammo = (float)plr.ShotgunAmmo / Astronaut.SHOTGUNAMMOINCREMENT;
                    if (plr.CurrentSpecialWeapon == Astronaut.SpecialWeapon.Shotgun)
                        selpos = SwapWeaponMainArcs[i].transform.position;

                    if ((plr.NewWeaponThatWasAcquired == Astronaut.SpecialWeapon.Shotgun)  && ((Time.time - plr.NewWeaponAcquiredTime) <= 2f))
                    {
                        SwapWeaponNewWepArc[i].enabled = true;
                        SwapWeaponNewWepArc[i].transform.localScale = Vector3.one * re;
                    } else
                    {
                        SwapWeaponNewWepArc[i].enabled = false;
                    }

                }
                else if (i == 2)
                {
                    //Machine Gun
                    arcammo = (float)plr.GatlingAmmo / Astronaut.MACHINEGUNAMMOINCREMENT;
                    if (plr.CurrentSpecialWeapon == Astronaut.SpecialWeapon.Gatling)
                        selpos = SwapWeaponMainArcs[i].transform.position;
                    if ((plr.NewWeaponThatWasAcquired == Astronaut.SpecialWeapon.Gatling)  && ((Time.time - plr.NewWeaponAcquiredTime) <= 2f))
                    {
                        SwapWeaponNewWepArc[i].enabled = true;
                        SwapWeaponNewWepArc[i].transform.localScale = Vector3.one * re;

                    }
                    else
                    {
                        SwapWeaponNewWepArc[i].enabled = false;
                    }
                }
                else if (i == 3)
                {
                    //Laser Rifle
                    arcammo = (float)plr.LaserAmmo / Astronaut.LASERGUNAMMOINCREMENT;
                    if (plr.CurrentSpecialWeapon == Astronaut.SpecialWeapon.Laser)
                        selpos = SwapWeaponMainArcs[i].transform.position;
                    if ((plr.NewWeaponThatWasAcquired == Astronaut.SpecialWeapon.Laser)  && ((Time.time - plr.NewWeaponAcquiredTime) <= 2f))
                    {
                        SwapWeaponNewWepArc[i].enabled = true;
                        SwapWeaponNewWepArc[i].transform.localScale = Vector3.one * re;
                    }
                    else
                    {
                        SwapWeaponNewWepArc[i].enabled = false;
                    }
                }
                else if (i == 4)
                {
                    //GrenadeLauncher
                    arcammo = (float)plr.GrenadeLauncherAmmo / Astronaut.GRENADELAUNCHERAMMOINCREMENT;
                    if (plr.CurrentSpecialWeapon == Astronaut.SpecialWeapon.GrenadeLauncher)
                        selpos = SwapWeaponMainArcs[i].transform.position;
                    if ((plr.NewWeaponThatWasAcquired == Astronaut.SpecialWeapon.GrenadeLauncher)  && ((Time.time - plr.NewWeaponAcquiredTime) <= 2f))
                    {
                        SwapWeaponNewWepArc[i].enabled = true;
                        SwapWeaponNewWepArc[i].transform.localScale = Vector3.one * re;
                    }
                    else
                    {
                        SwapWeaponNewWepArc[i].enabled = false;
                    }
                }
                else if (i == 5)
                {
                    //TeslaGun
                    arcammo = (float)plr.TeslaAmmo / Astronaut.TESLAGUNAMMOINCREMENT;
                    if (plr.CurrentSpecialWeapon == Astronaut.SpecialWeapon.TeslaGun)
                        selpos = SwapWeaponMainArcs[i].transform.position;
                    if ((plr.NewWeaponThatWasAcquired == Astronaut.SpecialWeapon.TeslaGun) && ((Time.time - plr.NewWeaponAcquiredTime) <= 2f))
                    {
                        SwapWeaponNewWepArc[i].enabled = true;
                        SwapWeaponNewWepArc[i].transform.localScale = Vector3.one * re;
                    }
                    else
                    {
                        SwapWeaponNewWepArc[i].enabled = false;
                    }
                }
                else if (i == 6)
                {
                    //Railgun
                    arcammo = (float)plr.RailGunAmmo/ Astronaut.RAILGUNAMMOINCREMENT;
                    if (plr.CurrentSpecialWeapon == Astronaut.SpecialWeapon.RailGun)
                      selpos = SwapWeaponMainArcs[i].transform.position;
                    
                    if ((plr.NewWeaponThatWasAcquired == Astronaut.SpecialWeapon.RailGun) && ((Time.time - plr.NewWeaponAcquiredTime) <= 2f))
                    {
                        SwapWeaponNewWepArc[i].enabled = true;
                        SwapWeaponNewWepArc[i].transform.localScale = Vector3.one * re;
                    }
                    else
                    {
                        SwapWeaponNewWepArc[i].enabled = false;
                    }
                    
                    
                }



                if ((arcammo <= 0f) && (i != 0))
                {
                    SwapWeaponMainArcs[i].enabled = false;
                    SwapWeaponSubArcs[i].enabled = false;
                    SwapWeaponImages[i].color = new Color(.8f, .8f, .8f, .4f);
                }
                else
                if (arcammo <= thr)
                {
                    SwapWeaponMainArcs[i].enabled = true;
                    SwapWeaponSubArcs[i].enabled = true;
                    swpcol = Color.Lerp(new Color(1f, 0f, 0f, 1f), new Color(1f, 1f, 1f, 1f), .5f + (Mathf.Sin((Time.time / .3f) * Mathf.PI) * .5f));
                    SwapWeaponImages[i].color = swpcol;
                    SwapWeaponMainArcs[i].fillAmount = (arcammo / thr);
                    SwapWeaponSubArcs[i].color = new Color(.5f, .0f, .0f, .3f);
                }
                else if (arcammo < .5f)
                {
                    SwapWeaponMainArcs[i].enabled = true;
                    SwapWeaponSubArcs[i].enabled = true;
                    swpcol = new Color(1f, .5f, 0f);
                    SwapWeaponImages[i].color = Color.white;
                    SwapWeaponMainArcs[i].fillAmount = arcammo;
                    SwapWeaponSubArcs[i].color = new Color(.5f, .5f, .5f, .3f);
                }
                else
                {
                    SwapWeaponMainArcs[i].enabled = true;
                    SwapWeaponSubArcs[i].enabled = true;
                    swpcol = new Color(1f, 1f, .5f);

                    if (arcammo <= 1f)
                    {
                        SwapWeaponMainArcs[i].color = swpcol;
                        SwapWeaponMainArcs[i].fillAmount = arcammo;
                        SwapWeaponSubArcs[i].color = new Color(.5f, .5f, .5f, .3f);

                    }
                    else
                    {
                        SwapWeaponMainArcs[i].color = swpcol;
                        SwapWeaponMainArcs[i].fillAmount = (arcammo % 1f);
                        SwapWeaponSubArcs[i].color = (swpcol * (1f - (.25f * ((int)arcammo))));

                    }



                }







            }
            SelectedCircle.transform.position = Vector3.Lerp(SelectedCircle.transform.position, selpos, .25f);
            if (plr.SwappingWeapons || ((Time.time - plr.NewWeaponAcquiredTime) < 2f))
            {
                if (SwapGroupScale < 1f)
                {
                    //Play a sound
                }
                SwapGroupScale = 1f;
                if (plr.SwappingWeapons)
                {
                    plr.NewWeaponAcquiredTime = -10f;
                }
            } else
            {


                
                if (SwapGroupScale < .01f)
                {
                    SwapGroupScale = 0f;
                } else
                {
                    SwapGroupScale = (SwapGroupScale * .8f);//SwapGroupScale = Mathf.Lerp(SwapGroupScale, 0f, Mathf.Clamp01(Time.deltaTime * 8f)); //Mathf.Clamp01(Time.time - plr.SwapWeaponViewTime)
                }
            }
            //Debug.Log(SwapGroupScale);
            SwapGroup.transform.localScale = (Vector3.one* BaseSwapGroupScale * SwapGroupScale);
            //SwapGroup.SetActive(SwapGroupScale > .01f);
            
        }

        if ((plr != null) && (plr.Alive) && (plr.PlayerHasControl) && (!plr.EtherealLock))
        {
            TotalGroup.SetActive(true);
        } else
        {
            TotalGroup.SetActive(false);

        }
            
        if ((plr != null) && (plr.Alive)&&(plr.PlayerHasControl) &&(!plr.EtherealLock) && (!plr.SwappingWeapons))
        {

            
            




            //plr.LastGunHUDAlertTime = ;
            Sprite spr = PistolFlatSprite;
            float ammovalue = 0f;
            Color wepcol = Color.white;
            string ammotext = "";
            string weaponname = "";
            if (plr.CurrentSpecialWeapon == Astronaut.SpecialWeapon.Pistol)
            {
                spr = PistolFlatSprite;
                ammovalue = (float)plr.PistolAmmo/Astronaut.MAXPISTOLAMMO;
                wepcol = Color.yellow;
                ammotext = ""+ plr.PistolAmmo;
                weaponname = "PISTOL";
            } else if (plr.CurrentSpecialWeapon == Astronaut.SpecialWeapon.Shotgun)
            {
                spr = ShotgunFlatSprite;
                ammovalue = (float)plr.ShotgunAmmo / Astronaut.SHOTGUNAMMOINCREMENT;
                wepcol = Color.yellow;
                ammotext = "" + plr.ShotgunAmmo;
                weaponname = "SHOTGUN";
            }
            else if (plr.CurrentSpecialWeapon == Astronaut.SpecialWeapon.Gatling)
            {
                spr = MachineGunFlatSprite;
                ammovalue = (float)plr.GatlingAmmo / Astronaut.MACHINEGUNAMMOINCREMENT;
                wepcol = Color.yellow;
                ammotext = "" + plr.GatlingAmmo;
                weaponname = "MACHINEGUN";
            }
            else if (plr.CurrentSpecialWeapon == Astronaut.SpecialWeapon.Laser)
            {
                spr = LaserBeamFlatSprite;
                ammovalue = plr.LaserAmmo / Astronaut.LASERGUNAMMOINCREMENT;
                wepcol = Color.yellow;
                ammotext = "" + (int)plr.LaserAmmo+"%";
                weaponname = "LASER RIFLE";
            }
            else if (plr.CurrentSpecialWeapon == Astronaut.SpecialWeapon.GrenadeLauncher)
            {
                spr = GrenadeLauncherFlatSprite;
                ammovalue = (float)plr.GrenadeLauncherAmmo/ Astronaut.GRENADELAUNCHERAMMOINCREMENT;
                wepcol = Color.yellow;
                ammotext = "" + plr.GrenadeLauncherAmmo;
                weaponname = "GRENADE LAUNCHER";
            }
            else if (plr.CurrentSpecialWeapon == Astronaut.SpecialWeapon.TeslaGun)
            {
                spr = TeslaCannonFlatSprite;
                ammovalue = plr.TeslaAmmo / Astronaut.TESLAGUNAMMOINCREMENT;
                wepcol = new Color(0.6f,0.6f,1f);
                ammotext = "" + (int)plr.TeslaAmmo + "%";
                weaponname = "TESLA GUN";
            }
            else if (plr.CurrentSpecialWeapon == Astronaut.SpecialWeapon.RailGun)
            {
                spr = RailGunFlatSprite;
                ammovalue = (float)plr.RailGunAmmo / Astronaut.RAILGUNAMMOINCREMENT;
                wepcol = Color.yellow;
                ammotext = "" + plr.RailGunAmmo + "";
                weaponname = "RAIL GUN";
            }
            
            CurrentWeaponEmptySpace.sprite = spr;
            CurrentWeaponImage.sprite = spr;

            CurrentWeapon2StockImage.sprite = spr;
            CurrentWeapon3StockImage.sprite = spr;
            CurrentWeapon4StockImage.sprite = spr;
            


            CurrentWeapon4StockImage.enabled = (ammovalue > 3f);
            CurrentWeapon4StockImage.color = wepcol * .25f;
            CurrentWeapon3StockImage.enabled = (ammovalue > 2f);
            CurrentWeapon3StockImage.color = wepcol * .5f;
            CurrentWeapon2StockImage.enabled = (ammovalue > 1f);
            CurrentWeapon2StockImage.color = wepcol * .75f;
            float alpha = .8f;
            
            if (ammovalue <= thr) 
            {
                wepcol = Color.Lerp(new Color(1f, 0f, 0f, alpha ), new Color(1f, 1f, 1f, alpha), .5f+(Mathf.Sin((Time.time / .3f)*Mathf.PI)*.5f));
                CurrentWeaponImage.color = wepcol;
                CurrentWeaponEmptySpace.color = Color.Lerp(new Color(1f, 0f, 0f, alpha * .4f), new Color(.5f,.5f,.5f,alpha*.4f), .5f + (Mathf.Cos((Time.time / .3f) * Mathf.PI) * .5f));
                CurrentWeaponEmptySpace.enabled = true;
                CurrentWeaponImage.fillAmount = (ammovalue/thr);
            } else if (ammovalue < .5f)
            {
                wepcol = new Color(1f,.5f,0f);
                CurrentWeaponImage.fillAmount = ammovalue;
                CurrentWeaponImage.color = wepcol;
                CurrentWeaponEmptySpace.enabled = true;
                CurrentWeaponEmptySpace.color = new Color(.5f, .5f, .5f, .5f);
            } else
            {
                CurrentWeaponImage.color = wepcol;
                if (ammovalue <= 1f)
                {
                    CurrentWeaponImage.fillAmount = ammovalue;
                    
                    CurrentWeaponEmptySpace.enabled = true;
                    CurrentWeaponEmptySpace.color = new Color(.5f, .5f, .5f, .5f);
                } else
                {
                    if ((ammovalue % 1f) == 0f)
                        CurrentWeaponImage.fillAmount = 1f;
                    else
                    CurrentWeaponImage.fillAmount = (ammovalue % 1f);
                    CurrentWeaponEmptySpace.enabled = true;
                    CurrentWeaponEmptySpace.color = new Color(.5f, .5f, .5f, .15f);
                }
                    
                

            }

            float sc = 1f+(1f*(1f-Mathf.Pow(Mathf.Clamp01((Time.time - Mathf.Max(plr.lastshottime,plr.lastpistolshottime))/2f),1f/3f)));
            CurrentWeaponImage.transform.localScale = Vector3.one * sc;
            CurrentWeaponEmptySpace.transform.localScale = Vector3.one * sc;
            AmmoGroup.SetActive(true);



            AmmoLabelString = ammotext;
            WeaponName = weaponname;

            if (plr.CurrentSpecialWeapon == Astronaut.SpecialWeapon.RailGun)
            {
                float rp = plr.RailGunCharge;
                if (plr.RailGunCharge > 0f)
                {
                    if (rp < 1f)
                    {
                        RailGunChargeBar.color = Color.Lerp(new Color(1f, 1f, 1f, .75f), new Color(1f, 1f, .5f, .75f), rp);
                        RailGunChargeBarGroup.transform.localScale = (Vector3.Lerp(RailGunChargeBarGroup.transform.localScale, Vector3.one, .1f));
                    }
                    else
                    {
                        RailGunChargeBar.color = ((((rp-1f)>=(3f/5f))&&((rp-1f)%(1f/8f))<(1f/16f))?Color.red:Color.Lerp(new Color(1f, 1f, 1f, .75f), new Color(1f, 1f, 0f, .75f), Mathf.Pow(Mathf.Sin(Mathf.PI*2f*(rp-1f)*5f),2f)));
                        RailGunChargeBarGroup.transform.localScale = (Vector3.Lerp(Vector3.one*1.5f, Vector3.one, rp-1f));

                    }

                    RailGunChargeBar.fillAmount = Mathf.Clamp01(plr.RailGunCharge);
                } else
                {
                    RailGunChargeBar.color = Color.Lerp(RailGunChargeBar.color,new Color(RailGunChargeBar.color.r, RailGunChargeBar.color.g, RailGunChargeBar.color.b,0f),.1f);
                    RailGunChargeBarGroup.transform.localScale = (Vector3.Lerp(RailGunChargeBarGroup.transform.localScale, Vector3.one, .1f));
                }
                
                RailGunChargeBarGroup.SetActive(true);
            } else
            {
                RailGunChargeBarGroup.SetActive(false);
                
            }

            

        } else
        {
            AmmoGroup.SetActive(false);
        }
        AmmoLabelText.text = AmmoLabelString;
        WeaponNameText.text = WeaponName;
    }
}
