using Baks.Runtime.Utils;
using UnityEngine;

namespace Baks
{
    [DisallowMultipleComponent]
    public class TriggerAudio : MonoBehaviour
    {
        [SerializeField] DialogueSO clipToPlay;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(GlobalTags.PlayerTag))
                Vocals.OnSay(clipToPlay);
        }
    }
}