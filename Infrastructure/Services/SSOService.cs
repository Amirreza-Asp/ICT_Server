using Application.Services.Interfaces;
using Application.Utility;
using AutoMapper;
using Domain.Dtos.Account.SSO;
using Domain.Entities.Account;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;

namespace Infrastructure.Services
{
    public class SSOService : ISSOService
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly IHttpContextAccessor _accessor;
        private readonly IWebHostEnvironment _hostEnv;
        private readonly IMapper _mapper;
        private readonly SSOModel _sso;

        public SSOService(IHttpClientFactory clientFactory, IHttpContextAccessor accessor, IWebHostEnvironment hostEnv, IMapper mapepr, IOptions<SSOModel> options)
        {
            _clientFactory = clientFactory;
            _accessor = accessor;
            _hostEnv = hostEnv;
            _mapper = mapepr;
            _sso = options.Value;
        }

        public async Task<ProfileRequest> GetProfileAsync(string token)
        {
            var httpClient = _clientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var jwksResponse = await httpClient.GetAsync($"{_sso.Url}/oauth2/jwks");
            if (jwksResponse.IsSuccessStatusCode)
            {
                // user info
                var userInfoResponse = await httpClient.GetAsync($"{_sso.Url}/api/v1/User/userinfo");
                if (userInfoResponse.IsSuccessStatusCode)
                {
                    var userInfoReadAsString = await userInfoResponse.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<ProfileRequest>(userInfoReadAsString);
                }
            }
            else
            {
                throw new Exception("کاربر در سیستم وچود ندارد");
            }

            return null;
        }

        public async Task<OAuthResponseToken> GetTokenAsync(String code)
        {
            var httpClient = _clientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(180);

            using MultipartFormDataContent multipartContent = new MultipartFormDataContent();
            multipartContent.Add(new StringContent("authorization_code", Encoding.UTF8, MediaTypeNames.Text.Plain), "grant_type");
            multipartContent.Add(new StringContent(code, Encoding.UTF8, MediaTypeNames.Text.Plain), "code");
            multipartContent.Add(new StringContent("openid profile", Encoding.UTF8, MediaTypeNames.Text.Plain), "scope");
            multipartContent.Add(new StringContent(_sso.Redirect, Encoding.UTF8, MediaTypeNames.Text.Plain), "redirect_uri");
            multipartContent.Add(new StringContent(_sso.ClientId, Encoding.UTF8, MediaTypeNames.Text.Plain), "client_id");
            multipartContent.Add(new StringContent(_sso.ClientSecret, Encoding.UTF8, MediaTypeNames.Text.Plain), "client_secret");


            var tokenResponse = await httpClient.PostAsync($"{_sso.Url}/oauth2/token", multipartContent);
            if (tokenResponse.IsSuccessStatusCode)
            {
                var tokenReadAsString = await tokenResponse.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<OAuthResponseToken>(tokenReadAsString);
            }

            return null;
        }

        public async Task<List<Company>> GetCompaniesAsync()
        {
            var path = _hostEnv.WebRootPath + "/Files/small-universities-json.txt";
            var data = await File.ReadAllTextAsync(path);

            var companies = JsonSerializer.Deserialize<List<CompsRequestData>>(data);

            return _mapper.Map<List<Company>>(companies);
        }
    }


}
