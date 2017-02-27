﻿using System;

namespace GRA.Data.Model
{
    public class PrizeWinner : Abstract.BaseDbEntity
    {
        public int? DrawingId { get; set; }
        public Drawing Drawing { get; set; }
        public int? TriggerId { get; set; }
        public Trigger Trigger { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime? RedeemedAt { get; set; }
        public int? MailId { get; set; }
    }
}
