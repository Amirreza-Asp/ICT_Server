using System.ComponentModel.DataAnnotations;

namespace Domain.Dtos.Programs
{
    public class ProgramDetails
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
}
