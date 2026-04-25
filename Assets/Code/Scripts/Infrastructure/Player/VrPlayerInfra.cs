using Application;
using UnityEngine;
using UnityEngine.InputSystem;
using UsefulTools.AutoGenerate;

namespace Code.Scripts.Infrastructure.Player
{
    public class VrPlayerInfra : InitializableMonoBehaviour, IInjectable<IInputDispatcher, VrPlayerMovementService>
    {
        [SerializeField] private Transform _cameraTransform;

        private IInputDispatcher _inputDispatcher;
        private VrPlayerMovementService _vrPlayerMovementService;

        private Vector2 _moveInputValue;

        public override void Initialize()
        {
            base.Initialize();

            if (_inputDispatcher == null)
            {
                Debug.LogError("[PlayerController] IInputDispatcher is null");
                Initialized = false;
                return;
            }

            if (_cameraTransform == null)
            {
                Debug.LogError("[PlayerController] CameraTransform is null");
                Initialized = false;
                return;
            }

            Registration();
            _inputDispatcher.EnableActionMap(ActionMaps.VRTransform);
            _inputDispatcher.EnableActionMap(ActionMaps.VRControllers);
            DebugGUI.ObserveVariable("MoveInput", () => _moveInputValue.ToString());
        }

        private void Update()
        {
            if (Initialized)
            {
                // カメラのXZ平面上の向きを抽出
                Vector3 forward = _cameraTransform.forward;
                Vector2 lookDir = new Vector2(forward.x, forward.z);

                // 視線の向きを更新
                _vrPlayerMovementService.UpdateLookDirection(lookDir);
            }
        }

        private void FixedUpdate()
        {
            if (Initialized)
            {
                _vrPlayerMovementService.ApplyGravity();
                _vrPlayerMovementService.Move(_moveInputValue);
            }
        }

        private void OnDestroy()
        {
            Registration(false);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInputValue = context.ReadValue<Vector2>();
        }

        public void OnLook(InputAction.CallbackContext context)
        {
        }

        public void OnHmdMove(InputAction.CallbackContext context)
        {
            Debug.Log($"HMDMove");
            Debug.Log($"{context.ReadValue<Vector3>()} HMD Position");
        }

        public void OnHmdRotate(InputAction.CallbackContext context)
        {
            Debug.Log($"HMDMove");
            Debug.Log($"{context.ReadValue<Vector3>()} HMD Position");
        }

        public void OnGripLeft(InputAction.CallbackContext context)
        {
            Debug.Log("GripLeft");
        }

        public void OnGripRight(InputAction.CallbackContext context)
        {
            Debug.Log("GripRight");
        }

        public void OnTriggerLeft(InputAction.CallbackContext context)
        {
            Debug.Log("TriggerLeft");
        }

        public void OnTriggerRight(InputAction.CallbackContext context)
        {
            Debug.Log("TriggerLeft");
        }

        public void Inject(IInputDispatcher inputDispatcher, VrPlayerMovementService vrPlayerMovementService)
        {
            _inputDispatcher = inputDispatcher;
            _vrPlayerMovementService = vrPlayerMovementService;
        }

        private void Registration(bool register = true)
        {
            Registration registration = new Registration();

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

            _inputDispatcher.ChangeRegistrationAll(
                ActionMaps.VRTransform,
                VRTransformActions.HeadPosition,
                OnHmdMove,
                registration);
            _inputDispatcher.ChangeRegistrationAll(
                ActionMaps.VRTransform,
                VRTransformActions.HeadRotation,
                OnHmdRotate,
                registration);
        }
    }
}