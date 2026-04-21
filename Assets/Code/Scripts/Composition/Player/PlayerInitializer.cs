using UnityEngine;

public class PlayerInitializer : MonoBehaviour
{
    [SerializeField] private PlayerView _playerView;
    [SerializeField] private PlayerController _playerController;

    public void Initialize()
    {
        
        _playerView.Initialize();
        _playerController.Initialize();
    }
}