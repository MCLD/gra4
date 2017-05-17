﻿using System;
using System.Collections.Generic;
using System.Linq;
using Bogus;
using GRA.Domain.Service;
using System.Threading.Tasks;
using GRA.Domain.Model;

namespace GRA.CommandLine.DataGenerator
{
    internal class User
    {
        private readonly SchoolService _schoolService;
        private readonly SiteService _siteService;
        public User(SchoolService schoolService,
            SiteService siteService)
        {
            _schoolService = schoolService 
                ?? throw new ArgumentNullException(nameof(schoolService));
            _siteService = siteService ?? throw new ArgumentNullException(nameof(siteService));
        }
        public async Task<IEnumerable<(Domain.Model.User user, string pass, int? schoolDistrictId)>>
            Generate(int siteId, int count)
        {
            var branches = await _siteService.GetAllBranches(true);
            var programs = await _siteService.GetProgramList();
            var schools = await _schoolService.GetSchoolsAsync();

            var testUsers = new Faker<Domain.Model.User>()
                .Rules((f, u) =>
                {
                    var branch = f.PickRandom(branches);
                    var program = f.PickRandom(programs);
                    u.BranchId = branch.Id;
                    u.Email = f.Person.Email;
                    u.FirstName = f.Person.FirstName;
                    u.LastName = f.Person.LastName;
                    u.PhoneNumber = f.Phone.PhoneNumber("###-###-####");
                    u.PostalCode = f.Address.ZipCode();
                    u.ProgramId = program.Id;
                    u.SiteId = siteId;
                    u.SystemId = branch.SystemId;
                    u.Username = f.Person.UserName;
                    if (program.AskAge && (f.Random.Bool() || program.AgeRequired))
                    {
                        u.Age = f.Random.Number(0, 104);
                    }
                    if (program.AskSchool && (f.Random.Bool() || program.SchoolRequired))
                    {
                        var school = f.PickRandom(schools);
                        if (f.Random.Int(0, 100) <= 15)
                        {
                            u.EnteredSchoolName = string.Join(" ", f.Lorem.Words(f.Random.Int(1, 3)));
                        }
                        else
                        {
                            u.SchoolId = school.Id;
                        }
                    }

                });

            var rand = new Bogus.Randomizer();
            var users = new List<(Domain.Model.User user, string pass, int? schoolDistrictId)>();
            for (int i = 0; i < count; i++)
            {
                var user = testUsers.Generate();
                int? schoolDistrictId = null;
                if(!string.IsNullOrEmpty(user.EnteredSchoolName))
                {
                    var school = rand.ListItem<School>(schools.ToList());
                    schoolDistrictId = school.SchoolDistrictId;
                }
                users.Add((user, "koala123", schoolDistrictId));
            }
            return users;
        }
    }
}
