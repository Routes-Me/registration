namespace RegistrationsService.Models
{
    public class SuccessResponse
    {
        public string message { get; set; }
    }

    public class ErrorResponse
    {
        public string error { get; set; }
    }

    public class UsersResponse
    {
        public string UserId { get; set; }
    }

    public class IdentitiesResponse
    {
        public string IdentityId { get; set; }
    }
}
