using Application;
using UnityEngine;
using UnityEngine.InputSystem;
using UsefulTools.AutoGenerate;

namespace Code.Scripts.Infrastructure.Player
{
    public class VrPlayerInfra : InitializableMonoBehaviour, IInjectable<IInputDispatcher, VrPlayerMovementService>
    {
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private Transform _bodyRoot;

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

            ChangeRegistration();
            _inputDispatcher.EnableActionMap(ActionMaps.VRTransform);
            _inputDispatcher.EnableActionMap(ActionMaps.VRControllers);
            DebugGUI.ObserveVariable("MoveInput", () => _moveInputValue.ToString());
        }

        private void OnDestroy()
        {
            ChangeRegistration(false);
        }

        public void OnHmdMove(InputAction.CallbackContext context)
        {
            
        }

        public void OnHmdRotate(InputAction.CallbackContext context)
        {
            
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

        private void FixedUpdate()
        {
            if (!Initialized) return;

            var moveCtx = _inputDispatcher.ReadValue<Vector2, VRControllersActions>(
                ActionMaps.VRControllers, VRControllersActions.Move);

            _moveInputValue = moveCtx.Phase switch
            {
                InputActionPhase.Disabled => Vector2.zero,
                InputActionPhase.Waiting => Vector2.zero,
                _ => moveCtx.Value
            };

            _vrPlayerMovementService.ApplyGravity();
            _vrPlayerMovementService.Move(_moveInputValue);
        }

        private void Update()
        {
            if (!Initialized) return;

            var hmdPosCtx = _inputDispatcher.ReadValue<Vector3, VRTransformActions>(
                ActionMaps.VRTransform, VRTransformActions.HeadPosition);

            var hmdRotCtx = _inputDispatcher.ReadValue<Quaternion, VRTransformActions>(
                ActionMaps.VRTransform, VRTransformActions.HeadRotation);

            if (!hmdRotCtx.IsActive) return;

            // bodyRotationをInfraから取得
            Quaternion bodyRotation = _bodyRoot.rotation;

            // LookDirectionの更新にbodyRotationを渡す
            _vrPlayerMovementService.OnHmdUpdate(hmdPosCtx.Value, hmdRotCtx.Value, bodyRotation);
        }

        private void ChangeRegistration(bool register = true)
        {
            _inputDispatcher.ChangeRegistrationStartCancelled(ActionMaps.VRControllers,
                VRControllersActions.PushGripLeft, OnGripLeft, register);
            _inputDispatcher.ChangeRegistrationStartCancelled(ActionMaps.VRControllers,
                VRControllersActions.PushGripRight, OnGripRight, register);
            _inputDispatcher.ChangeRegistrationStartCancelled(ActionMaps.VRControllers,
                VRControllersActions.PushTriggerLeft, OnTriggerLeft, register);
            _inputDispatcher.ChangeRegistrationStartCancelled(ActionMaps.VRControllers,
                VRControllersActions.PushTriggerRight, OnTriggerRight, register);
        }
    }
}