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
        /// 入力と視線方向、重力方向に基づいた移動ベクトルを計算する
        /// </summary>
        public static Vector3 CalculateMoveVector(Vector2 input, Vector3 gravityDirection, Vector2 lookDirection)
        {
            if (input.sqrMagnitude < 0.001f) return Vector3.zero;

            // カメラの向きに基づいた水平面上の基準軸を算出
            Vector3 lookForward = new Vector3(lookDirection.x, 0, lookDirection.y).normalized;
            Vector3 lookRight = new Vector3(lookDirection.y, 0, -lookDirection.x).normalized;

            // 入力をカメラ基準の水平面上の方向に変換
            Vector3 intendedDirection = (lookRight * input.x + lookForward * input.y);

            // 重力方向に基づいた接平面への投影
            Vector3 normalizedGravity = gravityDirection.normalized;
            
            // intendedDirection から重力方向の成分を取り除くことで接平面上のベクトルを得る
            Vector3 projectedVector = intendedDirection - Vector3.Dot(intendedDirection, normalizedGravity) * normalizedGravity;

            // ベクトルを正規化して元の入力強度を掛ける（斜面での減速を防ぐ）
            if (projectedVector.sqrMagnitude < 0.001f) return Vector3.zero;
            return projectedVector.normalized * input.magnitude;
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
            float speedToRemove = Mathf.Clamp(currentSpeedOnMoveDir, 0f, movePower.magnitude);
            
            return currentVelocity - (moveDir * speedToRemove);
        }
    }
}
