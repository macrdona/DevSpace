using AutoMapper;
using backend.Helpers.Wrappers;
using backend.Models;
using MongoDB.Driver;

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
