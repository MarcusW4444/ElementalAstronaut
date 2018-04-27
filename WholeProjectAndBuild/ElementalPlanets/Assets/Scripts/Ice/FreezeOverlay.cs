using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeOverlay : MonoBehaviour {

    // Use this for initialization
    public SpriteRenderer MySpriteRenderer;
    public SpriteRenderer MyParentRenderer;
    //This thing has material properties
    void Start () {
		
	}
	
	// Update is called once per frame
	void LateUpdate () {
        MySpriteRenderer.color = MyParentRenderer.color;

    }
}
