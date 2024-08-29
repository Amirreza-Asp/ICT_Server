using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.CQRS.Static.ProgramYears
{
    public class UpdateProgramYearCommand : IRequest<CommandResponse>
    {
        public Guid Id { get; set; }
        public String Year { get; set; }
    }

    public class UpdateProgramYearCommandHandler : IRequestHandler<UpdateProgramYearCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateProgramYearCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateProgramYearCommand request, CancellationToken cancellationToken)
        {
            if (_context.ProgramYears.Any(b => b.Id != request.Id && b.Year.Equals(request.Year)))
                return CommandResponse.Failure(400, "سال وارد شده در سیستم وجود دارد");

            var programYear =
                await _context.ProgramYears
                    .AsNoTracking()
                    .FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (programYear == null)
                return CommandResponse.Failure(400, "سال برنامه انتخاب شده در سیستم وجود ندارد");

            programYear.Year = request.Year;
            _context.ProgramYears.Update(programYear);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
