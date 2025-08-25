using System;
using System.Collections.Generic;

namespace Core.Repository
{
    public class MemoryRepository<TKey, TEntity> : IRepository<TKey, TEntity>
            where TKey : IEquatable<TKey>
    {
        private Dictionary<TKey, TEntity> _map = new();

        public TEntity? Get(TKey key)
        {
            return _map.GetValueOrDefault(key);
        }

        public TEntity Require(TKey key)
        {
            return _map[key];
        }

        public Dictionary<TKey, TEntity> GetAll()
        {
            return _map;
        }

        public bool Exists(TKey key)
        {
            return _map.ContainsKey(key);
        }

        public void Delete(TKey key)
        {
            _map.Remove(key);
        }

        public void Save(TKey key, TEntity entity)
        {
            _map.Add(key, entity);
        }

        public void SaveAll(Dictionary<TKey, TEntity> values)
        {
            _map = values;
        }

        public void Clear()
        {
            _map.Clear();
        }
    }
}