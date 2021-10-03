using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Interfaces
{
    interface IUpdateable<T>
    {
        public bool Update(T oldItem, T newItem);
    }
}
