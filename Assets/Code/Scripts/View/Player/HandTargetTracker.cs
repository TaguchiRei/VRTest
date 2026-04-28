using UnityEngine;

public class HandTargetTracker : MonoBehaviour
{
    [SerializeField] private Transform _tracker;

    /// <summary>
    /// Animationの更新より後に動かす
    /// </summary>
    private void LateUpdate()
    {
        transform.position = _tracker.position;
        transform.rotation = _tracker.rotation;
    }
}