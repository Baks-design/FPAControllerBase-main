using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Baks
{
    public class InputManager : Singleton<InputManager>, GameInput.IGameplayActions, GameInput.IUIActions, GameInput.IDeveloperActions
    {
        GameInput m_Input;

        //Gameplay
        public static Vector2 MovementAxis { get; private set; }
        public static event Action OnCrouchPressed = delegate { };
        public static event Action OnCrouchReleased = delegate { };
        public static event Action OnJumpPressed = delegate { };
        public static event Action OnSprintPressed = delegate { };
        public static event Action OnSprintReleased = delegate { };
        public static Vector2 LookAxis { get; private set; }
        public static event Action OnZoomPressed = delegate { };
        public static event Action OnZoomReleased = delegate { };
        public static event Action OnInteractPressed = delegate { };
        public static event Action OnInteractReleased = delegate { };

        //Shared by Maps
        public static event Action OnPausePressed = delegate { };
        public static event Action OnPauseReleased = delegate { };

        //Developer
        public static event Action OnConsolePressed = delegate { };
        public static event Action OnConsoleCanceled = delegate { };

        void OnEnable()
        {
            m_Input = new GameInput();
            m_Input.Gameplay.SetCallbacks(this);
            m_Input.UI.SetCallbacks(this);
            m_Input.Developer.SetCallbacks(this);
#if UNITY_EDITOR
            m_Input.Developer.Enable();
#endif
        }

        void Start() => EnableGameplayMap(); // //TODO: Move to Spawn

        void OnDisable() => DisableAllInputs();

        #region Maps
        public void EnableGameplayMap()
        {
            m_Input.Gameplay.Enable();
            m_Input.UI.Disable();
        }

        public void EnableUIMap()
        {
            m_Input.Gameplay.Disable();
            m_Input.UI.Enable();
        }

        public void DisableAllInputs()
        {
            m_Input.Gameplay.Disable();
            m_Input.UI.Disable();
#if UNITY_EDITOR
            m_Input.Developer.Disable();
#endif
        }
        #endregion

        #region Gameplay
        public void OnPause(InputAction.CallbackContext context)
        {
            if (context.started)
                OnPausePressed();

            if (context.canceled)
                OnPauseReleased();
        }

        public void OnMovement(InputAction.CallbackContext context) => MovementAxis = context.ReadValue<Vector2>();

        public void OnCrouch(InputAction.CallbackContext context)
        {
            if (context.started)
                OnCrouchPressed();

            if (context.canceled)
                OnCrouchReleased();
        }

        public void OnJump(InputAction.CallbackContext context)
        {
            if (context.performed)
                OnJumpPressed();
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
            if (context.started)
                OnSprintPressed();

            if (context.canceled)
                OnSprintReleased();
        }

        public void OnLook(InputAction.CallbackContext context) => LookAxis = context.ReadValue<Vector2>();

        public void OnZoom(InputAction.CallbackContext context)
        {
            if (context.started)
                OnZoomPressed();

            if (context.canceled)
                OnZoomReleased();
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
            if (context.started)
                OnInteractPressed();

            if (context.canceled)
                OnInteractReleased();
        }
        #endregion

        #region UI
        public void OnNavigate(InputAction.CallbackContext context) { }
        public void OnSubmit(InputAction.CallbackContext context) { }
        public void OnCancel(InputAction.CallbackContext context) { }
        public void OnPoint(InputAction.CallbackContext context) { }
        public void OnClick(InputAction.CallbackContext context) { }
        public void OnScrollWheel(InputAction.CallbackContext context) { }
        public void OnMiddleClick(InputAction.CallbackContext context) { }
        public void OnRightClick(InputAction.CallbackContext context) { }
        public void OnTrackedDevicePosition(InputAction.CallbackContext context) { }
        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) { }
        #endregion

        #region Developer
        public void OnToggleConsole(InputAction.CallbackContext context)
        {
            if (context.started)
                OnConsolePressed();

            if (context.canceled)
                OnConsoleCanceled();
        }
        #endregion
    }
}
