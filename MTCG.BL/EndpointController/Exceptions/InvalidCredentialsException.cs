namespace MTCG.BL.EndpointController.Exceptions
{
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException(string username) :
            base($"Invalid credentials for user {username} supplied!")
        { }
    }
}
