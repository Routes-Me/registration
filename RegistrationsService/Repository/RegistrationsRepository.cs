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

        public void RegisterDashboard()
        {
            
        }

        private async Task<dynamic> SignUp(RegistrationDto registrationDto)
        {
            try
            {
                if (registrationDto == null || string.IsNullOrEmpty(registrationDto.PhoneNumber) || string.IsNullOrEmpty(registrationDto.Email) || string.IsNullOrEmpty(registrationDto.Password) || string.IsNullOrEmpty(registrationDto.Name))
                    throw new ArgumentNullException(CommonMessage.PassValidData);

                IRestResponse postUser = PostAPI();

            }
            catch (Exception ex)
            {
                return ReturnResponse.ErrorResponse(CommonMessage.ExceptionMessage + ex.Message, StatusCodes.Status400BadRequest);
            }
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
