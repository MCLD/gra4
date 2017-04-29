﻿using System.Collections.Generic;
using GRA.Controllers.ViewModel.Avatar;
using GRA.Controllers.ViewModel.Challenges;
using GRA.Controllers.ViewModel.Join;
using GRA.Controllers.ViewModel.MissionControl.Participants;
using GRA.Domain.Model;
using System.Linq;

namespace GRA.Controllers
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<ViewModel.Shared.ProgramViewModel, GRA.Domain.Model.Program>().ReverseMap();
            CreateMap<SinglePageViewModel, User>().ReverseMap();
            CreateMap<Step1ViewModel, User>().ReverseMap();
            CreateMap<Step2ViewModel, User>().ReverseMap();
            CreateMap<Step3ViewModel, User>().ReverseMap();
            CreateMap<ParticipantsAddViewModel, User>().ReverseMap();
            CreateMap<TaskDetailViewModel, ChallengeTask>().ReverseMap();
            CreateMap<DynamicAvatarLayer, DynamicAvatarModel.DynamicAvatarLayer>()
                .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.DynamicAvatarItems.Select(_ => _.Id)))
                .ForMember(dest => dest.Colors, opt => opt.MapFrom(src => src.DynamicAvatarColors.Select(_ => _.Color)))
                .ReverseMap();
        }
    }
}
