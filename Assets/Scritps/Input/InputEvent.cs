using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

namespace Baks
{
    public class InputEvent
    {
        static List<InputEvent> m_InputEvents = new List<InputEvent>();

        public static void UpdateKeyStates()
        {
            foreach (var inputEvent in m_InputEvents)
                inputEvent.UpdateKeyState();
        }

        KeyControl m_keyControl;

        public InputEvent(KeyControl _keyControl)
        {
            m_keyControl = _keyControl;
            m_InputEvents.Add(this);
        }

        bool m_PressedLastFrame = false;
        float m_lastKeyPressedTimeStamp = 0f;

        public bool Pressed { get; private set; }
        public bool Held { get; private set; }
        public bool Released { get; private set; }
        public bool DoubleTapped { get; private set; }

        public event Action OnPressed = delegate { };
        public event Action OnHeld = delegate { };
        public event Action OnReleased = delegate { };
        public event Action OnDoubleTapped = delegate { };

        void UpdateKeyState()
        {
            var _pressedThisFrame = m_keyControl.isPressed;
            Pressed = !m_PressedLastFrame && _pressedThisFrame;
            Held = m_PressedLastFrame && _pressedThisFrame;
            Released = m_PressedLastFrame && !_pressedThisFrame;

            DoubleTapped = false;

            if (Pressed)
            {
                DoubleTapped = Time.time - m_lastKeyPressedTimeStamp < SO_Manager_Input.Instance.DoubleTapThreshold;
                m_lastKeyPressedTimeStamp = Time.time;
            }

            HandleKeyEventCallbacks();

            m_PressedLastFrame = _pressedThisFrame;
        }

        void HandleKeyEventCallbacks()
        {
            if (Pressed) OnPressed();
            if (Held) OnHeld();
            if (Released) OnReleased();
            if (DoubleTapped) OnDoubleTapped();
        }
    }
}
