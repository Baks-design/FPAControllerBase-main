using UnityEngine;

namespace Baks
{
    [CreateAssetMenu(menuName = "SO_Managers/SO_InputManager")]
    public class SO_Manager_Input : SO_Singleton<SO_Manager_Input>
    {
        [SerializeField] float m_doubleTapThreshold = .2f;

        public float DoubleTapThreshold => m_doubleTapThreshold;
    }
}

