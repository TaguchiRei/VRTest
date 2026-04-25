using Application;
using UnityEngine;
using UnityEngine.InputSystem;
using UsefulTools.AutoGenerate;

namespace Code.Scripts.Infrastructure.Player
{
    public class VrPlayerInfra : InitializableMonoBehaviour, IInjectable<IInputDispatcher, VrPlayerMovementService>
    {
        private IInputDispatcher _inputDispatcher;
        private VrPlayerMovementService _vrPlayerMovementService;

        private Vector2 _moveInputValue;
        private Vector3 _headPos;
        private Quaternion _headRot;
        private Vector3 _leftHandPos;
        private Quaternion _leftHandRot;
        private Vector3 _rightHandPos;
        private Quaternion _rightHandRot;

        public void Inject(IInputDispatcher inputDispatcher, VrPlayerMovementService vrPlayerMovementService)
        {
            _inputDispatcher = inputDispatcher;
            _vrPlayerMovementService = vrPlayerMovementService;
        }

        public override void Initialize()
        {
            base.Initialize();

            if (_inputDispatcher == null)
            {
                Debug.LogError("[PlayerController] IInputDispatcher is null");
                Initialized = false;
                return;
            }

            ChangeRegistration();
            DebugGUI.ObserveVariable("MoveInput", ObserveMoveInput);
        }

        private string ObserveMoveInput()
        {
            return _moveInputValue.ToString();
        }

        private void FixedUpdate()
        {
            if (Initialized)
            {
                // カメラの向き（頭の回転）から視線の向きを更新
                Vector3 forward = _headRot * Vector3.forward;
                Vector2 lookDir = new Vector2(forward.x, forward.z);
                _vrPlayerMovementService.UpdateLookDirection(lookDir);

                // 物理移動とトラッキング同期
                _vrPlayerMovementService.ApplyGravity();
                _vrPlayerMovementService.Move(_moveInputValue);
                _vrPlayerMovementService.HandleHeadAndBodyMovement(_headPos, _headRot);
                _vrPlayerMovementService.UpdateHandTransform(_leftHandPos, _rightHandPos, _leftHandRot, _rightHandRot);
            }
        }

        private void OnDestroy()
        {
            ChangeRegistration(false);
        }

        #region InputAction

        public void OnMove(InputAction.CallbackContext context) => _moveInputValue = context.ReadValue<Vector2>();

        public void OnHeadPosition(InputAction.CallbackContext context) => _headPos = context.ReadValue<Vector3>();
        public void OnHeadRotation(InputAction.CallbackContext context) => _headRot = context.ReadValue<Quaternion>();

        public void OnLeftHandPosition(InputAction.CallbackContext context) =>
            _leftHandPos = context.ReadValue<Vector3>();

        public void OnLeftHandRotation(InputAction.CallbackContext context) =>
            _leftHandRot = context.ReadValue<Quaternion>();

        public void OnRightHandPosition(InputAction.CallbackContext context) =>
            _rightHandPos = context.ReadValue<Vector3>();

        public void OnRightHandRotation(InputAction.CallbackContext context) =>
            _rightHandRot = context.ReadValue<Quaternion>();

        public void OnLook(InputAction.CallbackContext context)
        {
        }

        public void OnGripLeft(InputAction.CallbackContext context)
        {
        }

        public void OnGripRight(InputAction.CallbackContext context)
        {
        }

        public void OnTriggerLeft(InputAction.CallbackContext context)
        {
        }

        public void OnTriggerRight(InputAction.CallbackContext context)
        {
        }

        #endregion

        private void ChangeRegistration(bool register = true)
        {
            Registration registration = register ? Registration.Register : Registration.UnRegister;
            
            // VRTransform Actions
            _inputDispatcher.ChangeRegistrationAll(
                ActionMaps.VRTransform,
                VRTransformActions.HeadPosition,
                OnHeadPosition,
                registration);
            _inputDispatcher.ChangeRegistrationAll(
                ActionMaps.VRTransform,
                VRTransformActions.HeadRotation,
                OnHeadRotation,
                registration);
            _inputDispatcher.ChangeRegistrationAll(
                ActionMaps.VRTransform,
                VRTransformActions.LeftHandPosition,
                OnLeftHandPosition,
                registration);
            _inputDispatcher.ChangeRegistrationAll(
                ActionMaps.VRTransform,
                VRTransformActions.LeftHandRotation,
                OnLeftHandRotation,
                registration);
            _inputDispatcher.ChangeRegistrationAll(
                ActionMaps.VRTransform,
                VRTransformActions.RightHandPosition,
                OnRightHandPosition,
                registration);
            _inputDispatcher.ChangeRegistrationAll(
                ActionMaps.VRTransform,
                VRTransformActions.RightHandRotation,
                OnRightHandRotation,
                registration);

            // VRControllers Actions
            _inputDispatcher.ChangeRegistrationAll(
                ActionMaps.VRControllers,
                VRControllersActions.Move,
                OnMove,
                registration);

            _inputDispatcher.ChangeRegistrationAll(
                ActionMaps.VRControllers,
                VRControllersActions.Look,
                OnLook,
                registration);
            _inputDispatcher.ChangeRegistrationAll(
                ActionMaps.VRControllers,
                VRControllersActions.PushGripLeft,
                OnGripLeft,
                registration);
            _inputDispatcher.ChangeRegistrationAll(
                ActionMaps.VRControllers,
                VRControllersActions.PushGripRight,
                OnGripRight,
                registration);
            _inputDispatcher.ChangeRegistrationAll(
                ActionMaps.VRControllers,
                VRControllersActions.PushTriggerLeft,
                OnTriggerLeft,
                registration);
            _inputDispatcher.ChangeRegistrationAll(
                ActionMaps.VRControllers,
                VRControllersActions.PushTriggerRight,
                OnTriggerRight,
                registration);
        }
    }
}