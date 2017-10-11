using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementGoal : MonoBehaviour {

    // Use this for initialization
    public ParticleSystem AbsorptionParticles;
    public Astronaut.Element MyElementPrize = Astronaut.Element.None;
    public bool PickedUp = false;
	void Start () {
        PickedUp = false;

    }
	
	// Update is called once per frame
	void Update () {
		
	}
    private void OnTriggerEnter2D(Collider2D col)
    {
        Astronaut a = col.gameObject.GetComponent<Astronaut>();
        if ((a != null)&&(a.Alive))
        {
            if (!PickedUp)
            {
                PickedUp = true;
                this.gameObject.SetActive(false);
                foreach (ParticleSystem p in this.gameObject.GetComponentsInChildren<ParticleSystem>())
                {
                    p.transform.SetParent(null);
                    p.Stop();
                    GameObject.Destroy(p.gameObject,15f);
                }
                a.collectElement(MyElementPrize);
                ParticleSystem ps = GameObject.Instantiate(AbsorptionParticles,a.transform.position,new Quaternion()).GetComponent<ParticleSystem>();
                
                ps.gameObject.SetActive(true);
                ps.transform.SetParent(a.transform);
                ps.Play(true);
                GameObject.Destroy(ps.gameObject,15f);
                
            }
        }
    }
}
