using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using System;

namespace Baks
{
    [Serializable]
    public class CameraZoom
    {
        [Header("Zoom Settings")]
        [SerializeField, Range(20f, 60f)] float zoomFOV = 20f;
        [SerializeField] float zoomTransitionDuration = 0f;
        [SerializeField, CurveRange(EColor.Violet)] AnimationCurve zoomCurve = new AnimationCurve();

        [Space, Header("Run Settings")]
        [SerializeField, Range(60f, 100f)] float runFOV = 60f;
        [SerializeField] float runTransitionDuration = 0f, runReturnTransitionDuration = 0f;
        [SerializeField, CurveRange(EColor.Orange)] AnimationCurve runCurve = new AnimationCurve();

        float m_initFOV;
        bool m_running, m_zooming;
        Camera m_cam;
        IEnumerator m_ChangeFOVRoutine, m_ChangeRunFOVRoutine;

        public void Init(Camera cam)
        {
            m_cam = cam;
            m_initFOV = m_cam.fieldOfView;
        }

        public void ChangeFOV(MonoBehaviour mono)
        {
            if (m_running)
            {
                m_zooming = !m_zooming;
                return;
            }

            if (!ReferenceEquals(m_ChangeRunFOVRoutine, null))
                mono.StopCoroutine(m_ChangeRunFOVRoutine);
                
            if (!ReferenceEquals(m_ChangeFOVRoutine, null))
                mono.StopCoroutine(m_ChangeFOVRoutine);

            m_ChangeFOVRoutine = ChangeFOVRoutine();
            mono.StartCoroutine(m_ChangeFOVRoutine);
        }

        IEnumerator ChangeFOVRoutine()
        {
            var _percent = 0f;
            var _smoothPercent = 0f;
            var _speed = 1f / zoomTransitionDuration;
            var _currentFOV = m_cam.fieldOfView;
            var _targetFOV = m_zooming ? m_initFOV : zoomFOV;

            m_zooming = !m_zooming;

            while (_percent < 1f)
            {
                _percent += Time.deltaTime * _speed;
                _smoothPercent = zoomCurve.Evaluate(_percent);
                m_cam.fieldOfView = Mathf.Lerp(_currentFOV, _targetFOV, _smoothPercent);
                yield return null;
            }
        }

        public void ChangeRunFOV(bool _returning, MonoBehaviour _mono)
        {
            if (!ReferenceEquals(m_ChangeFOVRoutine, null))
                _mono.StopCoroutine(m_ChangeFOVRoutine);

            if (!ReferenceEquals(m_ChangeRunFOVRoutine, null))
                _mono.StopCoroutine(m_ChangeRunFOVRoutine);

            m_ChangeRunFOVRoutine = ChangeRunFOVRoutine(_returning);
            _mono.StartCoroutine(m_ChangeRunFOVRoutine);
        }

        IEnumerator ChangeRunFOVRoutine(bool returning)
        {
            var percent = 0f;
            var smoothPercent = 0f;
            var duration = returning ? runReturnTransitionDuration : runTransitionDuration;
            var speed = 1f / duration;
            var currentFOV = m_cam.fieldOfView;
            var targetFOV = returning ? m_initFOV : runFOV;

            m_running = !returning;

            while (percent < 1f)
            {
                percent += Time.deltaTime * speed;
                smoothPercent = runCurve.Evaluate(percent);
                m_cam.fieldOfView = Mathf.Lerp(currentFOV, targetFOV, smoothPercent);
                yield return null;
            }
        }
    }
}