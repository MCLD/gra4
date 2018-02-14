﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class HouseholdAddViewModel
    {
        public Domain.Model.User User { get; set; }
        public int Id { get; set; }
        public bool RequirePostalCode { get; set; }
        public bool ShowAge { get; set; }
        public bool ShowSchool { get; set; }
        public int? SchoolDistrictId { get; set; }
        public int? SchoolTypeId { get; set; }
        public string ProgramJson { get; set; }
        public SelectList BranchList { get; set; }
        public SelectList ProgramList { get; set; }
        public SelectList SystemList { get; set; }
        public SelectList SchoolList { get; set; }
        public SelectList SchoolDistrictList { get; set; }
        public SelectList SchoolTypeList { get; set; }
        public SelectList AskFirstTime { get; set; }

        [DisplayName("Is this your first time participating?")]
        [Required(ErrorMessage = "Please let us know if this is your first time participating in the program")]
        public string IsFirstTime { get; set; }

    }
}
