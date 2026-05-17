using UnityEngine;
using UsefulTools.Composition.Runtime.Input;
using UsefulTools.Infrastructure.Runtime.Input;
using UsefulVr.Composition.Runtime.Player;

namespace VRTest.Composition.Runtime.Boot
{
    public class VRTestSceneBoot : MonoBehaviour
    {
        [SerializeField] private VRTestSceneContainer _container;

        [SerializeField] private InputInitializer _inputInitializer;
        [SerializeField] private PlayerInitializer _playerInitializer;

        private void Start()
        {
            Inject();
            Initialize();
        }

        private void Inject()
        {
            if (_playerInitializer != null && _container.TryGet<IInputDispatcher>(out var arg_playerInitializer_0))
            {
                _playerInitializer.Inject(arg_playerInitializer_0);
            }
        }

        private void Initialize()
        {
            if (_inputInitializer != null) _inputInitializer.Initialize();
            if (_playerInitializer != null) _playerInitializer.Initialize();
        }
    }
}
