using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound  // this class holds all the data used to load & customize sounds, allowing for easier editing in the inspector
{
    public string name; // the name of this sound

    [HideInInspector]
    public AudioSource source;  // the audio source, or where the noise comes from (not included in the inspector because it's is set through functions)
    public AudioClip clip;  // the clip of audio being played
    
    [Range(0,1)]
    public float volume = 1;    // percent of volume played (0 is none, 1 is the clips original volume)
    [Range(-0.99f, 2f)]
    public float pitchAdjustment;   // adjustment of pitch to the audio clip

    public bool loop;   // should the audio loop constantly?

    public void InitializeAudioSource(AudioSource audioSource)  // takes in an audioSource, and modifies the audioSource to play the sound clip with the desired settings
    {
        source = audioSource;   // sets the audiosource to the inputted source
        source.clip = clip;
        source.volume = volume; // loads the settings to the audio source
        source.pitch = 1 + pitchAdjustment; // adds the pitch adjustment to the default pitch of 1

        source.loop = loop; // more settings transfer
        source.playOnAwake = false; // stop the sound from playing on awake
    }
}
