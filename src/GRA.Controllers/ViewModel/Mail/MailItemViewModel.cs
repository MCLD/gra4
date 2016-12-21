namespace GRA.Controllers.ViewModel.Mail
{
    public class MailItemViewModel
    {
        public int Id { get; set; }
        public string ToFrom { get; set; }
        public string Date { get; set; }
        public string Subject { get; set; }
        public bool IsNew { get; set; }
    }
}
