using UnityEngine;

namespace Code.Scripts.Domain.Player
{
    /// <summary>
    /// プレイヤーの移動状態を管理するエンティティ
    /// </summary>
    public class PlayerMovementEntity
    {
        public GravityValue Gravity { get; private set; }
        public MovePowerValue LastMovePower { get; private set; }
        public LookDirectionValue LookDirection { get; private set; }

        public PlayerMovementEntity(GravityValue gravity)
        {
            Gravity = gravity;
            LastMovePower = MovePowerValue.Zero;
            LookDirection = LookDirectionValue.Forward;
        }

        public void UpdateMovePower(Vector3 newPower)
        {
            LastMovePower = new MovePowerValue(newPower);
        }

        public void UpdateGravity(GravityValue gravity)
        {
            Gravity = gravity;
        }

        public void UpdateLookDirection(Vector2 direction)
        {
            LookDirection = new LookDirectionValue(direction);
        }
    }
}
