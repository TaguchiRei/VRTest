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

    [SerializeField] private HmdSettings _hmdSettings = new(
        neckHeight: 0.18f,
        headForwardOffset: 0.08f,
        yawWeight: 0.35f,
        pitchWeight: 1.0f,
        rollWeight: 0.65f,
        neckYawLimit: 70f
    );

    public void Initialize(InputDispatcher inputDispatcher)
    {
        vrPlayerView.Initialize();
        var gravity = new GravityValue(_gravityVector, _gravityPower);
        var moveSpeed = new MoveSpeed(_moveSpeed);
        var playerMovementEntity = new PlayerMovementEntity(gravity, moveSpeed);
        var estimator = new NeckRootEstimator
        {
            HmdSettings = _hmdSettings
        };

        vrPlayerInfra.Inject(inputDispatcher,
            new VrPlayerMovementService(
                vrPlayerView,
                playerMovementEntity,
                estimator
            )
        );
        vrPlayerInfra.Initialize();
    }
}