using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Static.IndicatorTypes
{
    public class UpdateIndicatorTypeCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public String Title { get; set; }
    }

    public class UpdateIndicatorTypeCommandHandler : IRequestHandler<UpdateIndicatorTypeCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateIndicatorTypeCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateIndicatorTypeCommand request, CancellationToken cancellationToken)
        {
            if (_context.IndicatorTypes.Any(b => b.Title == request.Title && b.Id != request.Id))
                return CommandResponse.Failure(400, "عنوان وارد شده تکراری است");

            var ict = await _context.IndicatorTypes.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (ict == null)
                return CommandResponse.Failure(400, "واحد انتخاب شده در سیستم وجود ندارد");


            ict.Title = request.Title;
            _context.IndicatorTypes.Update(ict);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(400, "مشکل داخلی سرور");
        }
    }
}
