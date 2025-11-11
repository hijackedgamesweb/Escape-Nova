using System;
using Code.Scripts.Patterns.Singleton;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    public AudioMixer audioMixer;
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxSource;

    private float _masterVolume = 1f;
    
    //M�todos para reproducir musica y SFX
    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);

        if (s != null) 
        {
            musicSource.clip = s.clip;
            musicSource.Play();
        }
        else 
        {
            Debug.Log("Sound not Found");
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);

        if (s != null)
        {
            sfxSource.PlayOneShot(s.clip);
        }
        else
        {
            Debug.Log("Sound not Found");
        }
    }
    
    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }


    //Metodos para configurar el sonido
    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }


    public void ToggleSFX()
    {
        sfxSource.mute = !sfxSource.mute;
    }

    
    public void SetMasterVolume(float volume)
    {
        _masterVolume = volume;
        audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
        if(volume == 0)
            audioMixer.SetFloat("MasterVolume", -80);
    }
    
    
    public void SetMusicVolume(float volume)
    {
        
        audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
        if(volume == 0)
            audioMixer.SetFloat("MusicVolume", -80);
    }


    public void SetSFXVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
        if(volume == 0)
            audioMixer.SetFloat("SFXVolume", -80);
    }
    
    
    public float GetSFXVolume()
    {
        return sfxSource.volume;
    }
    
    
    public float GetMusicVolume()
    {
        return musicSource.volume;
    }
    
    
    public float GetMasterVolume()
    {
        return _masterVolume;
    }
    
    
    public void StopMusic()
    {
        musicSource.Stop();
        musicSource.clip = null;
    }
}