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
        _bodyYawAccumulated = _modelRoot.eulerAngles.y; // 初期値をワールドYawで設定
    }

    public void AddForce(Vector3 force, ForceMode mode)
    {
        _rigidbody.AddForce(force, mode);
    }

    public void OnHmdUpdate(NeckTransform neckTransform)
    {
        // NeckRotationはワールド回転なのでそのまま適用
        _neckBone.rotation = neckTransform.NeckRotation;

        // 累積値に差分を加算（eulerAngles不連続問題を回避）
        _bodyYawAccumulated += neckTransform.BodyRotationY;

        _modelRoot.rotation = Quaternion.Euler(0f, _bodyYawAccumulated, 0f);
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