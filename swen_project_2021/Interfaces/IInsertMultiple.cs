using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Interfaces
{
    interface IInsertMultiple<T>
    {
        bool Insert(List<T> items);
    }
}
