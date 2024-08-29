using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.People
{
    public class UpdatePersonCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public String Name { get; set; }

        [Required]
        public String Family { get; set; }

        [Required]
        public String JobTitle { get; set; }

        [Required]
        public String Education { get; set; }

        [Required]
        public List<String> Expertises { get; set; }
    }

    public class UpdatePersonCommandHandler : IRequestHandler<UpdatePersonCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdatePersonCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
        {
            var person = await _context.People.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (person == null)
                return CommandResponse.Failure(400, "فرد انتخاب شده در سیستم وجود ندارد");


            person.Expertises = request.Expertises.Select(b => new Domain.Entities.Business.Expertise { Title = b }).ToList();
            person.JobTitle = request.JobTitle;
            person.Name = request.Name;
            person.Family = request.Family;
            person.Education = request.Education;


            _context.People.Update(person);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "مشکل داخلی سرور");
        }
    }
}
