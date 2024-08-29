using Domain.Entities.Account;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Business
{
    public class HardwareEquipment : BaseEntity
    {
        [Required]
        public String Title { get; set; }

        [Required]
        public String BrandName { get; set; }

        [Range(0, int.MaxValue)]
        public int Count { get; set; }

        public String Description { get; set; }

        [Required]
        public String SupportType { get; set; }

        public Guid CompanyId { get; set; }
        [ForeignKey(nameof(CompanyId))]
        public Company Company { get; set; }
    }
}
