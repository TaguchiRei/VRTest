using Code.Scripts.Domain.Player;
using UnityEngine;
using Domain;

namespace Application
{
    /// <summary>
    /// プレイヤーの移動ユースケースを制御するサービス
    /// </summary>
    public class VrPlayerMovementService
    {
        private readonly IVrMovementView _view;
        private readonly PlayerMovementEntity _entity;

        public VrPlayerMovementService(
            IVrMovementView view, PlayerMovementEntity entity)
        {
            _view = view;
            _entity = entity;
        }

        /// <summary>
        /// 視線の向きを更新する
        /// NeckTransformの回転（制限済み）から移動方向を決定する
        /// </summary>
        private void UpdateLookDirection(Quaternion neckRotation)
        {
            Vector3 forward = neckRotation * Vector3.forward;
            Vector2 horizontalDirection = new Vector2(forward.x, forward.z);

            horizontalDirection = horizontalDirection.sqrMagnitude > 0f
                ? horizontalDirection.normalized
                : Vector2.up;

            _entity.UpdateLookDirection(horizontalDirection);
        }

        /// <summary>
        /// 入力に基づいてプレイヤーを移動させる
        /// </summary>
        public void Move(Vector2 input)
        {
            // 現在の速度
            Vector3 currentVelocity = _view.Velocity;

            // 前回の移動成分を取り除いた速度を計算
            Vector3 velocityWithoutLastMove = MovementLogic.CalculateVelocityAfterStop(
                currentVelocity,
                _entity.LastMovePower.Value
            );

            // 新しい移動ベクトルを計算
            Vector3 newMoveVector = MovementLogic.CalculateMoveVector(
                input,
                _entity.Gravity.Direction,
                _entity.LookDirection.Value
            );

            // エンティティの状態を更新
            _entity.UpdateMovePower(newMoveVector);

            // Viewに反映
            _view.Velocity = velocityWithoutLastMove + newMoveVector;
        }

        /// <summary>
        /// 重力を適用する
        /// </summary>
        public void ApplyGravity()
        {
            _view.AddForce(_entity.Gravity.GravityForce, ForceMode.Acceleration);
        }
    }
}