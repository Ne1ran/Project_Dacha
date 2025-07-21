namespace Core.SceneManagement.Event
{
    public class SceneChangedEvent
    {
        public const string SCENE_LOADED = "SCENE_LOADED";
        public const string SCENE_UNLOADED = "SCENE_UNLOADED";
        public const string SCENE_PRELOAD = "SCENE_PRELOAD";
        public const string SCENE_PREUNLOAD = "SCENE_PREUNLOAD";

        public string SceneName { get; }
        public UnityEngine.SceneManagement.Scene? Scene { get; }

        public SceneChangedEvent(string sceneName, UnityEngine.SceneManagement.Scene? scene = null)
        {
            SceneName = sceneName;
            Scene = scene;
        }
    }
}