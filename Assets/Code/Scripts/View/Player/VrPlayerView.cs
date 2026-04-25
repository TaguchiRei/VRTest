using System;
using Application;
using UnityEngine;

public class VrPlayerView : InitializableMonoBehaviour, IVrMovementView
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform _neckTransform;
    [SerializeField] private Transform _headTransform;
    [SerializeField] private Transform _leftHandTransform;
    [SerializeField] private Transform _rightHandTransform;

    public Vector3 Velocity
    {
        get => _rigidbody.linearVelocity;
        set => _rigidbody.linearVelocity = value;
    }

    public Quaternion WorldRotation => transform.rotation;

    public override void Initialize()
    {
        base.Initialize();
        _rigidbody.useGravity = false;
    }

    public void AddForce(Vector3 force, ForceMode mode)
    {
        _rigidbody.AddForce(force, mode);
    }

    public void ShiftPosition(Vector3 worldDelta)
    {
        _rigidbody.MovePosition(_rigidbody.position + worldDelta);
    }

    public void UpdateNeckRotation(Quaternion rotation)
    {
        _neckTransform.localRotation = rotation;
    }

    public void UpdateHeadRotation(Quaternion rotation)
    {
        _headTransform.localRotation = rotation;
    }

    public void UpdateLeftHand(Vector3 position, Quaternion rotation)
    {
        _leftHandTransform.localPosition = position;
        _leftHandTransform.localRotation = rotation;
    }

    public void UpdateRightHand(Vector3 position, Quaternion rotation)
    {
        _rightHandTransform.localPosition = position;
        _rightHandTransform.localRotation = rotation;
    }
}