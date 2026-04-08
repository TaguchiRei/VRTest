using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(Rigidbody))]
public class RepulsionObject : MonoBehaviour
{
    [Header("反発力の基準")] [SerializeField] private float _baseRepulsiveForce = 100f;

    [Header("反発力の鋭さ（2以上で指数関数的に増加）")] [SerializeField]
    private float _forceCurve = 2.0f;

    [Header("最大反発力")] [SerializeField] private float _maxForce = 2000f;

    [Header("記録フレーム数")] [SerializeField] private int _savePositionCount = 10;

    private Queue<Vector3> _positions = new();
    private Rigidbody _rigidBody;
    private Vector3 _inPosition;

    public Action<RepulsionState> RepulsionStateChanged;

    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody>();
        // 初期位置で埋めておく
        for (int i = 0; i < _savePositionCount; i++) _positions.Enqueue(transform.position);
    }

    private void FixedUpdate()
    {
        //正確に指定したフレーム数だけ保存するためにFixedUpdate
        _positions.Enqueue(transform.position);
        if (_positions.Count > _savePositionCount) _positions.Dequeue();
    }

    public void SetInPosition()
    {
        // 侵入した瞬間の過去の位置を固定
        _inPosition = _positions.Peek();
    }

    public void AddRepulsionForce()
    {
        Vector3 diff = _inPosition - transform.position;
        float distance = diff.magnitude;
        if (distance < 0.001f) return;

        // 反発ベクトルを正規化
        Vector3 direction = diff.normalized;

        // 指数関数的に増加する反発力の計算
        float forceMagnitude = _baseRepulsiveForce * Mathf.Pow(distance, _forceCurve);

        // 過剰な力によるオブジェクトの消失を防ぐクランプ
        forceMagnitude = Mathf.Min(forceMagnitude, _maxForce);

        _rigidBody.AddForce(direction * forceMagnitude, ForceMode.Acceleration);
    }
}

public enum RepulsionState
{
    In,
    Stay,
    Out,
}