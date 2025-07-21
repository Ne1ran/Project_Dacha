using System;
using System.Collections.Generic;
using System.Linq;

namespace Core.Repository
{
    public class MemoryRepository<TKey, TEntity> : IRepository<TKey, TEntity>
            where TKey : IEquatable<TKey>
    {
        private readonly Dictionary<TKey, TEntity> _map = new();

        public TEntity? Get(TKey key)
        {
            return _map.GetValueOrDefault(key);
        }

        public TEntity Require(TKey key)
        {
            return _map[key];
        }

        public IReadOnlyList<TEntity> GetAll()
        {
            return _map.Values.ToList();
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

        public void Clear()
        {
            _map.Clear();
        }
    }
}