using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class DynamicAvatarLayer : Abstract.BaseDomainEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
    }
}
