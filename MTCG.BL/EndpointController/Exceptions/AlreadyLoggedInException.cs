using MTCG.Models;

namespace MTCG.BL.EndpointController.Exceptions
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
