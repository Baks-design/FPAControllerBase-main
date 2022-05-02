using System.Collections;
using Baks.Runtime.Utils;
using NaughtyAttributes;
using UnityEngine;
using TMPro;

namespace Baks
{
    [DisallowMultipleComponent]
    public class Door : Interactable
    {
        [Foldout("Type Configs"), SerializeField] bool IsRotatingDoor = true;
        [Foldout("Type Configs"), SerializeField] float Speed = 1f;

        [Foldout("Rotation Configs"), SerializeField] float RotationAmount = 90f;
        [Foldout("Rotation Configs"), SerializeField] float ForwardDirection = 0f;

        [Foldout("Sliding Configs"), SerializeField] Vector3 SlideDirection = Vector3.back;
        [Foldout("Sliding Configs"), SerializeField] float SlideAmount = 2f;

        Vector3 StartRotation, StartPosition, Forward, UserPosition;
        Transform m_Transform;
        Coroutine AnimationCoroutine;

        public bool IsOpen { get; private set; } = false;

        public override void Awake() => InitVars();

        public override void OnFocus() => print("Door Looking");

        public override void OnInteract() => OpenDoor(UserPosition);

        public override void OnLoseFocus() => CloseDoor();

        void InitVars()
        {
            StartRotation = transform.rotation.eulerAngles;
            Forward = transform.right;
            StartPosition = transform.position;
            m_Transform = transform;
            IsOpen = false;
            UserPosition = GameObject.FindGameObjectWithTag(GlobalTags.PlayerTag).transform.position;
        }

        public void OpenDoor(Vector3 UserPosition)
        {
            if (!IsOpen)
            {
                if (!ReferenceEquals(AnimationCoroutine, null))
                    StopCoroutine(AnimationCoroutine);

                if (IsRotatingDoor)
                {
                    var dot = Vector3.Dot(Forward, (UserPosition - m_Transform.position).normalized);
                    AnimationCoroutine = StartCoroutine(DoRotationOpen(dot));
                }
                else
                    AnimationCoroutine = StartCoroutine(DoSlidingOpen());
            }

            IEnumerator DoRotationOpen(float ForwardAmount)
            {
                IsOpen = true;

                var dir1 = Quaternion.Euler(new Vector3(0f, StartRotation.y + RotationAmount, 0f));
                var dir2 = Quaternion.Euler(new Vector3(0f, StartRotation.y - RotationAmount, 0f));

                var endRotation = ForwardAmount >= ForwardDirection ? dir1 : dir2;

                var time = 0f;
                while (time < 1f)
                {
                    m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, endRotation, time);
                    yield return null;
                    time += Time.deltaTime * Speed;
                }
            }

            IEnumerator DoSlidingOpen()
            {
                IsOpen = true;

                var startPosition = m_Transform.position;
                var endPosition = StartPosition + SlideAmount * SlideDirection;

                var time = 0f;
                while (time < 1f)
                {
                    m_Transform.position = Vector3.Lerp(startPosition, endPosition, time);
                    yield return null;
                    time += Time.deltaTime * Speed;
                }
            }
        }

        public void CloseDoor()
        {
            if (IsOpen)
            {
                if (!ReferenceEquals(AnimationCoroutine, null))
                    StopCoroutine(AnimationCoroutine);

                AnimationCoroutine = IsRotatingDoor ? StartCoroutine(DoRotationClose()) : StartCoroutine(DoSlidingClose());
            }

            IEnumerator DoRotationClose()
            {
                IsOpen = false;

                var startRotation = m_Transform.rotation;
                var endRotation = Quaternion.Euler(StartRotation);

                var time = 0f;
                while (time < 1f)
                {
                    m_Transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
                    yield return null;
                    time += Time.deltaTime * Speed;
                }
            }

            IEnumerator DoSlidingClose()
            {
                IsOpen = false;

                var endPosition = StartPosition;
                var startPosition = m_Transform.position;

                var time = 0f;
                while (time < 1f)
                {
                    m_Transform.position = Vector3.Lerp(startPosition, endPosition, time);
                    yield return null;
                    time += Time.deltaTime * Speed;
                }
            }
        }
    }
}