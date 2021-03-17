using System;
using RegistrationsService.Abstraction;
using RegistrationsService.Models;
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

        [HttpGet]
        [Route("registrations/dashboard-app")]
        public IActionResult RegisterDashboards()
        {
            try
            {
                _registrationsRepository.RegisterDashboard();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, CommonMessage.ExceptionMessage + ex.Message);
            }

            return StatusCode(StatusCodes.Status200OK, "");
        }
    }
}
