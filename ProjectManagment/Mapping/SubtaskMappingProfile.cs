using AutoMapper;
using Contracts.Subtasks;
using Domain;

namespace ProjectManagement.Mapping
{
    public sealed class SubtaskMappingProfile : Profile
    {
        public SubtaskMappingProfile()
        {
            CreateMap<Subtask, SubtaskResponse>();
        }
    }
}
