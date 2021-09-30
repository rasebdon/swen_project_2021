using MTCG.Models;
using System;

namespace MTCG.Controller.Exceptions
{
    [Serializable]
    class AlreadyLoggedInException : Exception
    {
        private readonly User _user;

        public AlreadyLoggedInException(User user) : base($"The user {user.Username} is already logged in!")
        {
            _user = user;
        }
    }
}
