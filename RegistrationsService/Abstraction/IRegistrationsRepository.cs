using RegistrationsService.Models.ResponseModel;
using System.Threading.Tasks;

namespace RegistrationsService.Abstraction
{
    public interface IRegistrationsRepository
    {
        Task RegisterScreenApp(RegistrationDto registrationDto);
        Task RegisterDashboard(RegistrationDto registrationDto);
        Task RegisterRoutesPayApp(RegistrationDto registrationDto);
        Task RegisterDriverApp(RegistrationDto registrationDto);
    }
}