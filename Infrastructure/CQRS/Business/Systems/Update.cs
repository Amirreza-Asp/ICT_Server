using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.Systems
{
    public class UpdateSystemCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public String Title { get; set; }

        public String Description { get; set; }

        [Required]
        public String ProgrammingLanguage { get; set; }

        [Required]
        public String Database { get; set; }

        [Required]
        public String Framework { get; set; }

        [Required]
        public String Development { get; set; }

        [Required]
        public String Company { get; set; }

        [Required]
        public String OS { get; set; }

        [Required]
        public String SupportType { get; set; }
    }

    public class UpdateSystemCommandHandler : IRequestHandler<UpdateSystemCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateSystemCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateSystemCommand request, CancellationToken cancellationToken)
        {
            var system = await _context.Systems.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (system == null)
                return CommandResponse.Failure(400, "سامانه انتخاب شده در سیستم وجود ندارد");

            system.Title = request.Title;
            system.Description = request.Description;
            system.OS = request.OS;
            system.BuildInCompany = request.Company;
            system.ProgrammingLanguage = request.ProgrammingLanguage;
            system.Database = request.Database;
            system.Framework = request.Framework;
            system.Development = request.Development;
            system.SupportType = request.SupportType;

            _context.Systems.Update(system);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "ویرایش سامانه با شکست مواجه شد");
        }
    }
}
