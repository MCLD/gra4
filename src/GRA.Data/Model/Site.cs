﻿using System;
using System.ComponentModel.DataAnnotations;

namespace GRA.Data.Model
{
    public class Site : Abstract.BaseDbEntity
    {
        [Required]
        [MaxLength(255)]
        public string Path { get; set; }
        [Required]
        [MaxLength(255)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string Domain { get; set; }

        public DateTime? RegistrationOpens { get; set; }
        public DateTime? ProgramStarts { get; set; }
        public DateTime? ProgramEnds { get; set; }
        public DateTime? AccessClosed { get; set; }
    }
}
