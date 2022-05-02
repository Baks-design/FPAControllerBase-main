using UnityEngine;

namespace Baks
{
    [CreateAssetMenu(menuName = "EntityConfig/Get Hit Effect Config")]
    public class GetHitEffectConfigSO : ScriptableObject
    {
        [SerializeField] Color m_GetHitFlashingColor;
        [SerializeField, Range(.1f, 1f)] float m_GetHitFlashingDuration = .5f;
        [SerializeField, Range(1f, 10f)] float m_GetHitFlashingSpeed = 3f;

        public Color GetHitFlashingColor => m_GetHitFlashingColor;
        public float GetHitFlashingDuration => m_GetHitFlashingDuration;
        public float GetHitFlashingSpeed => m_GetHitFlashingSpeed;
    }
}