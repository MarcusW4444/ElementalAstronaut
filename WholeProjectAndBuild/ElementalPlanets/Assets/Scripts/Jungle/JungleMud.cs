using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JungleMud : Hazard {

    // Use this for initialization
    //What does Mud do anyway?
    /*
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    */

    private void OnTriggerStay2D(Collider2D collision)
    {
        
        

        Astronaut a = collision.gameObject.GetComponent<Astronaut>();
        if ((a != null) && (a.Alive))
        {
            if (!this.Permafrozen)
            {
                if (!a.JungleMudTouching)
                {
                    //Play a jungle mud sound
                    Am.am.M.playGeneralSoundOneShot(Am.am.M.MudSquishSound, Am.am.M.PlayerAudioMixer, .5f, .5f, false, 3f);
                    a.MyMudSplashParticles.Emit(15);
                }
                a.JungleMudTouchTime = Time.time;
                a.JungleMudTouching = true;
            }
        }
    }
}
