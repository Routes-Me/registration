using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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

        public async Task<DriverRegistrationResponse> RegisterDriverApp(RegistrationDto registrationDto)
        {

            InvitationsDto invitationsDto = GetInvitation(registrationDto);

            registrationDto.InstitutionId = invitationsDto.InstitutionId;
            return await Register(registrationDto, "driver-app", "user", invitationsDto.InstitutionId);
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
            catch (Exception ex)
            {
                DeleteAPI(_appSettings.Host + _dependencies.UsersUrl + userId);
                DeleteAPI(_appSettings.Host + _dependencies.IdentitiesUrl + identityId);
                throw ex;
            }
            return Task.CompletedTask;
        }
        private DriverRegistrationResponse PostDrivers(RegistrationDto registrationDto, string application, string privilege)
        {
            registrationDto.PhoneNumber = registrationDto.phone.number;
            registrationDto.VerificationToken = registrationDto.phone.VerificationToken;
            UsersResponse userResponse = PostUsers(registrationDto);
            IdentitiesDto identityDto = new IdentitiesDto
            {
                UserId = userResponse.UserId,
                Email = registrationDto.Email,
                PhoneNumber = registrationDto.PhoneNumber,
                Roles = new RolesDto { Application = application, Privilege = privilege }
            };
            IdentitiesResponse identityResponse = new IdentitiesResponse();
            try
            {
                IRestResponse postedIdentityResponse = PostAPI(_dependencies.IdentitiesUrl, identityDto);
                if (postedIdentityResponse.StatusCode != HttpStatusCode.Created)
                {
                    throw new HttpListenerException((int)postedIdentityResponse.StatusCode, postedIdentityResponse.Content);
                }
                if (postedIdentityResponse.StatusCode == 0)
                {
                    throw new HttpListenerException(400, CommonMessage.ConnectionFailure);
                }

                if (!postedIdentityResponse.IsSuccessful)
                {
                    throw new HttpListenerException((int)postedIdentityResponse.StatusCode, postedIdentityResponse.Content);
                }
                else
                    identityResponse = JsonConvert.DeserializeObject<IdentitiesResponse>(postedIdentityResponse.Content);
            }
            catch (Exception ex)
            {
                DeleteAPI(_appSettings.Host + _dependencies.IdentitiesUrl + identityResponse.IdentityId);

                throw ex;
            }

            DriverDto Driver = new DriverDto
            {
                UserId = userResponse.UserId,
                InvitationId = registrationDto.InvitationId,
                Name = registrationDto.Name,
                AvatarUrl = registrationDto.avatarUrl,
                VehicleId = registrationDto.VehicleId,
                PhoneNumber = registrationDto.PhoneNumber,
                VerificationToken = registrationDto.VerificationToken
            };
            try
            {
                DriverRegistrationResponse response = new DriverRegistrationResponse();
                IRestResponse postedDriverResponse = PostAPI(_dependencies.DriversUrl, Driver);
                if (postedDriverResponse.StatusCode != HttpStatusCode.Created)
                {
                    throw new HttpListenerException((int)postedDriverResponse.StatusCode, postedDriverResponse.Content);
                }
                if (postedDriverResponse.StatusCode == 0)
                {
                    throw new HttpListenerException(400, CommonMessage.ConnectionFailure);
                }

                if (!postedDriverResponse.IsSuccessful)
                {
                    throw new HttpListenerException((int)postedDriverResponse.StatusCode, postedDriverResponse.Content);
                }
                else
                {
                    dynamic token = GetAPI(_dependencies.DriverIdentitiesUrl, "?phoneNumber=" + Driver.PhoneNumber + "&application=" + application);
                    response = JsonConvert.DeserializeObject<DriverRegistrationResponse>(token.Content);
                    return response;
                }

            }
            catch (Exception ex)
            {
                DeleteAPI(_appSettings.Host + _dependencies.UsersUrl + userResponse.UserId);
                throw ex;
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


        private async Task<DriverRegistrationResponse> Register(RegistrationDto registrationDto, string application, string privilege, string institutionId = "")
        {
            if (registrationDto == null || string.IsNullOrEmpty(registrationDto.Email) || string.IsNullOrEmpty(registrationDto.Name))
            {
                throw new ArgumentNullException(CommonMessage.PassValidData);
            }
            if (string.IsNullOrEmpty(registrationDto.Password) && application == "driver-app")
            {
                try
                {
                    return PostDrivers(registrationDto, application, privilege);
                }
                catch (Exception ex)
                {
                    throw ex;
                    //DeleteAPI(_appSettings.Host + _dependencies.DriversUrl + driverResponse.DriverId);
                }
            }

            if (string.IsNullOrEmpty(registrationDto.Password))
            {
                throw new ArgumentNullException(CommonMessage.PassValidData);
            }
            else
            {
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
                return null;
            }


        }
    }
}
