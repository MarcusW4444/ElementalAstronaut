using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBeam : MonoBehaviour {

    // Use this for initialization
    public bool BeamActive = false;
    public LineRenderer MyLineRenderer;
    public ParticleSystem EmitParticles,EyeFlareParticles,HitParticles,BurnParticles;

	void Start () {
		
	}

    // Update is called once per frame
    private AudioSource BeamSound, BeamHitSound, BeamScorchSound;
	void Update () {
        if (BurnParticles.isPlaying) BurnParticles.Stop();
        if (BeamActive)
        {

            
            if (!EmitParticles.isPlaying) EmitParticles.Play();
            if (!EyeFlareParticles.isPlaying) EyeFlareParticles.Play();
            Vector2 dir = new Vector2(this.transform.forward.x, this.transform.forward.y);
            Ray2D r = new Ray2D(this.transform.position, dir.normalized);

            float maxfiredist = 30f;
            RaycastHit2D re = Physics2D.Raycast(r.origin, r.direction, maxfiredist, LayerMask.GetMask(new string[] { "Geometry","Player"}));

            if (!(re.distance <= 0f)) {
                Astronaut a = re.collider.gameObject.GetComponent<Astronaut>();
                if ((a != null))
                {
                    if ((a.Alive))
                    {
                        float dps = 80f;
                        a.TakeDamage(dps * Time.deltaTime, new Vector3());
                        BurnParticles.transform.position = new Vector3(re.point.x, re.point.y, 0f);
                        BurnParticles.Emit(10);
                    }
                    HitParticles.transform.position = new Vector3(re.point.x, re.point.y, 0f);
                    HitParticles.Emit(1);
                } else if (re.collider.gameObject.layer == LayerMask.NameToLayer("Geometry"))
                {
                    HitParticles.transform.position = new Vector3(re.point.x,re.point.y,0f);
                    HitParticles.Emit(1);
                    BurnParticles.transform.position = new Vector3(re.point.x, re.point.y, 0f);
                    BurnParticles.Emit(1);
                }
                //Perform Raycasts
                MyLineRenderer.SetPosition(0, r.origin);
                MyLineRenderer.SetPosition(1, re.point);
            }
            else
            {
                //Miss
                MyLineRenderer.SetPosition(0,r.origin);
                MyLineRenderer.SetPosition(1, r.origin+((r.direction.normalized)*maxfiredist));
                
            }
                    MyLineRenderer.enabled = true;
        }
        else
        {
            MyLineRenderer.enabled = false;
            if (EmitParticles.isPlaying) EmitParticles.Stop();
            if (EyeFlareParticles.isPlaying) EyeFlareParticles.Stop();
        }
	}
    public void OnBeamHit(Collider2D col)
    {

    }
}
