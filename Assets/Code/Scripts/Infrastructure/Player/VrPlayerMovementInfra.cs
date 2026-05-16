using UnityEngine;
using UsefulTools.AutoGenerate;
using UsefulTools.Infrastructure.Runtime.Input;
using UsefulTools.UtilityUnity.Runtime.UtilityUnity;
using UsefulTools.Application.Runtime.Player;
using UsefulVr.Application.Runtime.Player;

namespace UsefulVr.Infrastructure.Runtime.Player
{
    public class VrPlayerMovementInfra : InitializableMonoBehaviour, IVrPlayerInfra
    {
        private IInputDispatcher _inputDispatcher;
        private VrPlayerMovementService _playerMovementService;

        public override void Initialize()
        {
            base.Initialize();
        }

        private void OnMove(InputContext<Vector2> inputContext)
        {
        }

        private void OnLook(InputContext<Vector2> inputContext)
        {
        }

        private void Registration(bool isRegister)
        {
            _inputDispatcher.RegistrationReadValue<Vector2, VRControllersActions>(
                ActionMaps.VRControllers,
                VRControllersActions.Move,
                OnMove, isRegister);
            _inputDispatcher.RegistrationReadValue<Vector2, VRControllersActions>(
                ActionMaps.VRControllers,
                VRControllersActions.Look,
                OnLook, isRegister);
        }
    }
}