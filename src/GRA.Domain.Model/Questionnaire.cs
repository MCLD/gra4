using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class Questionnaire : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }
        public int RelatedSystemId { get; set; }
        public int RelatedBranchId { get; set; }

        [MaxLength(255)]
        public string Name { get; set; }
        public ICollection<Question> Questions { get; set; }
        public bool IsValid { get; set; }
        public bool IsActive { get; set; }
    }
}
