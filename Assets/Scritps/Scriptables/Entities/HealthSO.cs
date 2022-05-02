using NaughtyAttributes;
using UnityEngine;

namespace Baks
{
    [CreateAssetMenu(menuName = "EntityConfig/Player's Health")]
    public class HealthSO : ScriptableObject
    {
        [Tooltip("The initial health")]
        [SerializeField, ReadOnly] float m_CurrentHealth;
        [SerializeField, Range(1, 100)] float m_MaxHealth = 100;
        [SerializeField, Range(1, 10)] float m_HealthValueIncrement = 1;
        [SerializeField, Range(1, 10)] float m_TimeBeforeRegenStarts = 3;
        [SerializeField, Range(.1f, 1)] float m_HealthTimeIncrement = .1f;

        public float MaxHealth => m_MaxHealth;
        public float CurrentHealth
        {
            get => m_CurrentHealth;
            set => m_CurrentHealth = value;
        }
        public float HealthValueIncrement => m_HealthValueIncrement;
        public float TimeBeforeRegenStarts => m_TimeBeforeRegenStarts;
        public float HealthTimeIncrement => m_HealthTimeIncrement;

        public void SetMaxHealth(float newValue) => m_MaxHealth = newValue;

        public void SetCurrentHealth(float newValue) => m_CurrentHealth = newValue;

        public void InflictDamage(float DamageValue) => m_CurrentHealth -= DamageValue;
    }
}