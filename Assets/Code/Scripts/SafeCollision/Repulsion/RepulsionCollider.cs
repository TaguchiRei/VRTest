using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 滑らかに反発するコライダー
/// </summary>
public class RepulsionCollider : MonoBehaviour
{
    private Dictionary<Collider, RepulsionObject> _repulsionObjects = new();

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<RepulsionObject>(out var repulsion))
        {
            repulsion.SetInPosition();
            repulsion.RepulsionStateChanged?.Invoke(RepulsionState.In);
            _repulsionObjects[other] = repulsion;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        foreach (var repulsionObject in _repulsionObjects.Values)
        {
            repulsionObject.RepulsionStateChanged?.Invoke(RepulsionState.Stay);
            repulsionObject.AddRepulsionForce();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (_repulsionObjects.TryGetValue(other, out var repulsion))
        {
            repulsion.RepulsionStateChanged?.Invoke(RepulsionState.Out);
            _repulsionObjects.Remove(other);
        }
    }
}