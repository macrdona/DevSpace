using AutoMapper;
using backend.Authorization;
using backend.Models;
using MongoDB.Driver;
using FakeItEasy;
using Microsoft.Extensions.Options;
using backend.Tests.Data;
using backend.Services;
using FluentAssertions;
using backend.Database;
using backend.Tests.Helpers;

namespace backend.Tests.Services
{
    public class AccountsServicesUnitTests
    {
        private readonly IMongoCollection<User> _userContext;
        private readonly IMapper _mapper;
        private readonly IJwtUtils _jwtUtils;
        private readonly IOptions<DatabaseSettings> _databaseSettings;
        private readonly IMongoClient _mongoClient;
        public AccountsServicesUnitTests()
        {
            _databaseSettings = A.Fake<IOptions<DatabaseSettings>>();
            _mapper = A.Fake<IMapper>();
            _jwtUtils = A.Fake<IJwtUtils>();

            _mongoClient = A.Fake<IMongoClient>();
            
            var mongoDatabase = A.Fake<IMongoDatabase>();
            A.CallTo(() => _mongoClient.GetDatabase(_databaseSettings.Value.Database,null)).Returns(mongoDatabase);

            _userContext = A.Fake<IMongoCollection<User>>();
            A.CallTo(() => mongoDatabase.GetCollection<User>("Accounts",null)).Returns(_userContext);
        }

        [Fact]
        public void AccountsServices_Register_ReturnAuthenticateResponse()
        {
            //Arrange
            var sampleUser = UserMockData.GetSampleUser();
            var sampleAuthenticateResponse = UserMockData.GetSampleAuthenticateResponseModel();
            var request = UserMockData.GetSampleRegistrationRequestModel();

            var mongoCollectionWrapper = A.Fake<IMongoCollectionWrapper<User>>();
            var findFluentWrapper = A.Fake<IFindFluentWrapper<User>>();
            var bCryptWrapper = A.Fake<IBCryptWrapper>();
            var service = new AccountsServices(_databaseSettings, _mapper, _jwtUtils, _mongoClient);

            A.CallTo(() => mongoCollectionWrapper.Find(x => x.Username == request.Username, null)).Returns(findFluentWrapper);
            A.CallTo(() => findFluentWrapper.FirstOrDefaultAsync()).Returns(sampleUser);
            A.CallTo(() => _mapper.Map<User>(request)).Returns(sampleUser);
            A.CallTo(() => mongoCollectionWrapper.CountDocumentsAsync(x => x.Username == sampleUser.Username,null,default)).Returns(sampleUser.UserId+1);
            A.CallTo(() => bCryptWrapper.HashPassword(sampleUser.Password)).Returns(sampleUser.Password);
            A.CallTo(() => _userContext.InsertOneAsync(sampleUser,null,default)).Returns(Task.CompletedTask);
            A.CallTo(() => _mapper.Map<AuthenticateResponse>(sampleUser)).Returns(sampleAuthenticateResponse);
            A.CallTo(() => _jwtUtils.GenerateToken(sampleUser)).Returns(sampleAuthenticateResponse.Token);

            //Act
            var result = service.Register(request).Result;

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType<AuthenticateResponse>();
            result.Should().BeSameAs(sampleAuthenticateResponse);
        }
    }
}
