namespace MTCG.DAL.Repositories
{
    public interface IRepository<T>
    {
        IEnumerable<T> GetAll();
        T? GetById(Guid id);
        bool Insert(T entity);
        bool Update(T entity);
        bool Delete(Guid id);
    }
}
