using Application;
using Code.Scripts.Domain.Player;
using Code.Scripts.Infrastructure.Player;
using UnityEngine;

public class PlayerInitializer : MonoBehaviour
{
    [SerializeField] private VrPlayerView _vrPlayerView;
    [SerializeField] private VrPlayerInfra _vrPlayerInfra;

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _gravityPower;
    [SerializeField] private Vector3 _gravityVector;

    public void Initialize(InputDispatcher inputDispatcher)
    {
        _vrPlayerView.Initialize();

        var torsoEntity = new TorsoEntity(
            _vrPlayerView.transform.position,
            _vrPlayerView.transform.rotation);
        
        _vrPlayerInfra.Inject(inputDispatcher,
            new VrPlayerMovementService(_vrPlayerView, new(new GravityValue(), new MoveSpeed()), torsoEntity));
        _vrPlayerInfra.Initialize();
    }
}