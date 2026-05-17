using UnityEngine;
using UsefulTools.Infrastructure.Runtime.Input;
using UsefulTools.UtilityUnity.Runtime.UtilityUnity;
using UsefulVr.View.Runtime.Player;

namespace UsefulTools.Composition.Runtime.Boot
{
    public class VRTestSceneBoot : MonoBehaviour
    {
        [SerializeField] private VRTestSceneContainer _container;

        [SerializeField] private VrPlayerMovementView _vrPlayerMovementView;
        [SerializeField] private InputDispatcher _inputDispatcher;

        private void Start()
        {
            Inject();
            Initialize();
        }

        private void Inject()
        {
        }

        private void Initialize()
        {
            if (_vrPlayerMovementView != null) _vrPlayerMovementView.Initialize();
            if (_inputDispatcher != null) _inputDispatcher.Initialize();
        }
    }
}
