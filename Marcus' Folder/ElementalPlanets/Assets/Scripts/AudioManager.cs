using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {

    // Use this for initialization
    public static AudioManager TheAudioManager;
    public static AudioManager AM;
	void Start () {
        if (TheAudioManager != null)
        {

            GameObject.Destroy(this.gameObject);
            return;
        }
        
        AudioManager.TheAudioManager = this;
        AudioManager.AM = AudioManager.TheAudioManager;
        GameObject.DontDestroyOnLoad(this.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
    

    public AudioSource AudioSourcePrefab;
    public AudioSource AudioGeneralPrefab;
    public AudioSource createGeneralAudioSource()
    {
        AudioSource a = GameObject.Instantiate(AudioGeneralPrefab).GetComponent<AudioSource>();
        return a;
    }
    public AudioSource createGeneralAudioSource(AudioClip clip, AudioMixerGroup mixer, float volume, float pitch, bool looped)
    {
        AudioSource a = GameObject.Instantiate(AudioGeneralPrefab).GetComponent<AudioSource>();
        a.outputAudioMixerGroup = mixer;
        a.clip = clip;
        a.volume = volume;
        a.pitch = pitch;
        a.loop = looped;
        return a;
    }

    public AudioSource createAudioSource(AudioClip clip, GameObject obj, Vector3 location,AudioMixerGroup mixer, float volume, float pitch, bool looped)
    {
        AudioSource a = GameObject.Instantiate(AudioSourcePrefab, obj.transform).GetComponent<AudioSource>();
        if (obj != null)
        {
            a.transform.position = obj.transform.position+location;
        } else
        {
            a.transform.position = location;
        }

        a.outputAudioMixerGroup = mixer;
        a.clip = clip;
        a.volume = volume;
        a.pitch = pitch;
        a.loop = looped;

        return a;
    }
    public AudioSource createAudioSource(GameObject obj)
    {
        AudioSource a = GameObject.Instantiate(AudioSourcePrefab, obj.transform).GetComponent<AudioSource>();
        a.transform.SetParent(obj.transform);
        a.transform.position = obj.transform.position;
        
        return a;
    }
    public AudioSource createAudioSource()
    {
        AudioSource a = GameObject.Instantiate(AudioSourcePrefab).GetComponent<AudioSource>();
        return a;
    }
    public AudioMixerGroup MusicAudioMixer,EnvironmentAudioMixer, PlayerAudioMixer;
    public void playSoundOnObject(GameObject obj,AudioClip clip,AudioMixerGroup mixer,float volume, float pitch, bool looped,float destroyafter)
    {
        
        AudioSource a = null;
        
        if (obj != null)
        {
            a = obj.GetComponent<AudioSource>();
        }
        
        if (a == null)
        {

            a = createAudioSource(obj);
            if (destroyafter > 0f) GameObject.Destroy(a.gameObject, destroyafter);
        }
        a.outputAudioMixerGroup = mixer;
        a.clip = clip;
        a.volume = volume;
        a.pitch = pitch;
        a.loop = looped;

        
    }
    public AudioSource playSoundAtPoint(Vector3 point, AudioClip clip, AudioMixerGroup mixer, float volume, float pitch, bool looped,float destroyafter)
    {

        AudioSource a = createAudioSource();
        a.outputAudioMixerGroup = mixer;
        a.transform.position = point;
        a.clip = clip;
        a.volume = volume;
        a.pitch = pitch;
        a.loop = looped;
        a.Play();
        if (destroyafter > 0f) GameObject.Destroy(a.gameObject, destroyafter);
        return a;
    }
    public AudioSource playGeneralSound(Vector3 point, AudioClip clip, AudioMixerGroup mixer, float volume, float pitch, bool looped, float destroyafter)
    {

        AudioSource a = createGeneralAudioSource();
        a.outputAudioMixerGroup = mixer;
        a.clip = clip;
        a.volume = volume;
        a.pitch = pitch;
        a.loop = looped;
        a.Play();
        if (destroyafter > 0f) GameObject.Destroy(a.gameObject, destroyafter);
        return a;
    }
    public AudioSource playGeneralSoundOneShot(AudioClip clip, AudioMixerGroup mixer, float volume, float pitch, bool looped, float destroyafter)
    {

        AudioSource a = createGeneralAudioSource();
        a.outputAudioMixerGroup = mixer;
        a.volume = 1f;
        a.pitch = pitch;
        a.loop = looped;
        a.PlayOneShot(clip,volume);
        if (destroyafter > 0f) GameObject.Destroy(a.gameObject, destroyafter);
        return a;
    }
    public AudioSource playInstanceSoundOnObject(GameObject obj, AudioClip clip,float volume,float pitch, bool looped, float destroyafter)
    {

        AudioSource a = createAudioSource(obj);
        
        a.clip = clip;
        a.volume = volume;
        a.pitch = pitch;
        a.loop = looped;
        a.Play();
        if (destroyafter > 0f) GameObject.Destroy(a.gameObject,destroyafter);
        return a;
    }


    public AudioSource CurrentMusic = null;
    public void playMusic(AudioClip clip, float volume, float pitch,bool looped)
    {
        if (CurrentMusic == null)
        {
            //Debug.Log("Create!");
            CurrentMusic = createGeneralAudioSource();
            CurrentMusic.outputAudioMixerGroup = MusicAudioMixer;
            

        }
        CurrentMusic.Stop();
        CurrentMusic.volume = volume;
        CurrentMusic.clip = clip;
        CurrentMusic.pitch = pitch;
        CurrentMusic.loop = looped;
        CurrentMusic.Play();

    }
    public void pauseMusic()
    {
        if (CurrentMusic != null)
        {
            CurrentMusic.Pause();
        }

    }
    public void StopMusic()
    {
        if (CurrentMusic != null)
        {
            CurrentMusic.Stop();
        }
    }

    
    public void crossfade(AudioSource a,float targetvolume,float overtime)
    {
        StartCoroutine(CrossfadeRoutine(a,targetvolume,overtime));
    }
    static IEnumerator CrossfadeRoutine(AudioSource a,float tovolume,float t)
    {
        
        float ctime = Time.time;
        float origvolume = a.volume;
        float lp = a.volume;
        
        while ((Time.time - ctime)<t) {
            if (a.gameObject.activeInHierarchy)
            {
                if (a.volume != lp)
                {
                    break;
                }
                a.volume = Mathf.Lerp(origvolume, tovolume, (Time.time - ctime) / t);
                lp = a.volume;
            }
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        if (a.volume == lp)
        {
            a.volume = tovolume;
        }
    }

    public void crosstune(AudioSource a, float targetpitch, float overtime)
    {
        StartCoroutine(CrosstuneRoutine(a, targetpitch, overtime));
    }
    static IEnumerator CrosstuneRoutine(AudioSource a, float topitch, float t)
    {

        float ctime = Time.time;
        float origpitch = a.pitch;
        
        float lp = origpitch;
        while ((Time.time - ctime) < t)
        {
            if (a.gameObject.activeInHierarchy)
            {
                if (a.pitch != lp) {
                    break;
                }
                a.pitch = Mathf.Lerp(origpitch, topitch, (Time.time - ctime) / t);
                lp = a.pitch;
            }
            yield return new WaitForSeconds(Time.fixedDeltaTime);
        }

        if (a.pitch == lp)
        {
            a.pitch = topitch;
        }
    }


    //Music
    public AudioClip PlanetSelectMusic, IcePlanetMusic, FirePlanetMusic, JunglePlanetMusic, JungleBranchesMusic, BossMusic, BossMusicPinch,LevelCompleteMusic;
    public bool soundaudiodivider;
    //Sounds
    public AudioClip PistolShoot, PistolEmpty, ShotgunShoot, MachinegunShootLoop, BeamRifleStart, BeamRifleLoop1, BeamRifleLoop2, BeamRifleHit, BeamRifleDamage,BulletMiss, BulletHitEnemy,HitMarker,AstronautFalling,AstronautLanding1,AstronautLanding2,AstronautDeath1, AstronautDeath2, AstronautDeath3,StimSound,JumpSound,DoubleJumpSound,JumpLand,ElementGetSound,ElementAbsorbingSound,FlameHoundShoot,LavaBurn,FireballHit,FireBallEnemyExplosion,FireElementPower,JungleElementPower,FireBossRoar,IceBossRoar,JungleBossRoar;
}
