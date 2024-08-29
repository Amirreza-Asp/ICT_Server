using Application.Services.Interfaces;
using Application.Utility;
using Domain;
using Domain.Dtos.Shared;
using Domain.Entities.Account;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Account.Users
{
    public class UserRequestCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid CompanyId { get; set; }
    }

    public class UserRequestCommandResponse : IRequestHandler<UserRequestCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _contextAcessor;
        private readonly ISSOService _ssoService;

        public UserRequestCommandResponse(ApplicationDbContext context, IHttpContextAccessor contextAcessor, ISSOService ssoService)
        {
            _context = context;
            _contextAcessor = contextAcessor;
            _ssoService = ssoService;
        }

        public async Task<CommandResponse> Handle(UserRequestCommand request, CancellationToken cancellationToken)
        {
            var uswTokenCookie = _contextAcessor.HttpContext.Request.Cookies[SD.UswToken];
            if (uswTokenCookie == null)
                return CommandResponse.Failure(400, "برای ارسال درخواست باید ابتدا توسط احراز هویت مرکزی وارد شوید");

            var uswToken = ProtectorData.Decrypt(uswTokenCookie);

            var userInfo = await _ssoService.GetProfileAsync(uswToken);

            if (userInfo == null || !userInfo.isSuccess)
                return CommandResponse.Failure(400, "اطلاعات شما در احراز هویت مرکزی پیدا نشد");

            if (_context.UsersJoinRequests.Where(b => b.NationalId == userInfo.data.nationalId).Count() == 5)
                return CommandResponse.Failure(400, "نمیتوان برای بیشتر از 5 دانشگاه درخواست کاربری بدون تاییدیه داشت");

            if (_context.UsersJoinRequests.Where(b => b.NationalId == userInfo.data.nationalId && b.CompanyId == request.CompanyId).Any())
                return CommandResponse.Failure(400, "برای این دانشگاه درخواست فرستاده اید");

            var userRequest = new UserJoinRequest
            {
                CompanyId = request.CompanyId,
                FullName = userInfo.data.firstName + " " + userInfo.data.lastName,
                Id = Guid.NewGuid(),
                NationalId = userInfo.data.nationalId,
                PhoneNumber = userInfo.data.mobile
            };

            _context.UsersJoinRequests.Add(userRequest);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "ارسال درخواست با شکست مواجه شد");
        }
    }
}
