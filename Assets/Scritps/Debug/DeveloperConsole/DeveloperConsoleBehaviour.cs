using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Baks
{
    [DisallowMultipleComponent]
    public class DeveloperConsoleBehaviour : MonoBehaviour
    {
        [SerializeField] string prefix = string.Empty;
        [SerializeField] ConsoleCommand[] commands = new ConsoleCommand[0];

        [Header("UI")]
        [SerializeField] GameObject uiCanvas;
        [SerializeField] TMP_InputField inputField;

        bool isPaused = false;
        static DeveloperConsoleBehaviour instance;
        DeveloperConsole developerConsole;

        DeveloperConsole DeveloperConsole
        {
            get
            {
                if (!ReferenceEquals(developerConsole, null)) return developerConsole;
                return developerConsole = new DeveloperConsole(prefix, commands);
            }
        }

        void Awake()
        {
            if (!ReferenceEquals(instance, null) && !ReferenceEquals(instance, this))
            {
                Destroy(gameObject);
                return;
            }

            instance = this;

            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            if (Keyboard.current.backquoteKey.wasPressedThisFrame)
            {
                isPaused = !isPaused;
                if (isPaused)
                {
                    Time.timeScale = 0f;
                    uiCanvas.SetActive(true);
                    inputField.ActivateInputField();
                }
                else
                {
                    Time.timeScale = 1f;
                    uiCanvas.SetActive(false);
                }
            }
        }

        public void ProcessCommand(string inputValue)
        {
            DeveloperConsole.ProcessCommand(inputValue);
            inputField.text = string.Empty;
        }
    }
}