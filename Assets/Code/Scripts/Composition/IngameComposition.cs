using System;
using UnityEngine;

public class IngameComposition : MonoBehaviour
{
    [SerializeField] private InputInitializer _inputInitializer;
    [SerializeField] private PlayerInitializer _playerInitializer;

    private void Start()
    {
        _inputInitializer.Initialize();
        
        _playerInitializer.Initialize(_inputInitializer.InputDispatcher);
    }
}