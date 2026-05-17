using System;
using System.Collections.Generic;
using UnityEngine;
namespace UsefulTools.Composition.Runtime.Boot
{
    public sealed class VRTestSceneContainer : MonoBehaviour
    {
        private static VRTestSceneContainer _instance;

        private readonly Dictionary<Type, object> _instances = new();

        public static void Register<T>(T instance)
        {
            if (_instance == null)
            {
                Debug.LogError("Container is not initialized.");
                return;
            }

            var type = typeof(T);

            if (_instance._instances.ContainsKey(type))
            {
                Debug.LogWarning($"{type.Name} already registered.");
                return;
            }

            _instance._instances.Add(type, instance);
        }

        public bool TryGet<T>(out T result)
        {
            if (_instances.TryGetValue(typeof(T), out var value))
            {
                result = (T)value;
                return true;
            }

            result = default;
            return false;
        }

        private void Awake()
        {
            _instance = this;
        }
    }
}
