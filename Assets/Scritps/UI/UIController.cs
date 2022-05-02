using System;
using System.Collections;
using Baks.Runtime.Utils;
using TMPro;
using UnityEngine;

namespace Baks
{
    [DisallowMultipleComponent]
    public class UIController : MonoBehaviour
    {
        [SerializeField] GameObject menu;
        [SerializeField] TextMeshProUGUI healthText;
        [SerializeField] TextMeshProUGUI staminaText;
        [SerializeField] TextMeshProUGUI subtitleText;

        bool isPaused = false, 
            m_InteractionPauseInput = false;

        public static Action<string, float> OnSetSubtitles;
        public static Action<bool> OnCursorState;

        void Awake() => ClearSubtitle();

        void OnEnable()
        {
            Damageable.OnDamage += UpdateHealth;
            Damageable.OnHeal += UpdateHealth;
            FPMovementController.OnStaminaChange += UpdateStamina;
            OnSetSubtitles += SetSubtitle;
            InputManager.OnPausePressed += OnPausePressed;
            InputManager.OnPauseReleased += OnPauseReleased;
        }

        void OnDisable()
        {
            Damageable.OnDamage -= UpdateHealth;
            Damageable.OnHeal -= UpdateHealth;
            FPMovementController.OnStaminaChange -= UpdateStamina;
            OnSetSubtitles -= SetSubtitle;
            InputManager.OnPausePressed -= OnPausePressed;
            InputManager.OnPauseReleased -= OnPauseReleased;
        }

        void Start()
        {
            UpdateHealth(100f);
            UpdateStamina(100f);
            menu.gameObject.SetActive(false);
            ActiveDeactiveGO(true);
        }

        void Update()
        {
            if (m_InteractionPauseInput)
            {
                isPaused = !isPaused;
                if (isPaused)
                {
                    Time.timeScale = 0f;
                    menu.gameObject.SetActive(true);
                    OnApplicationFocus(false);
                    ActiveDeactiveGO(false);
                }
                else
                {
                    Time.timeScale = 1f;
                    menu.gameObject.SetActive(false);
                    OnApplicationFocus(true);
                    ActiveDeactiveGO(true);
                }
            }
        }

        #region Inputs
        void OnPausePressed() => m_InteractionPauseInput = true;

        void OnPauseReleased() => m_InteractionPauseInput = false;
        #endregion

        #region Cursor
        void OnApplicationFocus(bool hasFocus) => Cursor.lockState = hasFocus ? CursorLockMode.Locked : CursorLockMode.None;
        #endregion

        #region Health/Stamina
        public void CloseMenu()
        {
            OnApplicationFocus(true);
            ActiveDeactiveGO(true);
            menu.gameObject.SetActive(false);
            Time.timeScale = 1f;
        }

        void ActiveDeactiveGO(bool state)
        {
            healthText.gameObject.SetActive(state);
            staminaText.gameObject.SetActive(state);
        }

        void UpdateHealth(float currentHealth) => healthText.text = currentHealth.ToString("00");

        void UpdateStamina(float currentStamina) => staminaText.text = currentStamina.ToString("00");
        #endregion

        #region Subtitles
        public void SetSubtitle(string subtitle, float delay)
        {
            subtitleText.text = subtitle;
            StartCoroutine(ClearAfterSeconds(delay));
        }

        IEnumerator ClearAfterSeconds(float delay)
        {
            yield return Helpers.GetWait(delay);
            ClearSubtitle();
        }

        void ClearSubtitle() => subtitleText.text = "";
        #endregion
    }
}