using System.Collections.Generic;

namespace MTCG.Interfaces
{
    public interface IDeletable<T>
    {
        bool Delete(T item);
    }
}
