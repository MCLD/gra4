namespace GRA.Controllers.ViewModel.Participants
{
    public class ParticipantsDetailViewModel
    {
        public GRA.Domain.Model.User User { get; set; }
        public int Id { get; set; }
        public bool CanEditDetails { get; set; }
    }
}
