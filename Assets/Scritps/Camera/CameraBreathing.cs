using UnityEngine;

namespace Baks
{
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public class CameraBreathing : MonoBehaviour
    {
        [Header("Data")]
        [SerializeField] PerlinNoiseData data;

        [Header("Axis")]
        [SerializeField] bool x = true;
        [SerializeField] bool y = true;
        [SerializeField] bool z = false;

        Vector3 m_finalRot, m_finalPos;
        Transform m_Transform;
        PerlinNoiseScroller m_perlinNoiseScroller;

        void Start()
        {
            m_perlinNoiseScroller = new PerlinNoiseScroller(data);
            m_Transform = transform;
        }

        void LateUpdate()
        {
            if (!ReferenceEquals(data, null))
            {
                m_perlinNoiseScroller.UpdateNoise();

                var _posOffset = Vector3.zero;
                var _rotOffset = Vector3.zero;

                switch (data.transformTarget)
                {
                    case TransformTarget.Position:
                        {
                            if (x) _posOffset.x += m_perlinNoiseScroller.Noise.x;
                            if (y) _posOffset.y += m_perlinNoiseScroller.Noise.y;
                            if (z) _posOffset.z += m_perlinNoiseScroller.Noise.z;

                            m_finalPos.x = x ? _posOffset.x : m_Transform.localPosition.x;
                            m_finalPos.y = y ? _posOffset.y : m_Transform.localPosition.y;
                            m_finalPos.z = z ? _posOffset.z : m_Transform.localPosition.z;

                            m_Transform.localPosition = m_finalPos;
                            break;
                        }
                    case TransformTarget.Rotation:
                        {
                            if (x) _rotOffset.x += m_perlinNoiseScroller.Noise.x;
                            if (y) _rotOffset.y += m_perlinNoiseScroller.Noise.y;
                            if (z) _rotOffset.z += m_perlinNoiseScroller.Noise.z;

                            m_finalRot.x = x ? _rotOffset.x : m_Transform.localEulerAngles.x;
                            m_finalRot.y = y ? _rotOffset.y : m_Transform.localEulerAngles.y;
                            m_finalRot.z = z ? _rotOffset.z : m_Transform.localEulerAngles.z;

                            m_Transform.localEulerAngles = m_finalRot;

                            break;
                        }
                    case TransformTarget.Both:
                        {
                            if (x)
                            {
                                _posOffset.x += m_perlinNoiseScroller.Noise.x;
                                _rotOffset.x += m_perlinNoiseScroller.Noise.x;
                            }
                            if (y)
                            {
                                _posOffset.y += m_perlinNoiseScroller.Noise.y;
                                _rotOffset.y += m_perlinNoiseScroller.Noise.y;
                            }
                            if (z)
                            {
                                _posOffset.z += m_perlinNoiseScroller.Noise.z;
                                _rotOffset.z += m_perlinNoiseScroller.Noise.z;
                            }

                            m_finalPos.x = x ? _posOffset.x : m_Transform.localPosition.x;
                            m_finalPos.y = y ? _posOffset.y : m_Transform.localPosition.y;
                            m_finalPos.z = z ? _posOffset.z : m_Transform.localPosition.z;

                            m_finalRot.x = x ? _rotOffset.x : m_Transform.localEulerAngles.x;
                            m_finalRot.y = y ? _rotOffset.y : m_Transform.localEulerAngles.y;
                            m_finalRot.z = z ? _rotOffset.z : m_Transform.localEulerAngles.z;

                            m_Transform.localPosition = m_finalPos;
                            m_Transform.localEulerAngles = m_finalRot;

                            break;
                        }
                }
            }
        }
    }
}
