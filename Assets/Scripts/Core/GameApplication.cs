using UnityEngine;

namespace Core
{
    public class GameApplication : MonoBehaviour
    {
        [RuntimeInitializeOnLoadMethod]
        private static void StartApplication()
        {
            Debug.Log("Starting Game Application");
            
            // Todo initialize services using DI, create everything needed
        }
    }
}