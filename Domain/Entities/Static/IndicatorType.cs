using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Static
{
    public class IndicatorType : BaseEntity
    {
        [Required]
        public string Title { get; set; }
    }
}
