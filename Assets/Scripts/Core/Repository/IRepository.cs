using System;
using System.Collections.Generic;

namespace Core.Repository
{
    public interface IRepository<TKey, TEntity>
            where TKey : IEquatable<TKey>
    {
        TEntity? Get(TKey key);

        TEntity Require(TKey key);

        Dictionary<TKey, TEntity> GetAll();

        bool Exists(TKey key);

        void Delete(TKey key);

        void Save(TKey key, TEntity entity);

        void SaveAll(Dictionary<TKey, TEntity> dictionary);

        void Clear();
    }
}