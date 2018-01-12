using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class GroupInfo : Abstract.BaseDomainEntity
    {
        public int SiteId { get; set; }
        [Required]
        public int UserId { get; set; }
        public User User { get; set; }
        [Required]
        [MaxLength(255)]
        [DisplayName("Group name")]
        public string Name { get; set; }
        [Required]
        public int GroupTypeId { get; set; }
        public GroupType GroupType { get; set; }
        public string GroupTypeName { get; set; }
    }
}
