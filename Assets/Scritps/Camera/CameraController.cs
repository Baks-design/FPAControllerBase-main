using NaughtyAttributes;
using UnityEngine;

namespace Baks
{
    [DisallowMultipleComponent]
    public class CameraController : MonoBehaviour
    {
        [Foldout("Custom Settings"), SerializeField] CameraZoom cameraZoom;
        [Foldout("Custom Settings"), SerializeField] CameraSwaying cameraSway;

        [Foldout("Look Settings"), SerializeField] Vector2 sensitivity = Vector2.zero;
        [Foldout("Look Settings"), SerializeField] Vector2 smoothAmount = Vector2.zero;
        [Foldout("Look Settings"), SerializeField, MinMaxSlider(-90f, 90f)] Vector2 lookAngleMinMax = Vector2.zero;

        float m_yaw, m_pitch, m_desiredYaw, m_desiredPitch;
        Transform m_pitchTranform, m_Transform;
        Camera m_cam;

        void OnEnable()
        {
            InputManager.OnZoomPressed += OnZoomPressed;
            InputManager.OnZoomReleased += OnZoomReleased;
        }

        void OnDisable()
        {
            InputManager.OnZoomPressed -= OnZoomPressed;
            InputManager.OnZoomReleased -= OnZoomReleased;
        }

        void Start()
        {
            GetComponents();
            InitValues();
            InitComponents();
        }

        void LateUpdate()
        {
            CalculateRotation();
            SmoothRotation();
            ApplyRotation();
        }

        void OnZoomPressed() => cameraZoom.ChangeFOV(this);

        void OnZoomReleased() => cameraZoom.ChangeFOV(this);

        void GetComponents()
        {
            m_Transform = transform;
            m_pitchTranform = transform.GetChild(0).transform;
            m_cam = GetComponentInChildren<Camera>();
        }

        void InitValues()
        {
            m_yaw = transform.eulerAngles.y;
            m_desiredYaw = m_yaw;
        }

        void InitComponents()
        {
            cameraZoom.Init(m_cam);
            cameraSway.Init(m_cam.transform);
        }

        void CalculateRotation()
        {
            m_desiredYaw += InputManager.LookAxis.x * sensitivity.x * Time.deltaTime;
            m_desiredPitch -= InputManager.LookAxis.y * sensitivity.y * Time.deltaTime;
            m_desiredPitch = Mathf.Clamp(m_desiredPitch, lookAngleMinMax.x, lookAngleMinMax.y);
        }

        void SmoothRotation()
        {
            m_yaw = Mathf.Lerp(m_yaw, m_desiredYaw, smoothAmount.x * Time.deltaTime);
            m_pitch = Mathf.Lerp(m_pitch, m_desiredPitch, smoothAmount.y * Time.deltaTime);
        }

        void ApplyRotation()
        {
            m_Transform.eulerAngles = new Vector3(0f, m_yaw, 0f);
            m_pitchTranform.localEulerAngles = new Vector3(m_pitch, 0f, 0f);
        }

        public void HandleSway(Vector3 _inputVector, float _rawXInput) => cameraSway.Sway(_inputVector, _rawXInput);

        public void ChangeRunFOV(bool _returning) => cameraZoom.ChangeRunFOV(_returning, this);
    }
}
