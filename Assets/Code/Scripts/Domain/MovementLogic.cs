using UnityEngine;

namespace Domain
{
    /// <summary>
    /// 重力方向に基づいた移動ベクトルの計算を行う純粋なロジッククラス
    /// </summary>
    public static class MovementLogic
    {
        /// <summary>
        /// 現在の速度から重力方向の速度成分を抽出する
        /// </summary>
        public static Vector3 GetGravityVelocity(Vector3 velocity, Vector3 gravityDirection)
        {
            Vector3 normalizedGravity = gravityDirection.normalized;
            return Vector3.Dot(velocity, normalizedGravity) * normalizedGravity;
        }

        /// <summary>
        /// 現在の速度から水平方向（重力と直交する方向）の速度成分を抽出する
        /// </summary>
        public static Vector3 GetTangentVelocity(Vector3 velocity, Vector3 gravityDirection)
        {
            return velocity - GetGravityVelocity(velocity, gravityDirection);
        }

        /// <summary>
        /// 重力方向に対して垂直な前方方向を算出する
        /// </summary>
        public static Vector3 GetTangentForward(Vector3 gravityDirection)
        {
            Vector3 normalizedGravity = gravityDirection.normalized;
            Vector3 right = GetTangentRight(normalizedGravity);
            
            // Cross(Gravity, Right) -> Forward
            return Vector3.Cross(normalizedGravity, right).normalized;
        }

        /// <summary>
        /// 2次元の入力を、重力方向に直交する平面上の3次元移動ベクトルに変換する
        /// </summary>
        public static Vector3 CalculateMoveVector(Vector2 input, Vector3 gravityDirection)
        {
            Vector3 normalizedGravity = gravityDirection.normalized;
            Vector3 right = GetTangentRight(normalizedGravity);
            Vector3 forward = GetTangentForward(normalizedGravity);

            return (right * input.x + forward * input.y);
        }

        /// <summary>
        /// 重力方向に対して垂直な右方向を算出する
        /// </summary>
        public static Vector3 GetTangentRight(Vector3 gravityDirection)
        {
            Vector3 normalizedGravity = gravityDirection.normalized;
            
            // Cross(Forward, Gravity) -> Right (重力が下向きの場合)
            Vector3 right = Vector3.Cross(Vector3.forward, normalizedGravity);
            if (right.sqrMagnitude < 0.001f)
            {
                right = Vector3.Cross(Vector3.right, normalizedGravity);
            }
            return right.normalized;
        }

        /// <summary>
        /// 移動入力を停止させる際の、新しい速度を計算する（自分が与えた移動成分のみを適切に減算する）
        /// </summary>
        public static Vector3 CalculateVelocityAfterStop(Vector3 currentVelocity, Vector3 movePower)
        {
            if (movePower.sqrMagnitude < 0.001f) return currentVelocity;

            Vector3 moveDir = movePower.normalized;
            
            // 現在の速度のうち、移動方向に向いている成分を抽出
            float currentSpeedOnMoveDir = Vector3.Dot(currentVelocity, moveDir);
            
            // 自分が与えた移動パワーのうち、まだ速度として残っている分だけを取り除く
            //（1.0与えて現在0.8なら0.8引く。1.2（加速中）なら1.0だけ引いて0.2残す）
            float speedToRemove = Mathf.Clamp(currentSpeedOnMoveDir, 0f, movePower.magnitude);
            
            return currentVelocity - (moveDir * speedToRemove);
        }
    }
}
