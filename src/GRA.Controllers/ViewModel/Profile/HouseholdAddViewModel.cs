using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.Profile
{
    public class HouseholdAddViewModel
    {
        public Domain.Model.User User { get; set; }
        public bool RequirePostalCode { get; set; }
        public SelectList BranchList { get; set; }
        public SelectList ProgramList { get; set; }
        public SelectList SystemList { get; set; }
    }
}
