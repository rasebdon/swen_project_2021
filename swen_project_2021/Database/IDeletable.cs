using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.DAL
{
    public interface IDeletable
    {
        void Delete(Database database);
    }
}
