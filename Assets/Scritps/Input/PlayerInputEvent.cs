using UnityEngine.InputSystem;

namespace Baks
{
    public class PlayerInputEvent : Singleton<PlayerInputEvent>
    {
        public static InputEvent MeleeInput { get; private set; }
        public static InputEvent ThrowInput { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            InitInputEvents();
        }

        void OnEnable() => UpdateManager.OnUpdate += OnUpdate;

        void OnDisable() => UpdateManager.OnUpdate -= OnUpdate;

        void OnUpdate(float dt) => InputEvent.UpdateKeyStates();

        void InitInputEvents()
        {
            var currentKeyboard = Keyboard.current;
            MeleeInput = new InputEvent(currentKeyboard.vKey);
            ThrowInput = new InputEvent(currentKeyboard.fKey);
        }
    }
}
