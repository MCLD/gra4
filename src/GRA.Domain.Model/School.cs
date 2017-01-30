using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class School : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }
        [Required]
        public int SchoolDistrictId { get; set; }
        [Required]
        public int SchoolTypeId { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
