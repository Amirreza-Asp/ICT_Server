using Application.Services.Interfaces;
using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.Programs
{
    public class ChangeProgramActiveCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }

    }

    public class ChangeProgramActiveCommandHandler : IRequestHandler<ChangeProgramActiveCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserAccessor _userAccessor;

        public ChangeProgramActiveCommandHandler(ApplicationDbContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(ChangeProgramActiveCommand request, CancellationToken cancellationToken)
        {
            var companyId = _userAccessor.GetCompanyId().Value;
            var program = await _context.Program.FirstOrDefaultAsync(b => b.Id == request.Id && b.CompanyId == companyId, cancellationToken);

            if (program == null)
                return CommandResponse.Failure(400, "برنامه انتخاب شده در سیستم وجود ندارد");

            var currentActiveProgram = await _context.Program.FirstOrDefaultAsync(b => b.IsActive && b.CompanyId == companyId, cancellationToken);

            if (currentActiveProgram != null)
            {
                if (currentActiveProgram.Id == request.Id)
                    return CommandResponse.Failure(400, "برنامه انتخاب شده فعال است");

                currentActiveProgram.IsActive = false;
                _context.Program.Update(currentActiveProgram);
            }

            program.IsActive = true;
            _context.Program.Update(program);


            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "عملیات با شکست مواجه شد");
        }
    }
}
