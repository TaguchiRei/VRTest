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
        /// 重力方向に対して垂直な基準方向（前方相当）を算出する
        /// </summary>
        public static Vector3 GetTangentForward(Vector3 gravityDirection)
        {
            Vector3 normalizedGravity = gravityDirection.normalized;
            
            // 重力方向が真上や真下の場合を考慮したクロス積
            Vector3 forward = Vector3.Cross(normalizedGravity, Vector3.forward);
            if (forward.sqrMagnitude < 0.001f)
            {
                forward = Vector3.Cross(normalizedGravity, Vector3.right);
            }
            return forward.normalized;
        }

        /// <summary>
        /// 2次元の入力を、重力方向に直交する平面上の3次元移動ベクトルに変換する
        /// </summary>
        public static Vector3 CalculateMoveVector(Vector2 input, Vector3 gravityDirection)
        {
            Vector3 normalizedGravity = gravityDirection.normalized;
            Vector3 right = GetTangentRight(normalizedGravity);
            Vector3 forward = Vector3.Cross(right, normalizedGravity);

            return (right * input.x + forward * input.y);
        }

        /// <summary>
        /// 重力方向に対して垂直な右方向を算出する
        /// </summary>
        public static Vector3 GetTangentRight(Vector3 gravityDirection)
        {
            Vector3 normalizedGravity = gravityDirection.normalized;
            Vector3 right = Vector3.Cross(normalizedGravity, Vector3.forward);
            if (right.sqrMagnitude < 0.001f)
            {
                right = Vector3.Cross(normalizedGravity, Vector3.right);
            }
            return right.normalized;
        }

        /// <summary>
        /// 移動入力を停止させる際の、新しい速度を計算する（移動成分のみを相殺する）
        /// </summary>
        public static Vector3 CalculateVelocityAfterStop(Vector3 currentVelocity, Vector3 movePower)
        {
            if (movePower == Vector3.zero) return currentVelocity;

            Vector3 moveDir = movePower.normalized;
            float currentSpeedOnMoveDir = Vector3.Dot(currentVelocity, moveDir);
            float previousMoveSpeed = movePower.magnitude;
            
            float resultSpeed = currentSpeedOnMoveDir - previousMoveSpeed;
            
            if (currentSpeedOnMoveDir > 0f && resultSpeed < 0f)
            {
                resultSpeed = 0f;
            }
            
            Vector3 nonMoveComponent = currentVelocity - (moveDir * currentSpeedOnMoveDir);
            return nonMoveComponent + (moveDir * resultSpeed);
        }
    }
}
