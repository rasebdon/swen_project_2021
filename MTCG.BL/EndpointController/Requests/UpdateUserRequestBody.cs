namespace MTCG.BL.EndpointController.Requests
{
    public class UpdateUserRequestBody : JsonRequestBody<CredentialsRequestBody>
    {
        public string Username { get; }
        public string? Bio { get; }
        public string? Image { get; }

        public UpdateUserRequestBody(string username, string? bio, string? image)
        {
            Username = username;
            Bio = bio;
            Image = image;
        }
    }
}
