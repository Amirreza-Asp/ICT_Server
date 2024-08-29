using Domain.Entities.Account;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Business
{
    public class CompanyIndicator
    {
        [ForeignKey(nameof(Company))]
        public Guid CompanyId { get; set; }

        [ForeignKey(nameof(Indicator))]
        public Guid IndicatorId { get; set; }

        public Company Company { get; set; }
        public Indicator Indicator { get; set; }
    }
}
