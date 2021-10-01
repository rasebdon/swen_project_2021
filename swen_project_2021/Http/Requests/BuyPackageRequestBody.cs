using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG.Http.Requests
{
    class BuyPackageRequestBody : JsonRequestBody<BuyPackageRequestBody>
    {
        public string PackageName { get; }
        public ushort PackageAmount { get; }

        public BuyPackageRequestBody(string packageName, ushort packageAmount)
        {
            PackageName = packageName;
            PackageAmount = packageAmount;
        }
    }
}
