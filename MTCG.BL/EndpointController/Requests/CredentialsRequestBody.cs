namespace MTCG.BL.Requests
{
    class CredentialsRequestBody : JsonRequestBody<CredentialsRequestBody>
    {
        public string Username { get; }
        public string Password { get; }

        public CredentialsRequestBody(string username, string password)
        {
            Username = username;
            Password = password;
        }
    }
}