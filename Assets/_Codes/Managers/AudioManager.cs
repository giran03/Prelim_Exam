using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] Sound[] musicSounds, sfxSounds;
    [SerializeField] AudioSource musicSource, sfxSource;

    public static AudioManager Instance;
    AudioSource currentSfxLooping;
    AudioSource currentMusic;
    bool hasTriggered;
    bool bgmPlaying;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }

    private void Update()
    {
        if (SceneHandler.Instance.CurrentScene() == "_Level-1" && !bgmPlaying)
        {
            bgmPlaying = true;
            PlayMusic("BGM");
        }
    }

    public void PlayMusic(string clipName)
    {
        Sound sound = Array.Find(musicSounds, x => x.clipName == clipName);
        if (sound != null)
        {
            musicSource.clip = sound.audioClip;
            musicSource.loop = true;
            musicSource.volume = sound.volume;
            musicSource.Play();
            currentMusic = musicSource;
        }
    }

    public void PlaySFX(string clipName, Vector3 position, bool playOnce = false, bool loop = false)
    {
        Sound sound = Array.Find(sfxSounds, x => x.clipName == clipName);
        if (sound != null)
        {
            sfxSource.loop = loop;

            if (loop)
            {
                if (currentSfxLooping != null)
                    currentSfxLooping.Stop();

                sfxSource.clip = sound.audioClip;
                sfxSource.volume = sound.volume;
                sfxSource.Play();
                currentSfxLooping = sfxSource;
            }
            else if (playOnce)
            {
                if (!hasTriggered)
                {
                    sfxSource.clip = sound.audioClip;
                    sfxSource.volume = sound.volume;
                    sfxSource.Play();
                    StartCoroutine(CooldownSetTimer(3f));
                }
            }
            else
            {
                AudioSource.PlayClipAtPoint(sound.audioClip, position);
                StartCoroutine(Cooldown(sound.audioClip));
            }
        }
    }

    public void ButtonPressSFX() => PlaySFX("Press", Camera.main.transform.position);

    public void StopMusic()
    {
        if (currentMusic != null)
        {
            currentMusic.Stop();
            currentMusic = null;
        }
    }

    public void StopSFX()
    {
        if (currentSfxLooping != null)
        {
            currentSfxLooping.Stop();
            currentSfxLooping = null;
        }
    }

    IEnumerator Cooldown(AudioClip audioClip)
    {
        yield return new WaitForSeconds(audioClip.length);
    }

    IEnumerator CooldownSetTimer(float timer)
    {
        hasTriggered = true;
        yield return new WaitForSeconds(timer);
        hasTriggered = false;
    }
}
