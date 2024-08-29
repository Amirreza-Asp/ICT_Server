using Domain.Dtos.Shared;
using Domain.SubEntities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.Transitions
{
    public class UpdateTransitionCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public String Title { get; set; }

        public String Contractor { get; set; }

        [Required]
        public DateTime StartedAt { get; set; }

        [Required]
        public DateTime Deadline { get; set; }

        public List<String> Financials { get; set; } = new List<String>();

        [Required]
        public Guid LeaderId { get; set; }
    }

    public class UpdateProjectCommandHandler : IRequestHandler<UpdateTransitionCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateProjectCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateTransitionCommand request, CancellationToken cancellationToken)
        {
            if (request.StartedAt > request.Deadline)
                return CommandResponse.Failure(400, "تاریخ شروع نمیتواند از تاریخ پایان کمتر باشد");

            if (!_context.People.Any(b => b.Id == request.LeaderId))
                return CommandResponse.Failure(400, "راهبر انتخاب شده نامعتبر است");

            var transition =
                await _context.Transitions
                    .Where(b => b.Id == request.Id)
                    .Include(b => b.Financials)
                    .FirstOrDefaultAsync(cancellationToken);

            if (transition == null)
                return CommandResponse.Failure(400, "گذار انتخاب شده وجود ندارد");

            transition.StartedAt = request.StartedAt;
            transition.Deadline = request.Deadline;
            transition.LeaderId = request.LeaderId;
            transition.Title = request.Title;
            transition.Contractor = request.Contractor;

            transition.Financials = request.Financials.Select(b => new Financial { Title = b }).ToList();

            _context.Transitions.Update(transition);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
