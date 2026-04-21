using Application;
using UnityEngine;


public class PlayerView : InitializableMonoBehaviour, IMovementView
{
    [SerializeField] private Rigidbody _rigidbody;

    public Vector3 Velocity
    {
        get => _rigidbody.linearVelocity;
        set => _rigidbody.linearVelocity = value;
    }

    public override void Initialize()
    {
        base.Initialize();
        _rigidbody.useGravity = false;
    }

    public void AddForce(Vector3 force, ForceMode mode)
    {
        _rigidbody.AddForce(force, mode);
    }
}