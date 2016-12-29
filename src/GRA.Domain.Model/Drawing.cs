using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Drawing : Abstract.BaseDomainEntity
    {
        [Required]
        public int DrawingCriteriaId { get; set; }
        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        [MaxLength(1000)]
        [Required]
        public string RedemptionInstructions { get; set; }
        [Range(1, Int32.MaxValue)]
        public int WinnerCount { get; set; }
        public string NotificationSubject { get; set; }
        public string NotificationMessage { get; set; }
    }
}
