using Domain.Entities.Business;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Account
{
    public class Company
    {
        [Key]
        public Guid Id { get; set; }

        public string Title { get; set; }
        public string Logo { get; set; }
        public string PortalUrl { get; set; }
        public String NationalSerial { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public string UniversityType { get; set; }
        public String Status { get; set; }
        public string CreateAt { get; set; }

        public Guid? ParentId { get; set; }
        [ForeignKey(nameof(ParentId))]
        public Company Parent { get; set; }

        public ICollection<BigGoal> BigGoals { get; set; } = new List<BigGoal>();
        public ICollection<Company> Childs { get; set; } = new List<Company>();
        public ICollection<Act> Acts { get; set; } = new List<Act>();
        public ICollection<CompanyIndicator> Indicators { get; set; } = new List<CompanyIndicator>();
    }
}
