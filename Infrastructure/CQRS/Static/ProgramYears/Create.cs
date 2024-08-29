using Domain.Dtos.Shared;
using Domain.Entities.Static;
using MediatR;

namespace Infrastructure.CQRS.Static.ProgramYears
{
    public class CreateProgramYearCommand : IRequest<CommandResponse>
    {
        public String Year { get; set; }
    }

    public class CreateProgramYearCommandHandler : IRequestHandler<CreateProgramYearCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateProgramYearCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(CreateProgramYearCommand request, CancellationToken cancellationToken)
        {
            if (_context.ProgramYears.Any(b => b.Year.Equals(request.Year)))
                return CommandResponse.Failure(400, "سال وارد شده در سیستم وجود دارد");

            var programYear = new ProgramYear { Id = Guid.NewGuid(), Year = request.Year };
            _context.ProgramYears.Add(programYear);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(programYear.Id);

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
