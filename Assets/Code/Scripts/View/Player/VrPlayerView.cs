using System;
using Application;
using UnityEngine;

public class VrPlayerView : InitializableMonoBehaviour, IVrMovementView
{
    [SerializeField] private Rigidbody _rigidbody;

    private float _bodyYawAccumulated;

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
        if (!Initialized) return;
        _rigidbody.AddForce(force, mode);
    }

    public void UpdateLeftHand(Vector3 position, Quaternion rotation)
    {
        if (!Initialized) return;
    }

    public void UpdateRightHand(Vector3 position, Quaternion rotation)
    {
        if (!Initialized) return;
    }
}