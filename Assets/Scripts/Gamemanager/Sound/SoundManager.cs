using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // Audio players components.
    public AudioSource EffectsSource;
    public AudioSource InGameSoundsSource;
    public AudioSource MusicSource;

    // Random pitch adjustment range.
    public float LowPitchRange = 0.9f;
    public float HighPitchRange = 1.1f;

    // Singleton instance.
    public static SoundManager Instance = null;

    // Initialize the singleton instance.
    private void Awake() {
        // If there is not already an instance of SoundManager, set it to this.
        if (Instance == null) {
            Instance = this;
        }
        //If an instance already exists, destroy whatever this object is to enforce the singleton.
        else if (Instance != this) {
            Destroy(gameObject);
        }
    }

    // Play a single clip through the sound effects source.
    public void Play(AudioClip clip) {
        EffectsSource.clip = clip;
        EffectsSource.Play();
    }

    public void PlayInGameSound(AudioClip clip) {
        InGameSoundsSource.clip = clip;
        InGameSoundsSource.Play();
    }

    // Play a single clip through the music source.
    public void PlayMusic(AudioClip clip) {
        MusicSource.clip = clip;
        MusicSource.Play();
    }

    // Play a random clip from an array, and randomize the pitch slightly.
    public void RandomSoundEffect(params AudioClip[] clips) {
        int randomIndex = Random.Range(0, clips.Length);
        float randomPitch = Random.Range(LowPitchRange, HighPitchRange);

        EffectsSource.pitch = randomPitch;
        EffectsSource.clip = clips[randomIndex];
        EffectsSource.Play();
    }

    private float WaitTimerEffect = 0f;
    public void PlayPicthedSoundEffect(AudioClip clip) {

        if (WaitTimerEffect <= Time.unscaledTime) {
            float randomPitch = Random.Range(LowPitchRange, HighPitchRange);

            EffectsSource.pitch = randomPitch;
            EffectsSource.PlayOneShot(clip);

            WaitTimerEffect = Time.unscaledTime + (clip.length * 0.8f);
        }
    }

    private float WaitTimerInGame = 0f;
    public void PlayPicthedInGameSoundEffect(AudioClip clip) {

        if (WaitTimerInGame <= Time.unscaledTime) {
            float randomPitch = Random.Range(LowPitchRange, HighPitchRange);

            InGameSoundsSource.pitch = randomPitch;
            InGameSoundsSource.PlayOneShot(clip);

            WaitTimerInGame = Time.unscaledTime + (clip.length * 0.8f);
        }
    }
}