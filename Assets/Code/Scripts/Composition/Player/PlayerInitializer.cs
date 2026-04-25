using Application;
using UnityEngine;

public class PlayerInitializer : MonoBehaviour
{
    [SerializeField] private VrPlayerView vrPlayerView;
    [SerializeField] private PlayerController _playerController;

    [SerializeField] private float _moveSpeed;
    [SerializeField] private float _gravityPower;
    [SerializeField] private Vector3 _gravityVector;

    public void Initialize(InputDispatcher inputDispatcher)
    {
        vrPlayerView.Initialize();
        _playerController.Inject(inputDispatcher,
            new VrPlayerMovementService(
                vrPlayerView, new(new(_gravityVector, _gravityPower), new(_moveSpeed))));
        _playerController.Initialize();
    }
}