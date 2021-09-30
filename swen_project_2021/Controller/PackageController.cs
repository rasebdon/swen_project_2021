using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTCG.Models;
using MTCG.Http;


namespace MTCG.Controller
{
    /// <summary>
    /// This controller manages the package related functions
    /// </summary>
    class PackageController : Singleton<PackageController>
    {

        void AddPackage(Package package, HttpAuthorization auth)
        {
            // Check if auth is related to an admin
            UserController.Instance.IsAdmin(auth);

            // Generate card instances
        }

        internal Package GetPackage(uint packageId)
        {
            throw new NotImplementedException();
        }
    }
}
