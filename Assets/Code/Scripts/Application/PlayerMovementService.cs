using Code.Scripts.Domain.Player;
using UnityEngine;
using Domain;

namespace Application
{
    /// <summary>
    /// プレイヤーの移動ユースケースを制御するサービス
    /// </summary>
    public class PlayerMovementService
    {
        private readonly IMovementView _view;
        private readonly PlayerMovementEntity _entity;

        public PlayerMovementService(IMovementView view, PlayerMovementEntity entity)
        {
            _view = view;
            _entity = entity;
        }

        /// <summary>
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

            // 新しい移動ベクトルを計算（視線の向きを考慮）
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
        /// 視線の向きを更新する
        /// </summary>
        public void UpdateLookDirection(Vector2 direction)
        {
            _entity.UpdateLookDirection(direction);
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