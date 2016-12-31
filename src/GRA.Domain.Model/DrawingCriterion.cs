﻿using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Domain.Model
{
    public class DrawingCriterion : Abstract.BaseDomainEntity
    {
        [Required]
        public int SiteId { get; set; }
        [MaxLength(255)]
        [Required]
        public string Name { get; set; }
        public int? ProgramId { get; set; }
        public int? SystemId { get; set; }
        public int? BranchId { get; set; }
        public int? PointsMinimum { get; set; }
        public int? PointsMaximum { get; set; }
        public DateTime? StartOfPeriod { get; set; }
        public DateTime? EndOfPeriod { get; set; }
        public int? ActivityAmount { get; set; }
        public int? PointTranslationId { get; set; }
    }
}
