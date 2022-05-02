using NaughtyAttributes;
using UnityEngine;

namespace Baks
{
    [CreateAssetMenu(menuName = "Subtitle/DialogueSO")]
    public class DialogueSO : ScriptableObject
    {
        [SerializeField] AudioClip m_Clip;
        [SerializeField, ResizableTextArea] string m_Subtitle;

        public AudioClip Clip => m_Clip;
        public string Subtitle => m_Subtitle;
    }
}