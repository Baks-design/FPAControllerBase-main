using UnityEngine;

namespace Baks
{
    [CreateAssetMenu(menuName = "EntitySfx/FootstepsSO Config")]
    public class FootstepsSO : ScriptableObject
    {
        [SerializeField] AudioClip[] m_Clips;

        public AudioClip[] Clips => m_Clips;
    }
}