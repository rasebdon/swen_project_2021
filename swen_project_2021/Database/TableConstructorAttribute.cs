using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DAL
{
    [System.AttributeUsage(AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
    sealed class TableConstructorAttribute : Attribute
    {
        public TableConstructorAttribute()
        {
        }
    }
}
