using UnityEngine;

namespace Code.Scripts.Domain.Player
{
    /// <summary>
    /// カメラが水平面上でどの方向を向いているかを保持する値オブジェクト
    /// </summary>
    public readonly struct LookDirectionValue
    {
        /// <summary>
        /// 水平面上の正規化された方向ベクトル
        /// </summary>
        public Vector2 Value { get; }

        public LookDirectionValue(Vector2 value)
        {
            Value = value.sqrMagnitude > 0 ? value.normalized : Vector2.up;
        }

        /// <summary>
        /// デフォルト値
        /// </summary>
        public static LookDirectionValue Forward => new LookDirectionValue(Vector2.up);
    }
}