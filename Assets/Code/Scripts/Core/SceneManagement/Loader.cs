namespace Code.Scripts.Core.SceneManagement
{
    public static class Loader
    {
        
        public static void Load(string sceneName)
        {
            SceneTransitionAnimator.Instance
                .Play(sceneName);
        }
    }
}