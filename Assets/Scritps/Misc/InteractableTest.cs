using UnityEngine;

namespace Baks
{
    [RequireComponent(typeof(Rigidbody))]
    [DisallowMultipleComponent]
    public class InteractableTest : Interactable
    {
        Collider m_collider;
        Rigidbody m_rigidbody;
        Transform m_Transform;
        Camera m_Camera;

        void Start()
        {
            TryGetComponent(out m_rigidbody);
            TryGetComponent(out m_collider);
            m_Camera = Camera.main;
            m_Transform = transform;
        }
        
        public override void OnFocus() {}

        public override void OnInteract()
        {
            m_collider.isTrigger = true;
            m_rigidbody.useGravity = false;
            m_Transform.SetParent(m_Camera.transform);
        }

        public override void OnLoseFocus()
        {
            m_collider.isTrigger = false;
            m_rigidbody.useGravity = true;
            m_Transform.SetParent(null);
        }
    }
}