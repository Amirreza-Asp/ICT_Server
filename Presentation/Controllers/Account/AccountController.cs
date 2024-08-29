using Application.Repositories;
using Application.Services.Interfaces;
using Application.Utility;
using AutoMapper;
using Domain;
using Domain.Dtos.Account.Acts;
using Domain.Dtos.Account.Cookies;
using Domain.Dtos.Account.User;
using Domain.Dtos.Shared;
using Domain.Entities.Account;
using Infrastructure.CQRS.Account.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;

namespace Presentation.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ISSOService _ssoService;
        private readonly IAuthService _authService;
        private readonly IMapper _mapper;
        private readonly IRepository<Company> _companyRepo;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IUserAccessor _userAccessor;
        private readonly IMemoryCache _memoryCache;
        private readonly IRepository<Act> _actRepo;
        private readonly IMediator _mediator;
        private readonly IRepository<User> _userRepo;
        private readonly SSOModel _sso;

        public AccountController(IOptions<SSOModel> ssoModelOptions, IHttpClientFactory clientFactory, ISSOService ssoService, IAuthService authService, IMapper mapper, IRepository<Company> companyRepo, IHttpContextAccessor contextAccessor, IUserAccessor userAccessor, IMemoryCache memoryCache, IRepository<Act> actRepo, IMediator mediator, IRepository<User> userRepo)
        {
            _clientFactory = clientFactory;
            _ssoService = ssoService;
            _authService = authService;
            _mapper = mapper;
            _companyRepo = companyRepo;
            _contextAccessor = contextAccessor;
            _userAccessor = userAccessor;
            _memoryCache = memoryCache;
            _actRepo = actRepo;
            _mediator = mediator;
            _sso = ssoModelOptions.Value;
            _userRepo = userRepo;
        }

        [Route("Login")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromQuery] String redirectUrl)
        {
            var state = Guid.NewGuid().ToString().Replace("-", "");
            HttpContext.Session.SetString("state", state);
            HttpContext.Session.SetString("redirect-url", redirectUrl);

            var url = $"https://sso.razi.ac.ir/oauth2/authorize?" +
                $"response_type=code" +
                $"&scope=openid profile" +
                $"&client_id={_sso.ClientId}" +
                $"&state={state}" +
                //$"&client-secret={SD.ClientSecret}" +
                $"&redirect_uri={_sso.Redirect}";

            return Redirect(url);
        }


        [Route("authorizeLogin")]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> AuthorizeLogin([FromQuery] string code, [FromQuery] string state)
        {
            var stateCheck = HttpContext.Session.GetString("state");
            if (string.IsNullOrEmpty(stateCheck) || stateCheck != state)
            {
                return BadRequest();
            }

            HttpContext.Session.Remove("state");
            HttpContext.Session.SetString("code", code);

            var uswToken = await _ssoService.GetTokenAsync(code);

            if (uswToken == null)
                return BadRequest();

            var userProfile = await _ssoService.GetProfileAsync(uswToken.access_token);

            if (!await _authService.ExistsAsync(userProfile.data.nationalId))
            {
                await _authService.CreateAsync(userProfile);
            }

            await _authService.LoginAsync(
                nationalId: userProfile.data.nationalId,
                uswToken: uswToken.access_token);


            var redirect = HttpContext.Session.GetString("redirect-url");
            HttpContext.Session.Remove("redirect-url");

            return Redirect(redirect);
        }


        [HttpPost]
        [AllowAnonymous]
        [Route("ChooseAct")]
        public async Task<CommandResponse> ChooseAct([FromBody] ChooseActDto command, CancellationToken cancellationToken)
        {
            return await _authService.ChooseActAsync(command);
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("GetUserActs")]
        public async Task<List<ActSummary>> GetUserActs()
        {
            var jsonUserTempInfo = HttpContext.Request.Cookies["user-temp-info"];
            var userTempInfo = JsonSerializer.Deserialize<UserTempDataCookies>(jsonUserTempInfo);

            var userId = Guid.Parse(ProtectorData.Decrypt(userTempInfo.UserId));
            var user = await _userRepo.FirstOrDefaultAsync(b => b.Id == userId);

            var data = await _actRepo.GetAllAsync<ActSummary>(b => b.UserId == userId);
            return data;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("UserRequest")]
        public async Task<CommandResponse> UserRequest([FromBody] UserRequestCommand command, CancellationToken cancellationToken)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        [HttpGet]
        [Route("Logout")]
        public async Task<CommandResponse> Logout()
        {
            return await _authService.LogoutAsync();
        }

        [HttpGet("Profile")]
        public async Task<IActionResult> Profile()
        {
            var hashedUswToken = HttpContext.Request.Cookies[SD.UswToken];
            var uswToken = ProtectorData.Decrypt(hashedUswToken);

            try
            {
                var userProfile = await _ssoService.GetProfileAsync(uswToken);
                if (userProfile == null || !userProfile.isSuccess)
                    return Unauthorized();

                var user = _mapper.Map<UserProfile>(userProfile.data);

                var company = await _companyRepo.FirstOrDefaultAsync(filter: b => b.Acts.Any(b => b.User.NationalId == user.NationalId));
                user.Company = company.Title;
                user.Permissions = await _authService.GetPermissionsAsync(user.NationalId);

                return Ok(user);
            }
            catch (Exception ex)
            {
                return Unauthorized();
            }
        }
    }
}
