using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Static.IndicatorCategories
{
    public class DeleteIndicatorCategoryCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }
    }


    public class DeleteIndicatorCategoryCommandHandler : IRequestHandler<DeleteIndicatorCategoryCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public DeleteIndicatorCategoryCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(DeleteIndicatorCategoryCommand request, CancellationToken cancellationToken)
        {
            var inc = await _context.IndicatorCategories.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (inc == null)
                return CommandResponse.Success();

            _context.IndicatorCategories.Remove(inc);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخل سرور");
        }
    }
}

