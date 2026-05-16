using UnityEngine;
using UsefulTools.Infrastructure.Runtime.Input;

namespace UsefulTools.Composition.Runtime.Input
{
    public class InputInitializer : MonoBehaviour
    {
        public InputDispatcher InputDispatcher;

        public void Initialize()
        {
            InputDispatcher.Initialize();
        }
    }
}