using System;
using RegistrationsService.Abstraction;
using RegistrationsService.Models;
using RegistrationsService.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RegistrationsService.Controllers
{
    [Route("api")]
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
        public IActionResult RegisterScreenApp(RegistrationDto registrationDto)
        {
            try
            {
                _registrationsRepository.RegisterScreenApp(registrationDto);
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
