using System;
using Application;
using UnityEngine;
using UnityEngine.InputSystem;
using UsefulTools.AutoGenerate;

public class PlayerController : InitializableMonoBehaviour, IInjectable<IInputDispatcher, PlayerMovementService>
{
    private IInputDispatcher _inputDispatcher;
    private PlayerMovementService _playerMovementService;

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

        Registration();
        DebugGUI.ObserveVariable("MoveInput", ObserveMoveInput);
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
            _playerMovementService.ApplyGravity();
            _playerMovementService.Move(_moveInputValue);
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

    public void Inject(IInputDispatcher inputDispatcher, PlayerMovementService playerMovementService)
    {
        _inputDispatcher = inputDispatcher;
        _playerMovementService = playerMovementService;
    }

    private void Registration(bool register = true)
    {
        Registration registration = new Registration();

        _inputDispatcher.ChangeActionRegistrationAll(
            nameof(ActionMaps.VRControllers),
            nameof(VRControllersActions.Move),
            OnMove,
            registration);

        _inputDispatcher.ChangeActionRegistrationAll(
            nameof(ActionMaps.VRControllers),
            nameof(VRControllersActions.Look),
            OnLook,
            registration);
        _inputDispatcher.ChangeActionRegistrationAll(
            nameof(ActionMaps.VRControllers),
            nameof(VRControllersActions.PushGripLeft),
            OnGripLeft,
            registration);
        _inputDispatcher.ChangeActionRegistrationAll(
            nameof(ActionMaps.VRControllers),
            nameof(VRControllersActions.PushGripRight),
            OnGripRight,
            registration);
        _inputDispatcher.ChangeActionRegistrationAll(
            nameof(ActionMaps.VRControllers),
            nameof(VRControllersActions.PushTriggerLeft),
            OnTriggerLeft,
            registration);
        _inputDispatcher.ChangeActionRegistrationAll(
            nameof(ActionMaps.VRControllers),
            nameof(VRControllersActions.PushTriggerRight),
            OnTriggerRight,
            registration);
    }
}