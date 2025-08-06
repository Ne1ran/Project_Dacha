using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Triggers;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Game.Utils
{
    [PublicAPI]
    public static class Utils
    {
        #region Collections

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (T? element in enumerable) {
                action.Invoke(element);
            }
        }

        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<int, T> action)
        {
            int i = 0;
            foreach (T? element in enumerable) {
                action.Invoke(i, element);
                i++;
            }
        }

        public static bool IsEmpty<T>(this IList<T> collection)
        {
            return collection.Count == 0;
        }

        public static T? GetStruct<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
                where T : struct
        {
            return collection.Where(predicate).Cast<T?>().FirstOrDefault();
        }

        public static bool ContainsStruct<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
                where T : struct
        {
            return GetStruct(collection, predicate) != null;
        }

        public static bool Contains<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
                where T : class
        {
            return collection.FirstOrDefault(predicate) != null;
        }

        public static bool Contains<T>(this IReadOnlyList<T> collection, T itemToSearch)
                where T : IEquatable<T>
        {
            for (int i = 0; i < collection.Count; i++) {
                if (collection[i].Equals(itemToSearch)) {
                    return true;
                }
            }

            return false;
        }

        public static object? Get(this IReadOnlyDictionary<string, object> dictionary, string key)
        {
            dictionary.TryGetValue(key, out object? result);
            return result;
        }

        public static object Require(this IReadOnlyDictionary<string, object> dictionary, string key)
        {
            object? result = Get(dictionary, key);

            if (result == null) {
                throw new NullReferenceException($"Entry with key '{key}' not found in dictionary");
            }

            return result;
        }

        public static T Require<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            try {
                return collection.First(predicate);
            } catch (InvalidOperationException) {
                throw new NullReferenceException($"Trying to require an item from collection, but it doesn't exist!");
            }
        }

        public static T Require<T>(this IReadOnlyDictionary<string, T> dictionary, string key)
        {
            if (!dictionary.TryGetValue(key, out T? result)) {
                throw new NullReferenceException($"Entry with key {key} not found in dictionary");
            }

            return result;
        }

        public static TValue Require<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key)
        {
            if (!dictionary.TryGetValue(key, out TValue? result)) {
                throw new NullReferenceException($"Entry with key {key} not found in dictionary");
            }

            return result!;
        }

        public static int IndexOf<T>(this IReadOnlyList<T> collection, T item)
                where T : class
        {
            for (int i = 0; i < collection.Count; i++) {
                if (collection[i].Equals(item)) {
                    return i;
                }
            }

            return -1;
        }

        public static TValue? GetOrDefault<TKey, TValue>(this Dictionary<TKey, TValue> target, TKey key, TValue? defaultValue = null)
                where TValue : class
        {
            return target.ContainsKey(key) ? target[key] : defaultValue;
        }

        public static string ToLowerFirstString(this Enum enumElement)
        {
            return enumElement.ToString().ToLowerFirst();
        }

        public static string ToUpperFirstString(this Enum enumElement)
        {
            return enumElement.ToString().ToUpperFirst();
        }

        #endregion

        #region GameObject

        public static T RequireComponentInChild<T>(this GameObject gameObject, string childName)
                where T : Component
        {
            return gameObject.transform.RequireComponentInChild<T>(childName);
        }

        public static T? GetComponentInChildrenOnly<T>(this GameObject gameObject)
                where T : Component
        {
            T? component = null;
            foreach (Transform child in gameObject.transform) {
                component = child.GetComponent<T>();
                if (component != null) {
                    break;
                } else {
                    component = GetComponentInChildrenOnly<T>(child.gameObject);
                    break;
                }
            }
            return component != null ? component : null;
        }

        public static T RequireComponentInChildrenOnly<T>(this GameObject gameObject)
                where T : Component
        {
            foreach (Transform child in gameObject.transform) {
                T found = child.GetComponentInChildren<T>();

                if (found) {
                    return found!;
                }
            }

            throw new NullReferenceException($"Component {typeof(T).Name} not found in children of {gameObject.name}");
        }

        public static T RequireComponentInChildren<T>(this GameObject gameObject, bool includeInactive = true)
        {
            return gameObject.GetComponentInChildren<T>(includeInactive)
                   ?? throw new NullReferenceException($"Component {typeof(T).Name} not found in children of object {gameObject.name}");
        }

        public static Component RequireComponent(this GameObject gameObject, Type componentType)
        {
            return gameObject.GetComponent(componentType)
                   ?? throw new NullReferenceException($"Component {componentType.Name} not found in object {gameObject.name}");
        }

        public static T RequireComponent<T>(this GameObject gameObject)
                where T : class
        {
            return (RequireComponent(gameObject, typeof(T)) as T)!;
        }

        public static void DestroyObject(this GameObject gameObject)
        {
            Object.Destroy(gameObject);
        }

        public static UniTask DestroyObjectAsync(this GameObject gameObject)
        {
            AsyncDestroyTrigger trigger = gameObject.GetAsyncDestroyTrigger();
            Object.Destroy(gameObject);
            return trigger.OnDestroyAsync();
        }

        public static void EnsureDestroyTriggerAdded(this GameObject gameObject)
        {
            gameObject.GetAsyncDestroyTrigger();
        }

        #endregion

        #region Transform

        public static Transform? GetChildRecursive(this Transform transform, string name)
        {
            foreach (Transform child in transform) {
                if (child.name == name) {
                    return child;
                }

                Transform? found = GetChildRecursive(child, name);
                if (found != null) {
                    return found;
                }
            }

            return null;
        }

        public static Transform RequireChildRecursive(this Transform transform, string name)
        {
            return GetChildRecursive(transform, name) ?? throw new NullReferenceException();
        }

        public static T? GetComponentInChild<T>(this Transform transform, string childName, bool includeInactive = true)
                where T : Component
        {
            foreach (Transform child in transform) {
                if (!includeInactive && !child.gameObject.activeSelf) {
                    continue;
                }

                if (child.name == childName) {
                    return child.GetComponent<T>();
                }

                T? found = GetComponentInChild<T>(child, childName, includeInactive);
                if (found) {
                    return found;
                }
            }

            return null;
        }

        public static T? GetComponentInChildrenNotRecursive<T>(this Transform transform)
                where T : Component
        {
            foreach (Transform child in transform) {
                T component = child.GetComponent<T>();

                if (component) {
                    return component;
                }
            }

            return null;
        }

        public static T? GetComponentInChildrenNotRecursive<T>(this Transform transform, string childName)
                where T : Component
        {
            foreach (Transform child in transform) {
                if (child.name == childName) {
                    return child.GetComponent<T>();
                }
            }

            return null;
        }

        public static List<T> GetComponentsInChildrenNotRecursive<T>(this Transform transform)
                where T : Component
        {
            List<T> result = new();
            foreach (Transform child in transform) {
                T component = child.GetComponent<T>();

                if (component) {
                    result.Add(component);
                }
            }

            return result;
        }

        public static RectTransform ToRect(this Transform transform)
        {
            return (RectTransform) transform;
        }

        public static void AddChild(this Transform transform, Transform child)
        {
            child.SetParent(transform, false);
        }

        public static void AddChild(this Transform transform, Component child)
        {
            child.transform.SetParent(transform, false);
        }

        public static void AddChild(this Transform transform, GameObject child)
        {
            child.transform.SetParent(transform, false);
        }

        public static void RemoveChild(this Transform transform, Component child)
        {
            child.transform.SetParent(null, false);
        }

        public static void ClearChildren(this Transform transform)
        {
            foreach (Transform child in transform) {
                Object.Destroy(child.gameObject);
            }
        }

        public static void LookAt2D(this Transform transform, Transform target)
        {
            transform.LookAt2D(target.position);
        }

        public static void LookAt2D(this Transform transform, Vector3 target)
        {
            Vector3 direction = MathUtils.Direction(transform.position, target);
            direction.y = transform.forward.y;
            if (direction == Vector3.zero) {
                return;
            }
            transform.forward = direction;
        }

        #endregion

        #region Component

        public static T? GetComponentInChildrenOnly<T>(this Component component)
                where T : Component
        {
            return GetComponentInChildrenOnly<T>(component.gameObject);
        }

        public static T RequireComponentInChildrenOnly<T>(this Component component)
                where T : Component
        {
            return RequireComponentInChildrenOnly<T>(component.gameObject);
        }

        public static T RequireComponentInChild<T>(this Component component, string childName)
                where T : Component
        {
            return GetComponentInChild<T>(component, childName)
                   ?? throw new NullReferenceException($"Component {typeof(T).Name} not found on child {childName} of object {component.name}");
        }

        public static T? GetComponentInChild<T>(this Component component, string childName)
                where T : Component
        {
            return GetComponentInChild<T>(component.transform, childName);
        }

        public static List<T> GetComponentsInAllChild<T>(this Component component, string childName)
                where T : Component
        {
            List<T> components = component.GetComponentsInChildren<T>().ToList();
            return components.FindAll(comp => comp.name == childName);
        }

        public static T? GetComponentInSiblings<T>(this Component component)
                where T : Component
        {
            if (component.transform.parent.AsNullable() == null) {
                return null;
            }

            foreach (Transform child in component.transform.parent) {
                if (ReferenceEquals(child, component.transform)) {
                    continue;
                }

                T found = child.GetComponent<T>();

                if (found.AsNullable() != null) {
                    return found;
                }
            }

            return null;
        }

        public static T RequireComponentInSiblings<T>(this Component component)
                where T : Component
        {
            return GetComponentInSiblings<T>(component)
                   ?? throw new NullReferenceException($"Component {typeof(T).Name} not found in siblings of object {component.name}");
        }

        public static T GetOrAddComponent<T>(this Component component)
                where T : Component
        {
            return component.GetComponent<T>() ?? component.gameObject.AddComponent<T>();
        }

        public static T RequireComponent<T>(this Component component)
                where T : class
        {
            return RequireComponent<T>(component.gameObject);
        }

        public static T RequireComponentInChildren<T>(this Component component, bool includeInactive = true)
        {
            return component.GetComponentInChildren<T>(includeInactive)
                   ?? throw new NullReferenceException($"Component {typeof(T).Name} not found in children of object {component.name}");
        }

        public static T RequireComponentInChildren<T>(this Component component, string name, bool includeInactive = true)
                where T : Component
        {
            T[] children = component.GetComponentsInChildren<T>(includeInactive);
            for (int i = 0; i < children.Length; i++) {
                T c = children[i];
                if (c.name != name) {
                    continue;
                }

                return c;
            }

            throw new NullReferenceException($"Object not found in children with name ={component.name}");
        }

        public static T RequireComponentInParent<T>(this Component component)
                where T : class
        {
            Transform? parent = component.transform.parent;

            if (!parent) {
                throw new NullReferenceException($"Object {component.name} don't has parent");
            }

            return parent.RequireComponent<T>();
        }

        public static Transform RequireChildRecursive(this Component component, string name)
        {
            return component.transform.GetChildRecursive(name)
                   ?? throw new NullReferenceException($"Child '{name}' not found in object '{component.name}'");
        }

        public static RectTransform RectTransform(this Component component)
        {
            return component.transform.ToRect();
        }

        public static void Toggle(this Component component)
        {
            component.SetActive(!component.gameObject.activeSelf);
        }

        public static void SetActive(this Component component, bool active)
        {
            component.gameObject.SetActive(active);
        }

        public static void DestroyObject(this Component component)
        {
            Object.Destroy(component.gameObject);
        }

        public static UniTask DestroyObjectAsync(this Component component)
        {
            return DestroyObjectAsync(component.gameObject);
        }

        public static void EnsureDestroyTriggerAdded(this Component component)
        {
            EnsureDestroyTriggerAdded(component.gameObject);
        }

        public static void CheckSingleComponent<T>(this T component)
                where T : MonoBehaviour
        {
            T[]? components = component.GetComponents<T>();

            if (components.Length > 1) {
                throw new($"There are more than 1 {typeof(T).Name} on {component.name}");
            }
        }

        public static IEnumerable<Transform> EnumerateAllChildren(this Transform transform)
        {
            foreach (Transform child in transform) {
                yield return child;

                foreach (Transform? subchild in child.EnumerateAllChildren()) {
                    yield return subchild;
                }
            }
        }

        #endregion

        #region Vectors

        public static Vector3 To3D(this Vector2 source)
        {
            return new(source.x, 0, source.y);
        }

        #endregion

        #region Other

        public static T? GetSceneRootObjectByName<T>(this Scene scene, string name)
                where T : Component
        {
            return scene.GetSceneRootObjectByName(name)?.GetComponent<T>();
        }

        public static GameObject GetSceneRootObjectByName(this Scene scene, string name)
        {
            GameObject[] rootGameObjects = scene.GetRootGameObjects();
            foreach (GameObject gameObject in rootGameObjects) {
                if (gameObject.name == name) {
                    return gameObject;
                }
            }

            return null;
        }

        public static T GetSceneRootObjectByType<T>(this Scene scene) where T : Component
        {
            GameObject[] rootGameObjects = scene.GetRootGameObjects();
            foreach (GameObject gameObject in rootGameObjects) {
                T component = gameObject.GetComponent<T>();
                if (component != null) {
                    return component;
                }
            }

            return null;
        }

        public static T? AsNullable<T>(this T? reference)
                where T : class
        {
            if (reference == null || reference.Equals(null)) {
                return null;
            }

            if (reference is Object unityObject) {
                return unityObject ? reference : null;
            }

            return reference;
        }

        public static string Capitalize(this string source)
        {
            return char.IsLower(source[0]) ? $"{char.ToUpper(source[0])}{source.Substring(1)}" : source;
        }

        public static string Uncapitalize(this string source)
        {
            return char.IsUpper(source[0]) ? $"{char.ToLower(source[0])}{source.Substring(1)}" : source;
        }

        public static T ParseEnum<T>(string text)
                where T : Enum
        {
            return (T) ParseEnum(text, typeof(T));
        }

        public static object ParseEnum(string text, Type enumType)
        {
            return Enum.Parse(enumType, text, true);
        }

        public static void AddSingleListener(this Button button, UnityAction? action)
        {
            button.onClick.RemoveAllListeners();

            if (action != null) {
                button.onClick.AddListener(action);
            }
        }

        public static void ClearListeners(this Button button)
        {
            button.onClick.RemoveAllListeners();
        }

        public static bool Contains(this string source, string value, StringComparison comparison)
        {
            int index = source.IndexOf(value, comparison);
            return index != -1;
        }

        public static TComponent NewObject<TComponent>(string name, Transform? parent = null)
                where TComponent : Component
        {
            GameObject obj = new(name);
            parent.AsNullable()?.AddChild(obj);
            return obj.AddComponent<TComponent>();
        }

        public static TimeSpan ToSeconds(this float seconds)
        {
            return TimeSpan.FromSeconds(seconds);
        }

        public static string ToFormattedString(this TimeSpan duration)
        {
            int days = duration.Days;
            int hours = duration.Hours;
            int minutes = duration.Minutes;
            int seconds = duration.Seconds;
            int milliseconds = duration.Milliseconds;

            if (milliseconds > 0) {
                seconds++;
            }

            if (seconds == 60) {
                minutes++;
                seconds = 0;
            }

            if (minutes == 60) {
                hours++;
                minutes = 0;
            }

            if (hours == 24) {
                days++;
                hours = 0;
            }

            return ConvertToString(days, hours, minutes, seconds);
        }

        public static string ToFormattedString(this DateTime duration)
        {
            int days = duration.Day;
            int hours = duration.Hour;
            int minutes = duration.Minute;
            int seconds = duration.Second;
            int milliseconds = duration.Millisecond;

            if (milliseconds > 0) {
                seconds++;
            }

            if (seconds == 60) {
                minutes++;
                seconds = 0;
            }

            if (minutes == 60) {
                hours++;
                minutes = 0;
            }

            if (hours == 24) {
                days++;
                hours = 0;
            }

            return ConvertToString(days, hours, minutes, seconds);
        }

        private static string ConvertToString(int days, int hours, int minutes, int seconds)
        {
            StringBuilder result = new();
            int appended = 0;

            if (days > 0) {
                result.Append($"{days}d ");
                appended++;
            }

            if (hours > 0) {
                result.Append($"{hours}h ");
                appended++;
            }

            if (minutes > 0 && appended < 2 && days == 0) {
                result.Append($"{minutes}m ");
                appended++;
            }

            if (seconds > 0 && appended < 2 && hours == 0) {
                result.Append(seconds < 10 ? $"0{seconds}s" : $"{seconds}s");
                appended++;
            }

            if (appended == 1) {
                result.Append(HandleAdditionalAppend(days, hours, minutes, seconds));
            }

            return result.ToString();
        }

        private static string HandleAdditionalAppend(int days, int hours, int minutes, int seconds)
        {
            if (days > 0 && hours == 0) {
                return ("00h ");
            }

            if (hours > 0 && minutes == 0) {
                return ("00m ");
            }

            if (minutes > 0 && seconds == 0) {
                return ("00s ");
            }

            return string.Empty;
        }

        #endregion
    }
}