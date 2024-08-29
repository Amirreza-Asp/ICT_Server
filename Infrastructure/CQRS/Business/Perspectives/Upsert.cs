using Application.Services.Interfaces;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.Perspectives
{
    public class UpsertPerspectiveCommand : IRequest<CommandResponse>
    {
        [Required]
        public String Content { get; set; }
    }

    public class UpsertPerspectiveCommandHandler : IRequestHandler<UpsertPerspectiveCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserAccessor _userAccessor;

        public UpsertPerspectiveCommandHandler(ApplicationDbContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(UpsertPerspectiveCommand request, CancellationToken cancellationToken)
        {
            var companyId = _userAccessor.GetCompanyId().Value;

            var activeProgramId =
                await _context.Program
                    .Where(b => b.CompanyId == companyId && b.IsActive)
                    .Select(b => b.Id)
                    .FirstOrDefaultAsync(cancellationToken);

            if (activeProgramId == default)
                return CommandResponse.Failure(400, "هیچ برنامه ای انتخاب نشده است");

            var perspective =
                await _context.Perspective
                    .FirstOrDefaultAsync(b => b.ProgramId == activeProgramId);

            if (perspective == null)
            {
                perspective = new Perspective
                {
                    Id = Guid.NewGuid(),
                    Content = request.Content,
                    ProgramId = activeProgramId
                };

                _context.Perspective.Add(perspective);
            }
            else
            {
                perspective.Content = request.Content;
                _context.Perspective.Update(perspective);
            }

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(perspective.Id);

            return CommandResponse.Failure(400, "عملیات با شکست مواجه شد");
        }
    }
}
