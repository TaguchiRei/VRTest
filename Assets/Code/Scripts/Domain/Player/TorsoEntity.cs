using UnityEngine;

namespace Code.Scripts.Domain.Player
{
    /// <summary>
    /// 胴体の状態を管理するエンティティ
    /// </summary>
    public class TorsoEntity
    {
        public Vector3 Position { get; private set; }
        public Quaternion Rotation { get; private set; }
        public Quaternion HeadLocalRotation { get; private set; }

        public TorsoEntity(Vector3 initialPosition, Quaternion initialRotation)
        {
            Position = initialPosition;
            Rotation = initialRotation;
            HeadLocalRotation = Quaternion.identity;
        }

        public void UpdateTorso(Vector3 position, Quaternion rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public void UpdateHeadLocalRotation(Quaternion localRotation)
        {
            HeadLocalRotation = localRotation;
        }
    }
}
