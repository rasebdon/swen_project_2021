using System;

namespace MTCG.Interfaces
{
    public interface ISelectable<T>
    {
        T Select(Guid id);
    }
}
