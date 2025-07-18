namespace Core.Repository
{
    public interface ISingleEntityRepository<TEntity>
    {
        TEntity? Get();

        TEntity Require();

        void Save(TEntity entity);
    }
}