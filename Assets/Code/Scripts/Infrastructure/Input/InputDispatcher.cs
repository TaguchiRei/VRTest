using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UsefulTools.AutoGenerate;

[RequireComponent(typeof(PlayerInput))]
public class InputDispatcher : InitializableMonoBehaviour, IInputDispatcher
{
    private PlayerInput _playerInput;

    public override void Initialize()
    {
        base.Initialize();
        
        _playerInput = GetComponent<PlayerInput>();
    }

    private void OnDestroy()
    {
        _playerInput.actions.Disable();
        _playerInput.actions = null;
    }


    public void ChangeRegistrationStarted<TAction>(ActionMaps actionMap, TAction actionName,
        Action<InputAction.CallbackContext> action,
        Registration registration) where TAction : Enum
    {
        var inputAction = GetAction(actionMap.ToString(), actionName.ToString());

        if (inputAction == null)
        {
            Debug.LogWarning($"[InputDispatcher] {actionMap}.{actionName} は見つかりませんでした。");
            return;
        }

        if (registration == Registration.Register)
        {
            inputAction.started += action;
        }
        else
        {
            inputAction.started -= action;
        }
    }

    public void ChangeRegistrationPerformed<TAction>(ActionMaps actionMap, TAction actionName,
        Action<InputAction.CallbackContext> action,
        Registration registration) where TAction : Enum
    {
        var inputAction = GetAction(actionMap.ToString(), actionName.ToString());

        if (inputAction == null)
        {
            Debug.LogWarning($"[InputDispatcher] {actionMap}.{actionName} は見つかりませんでした。");
            return;
        }

        if (registration == Registration.Register)
        {
            inputAction.performed += action;
        }
        else
        {
            inputAction.performed -= action;
        }
    }

    public void ChangeRegistrationCancelled<TAction>(ActionMaps actionMap, TAction actionName,
        Action<InputAction.CallbackContext> action,
        Registration registration) where TAction : Enum
    {
        var inputAction = GetAction(actionMap.ToString(), actionName.ToString());

        if (inputAction == null)
        {
            Debug.LogWarning($"[InputDispatcher] {actionMap}.{actionName} は見つかりませんでした。");
            return;
        }

        if (registration == Registration.Register)
        {
            inputAction.canceled += action;
        }
        else
        {
            inputAction.canceled -= action;
        }
    }

    public void ChangeRegistrationAll<TAction>(ActionMaps actionMap, TAction actionName,
        Action<InputAction.CallbackContext> action,
        Registration registration) where TAction : Enum
    {
        var inputAction = GetAction(actionMap.ToString(), actionName.ToString());

        if (inputAction == null)
        {
            Debug.LogWarning($"[InputDispatcher] {actionMap}.{actionName} は見つかりませんでした。");
            return;
        }

        if (registration == Registration.Register)
        {
            inputAction.started += action;
            inputAction.performed += action;
            inputAction.canceled += action;
        }
        else
        {
            inputAction.started -= action;
            inputAction.performed -= action;
            inputAction.canceled -= action;
        }
    }

    public void ChangeRegistrationStartCancelled<TAction>(ActionMaps actionMap, TAction actionName,
        Action<InputAction.CallbackContext> action,
        Registration registration) where TAction : Enum
    {
        var inputAction = GetAction(actionMap.ToString(), actionName.ToString());

        if (inputAction == null)
        {
            Debug.LogWarning($"[InputDispatcher] {actionMap}.{actionName} は見つかりませんでした。");
            return;
        }

        if (registration == Registration.Register)
        {
            inputAction.started += action;
            inputAction.canceled += action;
        }
        else
        {
            inputAction.started -= action;
            inputAction.canceled -= action;
        }
    }

    public void SwitchActionMap(ActionMaps actionMap)
    {
        if (_playerInput == null) return;

        var targetMap = _playerInput.actions.FindActionMap(actionMap.ToString());

        if (targetMap == null)
        {
            Debug.LogWarning($"[InputDispatcher] ActionMap {actionMap} は見つかりませんでした。");
            return;
        }

        foreach (var map in _playerInput.actions.actionMaps)
        {
            map.Disable();
        }

        targetMap.Enable();
    }

    public void EnableActionMap(ActionMaps actionMap)
    {
        if (_playerInput == null) return;

        var targetMap = _playerInput.actions.FindActionMap(actionMap.ToString());

        if (targetMap == null)
        {
            Debug.LogWarning($"[InputDispatcher] ActionMap {actionMap} は見つかりませんでした。");
            return;
        }

        targetMap.Enable();
    }

    public void DisableActionMap(ActionMaps actionMap)
    {
        if (_playerInput == null) return;

        var targetMap = _playerInput.actions.FindActionMap(actionMap.ToString());

        if (targetMap == null)
        {
            Debug.LogWarning($"[InputDispatcher] ActionMap {actionMap} は見つかりませんでした。");
            return;
        }

        targetMap.Disable();
    }

    public ActionMaps[] GetActiveActionMap()
    {
        if (_playerInput == null)
        {
            return Array.Empty<ActionMaps>();
        }

        var activeMaps = new System.Collections.Generic.List<ActionMaps>();

        foreach (var map in _playerInput.actions.actionMaps)
        {
            if (!map.enabled) continue;

            if (Enum.TryParse(map.name, out ActionMaps parsedMap))
            {
                activeMaps.Add(parsedMap);
            }
            else
            {
                Debug.LogWarning($"[InputDispatcher] ActionMap {map.name} は見つかりませんでした。");
            }
        }

        return activeMaps.ToArray();
    }

    public void EnableInput()
    {
        _playerInput.actions.Enable();
    }

    public void DisableInput()
    {
        _playerInput.actions.Disable();
    }

    private InputAction GetAction(string actionMap, string actionName)
    {
        if (_playerInput == null) return null;

        var map = _playerInput.actions.FindActionMap(actionMap);
        if (map == null)
        {
            return null;
        }

        var action = map.FindAction(actionName);
        if (action == null)
        {
            return null;
        }

        return action;
    }
}