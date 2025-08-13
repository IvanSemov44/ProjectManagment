using AutoMapper;
using Contracts.Users;
using Domain;

namespace ProjectManagement.Mapping
{
    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            CreateMap<RegisterUserRequest, User >();
        }
    }
}
