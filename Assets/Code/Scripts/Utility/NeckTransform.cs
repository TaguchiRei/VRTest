using UnityEngine;

public readonly struct NeckTransform
{
    public readonly Vector3 NeckPosition;
    public readonly Quaternion NeckRotation;

    /// <summary> Degree </summary>
    public readonly float BodyRotationY;

    public NeckTransform(Vector3 neckPosition, Quaternion neckRotation, float bodyRotation)
    {
        NeckPosition = neckPosition;
        NeckRotation = neckRotation;
        BodyRotationY = bodyRotation;
    }
}