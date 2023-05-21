using AutoMapper;
using backend.Models;

namespace backend.Helpers
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<RegistrationRequest, User>();
            CreateMap<User, AuthenticateResponse>();
        }
    }
}
