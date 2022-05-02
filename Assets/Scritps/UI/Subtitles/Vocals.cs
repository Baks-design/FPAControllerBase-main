using System;
using UnityEngine;

namespace Baks
{
    [RequireComponent(typeof(AudioSource))]
    [DisallowMultipleComponent]
    public class Vocals : MonoBehaviour
    {
        AudioSource m_Source;

        public static Action<DialogueSO> OnSay;

        void OnEnable() => OnSay += Say;

        void OnDisable() => OnSay -= Say;

        void Start() => TryGetComponent(out m_Source);

        void Say(DialogueSO clip)
        {
            if (m_Source.isPlaying)
                m_Source.Stop();

            m_Source.PlayOneShot(clip.Clip);

            UIController.OnSetSubtitles(clip.Subtitle, clip.Clip.length); 
        }
    }
}