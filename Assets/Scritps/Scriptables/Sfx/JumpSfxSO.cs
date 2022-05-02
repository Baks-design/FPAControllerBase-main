using UnityEngine;

namespace Baks
{
    [CreateAssetMenu(menuName = "EntitySfx/JumpSfxSO Config")]
    public class JumpSfxSO : ScriptableObject
    {
        [SerializeField] AudioClip[] m_jumpSounds;

        public AudioClip[] Clips => m_jumpSounds;
    }
}