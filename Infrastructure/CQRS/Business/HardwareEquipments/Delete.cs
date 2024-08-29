using Domain.Dtos.Shared;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.CQRS.Business.HardwareEquipments
{
    public class DeleteHardwareEquipmentCommand : IRequest<CommandResponse>
    {
        [Required]
        public Guid Id { get; set; }
    }

    public class DeleteHardwareEquipmentCommandHandler : IRequestHandler<DeleteHardwareEquipmentCommand, CommandResponse>
    {
        private readonly ApplicationDbContext _context;

        public DeleteHardwareEquipmentCommandHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CommandResponse> Handle(DeleteHardwareEquipmentCommand request, CancellationToken cancellationToken)
        {
            var hwe = await _context.HardwareEquipment.FirstOrDefaultAsync(b => b.Id == request.Id, cancellationToken);

            if (hwe == null)
                return CommandResponse.Failure(400, "سخت افزار انتخاب شده در سیستم وجود ندارد");

            _context.HardwareEquipment.Remove(hwe);

            if (await _context.SaveChangesAsync(cancellationToken) > 0)
                return CommandResponse.Success();

            return CommandResponse.Failure(500, "عملیات حذف با مشکل مواجه شد");
        }
    }

}
