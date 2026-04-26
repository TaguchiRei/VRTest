using System;
using Application;
using UnityEngine;

public class VrPlayerView : InitializableMonoBehaviour, IVrMovementView
{
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Transform _leftHandTransform;
    [SerializeField] private Transform _rightHandTransform;
    [SerializeField] private Transform _modelRoot;
    [SerializeField] private Transform _neckBone;

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

    public void OnHmdRotate(NeckTransform neckTransform)
    {
        _neckBone.position = neckTransform.NeckPosition;
        _neckBone.rotation = neckTransform.NeckRotation;

        Vector3 currentEuler = _modelRoot.rotation.eulerAngles;
        float bodyYaw = neckTransform.BodyRotationY;

        float newYaw = currentEuler.y + bodyYaw;

        _modelRoot.rotation = Quaternion.Euler(currentEuler.x, newYaw, currentEuler.z);
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
}