using UnityEngine;
using Code.Scripts.Domain.Player;
using UsefulVr.Domain.Runtime.Domain;

namespace UsefulVr.Application.Runtime.Player
{
    /// <summary>
    /// プレイヤー移動ユースケース
    /// </summary>
    public class VrPlayerMovementService
    {
        private const float CAMERA_OFFSET_THRESHOLD = 0.25f;

        private readonly IPlayerPresenter _playerPresenter;
        private readonly PlayerMovementEntity _entity;

        public VrPlayerMovementService(
            IPlayerPresenter playerPresenter,
            PlayerMovementEntity entity)
        {
            _playerPresenter = playerPresenter;
            _entity = entity;
        }

        /// <summary>
        /// 視線方向更新
        /// </summary>
        public void UpdateLookDirection(
            Quaternion neckRotation)
        {
            Vector3 gravity =
                _entity.Gravity.Direction.normalized;

            // 生forward
            Vector3 rawForward =
                neckRotation * Vector3.forward;

            // 接平面へ投影
            Vector3 tangentForward =
                Vector3.ProjectOnPlane(
                    rawForward,
                    gravity);

            // 真上/真下対策
            if (tangentForward.sqrMagnitude < 0.001f)
            {
                tangentForward =
                    MovementLogic.GetTangentForward(
                        gravity);
            }

            _entity.UpdateLookDirection(
                tangentForward.normalized);
        }

        /// <summary>
        /// HMD位置補正
        /// </summary>
        public void ApplyCameraOffset(
            Vector3 cameraLocalPosition)
        {
            Vector3 offset =
                Vector3.ProjectOnPlane(
                    cameraLocalPosition,
                    _entity.Gravity.Direction.normalized);

            if (offset.sqrMagnitude <
                CAMERA_OFFSET_THRESHOLD *
                CAMERA_OFFSET_THRESHOLD)
            {
                return;
            }

            _playerPresenter.SetOffset(offset);
        }

        /// <summary>
        /// 入力移動
        /// </summary>
        public void Move(Vector2 input)
        {
            Vector3 currentVelocity =
                _playerPresenter.Velocity;

            // 前回移動分除去
            Vector3 velocityWithoutLastMove =
                MovementLogic.CalculateVelocityAfterStop(
                    currentVelocity,
                    _entity.LastMovePower.Value);

            // 新規移動方向
            Vector3 moveVector =
                MovementLogic.CalculateMoveVector(
                    input,
                    _entity.Gravity.Direction,
                    _entity.LookDirection.Value);

            // Speed適用
            moveVector *= _entity.MoveSpeed.Value;

            // Entity更新
            _entity.UpdateMovePower(moveVector);

            // Velocity反映
            _playerPresenter.Velocity =
                velocityWithoutLastMove +
                moveVector;
        }

        /// <summary>
        /// 重力適用
        /// </summary>
        public void ApplyGravity()
        {
            _playerPresenter.AddForce(
                _entity.Gravity.GravityForce,
                ForceMode.Acceleration);
        }
    }
}