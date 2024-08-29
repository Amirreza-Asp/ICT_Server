using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.Programs
{
    public class DeleteProgramCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class DeleteProgramCommandHandler : IRequestHandler<DeleteProgramCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public DeleteProgramCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(DeleteProgramCommand request, CancellationToken cancellationToken)
        {
            var program = await _context.Program.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (program == null)
                return CommandResponse.Failure(400, "برنامه انتخاب شده در سیستم وجود ندارد");

            if (program.IsActive)
                return CommandResponse.Failure(400, "برنامه فعال را نمیتوان حذف کرد");

            _context.Program.Remove(program);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "حذف با شکست مواجه شد");
        }
    }
}
