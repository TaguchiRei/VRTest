using Application;
using Code.Scripts.Domain.Player;
using Code.Scripts.Infrastructure.Player;
using UnityEngine;

public class PlayerInitializer : MonoBehaviour
{
    [SerializeField] private VrPlayerView vrPlayerView;
    [SerializeField] private VrPlayerInfra vrPlayerInfra;

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _gravityPower;
    [SerializeField] private Vector3 _gravityVector;

    public void Initialize(InputDispatcher inputDispatcher)
    {
        vrPlayerView.Initialize();
        var gravity = new GravityValue(_gravityVector, _gravityPower);
        var moveSpeed = new MoveSpeed(_moveSpeed);
        var playerMovementEntity = new PlayerMovementEntity(gravity, moveSpeed);

        vrPlayerInfra.Inject(inputDispatcher,
            new VrPlayerMovementService(
                vrPlayerView,
                playerMovementEntity
            )
        );
        vrPlayerInfra.Initialize();
    }
}