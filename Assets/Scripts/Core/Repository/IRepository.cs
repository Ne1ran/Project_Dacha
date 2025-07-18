using System;
using System.Collections.Generic;

namespace Core.Repository
{
    public interface IRepository<in TKey, TEntity>
            where TKey : IEquatable<TKey>
    {
        TEntity? Get(TKey key);

        TEntity Require(TKey key);

        IReadOnlyList<TEntity> GetAll();

        bool Exists(TKey key);

        void Delete(TKey key);

        void Save(TKey key, TEntity entity);

        void Clear();
    }
}