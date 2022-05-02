using UnityEngine;

namespace Baks
{
    [DisallowMultipleComponent]
    public class InteractionController : PlayerComponent
    {
        [SerializeField] LayerMask interactionLayer = 8;
        [SerializeField] Vector3 interactionRayPoint = new Vector3(.5f, .5f, 0f);
        [SerializeField] float interactionDistance = 3f;
        [SerializeField] float sphereRadius = .3f;

        bool m_InteractInput, m_IsPicked;
        Interactable currentInteractable;
        Camera m_Camera;
        RaycastHit[] results = new RaycastHit[5];

        void OnEnable()
        {
            InputManager.OnInteractPressed += OnInteractPressed;
            InputManager.OnInteractReleased += OnInteractReleased;
        }

        void OnDisable()
        {
            InputManager.OnInteractPressed -= OnInteractPressed;
            InputManager.OnInteractReleased -= OnInteractReleased;
        }

        void Start()
        {
            InitComps();
            InitVars();
        }

        void Update()
        {
            HandleInteractionCheck();
            HandleInteractionInput();
        }

        void InitComps() => m_Camera = GetComponentInChildren<Camera>();

        void InitVars()
        {
            m_IsPicked = false;
            m_InteractInput = false;
            currentInteractable = null;
        }

        void OnInteractPressed() => m_InteractInput = true;

        void OnInteractReleased() => m_InteractInput = false;

        void HandleInteractionCheck()
        {
            for (var i = 0; i < CheckInteraction().isCollisions; i++)
            {
                if (results[i].collider.gameObject.layer == 8 && (ReferenceEquals(currentInteractable, null) ||
                    results[i].collider.gameObject.GetInstanceID() != currentInteractable.GetInstanceID()))
                {
                    results[i].collider.TryGetComponent(out currentInteractable);
                    if (currentInteractable)
                        currentInteractable.OnFocus();
                }
                else if (currentInteractable)
                {
                    currentInteractable.OnLoseFocus();
                    currentInteractable = null;
                }
            }
        }

        void HandleInteractionInput()
        {
            if (m_InteractInput && !ReferenceEquals(currentInteractable, null))
            {
                for (var i = 0; i < CheckInteraction().isCollisions; i++)
                {
                    if (!m_IsPicked)
                    {
                        currentInteractable.OnInteract();
                        m_IsPicked = true;
                    }
                    else
                    {
                        currentInteractable.OnLoseFocus();
                        m_IsPicked = false;
                    }
                }
            }
        }

        (int isCollisions, RaycastHit[] results) CheckInteraction()
        {
            var origin = m_Camera.transform.position;
            var direction = m_Camera.ViewportPointToRay(interactionRayPoint).direction;
            var isCollisions = Physics.SphereCastNonAlloc(origin, sphereRadius, direction, results, interactionDistance, interactionLayer);
            return (isCollisions, results);
        }
    }
}