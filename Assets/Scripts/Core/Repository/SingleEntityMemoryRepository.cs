namespace Core.Repository
{
    public class SingleEntityMemoryRepository<TEntity> : ISingleEntityRepository<TEntity>
    {
        private TEntity? _entity;

        public TEntity? Get()
        {
            return _entity;
        }

        public TEntity Require()
        {
            return _entity!;
        }

        public void Save(TEntity entity)
        {
            _entity = entity;
        }

        public void Clear()
        {
            _entity = default!;
        }
    }
}