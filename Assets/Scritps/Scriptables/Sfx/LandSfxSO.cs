using UnityEngine;

namespace Baks
{
    [CreateAssetMenu(menuName = "EntitySfx/JumpLandSO Config")]
    public class LandSfxSO : ScriptableObject
    {
        [SerializeField] AudioClip[] m_LandSounds;

        public AudioClip[] Clips => m_LandSounds;
    }
}