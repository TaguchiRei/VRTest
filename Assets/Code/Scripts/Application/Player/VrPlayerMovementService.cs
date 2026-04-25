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
        private readonly TorsoEntity _torsoEntity;
        private readonly NeckRootEstimator _neckEstimator = new();

        public VrPlayerMovementService(IVrMovementView view, PlayerMovementEntity entity, TorsoEntity torsoEntity)
        {
            _view = view;
            _entity = entity;
            _torsoEntity = torsoEntity;
        }

        public void UpdateHmdState(Vector3 hmdPosition, Quaternion hmdRotation)
        {
            var (neckPosition, weightedRotation) = _neckEstimator.EstimateNeckRootPosition(hmdRotation, hmdPosition);
            
            if (Vector3.Distance(neckPosition, _torsoEntity.Position) > 0.05f || 
                Quaternion.Angle(weightedRotation, _torsoEntity.Rotation) > 5f)
            {
                _torsoEntity.UpdateTorso(neckPosition, weightedRotation);
                _view.UpdateTorso(neckPosition, weightedRotation);
            }

            Quaternion headLocal = Quaternion.Inverse(_torsoEntity.Rotation) * hmdRotation;
            _torsoEntity.UpdateHeadLocalRotation(headLocal);
            _view.UpdateHeadLocalRotation(headLocal);
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

        public void UpdateHandPosition(
            Vector3 leftHandPosition,
            Vector3 rightHandPosition,
            Quaternion leftHandRotation,
            Quaternion rightHandRotation)
        {
            _view.UpdateLeftHand(leftHandPosition, leftHandRotation);
            _view.UpdateRightHand(rightHandPosition, rightHandRotation);
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