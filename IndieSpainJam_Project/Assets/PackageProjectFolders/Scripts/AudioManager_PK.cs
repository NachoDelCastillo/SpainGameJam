using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class AudioManager_PK : MonoBehaviour
{
    public Sound[] sounds;

    public static AudioManager_PK instance;
    public static AudioManager_PK GetInstance() { return instance; }

    void Awake()
    {
        // Singleton
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);

        foreach (Sound s in sounds)
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.music;
        }

        // Valores iniciales de los sliders de musica y sonido
        AudioVolume(PlayerPrefs.GetFloat("SoundVolume", 5), false);
        AudioVolume(PlayerPrefs.GetFloat("MusicVolume", 5), true);

        ChangeBackgroundMusic(SceneManager.GetActiveScene().name);
    }

    public void ChangeBackgroundMusic(string sceneName) // Cambia la muscia de fondo segun la escena, llamado desde 
    {
        switch (sceneName)
        {
            case "MainMenu_Scene":
                Stop("GameMusic");
                Play("MainMenu_Music", 1);
                break;
            default:
                Stop("MainMenu_Music");
                Play("GameMusic", 1);
                break;
        }
    }

    public void Play(string name, float pitch)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.pitch = pitch;
        s.source.Play();
    }

    public void Stop(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sound: " + name + " not found!");
            return;
        }
        s.source.Stop();
    }

    public void AudioVolume(float volume, bool isMusic) // Se llama cada vez que se modifica el sonido o la musica
    {
        // Almacenar valores en PlayerPref
        if (isMusic)  // Volumen Musica
            PlayerPrefs.SetFloat("MusicVolume", volume);

        if (!isMusic) // Volumen Sonido
            PlayerPrefs.SetFloat("SoundVolume", volume);

        // Convertir a un valor sobre 1
        volume = volume / 10;

        AudioSource[] AllaudioSources = GetComponents<AudioSource>();

        for (int i = 0; i < AllaudioSources.Length; i++)
        {
            if (sounds[i].music == isMusic)
                AllaudioSources[i].volume = volume * sounds[i].maxVolume; // Cambiar el volumen de todas las musicas/sonidos dependiendo de su "maxVolume"
        }
    }
}
