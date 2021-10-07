using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class audio_manager : MonoBehaviour
{

    public AudioMixer audio_mixer;
    public AudioMixerGroup master;

    [System.Serializable]
    public class Sound_AudioClip
    {
        public Sound sound;
        public AudioClip audioClip;
    }
    public Sound_AudioClip[] sound_array;

    public enum Sound
    {
        shoot_01,
        engine_01,
        explosion_01,
    }

    private void Start()
    {
        audio_mixer.SetFloat("master volume", -20f);
    }

    /// <summary>
    /// Plays the sound given to it
    /// </summary>
    /// <param name="sound">which sound do you want played</param>
    public void Play_Sound(Sound sound, Vector3 position)
    {
        GameObject sound_object = new GameObject("Sound");
        sound_object.transform.position = position;
        AudioSource audio_source = sound_object.AddComponent<AudioSource>();
        audio_source.outputAudioMixerGroup = master;
        audio_source.rolloffMode = AudioRolloffMode.Linear;
        audio_source.dopplerLevel = 0f;
        audio_source.spread = 180f;
        audio_source.spatialBlend = 1f;
        audio_source.maxDistance = 80f;
        audio_source.PlayOneShot(Get_Audio_Clip(sound));
        Destroy(sound_object, Get_Audio_Clip(sound).length);
    }

    /// <summary>
    /// Plays the sound given to it. Allows you to set its pitch
    /// </summary>
    /// <param name="sound">which sound do you want played</param>
    /// <param name="pitch">what pitch will it have</param>
    public void Play_Sound(Sound sound, Vector3 position, float pitch)
    {
        GameObject sound_object = new GameObject("Sound");
        sound_object.transform.position = position;
        AudioSource audio_source = sound_object.AddComponent<AudioSource>();
        audio_source.pitch = pitch;
        audio_source.outputAudioMixerGroup = master;
        audio_source.rolloffMode = AudioRolloffMode.Linear;
        audio_source.dopplerLevel = 0f;
        audio_source.spread = 180f;
        audio_source.spatialBlend = 1f;
        audio_source.maxDistance = 80f;
        audio_source.PlayOneShot(Get_Audio_Clip(sound));
        Destroy(sound_object, Get_Audio_Clip(sound).length);
    }

    /// <summary>
    /// Plays the sound given to it. Uses a pre-existing object to play the sound instead of spawning a new object
    /// </summary>
    /// <param name="sound">which sound do you want played</param>
    /// <param name="sound_object">what object will be playing it</param>
    public void Play_Sound(Sound sound, GameObject sound_object)
    {
        if (sound_object.GetComponent<AudioSource>() == null)
        { //Add an audiosource if the object doesn't already have one
            AudioSource audio_source = sound_object.AddComponent<AudioSource>();
            audio_source.outputAudioMixerGroup = master;
            audio_source.rolloffMode = AudioRolloffMode.Linear;
            audio_source.dopplerLevel = 0f;
            audio_source.spread = 180f;
            audio_source.spatialBlend = 1f;
            audio_source.maxDistance = 80f;
            audio_source.PlayOneShot(Get_Audio_Clip(sound));
        }
        else
        {
            AudioSource audio_source = sound_object.GetComponent<AudioSource>();
            audio_source.outputAudioMixerGroup = master;
            audio_source.rolloffMode = AudioRolloffMode.Linear;
            audio_source.dopplerLevel = 0f;
            audio_source.spread = 180f;
            audio_source.spatialBlend = 1f;
            audio_source.maxDistance = 80f;
            audio_source.PlayOneShot(Get_Audio_Clip(sound));
        }
    }

    /// <summary>
    /// Plays the sound given to it. Uses a pre-existing object to play the sound instead of spawning a new object. Allows you to set its pitch.
    /// </summary>
    /// <param name="sound">which sound do you want played</param>
    /// <param name="sound_object">what object will be playing it</param>
    /// <param name="pitch">what pitch will it have</param>
    public void Play_Sound(Sound sound, GameObject sound_object, float pitch)
    {
        if (sound_object.GetComponent<AudioSource>() == null)
        { //Add an audiosource if the object doesn't already have one
            AudioSource audio_source = sound_object.AddComponent<AudioSource>();
            audio_source.pitch = pitch;
            audio_source.outputAudioMixerGroup = master;
            audio_source.rolloffMode = AudioRolloffMode.Linear;
            audio_source.dopplerLevel = 0f;
            audio_source.spread = 180f;
            audio_source.spatialBlend = 1f;
            audio_source.maxDistance = 80f;
            audio_source.PlayOneShot(Get_Audio_Clip(sound));
        }
        else
        {
            AudioSource audio_source = sound_object.GetComponent<AudioSource>();
            audio_source.pitch = pitch;
            audio_source.outputAudioMixerGroup = master;
            audio_source.rolloffMode = AudioRolloffMode.Linear;
            audio_source.dopplerLevel = 0f;
            audio_source.spread = 180f;
            audio_source.spatialBlend = 1f;
            audio_source.maxDistance = 80f;
            audio_source.PlayOneShot(Get_Audio_Clip(sound));
        }
    }

    //Put this into the volume slider to change the master volume of the game
    public void Set_Volume(float volume)
    {
        audio_mixer.SetFloat("master volume", volume);
        //print(volume);
    }

    //This should get the sound requested from the array
    private AudioClip Get_Audio_Clip(Sound sound)
    {
        foreach (Sound_AudioClip sound_audioclip in sound_array)
        {
            if (sound_audioclip.sound == sound)
            {
                return sound_audioclip.audioClip;
            }
        }
        //Return with an error if the sound needed cant be found
        Debug.LogError("Sound " + sound + " not found!");
        return null;
    }
}
