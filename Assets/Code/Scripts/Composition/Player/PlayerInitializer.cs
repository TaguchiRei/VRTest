using Application;
using UnityEngine;

public class PlayerInitializer : MonoBehaviour
{
    [SerializeField] private PlayerView _playerView;
    [SerializeField] private PlayerController _playerController;

    [SerializeField] private float _gravityPower;
    [SerializeField] private Vector3 _gravityVector;

    public void Initialize(InputDispatcher inputDispatcher)
    {
        _playerView.Initialize();
        _playerController.Inject(inputDispatcher,
            new PlayerMovementService(_playerView, new(new(_gravityVector, _gravityPower))));
        _playerController.Initialize();
    }
}