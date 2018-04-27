using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Am
{



    public class am : MonoBehaviour
    {

        public static AudioManager M;
        public static AudioSource pointsound(AudioClip cl,Vector3 v)
        {

            return AudioManager.TheAudioManager.createAudioSource(cl, null,v,AudioManager.AM.EnvironmentAudioMixer, 1f, 1f, false);
        }
        public static AudioSource sound(AudioClip cl)
        {
            AudioSource a = AudioManager.TheAudioManager.createGeneralAudioSource(cl, AudioManager.AM.PlayerAudioMixer, 1f, 1f, false);
            a.Play();
            return a;
        }
        public static AudioSource loop(AudioClip cl)
        {
            AudioSource a = AudioManager.TheAudioManager.createGeneralAudioSource(cl, AudioManager.AM.PlayerAudioMixer, 1f, 1f, true);
            return a;
        }
        public static AudioSource createsound(AudioClip cl)
        {

            return AudioManager.TheAudioManager.createGeneralAudioSource(cl, AudioManager.AM.PlayerAudioMixer, 1f, 1f, false);
        }
        public static AudioSource oneshot(AudioClip cl)
        {
            return AudioManager.TheAudioManager.playGeneralSoundOneShot(cl, AudioManager.AM.PlayerAudioMixer, 1f, 1f, false,10f);
        }
        public static AudioSource oneshot(AudioClip cl,float vol)
        {
            return AudioManager.TheAudioManager.playGeneralSoundOneShot(cl, AudioManager.AM.PlayerAudioMixer, vol, 1f, false, 10f);
        }
    }

    
}

