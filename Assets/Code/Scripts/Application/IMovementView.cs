using UnityEngine;

namespace Application
{
    /// <summary>
    /// 移動処理においてApplicationからViewへ指示を出すためのインターフェース
    /// </summary>
    public interface IMovementView
    {
        Vector3 Velocity { get; set; }
        void AddForce(Vector3 force, ForceMode mode);
    }
}
