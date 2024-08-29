using Domain.Dtos.Shared;
using Domain.Entities.Static;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Static.IndicatorTypes
{
    public class CreateIndicatorTypeCommand : IRequest<CommandResponse>
    {
        [Required]
        public String Title { get; set; }
    }

    public class CreateIndicatorTypeCommandHandler : IRequestHandler<CreateIndicatorTypeCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateIndicatorTypeCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(CreateIndicatorTypeCommand request, CancellationToken cancellationToken)
        {
            if (_context.IndicatorTypes.Any(b => b.Title == request.Title))
                return CommandResponse.Failure(400, "عنوان وارد شده تکراری است");

            var ict = new IndicatorType { Id = Guid.NewGuid(), Title = request.Title };

            _context.IndicatorTypes.Add(ict);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(ict.Id);

            return CommandResponse.Failure(400, "مشکل داخلی سرور");
        }
    }
}
