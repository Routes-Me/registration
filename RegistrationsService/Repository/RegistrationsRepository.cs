using RegistrationsService.Abstraction;
using RegistrationsService.Models;
using RegistrationsService.Models.DBModels;
using RegistrationsService.Models.ResponseModel;
using RegistrationsService.Models.Common;
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Obfuscation;
using RestSharp;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace RegistrationsService.Repository
{
    public class RegistrationsRepository : IRegistrationsRepository
    {
        private readonly RegistrationsServiceContext _context;
        private readonly AppSettings _appSettings;
        private readonly Dependencies _dependencies;

        public RegistrationsRepository(IOptions<AppSettings> appSettings, RegistrationsServiceContext context, IOptions<Dependencies> dependencies)
        {
            _appSettings = appSettings.Value;
            _context = context;
            _dependencies = dependencies.Value;
        }

        public async Task RegisterScreenApp(RegistrationDto registrationDto)
        {
            await Register(registrationDto, "screen", "user");
        }

        public async Task RegisterRoutesPayApp(RegistrationDto registrationDto)
        {
            await Register(registrationDto, "routes pay", "user");
        }

        public async Task RegisterDriverApp(RegistrationDto registrationDto)
        {
            await Register(registrationDto, "driver", "user");
        }

        public async Task RegisterBusValidators(RegistrationDto registrationDto)
        {
            await Register(registrationDto, "bus-validator", "user");
        }

        public async Task RegisterDashboard(RegistrationDto registrationDto)
        {
            if (string.IsNullOrEmpty(registrationDto.Role))
                throw new ArgumentNullException(CommonMessage.RoleMissed);
            await Register(registrationDto, "dashboard", registrationDto.Role);
        }

        private Task Register(RegistrationDto registrationDto, string application, string privilege)
        {
            if (registrationDto == null || string.IsNullOrEmpty(registrationDto.PhoneNumber) || string.IsNullOrEmpty(registrationDto.Email) || string.IsNullOrEmpty(registrationDto.Password) || string.IsNullOrEmpty(registrationDto.Name))
                throw new ArgumentNullException(CommonMessage.PassValidData);

            UsersDto userDto = new UsersDto
            {
                Name = registrationDto.Name,
                PhoneNumber = registrationDto.PhoneNumber
            };
            IRestResponse postedUserResponse = PostAPI(_dependencies.UsersUrl, userDto);
            var userData = JsonConvert.DeserializeObject<UserData>(postedUserResponse.Content);

            IdentitiesDto identityDto = new IdentitiesDto
            {
                UserId = userData.UserId,
                Email = registrationDto.Email,
                PhoneNumber = registrationDto.PhoneNumber,
                Password = registrationDto.Password,
                Roles = new RolesDto { Application = application, Privilege = privilege}
            };
            try
            {
                PostAPI(_dependencies.IdentitiesUrl, identityDto);
            }
            catch (Exception)
            {
                DeleteAPI(_appSettings.Host + _dependencies.UsersUrl + userData.UserId);
                throw;
            }
            return Task.CompletedTask;
        }

        private IRestResponse PostAPI(string url, dynamic objectToSend)
        {
            var client = new RestClient(_appSettings.Host + url);
            var request = new RestRequest(Method.POST);
            string jsonToSend = JsonConvert.SerializeObject(objectToSend);
            request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;
            IRestResponse response = client.Execute(request);
            if (response.StatusCode != HttpStatusCode.Created)
                throw new Exception(response.Content);
            return response;
        }

        private IRestResponse DeleteAPI(string url)
        {
            var client = new RestClient(url);
            var request = new RestRequest(Method.DELETE);
            return client.Execute(request);
        }
    }
}
