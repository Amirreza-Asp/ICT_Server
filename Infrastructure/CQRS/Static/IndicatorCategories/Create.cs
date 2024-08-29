using Domain.Dtos.Shared;
using Domain.Entities.Static;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Static.IndicatorCategories
{
    public class CreateIndicatorCategoryCommand : IRequest<CommandResponse>
    {
        public Guid? ParentId { get; set; }

        [Required]
        public String Title { get; set; }
    }

    public class CreateIndicatorCategoryCommandHandler : IRequestHandler<CreateIndicatorCategoryCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public CreateIndicatorCategoryCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(CreateIndicatorCategoryCommand request, CancellationToken cancellationToken)
        {
            var indicatorCategory = new IndicatorCategory
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                ParentId = request.ParentId,
            };

            _context.IndicatorCategories.Add(indicatorCategory);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(indicatorCategory.Id);

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
