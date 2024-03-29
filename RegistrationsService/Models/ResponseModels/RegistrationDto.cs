﻿namespace RegistrationsService.Models.ResponseModel
{
    public class RegistrationDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }

        #nullable enable

        public string? PhoneNumber { get; set; }
        public string? InvitationId { get; set; }
        public bool? IsDashboard { get; set; }
    }
}
