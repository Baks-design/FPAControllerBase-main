using Baks.Runtime.Utils;
using UnityEngine;

namespace Baks
{
    [RequireComponent(typeof(BoxCollider))]
    [DisallowMultipleComponent]
    public class AudioSwap : MonoBehaviour
    {
        [SerializeField] AudioClip newTrack;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(GlobalTags.PlayerTag))
                AudioManager.Instance.SwapTrack(newTrack);
        }

        void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(GlobalTags.PlayerTag))
                AudioManager.Instance.ReturnToDefault();
        }
    }
}