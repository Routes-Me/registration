using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RegistrationsService.Models.ResponseModel
{
    public class RegistrationDto
    {
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        [Range(typeof(string), "manager", "officer", ErrorMessage = "Invalid value for role")]
        [AllowNull]
        public string Role { get; set; }
        [AllowNull]
        public string InstitutionId { get; set; }
    }
}
