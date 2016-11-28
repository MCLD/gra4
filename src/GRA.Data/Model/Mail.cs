using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Mail : Abstract.BaseDbEntity
    {
        [Required]
        int SiteId { get; set; }
        [Required]
        int ToUserId { get; set; }
        [Required]
        int FromUserId { get; set; }

        [Required]
        string Subject { get; set; }
        [Required]
        string Body { get; set; }
        [Required]
        bool IsNew { get; set; }
    }
}
