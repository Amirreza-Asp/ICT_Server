using Domain.Dtos.Shared;
using Domain.Entities.Business;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.People
{
    public class DeletePersonCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }
    }


    public class DeletePersonCommandHandler : IRequestHandler<DeletePersonCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public DeletePersonCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(DeletePersonCommand request, CancellationToken cancellationToken)
        {
            var person = await _context.People.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (person == null)
                return CommandResponse.Failure(400, "فرد مورد نظر در سیستم وجود ندارد");

            var transition = await _context.Transitions.FirstOrDefaultAsync(b => b.LeaderId == request.Id, cancellationToken);

            if (transition != null)
            {
                String type = transition.Type == TransitionType.Project ? "پروژه" : "اقدام";
                return CommandResponse.Failure(400, $"{person.Name} {person.Family} در {type} {transition.Title} واحد مجری است برای حذف این  فرد منصب را به شخص دیگری واگذار کنید");
            }

            _context.People.Remove(person);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
