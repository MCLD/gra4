﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GRA.Data.Model
{
    public class AuditLog : Abstract.BaseDbEntity
    {

        [Required]
        [MaxLength(255)]
        public string EntityType { get; set; }
        public int EntityId { get; set; }
        public string PreviousValue { get; set; }
        [Required]
        public string CurrentValue { get; set; }
        [Required]
        public int UpdatedBy { get; set; }
        [Required]
        public DateTime UpdatedAt { get; set; }
    }
}
