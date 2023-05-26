using backend.Helpers;
using backend.Helpers.Wrappers;
using backend.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using AutoMapper;
using BCrypt.Net;
using backend.Authorization;
using System.Linq.Expressions;
using Amazon.Runtime.Internal;

namespace backend.Services
{
    public interface IAccountServices
    {
        public Task<AuthenticateResponse> Register(RegistrationRequest request);
        public Task<AuthenticateResponse> Login(LoginRequest request);
        public Task<User> GetUser(int id);
    }

    public class AccountsServices : IAccountServices
    {
        private readonly IMapper _mapper;
        private readonly IJwtUtils _jwtUtils;
        private readonly IMongoCollectionWrapper<User> _userContext;
        private readonly IBCryptWrapper _bCryptWrapper;

        public AccountsServices(IMongoDatabaseWrapper mongoDatabase, IMapper mapper, IJwtUtils jwtUtils, IBCryptWrapper bCryptWrapper)
        {
            _mapper = mapper;
            _jwtUtils = jwtUtils;
            _userContext = mongoDatabase.GetCollection<User>("Accounts");
            _bCryptWrapper = bCryptWrapper;
        }

        public async Task<AuthenticateResponse> Register(RegistrationRequest request)
        {
            try
            {
                var user =  await _userContext.Find(x => x.Username == request.Username);
                AuthenticateResponse response;

                if (user != null)
                {
                    throw new AppException("Username '" + request.Username + "' is already taken");
                }
                else
                {
                    user = _mapper.Map<User>(request);
                    user.UserId = (int)await _userContext.CountDocumentsAsync(x => x.Username != request.Username) + 1;
                    user.Password = _bCryptWrapper.HashPassword(user.Password);
                    await _userContext.InsertOneAsync(user);

                    response = _mapper.Map<AuthenticateResponse>(user);
                    response.Token = _jwtUtils.GenerateToken(user);
                }

                return response;
            }
            catch (Exception ex)
            {
                throw new AppException(ex.Message);
            }
            
        }

        public async Task<AuthenticateResponse> Login(LoginRequest request)
        {
            try
            {
                var user = await _userContext.Find(x => x.Username == request.Username);

                if (user == null || !_bCryptWrapper.Verify(request.Password, user.Password))
                {
                    throw new AppException("Username or Password is incorrect");
                }

                var response = _mapper.Map<AuthenticateResponse>(user);
                response.Token = _jwtUtils.GenerateToken(user);

                return response;
            }
            catch (Exception ex)
            {
                throw new AppException(ex.Message);
            }
        }

        public async Task<User> GetUser(int id)
        {
            try
            {
                var user = await _userContext.Find(x => x.UserId == id);

                if (user == null) throw new AppException("User not found");

                return user;
            }
            catch (Exception ex)
            {
                throw new AppException(ex.Message);
            }
        }
    }
}
