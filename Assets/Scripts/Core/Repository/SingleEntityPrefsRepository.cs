using System;
using Core.Serialization;
using UnityEngine;

namespace Core.Repository
{
    public abstract class SingleEntityPrefsRepository<TEntity> : ISingleEntityRepository<TEntity>
            where TEntity : class
    {
        protected abstract string Key { get; }
        
        private readonly ISerializer _deserializer;
        
        private TEntity? _entity;

        public TEntity? Get()
        {
            if (_entity != null) {
                return _entity;
            }

            string data = PlayerPrefs.GetString(Key);
            _entity = string.IsNullOrEmpty(data) ? null : _deserializer.Deserialize<TEntity>(data);
            return _entity;
        }

        public TEntity Require()
        {
            TEntity? value = Get();
            if (value == null) {
                throw new NullReferenceException($"Object not found. Type={typeof(TEntity)}");
            }
            return _entity!;
        }

        public bool Exists()
        {
            return PlayerPrefs.HasKey(Key);
        }

        public void Save(TEntity entity)
        {
            _entity = entity;
            string data = _deserializer.Serialize(entity);
            PlayerPrefs.SetString(Key, data);
            PlayerPrefs.Save();
        }

        protected SingleEntityPrefsRepository(ISerializer deserializer)
        {
            _deserializer = deserializer;
        }
    }
}