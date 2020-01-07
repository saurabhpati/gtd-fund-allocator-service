using AutoMapper;
using GTDFundAllocatorService.Repository.Shared;

namespace GTDFundAllocatorService.Domain.Shared
{
    public class DomainProfile : Profile
    {
        public DomainProfile()
        {
            CreateMap<User, UserModel>().ReverseMap();
            CreateMap<UserRole, UserRoleModel>().ReverseMap();
            CreateMap<Role, RoleModel>().ReverseMap();
            CreateMap<Status, StatusModel>().ReverseMap();
            CreateMap<Fund, FundModel>().ReverseMap();
        }
    }
}
