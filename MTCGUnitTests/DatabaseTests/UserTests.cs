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

            try
            {
                MTCG.Http.HttpAuthorization auth = new("Basic", user.SessionToken);

                // Check if session key is correctly set
                Assert.IsNotNull(UserController.Instance.Authenticate(auth), "The session key was incorrectly set!");
                // Check if the user was inserted correctly
                Assert.IsNotNull(UserController.Instance.Select(user.ID), "User with the inserted id was not found!");
            }
            finally
            {
                // Clear session
                UserController.Instance.LoggedInUsers.Clear();

                // Delete created user
                UserController.Instance.Delete(user.ID);
            }
        }

        [TestMethod]
        public void UpdateUserELO()
        {
            User user = null;

            try
            {
                user = UserController.Instance.Register("dummy", "1234");

                int oldElo = user.ELO;
                user.ELO = ushort.MaxValue;

                bool success = UserController.Instance.UpdateUserELO(user);

                user = UserController.Instance.Select(user.ID);

                Assert.IsTrue(success);
                Assert.AreEqual(ushort.MaxValue, user.ELO);
            }
            finally
            {
                if(user != null)
                    UserController.Instance.Delete(user);
            }
        }

        [TestMethod]
        public void UpdateUserCoins()
        {
            User user = null;

            try
            {
                user = UserController.Instance.Register("dummy", "1234");

                uint oldCoins = user.Coins;
                user.Coins = ushort.MaxValue;

                bool success = UserController.Instance.UpdateUserCoins(user);

                user = UserController.Instance.Select(user.ID);

                Assert.IsTrue(success);
                Assert.AreEqual(ushort.MaxValue, user.Coins);
            }
            finally
            {
                if (user != null)
                    UserController.Instance.Delete(user);
            }
        }
    }
}
