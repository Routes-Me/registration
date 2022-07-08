﻿using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RegistrationsService.Abstraction;
using RegistrationsService.Models;
using RegistrationsService.Models.Common;
using RegistrationsService.Models.DBModels;
using RegistrationsService.Models.ResponseModels;
using RestSharp;
using System;
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

            InvitationsDto invitationsDto = GetInvitation(registrationDto);

            registrationDto.InstitutionId = invitationsDto.InstitutionId;
            await Register(registrationDto, "driver", "user", invitationsDto.InstitutionId);
        }

        public async Task RegisterBusValidators(RegistrationDto registrationDto)
        {
            await Register(registrationDto, "bus validator", "user");
        }

        public async Task RegisterEnterprisePromotionApp(RegistrationDto registrationDto)
        {
            await Register(registrationDto, "enterprise promotion", "user");
        }

        public async Task RegisterDashboard(RegistrationDto registrationDto)
        {
            InvitationsDto invitationsDto = GetInvitation(registrationDto);
            registrationDto.IsDashboard = true;
            await Register(registrationDto, invitationsDto.ApplicationId, invitationsDto.PrivilageId, invitationsDto.InstitutionId);
        }



        private UsersResponse PostUsers(RegistrationDto registrationDto)
        {
            UsersDto userDto = new UsersDto
            {
                Name = registrationDto.Name,
                Email = registrationDto.Email,
                PhoneNumber = registrationDto.PhoneNumber
            };
            IRestResponse postedUserResponse = PostAPI(_dependencies.UsersUrl, userDto);
            return JsonConvert.DeserializeObject<UsersResponse>(postedUserResponse.Content);
        }
        private Task PostOfficer(string userId, string identityId, string institutionId)
        {
            OfficersDto officersDto = new OfficersDto
            {
                UserId = userId,
                InstitutionId = institutionId
            };
            try
            {
                PostAPI(_dependencies.OfficersUrl, officersDto);
            }
            catch (Exception)
            {
                DeleteAPI(_appSettings.Host + _dependencies.UsersUrl + userId);
                DeleteAPI(_appSettings.Host + _dependencies.IdentitiesUrl + identityId);
                throw;
            }
            return Task.CompletedTask;
        }
        private DriverResponse PostDrivers(RegistrationDto registrationDto)
        {
            registrationDto.PhoneNumber = registrationDto.phone.number;
            UsersResponse userResponse = PostUsers(registrationDto);

            DriverDto Driver = new DriverDto
            {
                User_Id = userResponse.UserId,
                Institution_Id = registrationDto.InstitutionId,
                avatarUrl = registrationDto.avatarUrl,
                Vehicle_Id = registrationDto.VehicleId
            };
            try
            {
                IRestResponse postedDriverResponse = PostAPI(_dependencies.DriversUrl, Driver);
                return JsonConvert.DeserializeObject<DriverResponse>(postedDriverResponse.Content);
            }
            catch (Exception)
            {
                DeleteAPI(_appSettings.Host + _dependencies.UsersUrl + userResponse.UserId);
                throw;
            }
        }



        private IRestResponse PostAPI(string url, dynamic objectToSend)
        {
            RestClient client = new RestClient(_appSettings.Host + url);
            RestRequest request = new RestRequest(Method.POST);
            string jsonToSend = JsonConvert.SerializeObject(objectToSend);
            request.AddParameter("application/json; charset=utf-8", jsonToSend, ParameterType.RequestBody);
            request.RequestFormat = DataFormat.Json;
            IRestResponse response = client.Execute(request);
            if (response.StatusCode != HttpStatusCode.Created)
            {
                throw new Exception(response.Content);
            }

            return response;
        }

        private IRestResponse DeleteAPI(string url)
        {
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest(Method.DELETE);
            return client.Execute(request);
        }

        private dynamic GetAPI(string url, string query = "")
        {
            UriBuilder uriBuilder = new UriBuilder(_appSettings.Host + url);
            uriBuilder = AppendQueryToUrl(uriBuilder, query);
            RestClient client = new RestClient(uriBuilder.Uri);
            RestRequest request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);

            if (response.StatusCode == 0)
            {
                throw new HttpListenerException(400, CommonMessage.ConnectionFailure);
            }

            if (!response.IsSuccessful)
            {
                throw new HttpListenerException((int)response.StatusCode, response.Content);
            }

            return response;
        }


        private UriBuilder AppendQueryToUrl(UriBuilder baseUri, string queryToAppend)
        {
            if (baseUri.Query != null && baseUri.Query.Length > 1)
            {
                baseUri.Query = baseUri.Query.Substring(1) + "&" + queryToAppend;
            }
            else
            {
                baseUri.Query = queryToAppend;
            }

            return baseUri;
        }

        public InvitationsDto GetInvitation(RegistrationDto registrationDto)
        {
            List<InvitationsDto> invitationsDtoList = JsonConvert.DeserializeObject<GetInvitationsResponse>(GetAPI(_dependencies.InvitationsUrl + registrationDto.InvitationId).Content).data;
            if (!invitationsDtoList.Any())
            {
                throw new KeyNotFoundException(CommonMessage.InvitationNotFound);
            }

            InvitationsDto invitationsDto = invitationsDtoList.FirstOrDefault();
            if (string.IsNullOrEmpty(invitationsDto.ApplicationId) || string.IsNullOrEmpty(invitationsDto.PrivilageId))
            {
                throw new ArgumentNullException(CommonMessage.RoleMissed);
            }

            return invitationsDto;
        }


        private async Task Register(RegistrationDto registrationDto, string application, string privilege, string institutionId = "")
        {
            if (registrationDto == null || string.IsNullOrEmpty(registrationDto.Email) || string.IsNullOrEmpty(registrationDto.Name))
            {
                throw new ArgumentNullException(CommonMessage.PassValidData);
            }
            if (string.IsNullOrEmpty(registrationDto.Password) && application == "driver")
            {
                var driverResponse = new DriverResponse();
                try
                {
                    driverResponse = PostDrivers(registrationDto);
                    return;
                }
                catch (Exception ex)
                {
                    DeleteAPI(_appSettings.Host + _dependencies.DriversUrl + driverResponse.DriverId);
                }
               
            }

            if (string.IsNullOrEmpty(registrationDto.Password))
            {
                throw new ArgumentNullException(CommonMessage.PassValidData);
            }

            UsersResponse userResponse = PostUsers(registrationDto);

            IdentitiesDto identityDto = new IdentitiesDto
            {
                UserId = userResponse.UserId,
                Email = registrationDto.Email,
                PhoneNumber = registrationDto.PhoneNumber,
                Password = registrationDto.Password,
                Roles = new RolesDto { Application = application, Privilege = privilege }
            };
            IdentitiesResponse identityResponse = new IdentitiesResponse();
            try
            {
                IRestResponse postedIdentityResponse = PostAPI(_dependencies.IdentitiesUrl, identityDto);
                identityResponse = JsonConvert.DeserializeObject<IdentitiesResponse>(postedIdentityResponse.Content);
            }
            catch (Exception)
            {
                DeleteAPI(_appSettings.Host + _dependencies.UsersUrl + userResponse.UserId);
                throw;
            }

            if (registrationDto.IsDashboard == true)
            {
                await PostOfficer(userResponse.UserId, identityResponse.IdentityId, institutionId);
            }
        }
    }
}
