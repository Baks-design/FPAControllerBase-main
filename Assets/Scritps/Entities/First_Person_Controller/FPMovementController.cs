using System;
using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;
using Baks.Runtime.Utils;

namespace Baks
{
    [RequireComponent(typeof(CharacterController))]
    [DisallowMultipleComponent]
    public class FPMovementController : MonoBehaviour
    {
        [Foldout("Data Settings"), SerializeField, Expandable] HeadBobData headBobData;

        [Foldout("Locomotion Settings"), SerializeField] float crouchSpeed = 1f;
        [Foldout("Locomotion Settings"), SerializeField] float walkSpeed = 3f;
        [Foldout("Locomotion Settings"), SerializeField] float runSpeed = 6f;
        [Foldout("Locomotion Settings"), SerializeField] float jumpSpeed = 6f;
        [Foldout("Locomotion Settings"), SerializeField] float slideSpeed = 10f;
        [Foldout("Locomotion Settings"), SerializeField] float slopeSpeed = 1.5f;
        [Foldout("Locomotion Settings"), SerializeField, Range(0f, 1f)] float moveBackwardsSpeedPercent = .8f;
        [Foldout("Locomotion Settings"), SerializeField, Range(0f, 1f)] float moveSideSpeedPercent = .9f;

        [Foldout("Run Settings"), SerializeField, Range(-1f, 1f)] float canRunThreshold = -.1f;
        [Foldout("Run Settings"), SerializeField, CurveRange(EColor.Red)] AnimationCurve runTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Foldout("Crouch Settings"), SerializeField, Range(.2f, .9f)] float crouchPercent = .6f;
        [Foldout("Crouch Settings"), SerializeField] float crouchTransitionDuration = .5f;
        [Foldout("Crouch Settings"), SerializeField, CurveRange(EColor.Green)] AnimationCurve crouchTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Foldout("Slide Settings"), SerializeField, Range(.2f, .9f)] float slidePercent = .3f;
        [Foldout("Slide Settings"), SerializeField] float slideTransitionDuration = .3f;
        [Foldout("Slide Settings"), SerializeField] float maxSlideDuration = 1.25f;
        [Foldout("Slide Settings"), SerializeField, CurveRange(EColor.Blue)] AnimationCurve slideTransitionCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Foldout("Landing Settings"), SerializeField, Range(.05f, .5f)] float lowLandAmount = .1f;
        [Foldout("Landing Settings"), SerializeField, Range(.2f, .9f)] float highLandAmount = .4f;
        [Foldout("Landing Settings"), SerializeField] float landTimer = .5f;
        [Foldout("Landing Settings"), SerializeField] float landDuration = .5f;
        [Foldout("Landing Settings"), SerializeField, CurveRange(EColor.Yellow)] AnimationCurve landCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

        [Foldout("Gravity Settings"), SerializeField] float gravityMultiplier = 2.5f;
        [Foldout("Gravity Settings"), SerializeField] float stickToGroundForce = 1f;
        [Foldout("Gravity Settings"), SerializeField, Range(0f, 1f)] float rayLength = .1f;

        [Foldout("Falling Settings"), SerializeField] float m_fallingDamageThreshold = 3f;
        [Foldout("Falling Settings"), SerializeField] float m_fallingDamageMultiplier = 2.85f;

        [Foldout("Push Settings"), SerializeField] LayerMask pushLayers;
        [Foldout("Push Settings"), SerializeField] bool canPush = true;
        [Foldout("Push Settings"), SerializeField, Range(.5f, 5f)] float strength = 1f;

        [Foldout("Check Wall Settings"), SerializeField] LayerMask obstacleLayers = ~0;
        [Foldout("Check Wall Settings"), SerializeField, Range(0f, 1f)] float rayObstacleLength = .4f;
        [Foldout("Check Wall Settings"), SerializeField, Range(.01f, 1f)] float rayObstacleSphereRadius = .2f;

        [Foldout("Smooth Settings"), SerializeField, Range(1f, 100f)] float smoothRotateSpeed = 10f;
        [Foldout("Smooth Settings"), SerializeField, Range(1f, 100f)] float smoothInputSpeed = 10f;
        [Foldout("Smooth Settings"), SerializeField ,Range(1f, 100f)] float smoothVelocitySpeed = 3f;
        [Foldout("Smooth Settings"), SerializeField, Range(1f, 100f)] float smoothFinalDirectionSpeed = 10f;
        [Foldout("Smooth Settings"), SerializeField, Range(1f, 100f)] float smoothHeadBobSpeed = 5f;

        [Foldout("Stamina Settings"), SerializeField, Range(0f, 1000f)] float maxStamina = 100f;
        [Foldout("Stamina Settings"), SerializeField, Range(0f, 10f)] float staminaUseMultiplier = 5f;
        [Foldout("Stamina Settings"), SerializeField, Range(0f, 10f)] float timeBeforeStaminaRegenStarts = 5f;
        [Foldout("Stamina Settings"), SerializeField, Range(0f, 10f)] float staminaValueIncrement = 2f;
        [Foldout("Stamina Settings"), SerializeField, Range(0f, 1f)] float staminaTimeIncrement = .1f;

        bool m_isCrouching, m_isSliding, m_isRunning, m_previouslyGrounded, m_duringCrouchAnimation, m_duringRunAnimation, m_canRun, firstCall, damaged;
        float m_currentSpeed, m_smoothCurrentSpeed, m_finalSmoothCurrentSpeed, m_walkRunSpeedDifference, m_finalRayLength, m_slideStandHeightDifference;
        float m_initHeight, m_crouchHeight, m_slideHeight, m_inAirTimer, m_initCamHeight, m_crouchCamHeight, m_slideCamHeight, m_crouchStandHeightDifference;
        float fallDistance, currentStamina, startYPos, endYPos;
        Vector2 m_smoothInputVector;
        Vector3 m_finalMoveDir, m_smoothFinalMoveDir, m_finalMoveVector, m_initCenter, m_crouchCenter, m_slideCenter, hitNormal, hitPoint;
        RaycastHit m_hitInfo;
        IEnumerator m_LandRoutine;
        CollisionFlags m_CollisionFlags;
        Coroutine regeneratingStamina;
        Transform m_yawTransform, m_camTransform, m_Transform;
        CharacterController m_characterController;
        HeadBob m_headBob;
        CameraController m_cameraController;
        Damageable m_Health;
        FootstepsHandler m_footsteps;

        public bool IsCrouching => m_isCrouching;
        public bool IsRunning => m_isRunning;
        public bool IsWall => CheckIfWall();
        public bool IsGround => CheckIfGrounded();
        public bool JumpSfx { get; set; } = false;
        public bool LandSfx { get; set; } = false;

        public static Action<float> OnStaminaChange;

        void OnEnable()
        {
            InputManager.OnSprintPressed += OnSprintPressed;
            InputManager.OnSprintReleased += OnSprintReleased;
            InputManager.OnCrouchPressed += OnCrouchPressed;
            InputManager.OnCrouchReleased += OnCrouchReleased;
            InputManager.OnJumpPressed += OnJumpPressed;
        }

        void OnDisable()
        {
            InputManager.OnSprintPressed -= OnSprintPressed;
            InputManager.OnSprintReleased -= OnSprintReleased;
            InputManager.OnCrouchPressed -= OnCrouchPressed;
            InputManager.OnCrouchReleased -= OnCrouchReleased;
            InputManager.OnJumpPressed -= OnJumpPressed;
        }

        void Start()
        {
            GetComponents();
            InitVariables();
        }

        void Update()
        {
            RotateTowardsCamera();

            SmoothInput();
            SmoothSpeed();
            SmoothDir();

            CalculateMovementDirection();
            CalculateSpeed();
            CalculateFinalMovement();
            CalculateMoveOnSlope();

            HandleHeadBob();
            HandleRunFOV();
            HandleCameraSway();
            HandleLanding();

            ApplyGravity();
            ApplyMovement();
            HandleStamina();
            HandleFallDamage();

            m_previouslyGrounded = CheckIfGrounded();
        }

        #region Setup
        void OnSprintPressed()
        {
            if (!m_canRun)
                return;

            m_isRunning = true;
            ChangeToRunFOV();
        }
        void OnSprintReleased()
        {
            m_isRunning = false;
            ChangeToInitFOV();
        }
        void OnCrouchPressed() => HandleCrouchInput();
        void OnCrouchReleased() => ReturnToInitHeight();
        void OnJumpPressed() => HandleJump();

        void GetComponents()
        {
            m_Transform = transform;
            TryGetComponent(out m_characterController);
            TryGetComponent(out m_Health);
            m_footsteps = GetComponentInChildren<FootstepsHandler>();
            m_cameraController = GetComponentInChildren<CameraController>();
            m_yawTransform = m_cameraController.transform;
            m_camTransform = Camera.main.transform;
            m_headBob = new HeadBob(headBobData, moveBackwardsSpeedPercent, moveSideSpeedPercent);
        }

        void InitVariables()
        {
            m_characterController.center = new Vector3(0f, m_characterController.height / 2f + m_characterController.skinWidth, 0f);

            m_initCenter = m_characterController.center;
            m_initHeight = m_characterController.height;
            m_initCamHeight = m_yawTransform.localPosition.y;

            m_crouchHeight = m_initHeight * crouchPercent;
            m_crouchCenter = (m_crouchHeight / 2f + m_characterController.skinWidth) * Vector3.up;
            m_crouchStandHeightDifference = m_initHeight - m_crouchHeight;
            m_crouchCamHeight = m_initCamHeight - m_crouchStandHeightDifference;

            m_slideHeight = m_initHeight * slidePercent;
            m_slideCenter = (m_slideHeight / 2f + m_characterController.skinWidth) * Vector3.up;
            m_slideStandHeightDifference = m_initHeight - m_slideHeight;
            m_slideCamHeight = m_initCamHeight - m_slideStandHeightDifference;

            m_finalRayLength = rayLength + m_characterController.center.y;

            m_inAirTimer = 0f;
            m_headBob.CurrentStateHeight = m_initCamHeight;

            m_walkRunSpeedDifference = runSpeed - walkSpeed;

            currentStamina = maxStamina;

            firstCall = true;
            damaged = false;
            m_canRun = true;
            m_previouslyGrounded = false;
        }
        #endregion

        #region Smooth
        void SmoothInput() => m_smoothInputVector = Vector2.Lerp(m_smoothInputVector, InputManager.MovementAxis, Time.deltaTime * smoothInputSpeed);

        void SmoothSpeed()
        {
            m_smoothCurrentSpeed = Mathf.Lerp(m_smoothCurrentSpeed, m_currentSpeed, Time.deltaTime * smoothVelocitySpeed);
            if (m_isRunning && CanRun() && !m_isSliding)
            {
                var walkRunPercent = Mathf.InverseLerp(walkSpeed, runSpeed, m_smoothCurrentSpeed);
                m_finalSmoothCurrentSpeed = runTransitionCurve.Evaluate(walkRunPercent) * m_walkRunSpeedDifference + walkSpeed;
                return;
            }
            m_finalSmoothCurrentSpeed = m_smoothCurrentSpeed;
        }

        void SmoothDir() => m_smoothFinalMoveDir = Vector3.Lerp(m_smoothFinalMoveDir, m_finalMoveDir, Time.deltaTime * smoothFinalDirectionSpeed);
        #endregion

        #region Check
        bool CheckIfGrounded() => m_CollisionFlags == CollisionFlags.Below;

        bool CheckIfWall()
        {
            var _hitWall = false;
            if (InputManager.MovementAxis != Vector2.zero && m_finalMoveDir.sqrMagnitude > 0f)
                _hitWall = Physics.SphereCast(m_Transform.position + m_characterController.center, rayObstacleSphereRadius, m_finalMoveDir, out var hit, rayObstacleLength, obstacleLayers);
            return _hitWall ? true : false;
        }

        bool CheckIfRoof() => m_CollisionFlags == CollisionFlags.Above;
        #endregion

        #region Can
        bool CanRun()
        {
            var _normalizedDir = Vector3.zero;
            if (m_smoothFinalMoveDir != Vector3.zero)
                _normalizedDir = m_smoothFinalMoveDir.normalized;
            var _dot = Vector3.Dot(m_Transform.forward, _normalizedDir);
            return _dot >= canRunThreshold && !m_isCrouching ? true : false;
        }

        bool CanJump() => !m_isSliding && !m_isCrouching && CheckIfGrounded();

        bool CanSlid() => Vector3.Angle(Vector3.up, hitNormal) <= m_characterController.slopeLimit;
        #endregion

        #region Movements
        void HandleCrouchInput()
        {
            if (!CheckIfGrounded())
                return;

            if (m_isRunning && !m_isCrouching && InputManager.MovementAxis != Vector2.zero && CanRun())
                HandleSlide();
            else
                HandleCrouch();
        }

        void HandleJump()
        {
            if (!CanJump())
                return;

            m_finalMoveVector.y = jumpSpeed;
            m_previouslyGrounded = true;
            JumpSfx = true;
        }

        void HandleSlide()
        {
            m_isSliding = true;

            m_headBob.CurrentStateHeight = m_slideCamHeight;

            DOTween.To(() => m_characterController.height, x => m_characterController.height = x, m_slideHeight, slideTransitionDuration).SetEase(slideTransitionCurve);
            DOTween.To(() => m_characterController.center, x => m_characterController.center = x, m_slideCenter, slideTransitionDuration).SetEase(slideTransitionCurve);

            m_yawTransform.DOLocalMoveY(m_slideCamHeight, slideTransitionDuration).SetEase(slideTransitionCurve);

            this.CallWithDelay(ReturnToInitHeight, maxSlideDuration);
        }

        void ReturnToInitHeight()
        {
            if (CheckIfRoof())
            {
                DOTween.Kill(this);
                m_isSliding = false;
                HandleCrouch();
                return;
            }

            if (!m_isSliding)
                return;

            DOTween.Kill(this);

            m_isSliding = false;

            m_headBob.CurrentStateHeight = m_initCamHeight;

            DOTween.To(() => m_characterController.height, x => m_characterController.height = x, m_initHeight, slideTransitionDuration).SetEase(slideTransitionCurve);
            DOTween.To(() => m_characterController.center, x => m_characterController.center = x, m_initCenter, slideTransitionDuration).SetEase(slideTransitionCurve);

            m_yawTransform.DOLocalMoveY(m_initCamHeight, slideTransitionDuration).SetEase(slideTransitionCurve);
        }

        void HandleCrouch()
        {
            if (m_isCrouching)
                if (CheckIfRoof())
                    return;

            if (!ReferenceEquals(m_LandRoutine, null))
                StopCoroutine(m_LandRoutine);

            m_duringCrouchAnimation = true;

            var currentHeight = m_characterController.height;
            var currentCenter = m_characterController.center;

            var desiredHeight = m_isCrouching ? m_initHeight : m_crouchHeight;
            var desiredCenter = m_isCrouching ? m_initCenter : m_crouchCenter;

            var camPos = m_yawTransform.localPosition;
            var camCurrentHeight = camPos.y;
            var camDesiredHeight = m_isCrouching ? m_initCamHeight : m_crouchCamHeight;

            m_isCrouching = !m_isCrouching;
            m_headBob.CurrentStateHeight = m_isCrouching ? m_crouchCamHeight : m_initCamHeight;

            DOTween.To(() => m_characterController.height, x => m_characterController.height = x, desiredHeight, crouchTransitionDuration).SetEase(crouchTransitionCurve);
            DOTween.To(() => m_characterController.center, x => m_characterController.center = x, desiredCenter, crouchTransitionDuration).SetEase(crouchTransitionCurve);

            m_yawTransform.DOLocalMoveY(camDesiredHeight, crouchTransitionDuration).SetEase(crouchTransitionCurve).OnComplete(delegate { m_duringCrouchAnimation = false; });
        }

        void HandleLanding()
        {
            if (!m_previouslyGrounded && CheckIfGrounded())
                InvokeLandingRoutine();
        }

        void InvokeLandingRoutine()
        {
            if (!ReferenceEquals(m_LandRoutine, null))
                StopCoroutine(m_LandRoutine);

            m_LandRoutine = LandingRoutine();
            StartCoroutine(m_LandRoutine);
        }

        IEnumerator LandingRoutine()
        {
            var percent = 0f;
            var landAmount = 0f;

            var speed = 1f / landDuration;

            var localPos = m_yawTransform.localPosition;
            var initLandHeight = localPos.y;

            landAmount = m_inAirTimer > landTimer ? highLandAmount : lowLandAmount;

            while (percent < 1f)
            {
                percent += Time.deltaTime * speed;
                var desiredY = landCurve.Evaluate(percent) * landAmount;
                localPos.y = initLandHeight + desiredY;
                m_yawTransform.localPosition = localPos;
                yield return null;
            }
        }
        #endregion

        #region Camera
        void HandleHeadBob()
        {
            if (InputManager.MovementAxis != Vector2.zero && CheckIfGrounded() && !CheckIfWall())
            {
                if (!m_duringCrouchAnimation && !m_isSliding)
                {
                    m_headBob.ScrollHeadBob(m_isRunning && CanRun(), m_isCrouching, InputManager.MovementAxis);
                    m_yawTransform.localPosition = Vector3.Lerp(m_yawTransform.localPosition, (Vector3.up * m_headBob.CurrentStateHeight) + m_headBob.FinalOffset, Time.deltaTime * smoothHeadBobSpeed);
                }
            }
            else
            {
                if (!m_headBob.Resetted)
                    m_headBob.ResetHeadBob();

                if (!m_duringCrouchAnimation)
                    m_yawTransform.localPosition = Vector3.Lerp(m_yawTransform.localPosition, new Vector3(0f, m_headBob.CurrentStateHeight, 0f), Time.deltaTime * smoothHeadBobSpeed);
            }
        }

        void RotateTowardsCamera() => m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, m_yawTransform.rotation, Time.deltaTime * smoothRotateSpeed);

        void HandleCameraSway() => m_cameraController.HandleSway(m_smoothInputVector, InputManager.MovementAxis.x);

        void HandleRunFOV()
        {
            if (!m_duringRunAnimation && InputManager.MovementAxis != Vector2.zero && !CheckIfWall())
            {
                if (m_isRunning && CanRun())
                {
                    m_duringRunAnimation = true;
                    m_cameraController.ChangeRunFOV(false);
                }
            }

            if (m_duringRunAnimation)
            {
                if (InputManager.MovementAxis == Vector2.zero || !CanRun() || CheckIfWall())
                {
                    m_duringRunAnimation = false;
                    m_cameraController.ChangeRunFOV(true);
                }
            }
        }

        void ChangeToRunFOV()
        {
            if (!CanRun() || InputManager.MovementAxis == Vector2.zero)
                return;

            m_duringRunAnimation = true;
            m_cameraController.ChangeRunFOV(false);
        }

        void ChangeToInitFOV()
        {
            if (!m_duringRunAnimation)
                return;

            m_duringRunAnimation = false;
            m_cameraController.ChangeRunFOV(true);
        }
        #endregion

        #region Calculate
        void CalculateMovementDirection()
        {
            var vDir = m_Transform.forward * m_smoothInputVector.y;
            var hDir = m_Transform.right * m_smoothInputVector.x;
            var desiredDir = vDir + hDir;
            var flattenDir = FlattenVectorOnSlopes(desiredDir);
            m_finalMoveDir = flattenDir;
        }

        Vector3 FlattenVectorOnSlopes(Vector3 vectorToFlat)
        {
            if (CheckIfGrounded())
                vectorToFlat = Vector3.ProjectOnPlane(vectorToFlat, m_hitInfo.normal);
            return vectorToFlat;
        }

        void CalculateSpeed()
        {
            m_currentSpeed = m_isRunning && CanRun() ? runSpeed : walkSpeed;
            m_currentSpeed = m_isCrouching ? crouchSpeed : m_currentSpeed;
            m_currentSpeed = m_isSliding ? slideSpeed : m_currentSpeed;
            m_currentSpeed = InputManager.MovementAxis == Vector2.zero ? 0f : m_currentSpeed;
            m_currentSpeed = InputManager.MovementAxis.y == -1f ? m_currentSpeed * moveBackwardsSpeedPercent : m_currentSpeed;
            m_currentSpeed = InputManager.MovementAxis.x != 0f && InputManager.MovementAxis.y == 0f ? m_currentSpeed * moveSideSpeedPercent : m_currentSpeed;
        }

        void CalculateFinalMovement()
        {
            var finalVector = m_smoothFinalMoveDir * m_finalSmoothCurrentSpeed * 1f;

            m_finalMoveVector.x = finalVector.x;
            m_finalMoveVector.z = finalVector.z;

            if (CheckIfGrounded())
                m_finalMoveVector.y += finalVector.y;
        }

        void CalculateMoveOnSlope()
        {
            if (!CanSlid())
            {
                m_finalMoveVector.x += ((1f - hitNormal.y) * hitNormal.x) * slopeSpeed;
                m_finalMoveVector.z += ((1f - hitNormal.y) * hitNormal.z) * slopeSpeed;
            }
        }

        void ApplyGravity()
        {
            if (CheckIfGrounded())
            {
                m_inAirTimer = 0f;
                m_finalMoveVector.y = Mathf.Clamp(m_finalMoveVector.y -= stickToGroundForce * Time.deltaTime, -stickToGroundForce, jumpSpeed);
            }
            else
            {
                m_inAirTimer += Time.deltaTime;
                m_finalMoveVector += Physics.gravity * gravityMultiplier * Time.deltaTime;
            }
        }

        void ApplyMovement() => m_CollisionFlags = m_characterController.Move(m_finalMoveVector * Time.deltaTime);
        #endregion  

        #region Stamina
        void HandleFallDamage()
        {
            if (!CheckIfGrounded())
            {
                if (transform.position.y > startYPos)
                    firstCall = true;

                if (firstCall)
                {
                    firstCall = false;
                    damaged = true;
                    startYPos = transform.position.y;
                }
            }
            else
            {
                endYPos = transform.position.y;
                if (damaged && (startYPos - endYPos) > m_fallingDamageThreshold)
                {
                    var amount = startYPos - endYPos - m_fallingDamageThreshold;
                    var damage = (m_fallingDamageMultiplier == 0f) ? amount : amount * m_fallingDamageMultiplier;
                    FallingDamageAlert(damage);

                    damaged = false;
                    firstCall = true;
                }
            }
        }

        void FallingDamageAlert(float value)
        {
            var valueClamped = Mathf.Clamp(value, 0f, 100f);
            Damageable.OnTakeDamage(valueClamped);
            LandSfx = true;
        }

        void HandleStamina()
        {
            if (m_isRunning && InputManager.MovementAxis != Vector2.zero)
            {
                if (!ReferenceEquals(regeneratingStamina, null))
                {
                    StopCoroutine(regeneratingStamina);
                    regeneratingStamina = null;
                }

                currentStamina -= staminaUseMultiplier * Time.deltaTime;

                if (currentStamina < 0f)
                    currentStamina = 0f;

                OnStaminaChange?.Invoke(currentStamina);

                if (currentStamina <= 0f)
                    m_canRun = false;
            }

            if (!m_isRunning && currentStamina < maxStamina && ReferenceEquals(regeneratingStamina, null))
                regeneratingStamina = StartCoroutine(RegenerateStamina());
        }

        IEnumerator RegenerateStamina()
        {
            yield return Helpers.GetWait(timeBeforeStaminaRegenStarts);

            while (currentStamina < maxStamina)
            {
                if (currentStamina > 0f)
                    m_canRun = true;

                currentStamina += staminaValueIncrement;

                if (currentStamina > maxStamina)
                    currentStamina = maxStamina;

                OnStaminaChange?.Invoke(currentStamina);

                yield return Helpers.GetWait(staminaTimeIncrement);
            }

            regeneratingStamina = null;
        }
        #endregion

        #region Collisions
        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            hitNormal = hit.normal;
            hitPoint = hit.point;

            if (canPush)
                PushRigidBodies(hit);
        }

        void PushRigidBodies(ControllerColliderHit hit)
        {
            var body = hit.collider.attachedRigidbody;
            if (ReferenceEquals(body, null) || body.isKinematic)
                return;

            var bodyLayerMask = 1 << body.gameObject.layer;
            if ((bodyLayerMask & pushLayers.value) == 0)
                return;

            if (hit.moveDirection.y < -.3f)
                return;

            var pushDir = new Vector3(hit.moveDirection.x, 0f, hit.moveDirection.z);
            body.AddForce(pushDir * strength, ForceMode.Impulse);
        }
        #endregion
    }
}