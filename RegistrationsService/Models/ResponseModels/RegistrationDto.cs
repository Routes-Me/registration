namespace RegistrationsService.Models.ResponseModel
{
    public class RegistrationDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }

        #nullable enable

        public Phone? phone { get; set; }
        public string? PhoneNumber { get; set; }
        public string? avatarUrl { get; set; }
        public string? invitationToken { get; set; }
        public string? VehicleId { get; set; }
        public string? InvitationId { get; set; }
        public string? InstitutionId { get; set; }
        public bool? IsDashboard { get; set; }
    }

    public class Phone
    {
        public string? number { get; set; }
        public string? verificationToken { get; set; }
    }

}
