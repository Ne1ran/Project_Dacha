namespace Core.Scene.Event
{
    public class SceneChangedEvent
    {
        public static string SCENE_LOADED = "SCENE_LOADED";
        public static string SCENE_UNLOADED = "SCENE_UNLOADED";
        public static string SCENE_PRELOAD = "SCENE_PRELOAD";
        public static string SCENE_PREUNLOAD = "SCENE_PREUNLOAD";

        public string SceneName;

        public SceneChangedEvent(string sceneName)
        {
            SceneName = sceneName;
        }
    }
}