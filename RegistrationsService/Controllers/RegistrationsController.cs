using System;
using System.Threading.Tasks;
using RegistrationsService.Abstraction;
using RegistrationsService.Models;
using RegistrationsService.Models.ResponseModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace RegistrationsService.Controllers
{
    [ApiController]
    [ApiVersion( "1.0" )]
    [Route("v{version:apiVersion}/")]
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
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, CommonMessage.ExceptionMessage + ex.Message);
            }
            return StatusCode(StatusCodes.Status201Created, CommonMessage.ScreenAppRegistered);
        }

        [HttpPost]
        [Route("registrations/dashboard-app")]
        public async Task<IActionResult> RegisterDashboard(RegistrationDto registrationDto)
        {
            try
            {
                await _registrationsRepository.RegisterDashboard(registrationDto);
            }
            catch (ArgumentNullException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, CommonMessage.ExceptionMessage + ex.Message);
            }
            return StatusCode(StatusCodes.Status201Created, CommonMessage.DashboardRegistered);
        }

        [HttpPost]
        [Route("registrations/routes-pay-app")]
        public async Task<IActionResult> RegisterRoutesPayApp(RegistrationDto registrationDto)
        {
            try
            {
                await _registrationsRepository.RegisterRoutesPayApp(registrationDto);
            }
            catch (ArgumentNullException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, CommonMessage.ExceptionMessage + ex.Message);
            }
            return StatusCode(StatusCodes.Status201Created, CommonMessage.RoutesPayAppRegistered);
        }

        [HttpPost]
        [Route("registrations/driver-app")]
        public async Task<IActionResult> RegisterDriverApp(RegistrationDto registrationDto)
        {
            try
            {
                await _registrationsRepository.RegisterDriverApp(registrationDto);
            }
            catch (ArgumentNullException ex)
            {
                return StatusCode(StatusCodes.Status422UnprocessableEntity, ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, CommonMessage.ExceptionMessage + ex.Message);
            }
            return StatusCode(StatusCodes.Status201Created, CommonMessage.DriverAppRegistered);
        }
    }
}
