namespace RegistrationsService.Models.ResponseModels
{
    public class DriverDto
    {
        public string UserId { get; set; }
        public string InvitationId { get; set; }
        public string Name { get; set; }
        public string AvatarUrl { get; set; }
        public string VehicleId { get; set; }
        public string PhoneNumber { get; set; }
        public string VerificationToken { get; set; }

    }

    public class DriverRegistrationResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
