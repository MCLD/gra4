using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GRA.CommandLine.Base;
using GRA.Domain.Model.Filters;
using GRA.Domain.Service;
using Microsoft.Extensions.CommandLineUtils;

namespace GRA.CommandLine.Commands
{
    internal class UserCommand : BaseCommand
    {
        private readonly DataGenerator.DateTime _dateTimeDataGenerator;
        private readonly DataGenerator.User _userDataGenerator;
        private readonly ReportService _reportService;
        public UserCommand(ServiceFacade facade,
            DataGenerator.DateTime dateTimeDataGenerator,
            DataGenerator.User userDataGenerator,
            ReportService reportService) : base(facade)
        {
            _dateTimeDataGenerator = dateTimeDataGenerator
                ?? throw new ArgumentNullException(nameof(dateTimeDataGenerator));
            _userDataGenerator = userDataGenerator
                ?? throw new ArgumentNullException(nameof(userDataGenerator));
            _reportService = reportService
                ?? throw new ArgumentNullException(nameof(reportService));

            _facade.App.Command("user", _ =>
            {
                _.Description = "Create, read, update, or delete users";
                _.HelpOption("-?|-h|--help");

                var createRandomOption = _.Option("-cr|--createrandom <count>",
                    "Create <count> random users",
                    CommandOptionType.SingleValue);

                var displayStatusOption = _.Option("-q|--quiet",
                    "Suppress status while creating users",
                    CommandOptionType.NoValue);

                var countCommand = _.Command("count", _c =>
                {
                    _c.Description = "Get a total number of users in a site.";
                    _c.HelpOption("-?|-h|--help");

                    _c.OnExecute(async () =>
                    {
                        await EnsureUserAndSiteLoaded();
                        return await DisplayUserCount();
                    });
                });

                _.OnExecute(async () =>
                {
                    bool quiet = displayStatusOption.HasValue()
                        && displayStatusOption.Value().Equals("on", StringComparison.CurrentCultureIgnoreCase);

                    if (createRandomOption.HasValue())
                    {
                        if (!int.TryParse(createRandomOption.Value(), out int howMany))
                        {
                            throw new ArgumentException("Error: <count> must be a number of users to create.");
                        }
                        return await CreateUsers(howMany, quiet);
                    }
                    else
                    {
                        _.ShowHelp();
                        return 2;
                    }
                });
            }, throwOnUnexpectedArg: true);
        }

        private async Task<int> DisplayUserCount()
        {
            var users = await _facade.UserService.GetPaginatedUserListAsync(new UserFilter());
            var report = await _reportService
                .GetCurrentStatsAsync(new Domain.Model.StatusSummary());
            Console.WriteLine($"Total users in {Site.Name}: {users.Count}; achievers: {report.Achievers}");
            return 0;
        }

        private async Task<int> CreateUsers(int howMany, bool quiet)
        {
            int created = 0;

            var issues = new List<string>();

            // make the participants
            var users = await _userDataGenerator.Generate(Site.Id, howMany);

            var minDateTime = DateTime.MaxValue;
            var maxDateTime = DateTime.MinValue;

            if (!quiet)
            {
                Console.Write($"Inserting {howMany} users... ");
            }

            ProgressBar progress = quiet ? null : new ProgressBar();
            try
            {
                // insert the participants
                foreach (var user in users)
                {
                    // set an appropriate random date and time for insertion
                    var setDateTime = _dateTimeDataGenerator.SetRandom(Site);

                    if (setDateTime < minDateTime)
                    {
                        minDateTime = setDateTime;
                    }
                    if (setDateTime > maxDateTime)
                    {
                        maxDateTime = setDateTime;
                    }

                    // insert the created user
                    try
                    {
                        await _facade
                            .UserService
                            .RegisterUserAsync(user.User, user.Password, user.SchoolDistrictId);
                        created++;
                    }
                    catch (GraException gex)
                    {
                        issues.Add($"Username: {user.User.Username} - {gex.Message}");
                    }
                    if (progress != null)
                    {
                        progress.Report((double)created / howMany);
                    }
                }
            }
            finally
            {
                if (progress != null)
                {
                    progress.Dispose();
                }
            }

            Console.WriteLine($"Created {created} random users in {Site.Name}.");
            Console.WriteLine($"Users registered between {minDateTime} and {maxDateTime}.");

            if (issues.Count > 0)
            {
                Console.WriteLine("Some issues were encountered:");
                foreach (string issue in issues)
                {
                    Console.WriteLine($"- {issue}");
                }
            }

            await DisplayUserCount();
            return howMany == created ? 0 : 1;
        }
    }
}
