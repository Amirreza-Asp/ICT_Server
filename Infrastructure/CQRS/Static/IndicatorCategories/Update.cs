using Domain.Dtos.Shared;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Static.IndicatorCategories
{
    public class UpdateIndicatorCategoryCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public String Title { get; set; }
    }

    public class UpdateIndicatorCategoryCommandHandler : IRequestHandler<UpdateIndicatorCategoryCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateIndicatorCategoryCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateIndicatorCategoryCommand request, CancellationToken cancellationToken)
        {
            var inc = await _context.IndicatorCategories.FindAsync(request.Id);

            if (inc == null)
                return CommandResponse.Failure(400, "طبقه بندی مورد نظر در سیستم وجود ندارد");

            if (inc.Title == request.Title)
                return CommandResponse.Success();

            inc.Title = request.Title;

            _context.IndicatorCategories.Update(inc);

            if (await _context.SaveChangesAsync() > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
