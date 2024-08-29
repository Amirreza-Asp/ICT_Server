using Application.Services.Interfaces;
using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.Programs
{
    public class UpdateProgramCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }

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

    public class UpdateProgramCommandHandler : IRequestHandler<UpdateProgramCommand, CommandResponse>
    {
        private readonly IUserAccessor _userAccessor;
        private readonly ApplicationDbContext _context;

        public UpdateProgramCommandHandler(ApplicationDbContext context, IUserAccessor userAccessor)
        {
            _context = context;
            _userAccessor = userAccessor;
        }

        public async Task<CommandResponse> Handle(UpdateProgramCommand request, CancellationToken cancellationToken)
        {
            if (request.StartedAt > request.EndAt)
                return CommandResponse.Failure(400, "تاریخ شروع نمیتواند از پایان بیشتر باشد");

            var program = await _context.Program.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);
            var companyId = _userAccessor.GetCompanyId().Value;

            if (program == null)
                return CommandResponse.Failure(400, "برنامه انتخاب شده در سیستم وجود ندارد");

            program.StartedAt = request.StartedAt;
            program.EndAt = request.EndAt;
            program.Title = request.Title;
            program.Description = request.Description;
            program.IsActive = request.IsActive;


            if (request.IsActive)
            {
                var activeProgram =
                       await _context.Program
                           .Where(b => b.IsActive && b.CompanyId == companyId)
                           .FirstOrDefaultAsync(cancellationToken);

                if (activeProgram != null)
                {
                    activeProgram.IsActive = false;
                    _context.Program.Update(activeProgram);
                }
            }

            _context.Program.Update(program);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "ویرایش با شکست مواجه شد");
        }
    }
}
