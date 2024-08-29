using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Static
{
    public class IndicatorCategory : BaseEntity
    {
        [Required]
        public string Title { get; set; }

        public IndicatorCategory Parent { get; set; }
        public Guid? ParentId { get; set; }

        public ICollection<IndicatorCategory> Childs { get; set; } = new List<IndicatorCategory>();

    }
}
