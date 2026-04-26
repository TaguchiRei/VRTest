using UnityEngine;

namespace Application
{
    /// <summary>
    /// 移動処理においてApplicationからViewへ指示を出すためのインターフェース
    /// </summary>
    public interface IVrMovementView
    {
        Vector3 Velocity { get; set; }
        void AddForce(Vector3 force, ForceMode mode);
        void OnHmdUpdate(NeckTransform neckTransform);
        void UpdateLeftHand(Vector3 position, Quaternion rotation);
        void UpdateRightHand(Vector3 position, Quaternion rotation);
    }
}