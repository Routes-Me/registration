using RegistrationsService.Abstraction;
using RegistrationsService.Models;
using RegistrationsService.Models.DBModels;
using RegistrationsService.Models.ResponseModel;
using RegistrationsService.Models.Common;
using System;
using Encryption;
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
        EncryptionClass encryption = new EncryptionClass();

        public RegistrationsRepository(IOptions<AppSettings> appSettings, RegistrationsServiceContext context, IOptions<Dependencies> dependencies)
        {
            _appSettings = appSettings.Value;
            _context = context;
            _dependencies = dependencies.Value;
        }

        public void RegisterScreenApp(RegistrationDto registrationDto)
        {
            
        }

        private void Register(RegistrationDto registrationDto)
        {
            if (registrationDto == null || string.IsNullOrEmpty(registrationDto.PhoneNumber) || string.IsNullOrEmpty(registrationDto.Email) || string.IsNullOrEmpty(registrationDto.Password) || string.IsNullOrEmpty(registrationDto.Name))
                throw new ArgumentNullException(CommonMessage.PassValidData);

            UsersDto userDto = new UsersDto
            {
                Name = registrationDto.Name,
                PhoneNumber = registrationDto.PhoneNumber
            };
            IRestResponse postedUser = PostAPI(_dependencies.UsersUrl, userDto);
            if (postedUser.StatusCode != HttpStatusCode.Created)
                throw new Exception();

            var userData = JsonConvert.DeserializeObject<PostUserData>(postedUser.Content);

            IdentitiesDto identityDto = new IdentitiesDto
            {
                UserId = userData.UserId,
                Email = registrationDto.Email,
                PhoneNumber = registrationDto.PhoneNumber,
                Password = registrationDto.Password,
                // Roles = registrationDto.
            };
            IRestResponse postedIdentity = PostAPI(_dependencies.IdentitiesUrl, identityDto);
            if (postedIdentity.StatusCode != HttpStatusCode.Created)
                throw new Exception();

        }

        private IRestResponse PostAPI(string url, dynamic objectToSend)
        {
            var client = new RestClient(_appSettings.Host + url);
            var request = new RestRequest(Method.POST);
            string jsonToSend = JsonConvert.SerializeObject(objectToSend);
            request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;
            return client.Execute(request);
        }
    }
}
