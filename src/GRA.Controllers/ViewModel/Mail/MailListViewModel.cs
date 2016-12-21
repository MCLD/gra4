using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.Mail
{
    public class MailListViewModel
    {
        public List<MailItemViewModel> Mail { get; set; }
        public PaginateViewModel PaginateModel { get; set; }
    }
}
