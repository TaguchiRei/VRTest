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
            DebugGUI.ObserveVariable("MoveInput", ObserveMoveInput);
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

        private string ObserveMoveInput()
        {
            return _moveInputValue.ToString();
        }

        private void OnDestroy()
        {
            Registration(false);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            _moveInputValue = context.ReadValue<Vector2>();
        }

        private void FixedUpdate()
        {
            if (Initialized)
            {
                _vrPlayerMovementService.ApplyGravity();
                _vrPlayerMovementService.Move(_moveInputValue);
            }
        }

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
        }
    }
}