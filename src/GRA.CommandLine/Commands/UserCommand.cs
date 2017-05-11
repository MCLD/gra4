using System;
using System.Threading.Tasks;
using GRA.CommandLine.Base;
using GRA.Domain.Model.Filters;
using Microsoft.Extensions.CommandLineUtils;

namespace GRA.CommandLine.Commands
{
    internal class UserCommand : BaseCommand
    {
        public UserCommand(ServiceFacade facade,
            DataGenerator.User userDataGenerator) : base(facade)
        {
            _facade.App.Command("user", _ =>
            {
                _.Description = "Create, read, update, or delete users";
                _.HelpOption("-?|-h|--help");

                var createRandomOption = _.Option("-cr|--createrandom <count>",
                    "Create <count> random users",
                    CommandOptionType.SingleValue);

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
                    if (createRandomOption.HasValue())
                    {
                        if (!int.TryParse(createRandomOption.Value(), out int howMany))
                        {
                            throw new ArgumentException("Error: <count> must be a number of users to create.");
                        }
                        return await CreateUsers(userDataGenerator, howMany);
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
            Console.WriteLine($"Total users in {Site.Name}: {users.Count}");
            return 0;
        }

        private async Task<int> CreateUsers(DataGenerator.User userDataGenerator, int howMany)
        {
            int created = 0;

            // make the participants
            var users = await userDataGenerator.Generate(Site.Id, howMany);

            // insert the participants
            foreach (var user in users)
            {
                await _facade
                    .UserService
                    .RegisterUserAsync(user.user, user.pass, user.schoolDistrictId);
                created++;
            }

            Console.WriteLine($"Created {created} random users in {Site.Name}.");
            await DisplayUserCount();
            return howMany == created ? 0 : 1;
        }
    }
}
