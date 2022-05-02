using NaughtyAttributes;
using UnityEngine;

namespace Baks
{
    [CreateAssetMenu(menuName = "EntityConfig/Health Config")]
    public class HealthConfigSO : ScriptableObject
    {
        [InfoBox("Usado em caso de falha no HealthSO", EInfoBoxType.Warning)]
        [SerializeField, Range(1f, 100f)] float m_InitialHealth = 100f;

        public float InitialHealth => m_InitialHealth;
    }
}