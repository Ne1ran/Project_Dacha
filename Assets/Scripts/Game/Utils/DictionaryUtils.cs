using System.Collections.Generic;
using UnityEngine;

namespace Game.Utils
{
    public static class DictionaryUtils
    {
        public static void AddOrSum<T>(this Dictionary<T, int> thisDictionary, Dictionary<T, int> dictionary)
        {
            foreach ((T type, int value) in dictionary) {
                AddOrSum(thisDictionary, type, value);
            }
        }
        
        public static void AddOrSum<T>(this Dictionary<T, float> thisDictionary, IReadOnlyDictionary<T, float> dictionary)
        {
            foreach ((T type, float value) in dictionary) {
                AddOrSum(thisDictionary, type, value);
            }
        }
        
        public static void AddOrSum<T>(this Dictionary<T, int> thisDictionary, IReadOnlyDictionary<T, int> dictionary)
        {
            foreach ((T type, int value) in dictionary) {
                AddOrSum(thisDictionary, type, value);
            }
        }
        
        public static void AddOrSum<T>(this Dictionary<T, float> thisDictionary, Dictionary<T, float> dictionary)
        {
            foreach ((T type, float value) in dictionary) {
                AddOrSum(thisDictionary, type, value);
            }
        }
        
        public static void AddOrSum<T>(this Dictionary<T, int> thisDictionary, IReadOnlyDictionary<T, float> dictionary)
        {
            foreach ((T type, float value) in dictionary) {
                AddOrSum(thisDictionary, type, Mathf.CeilToInt(value));
            }
        }
        
        public static void AddOrSum<T>(this Dictionary<T, int> dictionary, T type, int value)
        {
            if (!dictionary.TryAdd(type, value)) {
                dictionary[type] += value;
            }
        }
        
        public static void AddOrSum<T>(this Dictionary<T, float> dictionary, T type, float value)
        {
            if (!dictionary.TryAdd(type, value)) {
                dictionary[type] += value;
            }
        }
        
        public static void SubtractIfPossible<T>(this Dictionary<T, int> thisDictionary, IReadOnlyDictionary<T, int> dictionary)
        {
            foreach ((T type, int value) in dictionary) {
                SubtractIfPossible(thisDictionary, type, value);
            }
        }
        
        public static void SubtractIfPossible<T>(this Dictionary<T, int> dictionary, T type, int value)
        {
            if (dictionary.ContainsKey(type)) {
                dictionary[type] -= value;
            } else {
                throw new($"The key was not found when subtracting. key={type}");
            }
        }
    }
}