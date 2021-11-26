using System.Collections;

namespace MTCG.DAL.Repositories
{
    public interface IRepository<T>
    {
        IEnumerable GetAll();
        T? GetById(Guid id);
        bool Insert(T entity);
        bool Update(T entityOld, T entityNew);
        bool Delete(T entity);
    }
}
