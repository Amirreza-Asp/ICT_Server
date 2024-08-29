using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.HardwareEquipments
{
    public class UpdateHardwareEquipmentCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public String Title { get; set; }

        [Required]
        public String BrandName { get; set; }

        [Range(1, int.MaxValue)]
        public int Count { get; set; }

        public String Description { get; set; }

        [Required]
        public String SupportType { get; set; }
    }

    public class UpdateHardwareEquipmentCommandHandler : IRequestHandler<UpdateHardwareEquipmentCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public UpdateHardwareEquipmentCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(UpdateHardwareEquipmentCommand request, CancellationToken cancellationToken)
        {
            var hwe = await _context.HardwareEquipment.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (hwe == null)
                return CommandResponse.Failure(400, "سخت افزار انتخاب شده در سیستم وجود ندارد");

            hwe.Title = request.Title;
            hwe.Description = request.Description;
            hwe.SupportType = request.SupportType;
            hwe.BrandName = request.BrandName;
            hwe.Count = request.Count;

            _context.HardwareEquipment.Update(hwe);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "ویرایش با خطا روبرو شد");
        }
    }
}
