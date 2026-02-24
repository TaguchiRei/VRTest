using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RepulsionObject : MonoBehaviour
{
    public Rigidbody Rigid;

    [Header("何フレーム分の座標を記録するか")] [SerializeField]
    private int savePositionCount = 10;

    private Queue<Vector3> Positions = new();
    private int _frameCount = 0;

    public void Update()
    {
        Positions.Enqueue(transform.position);
        if (Positions.Count < savePositionCount) return;
        Positions.Dequeue();
    }

    public Vector3 GetOldPosition()
    {
        return Positions.Peek();
    }
}