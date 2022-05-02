using System;
using System.Collections;
using Baks.Runtime.Utils;
using NaughtyAttributes;
using UnityEngine;

namespace Baks
{
    [DisallowMultipleComponent]
    public class Damageable : MonoBehaviour
    {
        [Foldout("Health"), SerializeField, Expandable] HealthConfigSO m_HealthConfigSO;
        [Foldout("Health"), SerializeField, Expandable] HealthSO m_CurrentHealthSO;

        [Foldout("Combat"), SerializeField] bool m_EnableMeshRenderer;
        [Foldout("Combat"), SerializeField, EnableIf("m_EnableMeshRenderer")] Renderer m_MainMeshRenderer;
        [Foldout("Combat"), SerializeField, EnableIf("m_EnableMeshRenderer"), Expandable] GetHitEffectConfigSO m_GetHitEffectSO;

        Coroutine regenerationHealth;

        public GetHitEffectConfigSO GetHitEffectConfig => m_GetHitEffectSO;
        public Renderer MainMeshRenderer => m_MainMeshRenderer;
        public bool GetHit { get; private set; }
        public bool IsDead { get; private set; }

        public static Action<float> OnTakeDamage;
        public static Action<float> OnDamage;
        public static Action<float> OnHeal;

        void Awake()
        {
            if (ReferenceEquals(m_CurrentHealthSO, null))
            {
                m_CurrentHealthSO = ScriptableObject.CreateInstance<HealthSO>();
                m_CurrentHealthSO.SetMaxHealth(m_HealthConfigSO.InitialHealth);
                m_CurrentHealthSO.SetCurrentHealth(m_HealthConfigSO.InitialHealth);
            }
        }

        void OnEnable() => OnTakeDamage += ApplyDamage;

        void OnDisable() => OnTakeDamage -= ApplyDamage;

        void Start() => m_CurrentHealthSO.CurrentHealth = m_CurrentHealthSO.MaxHealth;

        void ApplyDamage(float dmg)
        {
            if (IsDead)
                return;

            m_CurrentHealthSO.CurrentHealth -= dmg;

            OnDamage?.Invoke(m_CurrentHealthSO.CurrentHealth);

            GetHit = true;

            if (m_CurrentHealthSO.CurrentHealth <= 0f)
                KillPlayer();
            else if (!ReferenceEquals(regenerationHealth, null))
                StopCoroutine(regenerationHealth);

            regenerationHealth = StartCoroutine(RegenerationHealth());
        }

        void KillPlayer()
        {
            m_CurrentHealthSO.CurrentHealth = 0f;

            IsDead = true;

            if (!ReferenceEquals(regenerationHealth, null))
                StopCoroutine(regenerationHealth);
        }
        
        IEnumerator RegenerationHealth()
        {
            yield return Helpers.GetWait(m_CurrentHealthSO.TimeBeforeRegenStarts);

            while (m_CurrentHealthSO.CurrentHealth < m_CurrentHealthSO.MaxHealth)
            {
                m_CurrentHealthSO.CurrentHealth += m_CurrentHealthSO.HealthValueIncrement;

                if (m_CurrentHealthSO.CurrentHealth > m_CurrentHealthSO.MaxHealth)
                    m_CurrentHealthSO.CurrentHealth = m_CurrentHealthSO.MaxHealth;

                OnHeal?.Invoke(m_CurrentHealthSO.CurrentHealth);

                yield return Helpers.GetWait(m_CurrentHealthSO.HealthTimeIncrement);
            }

            regenerationHealth = null;
        }
    }
}