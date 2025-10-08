using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

namespace Core.Descriptors.Descriptor
{
    public class Descriptor<TKey, TModel> : ScriptableObject
    {
        [Searchable]
        [SerializeField]
        private SerializedDictionary<TKey, TModel> _values = new();

        public SerializedDictionary<TKey, TModel> Values => _values;

        public TModel Require(TKey key)
        {
            TModel? model = Get(key);
            if (model == null) {
                throw new KeyNotFoundException($"Descriptor not found with key:{key}");
            }

            return model;
        }

        public TModel? Get(TKey key)
        {
            return Values.GetValueOrDefault(key);
        }

#if UNITY_EDITOR
        public void SetValues(SerializedDictionary<TKey, TModel> values)
        {
            _values = values;
        }
#endif
    }
}