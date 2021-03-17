using RegistrationsService.Models.ResponseModel;

namespace RegistrationsService.Abstraction
{
    public interface IRegistrationsRepository
    {
        void RegisterScreenApp(RegistrationDto registrationDto);
    }
}