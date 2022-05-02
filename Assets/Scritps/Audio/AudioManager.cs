using System.Collections;
using System;
using UnityEngine;
using UnityEngine.Audio;
using Baks.Runtime.Utils;
using NaughtyAttributes;

namespace Baks
{
    [DisallowMultipleComponent]
    public class AudioManager : MonoBehaviour 
    {
        [Foldout("Config Settings"), SerializeField] AudioMixer audioMixer;
        [Foldout("Config Settings"), SerializeField, Range(0f, 1f)] float _masterVolume = 1f;
        [Foldout("Config Settings"), SerializeField, Range(0f, 1f)] float _musicVolume = 1f;
        [Foldout("Config Settings"), SerializeField, Range(0f, 1f)] float _sfxVolume = 1f;

        [SerializeField] AudioClip defaultAmbience;
        AudioSource track01, track02;
        bool isPlayingTrack01;

        public static Action<float> OnChangeMasterVolume;
        public static Action<float> OnChangeMusicVolume;
        public static Action<float> OnChangeSFXVolume;

        public static AudioManager Instance;

        void Awake()
        {
            if (Instance == null)
                Instance = this;
        }

        void Start()
        {
            track01 = gameObject.AddComponent<AudioSource>();
            track02 = gameObject.AddComponent<AudioSource>();
            isPlayingTrack01 = true;

            SwapTrack(defaultAmbience);
        }

        void OnEnable()
        {
            OnChangeMasterVolume += ChangeMasterVolume;
            OnChangeMusicVolume += ChangeMusicVolume;
            OnChangeSFXVolume += ChangeSFXVolume;
        }

        void OnDisable()
        {
            OnChangeMasterVolume -= ChangeMasterVolume;
            OnChangeMusicVolume -= ChangeMusicVolume;
            OnChangeSFXVolume -= ChangeSFXVolume;
        }

        void OnValidate()
        {
            if (Application.isPlaying)
            {
                SetGroupVolume(GlobalTags.MasterVolume, _masterVolume);
                SetGroupVolume(GlobalTags.MusicVolume, _musicVolume);
                SetGroupVolume(GlobalTags.SFXVolume, _sfxVolume);
            }
        }

        #region Change Volumes
        void ChangeMasterVolume(float newVolume)
        {
            _masterVolume = newVolume;
            SetGroupVolume(GlobalTags.MasterVolume, _masterVolume);
        }

        void ChangeMusicVolume(float newVolume)
        {
            _musicVolume = newVolume;
            SetGroupVolume(GlobalTags.MusicVolume, _musicVolume);
        }

        void ChangeSFXVolume(float newVolume)
        {
            _sfxVolume = newVolume;
            SetGroupVolume(GlobalTags.SFXVolume, _sfxVolume);
        }

        void SetGroupVolume(string parameterName, float normalizedVolume)
        {
            if (!audioMixer.SetFloat(parameterName, NormalizedToMixerValue(normalizedVolume)))
                Debug.LogError("The AudioMixer parameter was not found");
        }

        float NormalizedToMixerValue(float normalizedValue) => (normalizedValue - 1f) * 80f;
        #endregion

        #region Fade Tracks
        public void ReturnToDefault() => SwapTrack(defaultAmbience);

        public void SwapTrack(AudioClip newClip)
        {
            StopAllCoroutines();
            StartCoroutine(FadeTrack(newClip));
            isPlayingTrack01 = !isPlayingTrack01;
        }

        IEnumerator FadeTrack(AudioClip newClip)
        {
            var timeToFade = 1.25f;
            var timeElapsed = 0f;

            if (isPlayingTrack01)
            {
                track02.clip = newClip;
                track02.Play();

                while (timeElapsed < timeToFade)
                {
                    track02.volume = Mathf.Lerp(0f, 1f, timeElapsed / timeToFade);
                    track01.volume = Mathf.Lerp(1f, 0f, timeElapsed / timeToFade);
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }
                
                track01.Stop();
            }
            else
            {
                track01.clip = newClip;
                track01.Play();

                while (timeElapsed < timeToFade)
                {
                    track01.volume = Mathf.Lerp(0f, 1f, timeElapsed / timeToFade);
                    track02.volume = Mathf.Lerp(1f, 0f, timeElapsed / timeToFade);
                    timeElapsed += Time.deltaTime;
                    yield return null;
                }

                track02.Stop();
            }
        }
        #endregion
    }
}