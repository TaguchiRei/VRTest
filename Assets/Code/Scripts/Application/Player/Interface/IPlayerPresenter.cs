using UnityEngine;

public interface IPlayerPresenter
{
    /// <summary> Rigidbody.linerVelocity </summary>
    Vector3 Velocity { get; set; }

    Vector3 GravityVector { get; set; }

    float GravityPower { get; set; }

    void AddForce(Vector3 force, ForceMode mode);

    /// <summary> カメラの座標をプレイヤー座標に同期する </summary>
    void SetOffset(Vector3 offset);
}