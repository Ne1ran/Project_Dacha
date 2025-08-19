using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Serialization;

namespace Core.Descriptors.Descriptor
{
    // todo neiran finalize and use if needed
    public class Descriptor<TKey, TModel> : SerializedScriptableObject
    {
        [OdinSerialize]
        public Dictionary<TKey, TModel> Items { get; set; } = new();

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