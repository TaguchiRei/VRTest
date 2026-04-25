using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private Transform _target;

    private Vector3 _offset;

    private void Awake()
    {
        _offset = Quaternion.Inverse(_target.rotation)
                  * (transform.position - _target.position);
    }

    public void LateUpdate()
    {
        transform.position = _target.position + _target.rotation * _offset;
        transform.rotation = _target.rotation;
    }
}