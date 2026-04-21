using UnityEngine;

public class InputInitializer : MonoBehaviour
{
    public InputDispatcher _inputDispatcher;

    private void Initialize()
    {
        _inputDispatcher.Initialize();
    }
}