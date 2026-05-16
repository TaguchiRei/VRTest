using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UsefulTools.AutoGenerate;
using UsefulTools.UtilityUnity.Runtime.UtilityUnity;

namespace UsefulTools.Infrastructure.Runtime.Input
{
    public class InputDispatcher : InitializableMonoBehaviour, IInputDispatcher
    {
        [SerializeField] private InputActionAsset _actionAsset;

        private readonly Dictionary<Delegate, Action> _registeredActions = new();

        public override void Initialize()
        {
            base.Initialize();
            _actionAsset.Enable();
        }

        private void Update()
        {
            foreach (var updateAction in _registeredActions.Values)
                updateAction();
        }

        private void OnDestroy()
        {
            _registeredActions.Clear();
            _actionAsset.Disable();
        }

        public InputContext<T> ReadValue<T, TAction>(ActionMaps actionMap, TAction actionName)
            where T : unmanaged
            where TAction : Enum
        {
            var action = GetAction(actionMap.ToString(), actionName.ToString());
            if (action == null)
            {
                Debug.LogWarning($"[InputDispatcher] {actionMap}.{actionName} は見つかりませんでした。");
                return new InputContext<T>(InputActionPhase.Disabled, default);
            }

            return new InputContext<T>(action.phase, action.ReadValue<T>());
        }

        public void RegistrationReadValue<T, TAction>(ActionMaps actionMap, TAction actionName,
            Action<InputContext<T>> action, bool isRegister) where T : unmanaged where TAction : Enum
        {
            if (isRegister)
            {
                if (_registeredActions.ContainsKey(action))
                    return;

                void UpdateAction()
                {
                    action?.Invoke(ReadValue<T, TAction>(actionMap, actionName));
                }

                _registeredActions.Add(action, UpdateAction);
            }
            else
            {
                _registeredActions.Remove(action);
            }
        }

        public void RegistrationStarted<TAction>(ActionMaps actionMap, TAction actionName,
            Action<InputAction.CallbackContext> action, bool isRegister) where TAction : Enum
        {
            var inputAction = GetAction(actionMap.ToString(), actionName.ToString());
            if (inputAction == null)
            {
                Debug.LogWarning($"[InputDispatcher] {actionMap}.{actionName} は見つかりませんでした。");
                return;
            }

            if (isRegister)
            {
                inputAction.started += action;
            }
            else
            {
                inputAction.started -= action;
            }
        }

        public void RegistrationCancelled<TAction>(ActionMaps actionMap, TAction actionName,
            Action<InputAction.CallbackContext> action, bool isRegister) where TAction : Enum
        {
            var inputAction = GetAction(actionMap.ToString(), actionName.ToString());
            if (inputAction == null)
            {
                Debug.LogWarning($"[InputDispatcher] {actionMap}.{actionName} は見つかりませんでした。");
                return;
            }

            if (isRegister)
            {
                inputAction.canceled += action;
            }
            else
            {
                inputAction.canceled -= action;
            }
        }

        public void RegistrationStartCancelled<TAction>(ActionMaps actionMap, TAction actionName,
            Action<InputAction.CallbackContext> action, bool isRegister) where TAction : Enum
        {
            var inputAction = GetAction(actionMap.ToString(), actionName.ToString());
            if (inputAction == null)
            {
                Debug.LogWarning($"[InputDispatcher] {actionMap}.{actionName} は見つかりませんでした。");
                return;
            }

            if (isRegister)
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

        public void RegistrationPerformed<TAction>(ActionMaps actionMap, TAction actionName,
            Action<InputAction.CallbackContext> action, bool isRegister) where TAction : Enum
        {
            var inputAction = GetAction(actionMap.ToString(), actionName.ToString());
            if (inputAction == null)
            {
                Debug.LogWarning($"[InputDispatcher] {actionMap}.{actionName} は見つかりませんでした。");
                return;
            }

            if (isRegister)
            {
                inputAction.performed += action;
            }
            else
            {
                inputAction.performed -= action;
            }
        }

        public void RegistrationAll<TAction>(ActionMaps actionMap, TAction actionName,
            Action<InputAction.CallbackContext> action, bool isRegister) where TAction : Enum
        {
            var inputAction = GetAction(actionMap.ToString(), actionName.ToString());
            if (inputAction == null)
            {
                Debug.LogWarning($"[InputDispatcher] {actionMap}.{actionName} は見つかりませんでした。");
                return;
            }

            if (isRegister)
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

        public void SwitchActionMap(ActionMaps actionMap)
        {
            foreach (var map in _actionAsset.actionMaps) map.Disable();
            FindMap(actionMap)?.Enable();
        }

        public void EnableActionMap(ActionMaps actionMap) => FindMap(actionMap)?.Enable();
        public void DisableActionMap(ActionMaps actionMap) => FindMap(actionMap)?.Disable();

        public ActionMaps[] GetActiveActionMap()
        {
            var activeMaps = new List<ActionMaps>();
            foreach (var map in _actionAsset.actionMaps)
            {
                if (!map.enabled) continue;
                if (Enum.TryParse(map.name, out ActionMaps parsed)) activeMaps.Add(parsed);
                else Debug.LogWarning($"[InputDispatcher] ActionMap {map.name} は Enum に存在しません。");
            }

            return activeMaps.ToArray();
        }

        public void EnableInput() => _actionAsset.Enable();
        public void DisableInput() => _actionAsset.Disable();

        private InputActionMap FindMap(ActionMaps actionMap)
        {
            var map = _actionAsset.FindActionMap(actionMap.ToString());
            if (map == null) Debug.LogWarning($"[InputDispatcher] ActionMap {actionMap} は見つかりませんでした。");
            return map;
        }

        private InputAction GetAction(string actionMap, string actionName)
        {
            return _actionAsset.FindActionMap(actionMap)?.FindAction(actionName);
        }
    }
}