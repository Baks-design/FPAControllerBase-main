using NaughtyAttributes;
using UnityEngine;

namespace Baks
{
    [RequireComponent(typeof(Camera))]
    [DisallowMultipleComponent]
    public class CameraImpulse : MonoBehaviour
    {
        [SerializeField] bool DeltaMovement = true;
        [SerializeField] float Duration = 1f;
        [SerializeField] float Speed = 10f;
        [SerializeField] Vector3 Amount = new Vector3(1f, 1f, 0f);
        [SerializeField, CurveRange(EColor.Indigo)] AnimationCurve Curve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

        bool destroyAfterPlay;
        float time, lastFoV, nextFoV;
        Vector3 lastPos, nextPos;
        Camera Camera;

        void Start()
        {
            TryGetComponent(out Camera);
            time = 0f;
        }

        void LateUpdate()
        {
            if (time > 0f)
            {
                time -= Time.deltaTime;
                if (time > 0f)
                {
                    nextPos = (Mathf.PerlinNoise(time * Speed, time * Speed * 2f) - .5f) * Amount.x * transform.right * Curve.Evaluate(1f - time / Duration) +
                              (Mathf.PerlinNoise(time * Speed * 2f, time * Speed) - .5f) * Amount.y * transform.up * Curve.Evaluate(1f - time / Duration);
                    nextFoV = (Mathf.PerlinNoise(time * Speed * 2f, time * Speed * 2f) - .5f) * Amount.z * Curve.Evaluate(1f - time / Duration);

                    Camera.fieldOfView += (nextFoV - lastFoV);
                    Camera.transform.Translate(DeltaMovement ? (nextPos - lastPos) : nextPos);

                    lastPos = nextPos;
                    lastFoV = nextFoV;
                }
                else
                {
                    ResetCam();

                    if (destroyAfterPlay)
                        Destroy(this);
                }
            }
        }

        public static void ShakeOnce(float duration = 1f, float speed = 10f, Vector3? amount = null, Camera camera = null, bool deltaMovement = true, AnimationCurve curve = null)
        {
            var instance = (!ReferenceEquals(camera, null) ? camera : Camera.main).gameObject.AddComponent<CameraImpulse>();
            instance.Duration = duration;
            instance.Speed = speed;

            if (!ReferenceEquals(amount, null))
                instance.Amount = (Vector3)amount;
            if (!ReferenceEquals(curve, null))
                instance.Curve = curve;
                
            instance.DeltaMovement = deltaMovement;
            instance.destroyAfterPlay = true;
            instance.Shake();
        }

        public void Shake()
        {
            ResetCam();
            time = Duration;
        }

        void ResetCam()
        {
            Camera.transform.Translate(DeltaMovement ? -lastPos : Vector3.zero);
            Camera.fieldOfView -= lastFoV;
            lastPos = nextPos = Vector3.zero;
            lastFoV = nextFoV = 0f;
        }
    }
}