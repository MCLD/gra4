﻿using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Mail : Abstract.BaseDbEntity
    {
        [Required]
        public int SiteId { get; set; }
        public int? ToUserId { get; set; }
        [Required]
        public int FromUserId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Subject { get; set; }
        [Required]
        [MaxLength(2000)]
        public string Body { get; set; }
        [Required]
        public bool IsNew { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
        public int? InReplyToId { get; set; }
        public int? ThreadId { get; set; }
    }
}
