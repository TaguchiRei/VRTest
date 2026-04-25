using System;
using Application;
using UnityEngine;

public class VrPlayerView : InitializableMonoBehaviour, IVrMovementView
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Transform _leftHandTransform;
    [SerializeField] private Transform _rightHandTransform;
    [SerializeField] private Transform _torsoTransform;
    [SerializeField] private Transform _headTransform;

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

    public void CameraRotation(Quaternion rotation)
    {
        _cameraTransform.rotation = rotation;
    }

    public void UpdateLeftHand(Vector3 position, Quaternion rotation)
    {
        _leftHandTransform.position = position;
        _leftHandTransform.rotation = rotation;
    }

    public void UpdateRightHand(Vector3 position, Quaternion rotation)
    {
        _rightHandTransform.position = position;
        _rightHandTransform.rotation = rotation;
    }

    public void UpdateTorso(Vector3 position, Quaternion rotation)
    {
        if (_torsoTransform != null)
        {
            _torsoTransform.position = position;
            _torsoTransform.rotation = rotation;
        }
    }

    public void UpdateHeadLocalRotation(Quaternion localRotation)
    {
        if (_headTransform != null)
        {
            _headTransform.localRotation = localRotation;
        }
    }
}