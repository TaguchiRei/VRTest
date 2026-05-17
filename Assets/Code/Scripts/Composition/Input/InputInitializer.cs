using UnityEngine;
using UsefulTools.Infrastructure.Runtime.Input;
using UsefulTools.UtilityUnity.Runtime.UtilityUnity;
using VRTest.Composition.Runtime.Boot;

namespace UsefulTools.Composition.Runtime.Input
{
    public class InputInitializer : InitializerBase
    {
        [SerializeField] private InputDispatcher _inputDispatcher;

        private void Awake()
        {
            VRTestSceneContainer.Register<IInputDispatcher>(_inputDispatcher);
        }

        public override void Initialize()
        {
            base.Initialize();
            _inputDispatcher.Initialize();
        }
    }
}