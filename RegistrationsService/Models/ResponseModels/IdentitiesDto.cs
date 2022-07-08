using System.Collections.Generic;

namespace RegistrationsService.Models.ResponseModels
{
    public class IdentitiesDto
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public RolesDto Roles { get; set; }
    }
}
