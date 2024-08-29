using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.Business
{
    public enum IndicatorPeriod
    {
        [Display(Name = "روزانه")]
        Day = 0,
        [Display(Name = "هفتگی")]
        Week = 10,
        [Display(Name = "ماهیانه")]
        Month = 20,
        [Display(Name = "سه ماهه")]
        Season = 30,
        [Display(Name = "شش ماهه")]
        HalfYear = 40,
        [Display(Name = "سالانه")]
        Year = 50,
    }
}
