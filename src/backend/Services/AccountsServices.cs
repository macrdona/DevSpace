using backend.Helpers;
using backend.Models;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using AutoMapper;
using BCrypt.Net;

namespace backend.Services
{
    public interface IAccountServices
    {
        public async Task Register(RegistrationRequest request) { }
    }
    public class AccountsServices : IAccountServices
    {
        private readonly IMongoCollection<User> _userContext;
        private readonly IMapper _mapper;

        public AccountsServices(IOptions<DatabaseSettings> databaseSettings, IMapper mapper)
        {
            var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseSettings.Value.Database);
            _userContext = mongoDatabase.GetCollection<User>("Accounts");
            _mapper = mapper;
        }

        public async Task Register(RegistrationRequest request)
        {
            try
            {
                var user = await _userContext.Find(x => x.Username == request.Username).FirstOrDefaultAsync();
                
                if (user != null) throw new AppException("Username '" + request.Username + "' is already taken");

                //request -> user
                user = _mapper.Map<User>(request);

                var id = await _userContext.CountDocumentsAsync(x => x.Username != request.Username) + 1;
                user.UserId = id.ToString();

                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                await _userContext.InsertOneAsync(user);
            }
            catch (Exception ex)
            {
                throw new AppException(ex.Message);
            }
            
        }
    }
}
