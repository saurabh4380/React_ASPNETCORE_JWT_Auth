using AutoMapper;
using React_ASPNETCORE_JWT_Auth.DataAccess.Entities;
using React_ASPNETCORE_JWT_Auth.Models;

namespace React_ASPNETCORE_JWT_Auth.Configurations
{
    /// <summary>
    /// Config class used to configure the Automapper mappings for the respective Source and Destination classes.
    /// </summary>
    public class AutoMapperConfigurations : Profile
    {
        public AutoMapperConfigurations()
        {
            CreateMap<User, UserAuthModel>().ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.FirstName));

            CreateMap<UserAuthModel, User>().ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Name));
        }
    }
}
