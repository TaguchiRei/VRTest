using System;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private Transform _target;

    public void LateUpdate()
    {
        transform.position = _target.position;
        transform.rotation = _target.rotation;
    }
}