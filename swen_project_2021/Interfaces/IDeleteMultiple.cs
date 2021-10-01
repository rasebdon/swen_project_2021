using System.Collections.Generic;

namespace MTCG.Interfaces
{
    public interface IDeleteMultiple<T>
    {
        bool Delete(List<T> items);
    }
}
