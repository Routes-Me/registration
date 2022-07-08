using RegistrationsService.Models.ResponseModels;
using System.Threading.Tasks;

namespace RegistrationsService.Abstraction
{
    public interface IRegistrationsRepository
    {
        Task RegisterScreenApp(RegistrationDto registrationDto);
        Task RegisterDashboard(RegistrationDto registrationDto);
        Task RegisterRoutesPayApp(RegistrationDto registrationDto);
        Task RegisterDriverApp(RegistrationDto registrationDto);
        Task RegisterBusValidators(RegistrationDto registrationDto);
        Task RegisterEnterprisePromotionApp(RegistrationDto registrationDto);
    }
}