using UnityEngine;

namespace Baks
{
    public abstract class SO_Singleton<T> : SO_Manager where T : SO_Manager
    {
        static T m_instance;

        public static T Instance
        {
            get
            {
                if (!m_instance)
                {
                    var m_results = Resources.FindObjectsOfTypeAll<T>();
                    if (m_results.Length > 0)
                        m_instance = m_results[0];
                }

                if (!m_instance)
                    m_instance = ScriptableObject.CreateInstance<T>();

                return m_instance;
            }
        }

        public override void OnGameStart()
        {
            if (ReferenceEquals(m_instance, null))
                m_instance = this as T;
            else
                Destroy(this);
        }

        public override void OnGameEnd() { }
    }

    public abstract class SO_Manager : ScriptableObject
    {
        public abstract void OnGameStart();
        public abstract void OnGameEnd();
    }
}
