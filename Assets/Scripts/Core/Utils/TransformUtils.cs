using UnityEngine;

namespace Core.Utils
{
    public static class TransformUtils
    {
        public static T GetComponentInChildren<T>(this Transform transform, string name, bool includeInactive = false)
                where T : Component
        {
            T[] all = transform.GetComponentsInChildren<T>(includeInactive);
            // ReSharper disable once ForCanBeConvertedToForeach
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (int i = 0; i < all.Length; i++) {
                T c = all[i];
                if (c.name != name) {
                    continue;
                }

                return c;
            }

            return null;
        }
        
        public static T RequireComponentInChildren<T>(this Transform transform, string name, bool includeInactive = false)
                where T : Component
        {
            T[] all = transform.GetComponentsInChildren<T>(includeInactive);
            // ReSharper disable once ForCanBeConvertedToForeach
            // ReSharper disable once LoopCanBeConvertedToQuery
            for (int i = 0; i < all.Length; i++) {
                T c = all[i];
                if (c.name != name) {
                    continue;
                }

                return c;
            }

            throw new MissingComponentException($"Component could not be found with name={name}");
        }
    }
}