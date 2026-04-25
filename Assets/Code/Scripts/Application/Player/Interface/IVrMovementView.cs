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
        void UpdateLeftHand(Vector3 position, Quaternion rotation);
        void UpdateRightHand(Vector3 position, Quaternion rotation);
        void UpdateTorso(Vector3 position, Quaternion rotation);
        void UpdateHeadLocalRotation(Quaternion localRotation);
    }
}