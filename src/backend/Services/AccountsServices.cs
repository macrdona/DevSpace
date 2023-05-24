using backend.Helpers;
using backend.Helpers.Wrappers;
using backend.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using AutoMapper;
using BCrypt.Net;
using backend.Authorization;
using backend.Database;
using System.Linq.Expressions;
using Amazon.Runtime.Internal;

namespace backend.Services
{
    public interface IAccountServices
    {
        public Task<AuthenticateResponse> Register(RegistrationRequest request);
        public Task<AuthenticateResponse> Login(LoginRequest request);
        public Task<User> GetUser(int id);
        public Task<User> FindUser(Expression<Func<User, bool>> predicate);
    }

    public class AccountsServices : IAccountServices
    {
        private readonly IMongoCollectionWrapper<User> _userContext;
        private readonly IMapper _mapper;
        private readonly IJwtUtils _jwtUtils;

        public AccountsServices(IMongoCollectionWrapper<User> _mongoCollection, IMapper mapper, IJwtUtils jwtUtils)
        {
            _mapper = mapper;
            _jwtUtils = jwtUtils;
            _userContext = _mongoCollection;
        }

        public async Task<AuthenticateResponse> Register(RegistrationRequest request)
        {
            try
            {
                var user = await FindUser(x => x.Username == request.Username);
                
                if (user != null) throw new AppException("Username '" + request.Username + "' is already taken");

                //request -> user
                user = _mapper.Map<User>(request);

                user.UserId = (int)await _userContext.CountDocumentsAsync(x => x.Username != request.Username) + 1;

                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                await _userContext.InsertOneAsync(user);

                var response = _mapper.Map<AuthenticateResponse>(user);
                response.Token = _jwtUtils.GenerateToken(user);

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
                var user = await FindUser(x => x.Username == request.Username);

                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
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
                var user = await FindUser(x => x.UserId == id);

                if (user == null) throw new AppException("User not found");

                return user;
            }
            catch (Exception ex)
            {
                throw new AppException(ex.Message);
            }
        }

        public virtual async Task<User> FindUser(Expression<Func<User, bool>> predicate)
        {
            var query = _userContext.Find(predicate);
            return await query.FirstOrDefaultAsync();
        }

    }
}
