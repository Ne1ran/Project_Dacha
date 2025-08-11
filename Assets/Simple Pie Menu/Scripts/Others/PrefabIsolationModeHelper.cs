using UnityEngine;

namespace Simple_Pie_Menu.Scripts.Others
{
    public class PrefabIsolationModeHelper : MonoBehaviour
    {
        public static bool IsInPrefabIsolationMode()
        {
#if UNITY_EDITOR
            if (UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage() != null) {
                return true;
            }
#endif

            return false;
        }
    }
}
