﻿using System.Collections.Generic;

namespace MTCG.Interfaces
{
    public interface IInsertable<T>
    {
        bool Insert(T item);
    }
}
