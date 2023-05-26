using AutoMapper;
using backend.Authorization;
using backend.Models;
using FakeItEasy;
using backend.Tests.Data;
using backend.Services;
using FluentAssertions;
using backend.Helpers.Wrappers;
using backend.Helpers;
using Moq;
using System.Linq.Expressions;


namespace backend.Tests.Services
{
    public class AccountsServicesUnitTests
    {
        private readonly Mock<IMongoCollectionWrapper<User>> _userContext;
        private readonly IMapper _mapper;
        private readonly Mock<IJwtUtils> _jwtUtils;
        private readonly Mock<IBCryptWrapper> _bCryptWrapper;
        private readonly Mock<IMongoDatabaseWrapper> _mongoDatabase;
        public AccountsServicesUnitTests()
        {
            //configure mapper for unit testing
           var profile = new Helpers.AutoMapper();
           var configuration = new MapperConfiguration(cfg => cfg.AddProfile(profile));
            _mapper = new Mapper(configuration);

            _jwtUtils = new Mock<IJwtUtils>();
            _userContext = new Mock<IMongoCollectionWrapper<User>>();
            _bCryptWrapper = new Mock<IBCryptWrapper>();
            _mongoDatabase = new Mock<IMongoDatabaseWrapper>();
        }

        [Theory]
        [MemberData(nameof(UserMockData.GetSampleRegistrationRequestModel), MemberType = typeof(UserMockData))]
        public async Task AccountsServices_Register_ReturnAuthenticateResponse(RegistrationRequest request, User sampleUser, AuthenticateResponse sampleAuthenticateResponse)
        {
            //Arrange
            User nullUser = null;

            _mongoDatabase.Setup(mock => mock.GetCollection<User>(It.IsAny<string>(), null)).Returns(_userContext.Object);
            _userContext.Setup(mock => mock.Find(It.IsAny<Expression<Func<User, bool>>>(), null)).ReturnsAsync(nullUser);
            _userContext.Setup(mock => mock.CountDocumentsAsync(It.IsAny<Expression<Func<User, bool>>>(), null, default)).ReturnsAsync(sampleUser.UserId-1);
            _bCryptWrapper.Setup(mock => mock.HashPassword(It.IsAny<string>())).Returns(sampleUser.Password);
            _userContext.Setup(mock => mock.InsertOneAsync(It.IsAny<User>(), null, default)).Returns(Task.CompletedTask);
            _jwtUtils.Setup(mock => mock.GenerateToken(It.IsAny<User>())).Returns(sampleAuthenticateResponse.Token);

            var service = new AccountsServices(_mongoDatabase.Object, _mapper, _jwtUtils.Object, _bCryptWrapper.Object);

            //Act
            var response = await service.Register(request);

            //Assert
            Assert.NotNull(response);
            Assert.Equal(response.Token,sampleAuthenticateResponse.Token);
        }

        [Theory]
        [MemberData(nameof(UserMockData.GetSampleRegistrationRequestModel), MemberType = typeof(UserMockData))]
        public async Task AccountsServices_Register_ThrowsAppException(RegistrationRequest request, User sampleUser, AuthenticateResponse sampleAuthenticateResponse)
        {
            //Arrange
            _mongoDatabase.Setup(mock => mock.GetCollection<User>(It.IsAny<string>(),null)).Returns(_userContext.Object);
            _userContext.Setup(mock => mock.Find(It.IsAny<Expression<Func<User,bool>>>(),null)).ReturnsAsync(sampleUser);
            var service = new AccountsServices(_mongoDatabase.Object, _mapper, _jwtUtils.Object, _bCryptWrapper.Object);

            //Act
            //Assert
            await Assert.ThrowsAnyAsync<AppException>(async () => await service.Register(request));
        }
    }
}
