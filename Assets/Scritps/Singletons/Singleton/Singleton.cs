using UnityEngine;

namespace Baks
{
    public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        static T m_instance;

        public static T Instance
        {
            get
            {
                if (ReferenceEquals(m_instance, null))
                    m_instance = FindObjectOfType<T>();

                if (ReferenceEquals(m_instance, null))
                {
                    var _instance = new GameObject(typeof(T).Name);
                    m_instance = _instance.AddComponent<T>();
                }
                
                return m_instance;
            }
        }

        protected virtual void Awake()
        {
            if (ReferenceEquals(m_instance, null))
                m_instance = this as T;
            else
                Destroy(this.gameObject);
        }
    }
}
