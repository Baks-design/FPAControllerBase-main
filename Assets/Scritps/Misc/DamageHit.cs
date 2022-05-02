using Baks.Runtime.Utils;
using UnityEngine;

namespace Baks
{
    [DisallowMultipleComponent]
    public class DamageHit : MonoBehaviour
    {
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(GlobalTags.PlayerTag))
                Damageable.OnTakeDamage(15);
        }
    }
}