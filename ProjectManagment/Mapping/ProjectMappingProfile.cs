using AutoMapper;
using Contracts.Projects;
using Domain;

namespace ProjectManagement.Mapping
{
    public sealed class ProjectMappingProfile : Profile
    {
        public ProjectMappingProfile()
        {
            CreateMap<Project, ProjectResponse>();
            CreateMap<CreateProjectRequest, Project>();
        }
    }
}
