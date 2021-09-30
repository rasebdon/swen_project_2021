using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MTCG.Models;
using MTCG.Controller;
using MTCG;

namespace MTCGUnitTests.DatabaseTests
{
    [TestClass]
    public class UserTests
    {
        [TestMethod]
        public void RegisterAndLoginUser()
        {
            UserController.Instance.Register("dummy", "1234");
            User user = UserController.Instance.Login("dummy", "1234");

            MTCG.Http.HttpAuthorization auth = new("Basic", user.SessionToken);

            try
            {
                // Check if session key is correctly set
                Assert.IsNotNull(UserController.Instance.Authenticate(auth), "The session key was incorrectly set!");
                // Check if the user was inserted correctly
                Assert.IsNotNull(UserController.Instance.GetUser(user.ID), "User with the inserted id was not found!");
            }
            finally
            {
                // Clear session
                UserController.Instance.LoggedInUsers.Clear();

                // Delete created user
                UserController.Instance.DeleteUser(user.ID);
            }
        }
    }
}
