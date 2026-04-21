using UnityEngine;

public class PlayerView : InitializableMonoBehaviour
{
    [SerializeField] private Rigidbody _rigidbody;

    private Vector3 _moveVector;
    private Vector3 _gravityVector;
    private DivisionVector _divisionVector;

    public override void Initialize()
    {
        base.Initialize();
        _divisionVector = new DivisionVector(_rigidbody);
    }

    public void MovePlayer(Vector2 moveVector)
    {
        _divisionVector.MoveVectorChange(moveVector);
    }
}