﻿using GRA.Controllers.ViewModel.Shared;
using System.Collections.Generic;

namespace GRA.Controllers.ViewModel.MissionControl.Participants
{
    public class HouseholdListViewModel
    {
        public IEnumerable<GRA.Domain.Model.User> Users { get; set; }
        public int Id { get; set; }
        public int HouseholdCount { get; set; }
        public int PrizeCount { get; set; }
        public int? HeadOfHouseholdId { get; set; }
        public bool HasAccount { get; set; }
        public bool CanEditDetails { get; set; }
        public bool CanLogActivity { get; set; }
        public bool CanReadMail { get; set; }
        public bool CanViewPrizes { get; set; }
        public GRA.Domain.Model.User Head { get; set; }
        public string UserSelection { get; set; }
        public int MinutesRead { get; set; }
        public string MinutesReadMessage { get; set; }
        public bool ShowSecretCode { get; set; }
        public string SecretCode { get; set; }
        public string SecretCodeMessage { get; set; }

        public int SystemId { get; set; }

        public IEnumerable<GRA.Domain.Model.Branch> BranchList { get; set; }
        public IEnumerable<GRA.Domain.Model.System> SystemList { get; set; }
    }
}
