using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.CQRS.Static.ProgramYears
{
    public class RemoveProgramYearCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
    }

    public class RemoveProgramYearCommandHandler : IRequestHandler<RemoveProgramYearCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public RemoveProgramYearCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(RemoveProgramYearCommand request, CancellationToken cancellationToken)
        {
            var programYear =
                await _context.ProgramYears
                    .Where(b => b.Id == request.Id)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(cancellationToken);

            if (programYear == null)
                return CommandResponse.Success();

            _context.ProgramYears.Remove(programYear);
            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
