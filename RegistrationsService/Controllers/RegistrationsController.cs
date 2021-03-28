using System;
using System.Threading.Tasks;
using RegistrationsService.Abstraction;
using RegistrationsService.Models;
using RegistrationsService.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RegistrationsService.Controllers
{
    [Route("v1")]
    [ApiController]
    public class RegistrationsController : ControllerBase
    {
        private readonly IRegistrationsRepository _registrationsRepository;
        public RegistrationsController(IRegistrationsRepository registrationsRepository)
        {
            _registrationsRepository = registrationsRepository;
        }

        [HttpPost]
        [Route("registrations/screen-app")]
        public async Task<IActionResult> RegisterScreenApp(RegistrationDto registrationDto)
        {
            try
            {
                await _registrationsRepository.RegisterScreenApp(registrationDto);
            }
            catch (ArgumentNullException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, CommonMessage.ExceptionMessage + ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, CommonMessage.ExceptionMessage + ex.Message);
            }
            return StatusCode(StatusCodes.Status200OK, CommonMessage.ScreenAppRegistered);
        }
    }
}
