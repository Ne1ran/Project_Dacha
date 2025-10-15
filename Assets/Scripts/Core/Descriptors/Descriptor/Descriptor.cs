using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace Core.Descriptors.Descriptor
{
    public class Descriptor<TKey, TModel> : ScriptableObject
    {
        [Searchable]
        [SerializeField]
        private SerializedDictionary<TKey, TModel> _items = new();

        public SerializedDictionary<TKey, TModel> Items => _items;

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
            return Items.GetValueOrDefault(key);
        }
    }
}