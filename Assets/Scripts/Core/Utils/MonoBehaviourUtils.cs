using UnityEngine;

namespace Core.Utils
{
    public static class MonoBehaviourUtils
    {
        public static T GetComponentInChildren<T>(this MonoBehaviour monoBehaviour, string name, bool includeInactive = false)
                where T : Component
        {
            return monoBehaviour.transform.GetComponentInChildren<T>(name, includeInactive);
        }
        
        public static T RequireComponentInChildren<T>(this MonoBehaviour monoBehaviour, string name, bool includeInactive = false)
                where T : Component
        {
            return monoBehaviour.transform.RequireComponentInChildren<T>(name, includeInactive);
        }
    }
}