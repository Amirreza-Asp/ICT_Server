using Application.Services.Interfaces;
using Domain.Dtos.Shared;
using Domain.Entities.Business;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.Programs
{
    public class CreateProgramCommand : IRequest<CommandResponse>
    {
        [Required]
        public String Title { get; set; }

        [Required]
        public DateTime StartedAt { get; set; }

        [Required]
        public DateTime EndAt { get; set; }

        [Required]
        public String Description { get; set; }

        [Required]
        public bool IsActive { get; set; }

    }

    public class CreateProgramCommandHandler : IRequestHandler<CreateProgramCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserAccessor _userAccessor;

        public CreateProgramCommandHandler(ApplicationDbContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(CreateProgramCommand request, CancellationToken cancellationToken)
        {
            if (request.StartedAt > request.EndAt)
                return CommandResponse.Failure(400, "تاریخ شروع نمیتواند بیشتر از پایان باشد");

            var companyId = _userAccessor.GetCompanyId();

            var program = new Program
            {
                Id = Guid.NewGuid(),
                Description = request.Description,
                EndAt = request.EndAt,
                StartedAt = request.StartedAt,
                IsActive = request.IsActive,
                Title = request.Title,
                CompanyId = companyId.Value
            };


            var activeProgram =
                    await _context.Program
                        .Where(b => b.IsActive && b.CompanyId == companyId.Value)
                        .FirstOrDefaultAsync(cancellationToken);

            if (activeProgram == null)
                program.IsActive = true;
            else if (request.IsActive)
            {
                activeProgram.IsActive = false;
                _context.Program.Update(activeProgram);
            }

            _context.Program.Add(program);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success(new { id = program.Id, isActive = program.IsActive });

            return CommandResponse.Failure(500, "افزودن برنامه با شکست مواجه شد");
        }

    }
}
