using UnityEngine;

namespace Code.Scripts.Core.Initialization
{
    public static class AppInitializer
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void InitializeApplication()
        {
            Application.runInBackground = true;
        }
    }
}