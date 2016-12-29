using System;

namespace GRA.Domain.Model
{
    public class DrawingWinner
    {
        public int DrawingId { get; set; }
        public int UserId { get; set; }
        public DateTime? RedeemedAt { get; set; }
        public int? MailId { get; set; }
    }
}
