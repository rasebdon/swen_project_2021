using System;

namespace MTCG.Interfaces
{
    public interface IAsyncSelectable<T>
    {
        T SelectAsync(Guid id);
    }
}
