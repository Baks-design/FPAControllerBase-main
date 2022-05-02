using UnityEngine;

namespace Baks
{
    public class Player : MonoBehaviour
    {
        PlayerComponent[] m_playerComponents;
        
        void Awake() => InitPlayerComponents();

        public void InitPlayerComponents()
        {
            m_playerComponents = GetComponentsInChildren<PlayerComponent>();
            foreach(PlayerComponent _baseComp in m_playerComponents)
                _baseComp.InitPlayerReference(this);
        }

        public T FetchComponent<T>() where T : PlayerComponent
        {
            T _temp = default(T);
            
            foreach (PlayerComponent playerComponent in m_playerComponents)
                if (playerComponent is T)
                    _temp = playerComponent as T;

            return _temp;
        }
    }
}
