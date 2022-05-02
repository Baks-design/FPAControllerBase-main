using System;
using UnityEngine;

namespace Baks
{
    [Serializable]
    public class CameraSwaying
    {
        [Header("Sway Settings")]
        [SerializeField] float swayAmount = 0f;
        [SerializeField] float swaySpeed = 0f;
        [SerializeField] float returnSpeed = 0f;
        [SerializeField] float changeDirectionMultiplier = 0f;
        [SerializeField] AnimationCurve swayCurve = new AnimationCurve();

        bool m_diffrentDirection;
        float _scrollSpeed, m_xAmountThisFrame, m_xAmountPreviousFrame;
        Transform m_camTransform;

        public void Init(Transform _cam) => m_camTransform = _cam;

        public void Sway(Vector3 _inputVector, float _rawXInput)
        {
            var _xAmount = _inputVector.x;

            m_xAmountThisFrame = _rawXInput;

            if (_rawXInput != 0f)
            {
                if (m_xAmountThisFrame != m_xAmountPreviousFrame && m_xAmountPreviousFrame != 0)
                    m_diffrentDirection = true;

                var _speedMultiplier = m_diffrentDirection ? changeDirectionMultiplier : 1;
                _scrollSpeed += (_xAmount * swaySpeed * Time.deltaTime * _speedMultiplier);
            }
            else
            {
                if (m_xAmountThisFrame == m_xAmountPreviousFrame)
                    m_diffrentDirection = false;

                _scrollSpeed = Mathf.Lerp(_scrollSpeed, 0, Time.deltaTime * returnSpeed);
            }

            _scrollSpeed = Mathf.Clamp(_scrollSpeed, -1, 1);

            float _swayFinalAmount;
            _swayFinalAmount = _scrollSpeed < 0 ? -swayCurve.Evaluate(_scrollSpeed) * -swayAmount : swayCurve.Evaluate(_scrollSpeed) * -swayAmount;

            Vector3 _swayVector;
            _swayVector.z = _swayFinalAmount;

            m_camTransform.localEulerAngles = new Vector3(m_camTransform.localEulerAngles.x, m_camTransform.localEulerAngles.y, _swayVector.z);

            m_xAmountPreviousFrame = m_xAmountThisFrame;
        }
    }
}