using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    // Use this for initialization
    public bool IceWorldCompleted = false, FireWorldCompleted = false, JungleWorldCompleted = false, VoidWorldCompleted = false;
    public int StimPackInventory = 0, ShotgunInventory = 0, MachineGunInventory = 0;
    public float LaserRifleInventory = 0f;
    public float TeslaInventory = 0f;
    public int GrenadeLauncherInventory = 0;
    public int IceVitaLevelAchieved = -1, FireVitaLevelAchieved = -1, JungleVitaLevelAchieved = -1, VoidVitaLevelAchieved = -1;
    public int TotalDeaths = 0;

    public static GameManager TheGameManager = null;
    public Astronaut.Element ElementStage = Astronaut.Element.None;
    void Start () {
		if (TheGameManager != null)
        {
            
            GameObject.Destroy(this.gameObject);
            return;
        }
        GameManager.TheGameManager = this;
        GameObject.DontDestroyOnLoad(this.gameObject);
	}
    public void completeStage(Astronaut.Element stageelement)
    {

        switch (stageelement)
        {
            case Astronaut.Element.Fire:
                {
                    FireVitaLevelAchieved = Mathf.Max(Astronaut.TheAstronaut.VitaLevel,FireVitaLevelAchieved);
                    SceneManager.LoadScene(0);
                    break;
                }
            case Astronaut.Element.Ice:
                {
                    IceVitaLevelAchieved = Mathf.Max(Astronaut.TheAstronaut.VitaLevel, IceVitaLevelAchieved);
                    SceneManager.LoadScene(0);
                    break;
                }
            case Astronaut.Element.Grass:
                {
                    JungleVitaLevelAchieved = Mathf.Max(Astronaut.TheAstronaut.VitaLevel, JungleVitaLevelAchieved);
                    SceneManager.LoadScene(0);
                    break;
                }
            case Astronaut.Element.Void:
                {
                    //If you complete this stage, you've beaten the game
                    break;
                }
        }

        ShotgunInventory = Astronaut.TheAstronaut.ShotgunAmmo;
        MachineGunInventory = Astronaut.TheAstronaut.GatlingAmmo;
        LaserRifleInventory = Astronaut.TheAstronaut.LaserAmmo;
        GrenadeLauncherInventory = Astronaut.TheAstronaut.GrenadeLauncherAmmo;
        TeslaInventory = Astronaut.TheAstronaut.TeslaAmmo;
        StimPackInventory = Astronaut.TheAstronaut.StimPacks;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
