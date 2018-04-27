using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour {
    public TutorialSystem.TutorialTip TipType;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Astronaut a = collision.GetComponent<Astronaut>();
        if (a != null)
        {
            GameManager.TheGameManager.showTutorialTip(TipType);
        }
    }
}
