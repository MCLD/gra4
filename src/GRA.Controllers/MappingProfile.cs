using GRA.Controllers.ViewModel.Challenges;
using GRA.Controllers.ViewModel.Join;
using GRA.Domain.Model;

namespace GRA.Controllers
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<ViewModel.Shared.ProgramViewModel, GRA.Domain.Model.Program>().ReverseMap();
            CreateMap<SinglePageViewModel, User>().ReverseMap();
            CreateMap<TaskDetailViewModel, ChallengeTask>().ReverseMap();
        }
    }
}
