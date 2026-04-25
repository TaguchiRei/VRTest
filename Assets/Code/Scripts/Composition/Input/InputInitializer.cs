using UnityEngine;

public class InputInitializer : MonoBehaviour
{
    public InputDispatcher InputDispatcher;

    public void Initialize()
    {
        InputDispatcher.Initialize();
    }
}