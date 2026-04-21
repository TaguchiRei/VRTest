using Code.Scripts.Domain.Player;
using UnityEngine;

namespace Domain
{
    /// <summary>
    /// プレイヤーの移動状態を管理するエンティティ
    /// </summary>
    public class PlayerMovementEntity
    {
        public GravityValue Gravity { get; private set; }
        public MovePowerValue LastMovePower { get; private set; }

        public MoveSpeed MoveSpeed { get; private set; }

        public PlayerMovementEntity(GravityValue gravity, MoveSpeed moveSpeed)
        {
            Gravity = gravity;
            MoveSpeed = moveSpeed;
            LastMovePower = MovePowerValue.Zero;
        }

        public void UpdateMovePower(Vector3 newPower)
        {
            LastMovePower = new MovePowerValue(newPower);
        }

        public void UpdateGravity(GravityValue gravity)
        {
            Gravity = gravity;
        }
    }
}