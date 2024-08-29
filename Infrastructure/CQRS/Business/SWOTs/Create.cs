using Application.Services.Interfaces;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.SOWTs
{
    public class CreateSWOTCommand : IRequest<CommandResponse>
    {
        [Required]
        public String Content { get; set; }

        [Required]
        public int Type { get; set; }
    }

    public class CreateSWOTCommandHandler : IRequestHandler<CreateSWOTCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserAccessor _userAccessor;

        public CreateSWOTCommandHandler(ApplicationDbContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(CreateSWOTCommand request, CancellationToken cancellationToken)
        {
            var companyId = _userAccessor.GetCompanyId().Value;
            var activeProgramId =
                    await _context.Program
                        .Where(b => b.IsActive && b.CompanyId == companyId)
                        .Select(b => b.Id)
                        .FirstOrDefaultAsync(cancellationToken);

            if (activeProgramId == default)
                return CommandResponse.Failure(400, "هیج برنامه ای انتخاب نشده است");

            var sowt =
                new SWOT
                {
                    Content = request.Content,
                    Id = Guid.NewGuid(),
                    ProgramId = activeProgramId,
                    Type = (SWOTType)request.Type
                };

            _context.SWOT.Add(sowt);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(sowt.Id);

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
