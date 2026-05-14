using UnityEngine;

public interface IPlayerPresenter
{
    Vector3 Velocity { get; set; }

    void AddForce(Vector3 force, ForceMode mode);

    /// <summary> カメラの座標をプレイヤー座標に同期する </summary>
    void ResetPosition();
}