using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour   // manages all game sound
{
    public static AudioManager instance;    // singleton

    public Sound[] sounds;  // array of sounds class, stores the sounds used in the game

    void Awake()
    {
        // initializing singleton
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        // end of sington

        foreach (Sound sound in sounds)
        {   // initializes the sounds with new audioSources to be used in game. Any changes made during runtime will not be saved of put into effect
            sound.InitializeAudioSource(gameObject.AddComponent<AudioSource>());
        }
    }

    public void PlaySound(string name)     // plays the sound with the matching name inputted. Used throughgout code files to play sounds
    {
        Sound sound = Array.Find(sounds, Sound => Sound.name == name);  // looks for a sound in the sound array with the a matching name, and saves that sound

        if (sound != null)
        {
        //Debug.Log("Playing sound " + name);
        sound.source.Play();    // if we found a sound, play it
        }
        else
        {   // otherwise, notify that no sound has this name
            Debug.LogWarning("Sound " + name + " not found");
        }
    }

    public void StopSound(string name)  // the exact same code as the play sound function, except it stops the sound with the inputted string instead of playing it
    {
        Sound sound = Array.Find(sounds, Sound => Sound.name == name);

        if (sound != null)
        {
            //Debug.Log("Playing sound " + name);
            sound.source.Stop();    // stops the sound, if found
        }
        else
        {
            Debug.LogWarning("Sound " + name + " not found");
        }
    }

    public void StopAllSounds()
    {   // loops through all sounds, and stops them all. Used to completely silence the game
        foreach (Sound sound in sounds)
        {
            sound.source.Stop();
        }
    }
}
