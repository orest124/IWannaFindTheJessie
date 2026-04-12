using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{
    public AudioClip[] _sounds;
    public AudioClip[] _sounds2;
    private AudioSource audioSrc => GetComponent<AudioSource>();
    public void PlaySound(AudioClip clip, float volume = 1f, bool destroyed = false, float p1 = 0.85f, float p2 = 1.2f,bool pitch = true)
    {
        if(pitch)audioSrc.pitch = Random.Range(p1,p2);
        audioSrc.PlayOneShot(clip, volume);
            
        
    }
    
    public void PlayTheme(int n)
    {
        if(audioSrc.clip == _sounds[n]) return;
        audioSrc.clip = _sounds[n];
        audioSrc.Play();
        
    }

    public void HasLoop(bool itIs = true)
    {
        audioSrc.loop = itIs;
    }
    public void StopMusic()
    {
        audioSrc.Stop();
    }
}
