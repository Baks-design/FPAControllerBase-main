using Baks.Runtime.Utils;
using NaughtyAttributes;
using UnityEngine;

namespace Baks
{
    [RequireComponent(typeof(AudioSource), typeof(AudioListener))]
    [DisallowMultipleComponent]
    public class FootstepsHandler : MonoBehaviour
    {
        [Foldout("Config"), SerializeField] float raySphereCheck = .5f;
        [Foldout("Config"), SerializeField] float rayFootstepsDistance = 3f;
        [Foldout("Config"), SerializeField] float baseStepSpeed = .5f;
        [Foldout("Config"), SerializeField] float crouchStepMultiplier = 1.5f;
        [Foldout("Config"), SerializeField] float sprintStepMultiplier = .6f;

        [Foldout("WoodClips"), SerializeField, Expandable] FootstepsSO m_WoodWalkClipsSO;
        [Foldout("WoodClips"), SerializeField, Expandable] FootstepsSO m_WoodRunClipsSO;
        [Foldout("WoodClips"), SerializeField, Expandable] JumpSfxSO m_WoodJumpClipsSO;
        [Foldout("WoodClips"), SerializeField, Expandable] LandSfxSO m_WoodLandClipsSO;

        float footStepTimer;
        RaycastHit[] results = new RaycastHit[5];
        FPMovementController m_movementController;
        AudioSource footStepAudioSource;
        Transform m_Transform;

        float GetCurrentOffset => m_movementController.IsCrouching ?
                                  baseStepSpeed * crouchStepMultiplier : m_movementController.IsRunning ?
                                  baseStepSpeed * sprintStepMultiplier : baseStepSpeed;

        void Start()
        {
            TryGetComponent(out footStepAudioSource);
            m_movementController = GetComponentInParent<FPMovementController>();
            m_Transform = transform;
            footStepTimer = 0f;
        }

        void Update()
        {
            HandleFootsteps();
            HandleJumpingSfx();
            HandleLandingSfx();
        }

        void HandleFootsteps()
        {
            if (m_movementController.IsWall)
                return;
            if (!m_movementController.IsGround)
                return;
            if (InputManager.MovementAxis == Vector2.zero)
                return;

            footStepTimer -= Time.deltaTime;
            if (footStepTimer <= 0f)
            {
                for (var i = 0; i < CheckFloor().isCollisions; i++)
                {
                    if (m_movementController.IsRunning)
                    {
                        switch (CheckFloor().results[i].collider.tag)
                        {
                            case GlobalTags.WoodFloorTag:
                                footStepAudioSource.PlayOneShot(m_WoodRunClipsSO.Clips[Random.Range(0, m_WoodRunClipsSO.Clips.Length - 1)]);
                                break;
                            default:
                                footStepAudioSource.PlayOneShot(m_WoodRunClipsSO.Clips[Random.Range(0, m_WoodRunClipsSO.Clips.Length - 1)]);
                                break;
                        }
                    }
                    else
                    {
                        switch (CheckFloor().results[i].collider.tag)
                        {
                            case GlobalTags.WoodFloorTag:
                                footStepAudioSource.PlayOneShot(m_WoodWalkClipsSO.Clips[Random.Range(0, m_WoodWalkClipsSO.Clips.Length - 1)]);
                                break;
                            default:
                                footStepAudioSource.PlayOneShot(m_WoodWalkClipsSO.Clips[Random.Range(0, m_WoodWalkClipsSO.Clips.Length - 1)]);
                                break;
                        }
                    }
                }
                footStepTimer = GetCurrentOffset;
            }
        }

        void HandleJumpingSfx()
        {
            if (!m_movementController.JumpSfx)
                return;

            for (var i = 0; i < CheckFloor().isCollisions; i++)
            {
                switch (CheckFloor().results[i].collider.tag)
                {
                    case GlobalTags.WoodFloorTag:
                        footStepAudioSource.PlayOneShot(m_WoodJumpClipsSO.Clips[Random.Range(0, m_WoodJumpClipsSO.Clips.Length - 1)]);
                        break;
                    default:
                        footStepAudioSource.PlayOneShot(m_WoodJumpClipsSO.Clips[Random.Range(0, m_WoodJumpClipsSO.Clips.Length - 1)]);
                        break;
                }
                m_movementController.JumpSfx = false;
            }
        }

        void HandleLandingSfx()
        {
            if (!m_movementController.LandSfx)
                return;

            for (var i = 0; i < CheckFloor().isCollisions; i++)
            {
                switch (CheckFloor().results[i].collider.tag)
                {
                    case GlobalTags.WoodFloorTag:
                        footStepAudioSource.PlayOneShot(m_WoodLandClipsSO.Clips[Random.Range(0, m_WoodLandClipsSO.Clips.Length - 1)]);
                        break;
                    default:
                        footStepAudioSource.PlayOneShot(m_WoodLandClipsSO.Clips[Random.Range(0, m_WoodLandClipsSO.Clips.Length - 1)]);
                        break;
                }
                m_movementController.LandSfx = false;
            }
        }

        (int isCollisions, RaycastHit[] results) CheckFloor()
        {
            var isCollisions = Physics.SphereCastNonAlloc(m_Transform.position, raySphereCheck, Vector3.down, results, rayFootstepsDistance);
            return (isCollisions, results);
        }
    }
}