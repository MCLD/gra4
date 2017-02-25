using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class DynamicAvatarLayer : Abstract.BaseDbEntity
    {
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        public int Position { get; set; }
    }
}
