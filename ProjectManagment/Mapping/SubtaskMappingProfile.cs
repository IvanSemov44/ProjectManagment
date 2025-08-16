using AutoMapper;
using Contracts.Subtasks;
using Domain.Models;

namespace ProjectManagement.Mapping
{
    public sealed class SubtaskMappingProfile : Profile
    {
        public SubtaskMappingProfile()
        {
            CreateMap<Subtask, SubtaskResponse>();
            CreateMap<CreateSubtaskRequest, Subtask>();
            CreateMap<UpdateSubtaskRequest, Subtask>().ReverseMap();
            CreateMap<PagedList<Subtask>, PagedList<SubtaskResponse>>();
        }
    }
}
