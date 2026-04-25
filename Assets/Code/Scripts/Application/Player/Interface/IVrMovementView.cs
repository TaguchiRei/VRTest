using UnityEngine;

namespace Application
{
    /// <summary>
    /// 移動処理においてApplicationからViewへ指示を出すためのインターフェース
    /// </summary>
    public interface IVrMovementView
    {
        Vector3 Velocity { get; set; }
        Quaternion WorldRotation { get; }
        void AddForce(Vector3 force, ForceMode mode);
        void ShiftPosition(Vector3 worldDelta);
        void UpdateNeckRotation(Quaternion rotation);
        void UpdateHeadRotation(Quaternion rotation);
        void UpdateLeftHand(Vector3 position, Quaternion rotation);
        void UpdateRightHand(Vector3 position, Quaternion rotation);
    }
}