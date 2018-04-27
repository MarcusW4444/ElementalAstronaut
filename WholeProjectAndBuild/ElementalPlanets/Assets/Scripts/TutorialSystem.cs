using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialSystem : MonoBehaviour {

    // Use this for initialization
    public enum TutorialTip { SkipTip,IgnoreTips,WalkMovement, Jump, DoubleJump, Injured, VitaPower, VitaDifficulty, VitaDropped,TooManyDeaths, Revive, OnRevived,Shoot, SwapWeapons, SwapPistol, Shotgun, Laser, Machinegun, ElementalPower, MeltableWall, ChoosePlanet,UseElement,ScrollElement,PistolFastFire, count };
    public static bool[] TutorialHintArray;
    public static bool[] TutorialQueuedArray;
}
