using AutoMapper;
using backend.Authorization;
using backend.Models;
using backend.Tests.Data;
using backend.Services;
using backend.Helpers.Wrappers;
using backend.Helpers;
using Moq;
using System.Linq.Expressions;
using MongoDB.Driver;

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
        [InlineData(true)] //no user has the same username in database
        [InlineData(false)] //user with the same username exists
        public async Task AccountsServices_Register_ReturnAuthenticateResponseOrException(bool isUsernameValid)
        {
            var request = AccountsMockData.GetSampleRegistrationRequest();
            var sampleAuthenticateResponse = AccountsMockData.GetSampleAuthenticateResponse();
            var sampleUser = AccountsMockData.GetSampleUser();

            if (isUsernameValid)
            {
                //Arrange
                _mongoDatabase.Setup(mock => mock.GetCollection<User>(It.IsAny<string>(), null)).Returns(_userContext.Object);
                _userContext.Setup(mock => mock.Find(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(AccountsMockData.GetSampleNullUser());
                _userContext.Setup(mock => mock.CountDocumentsAsync(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(sampleUser.UserId - 1);
                _bCryptWrapper.Setup(mock => mock.HashPassword(It.IsAny<string>())).Returns(sampleUser.Password);
                _userContext.Setup(mock => mock.InsertOneAsync(It.IsAny<User>())).Returns(Task.CompletedTask);
                _jwtUtils.Setup(mock => mock.GenerateToken(It.IsAny<User>())).Returns(sampleAuthenticateResponse.Token);

                var service = new AccountsServices(_mongoDatabase.Object, _mapper, _jwtUtils.Object, _bCryptWrapper.Object);

                //Act
                var action = async () => await service.Register(request);
                var response = await action.Invoke();

                //Assert
                Assert.NotNull(response);
                Assert.Equal(response.Token, sampleAuthenticateResponse.Token);
            }
            else
            {
                //Arrange
                _mongoDatabase.Setup(mock => mock.GetCollection<User>(It.IsAny<string>(), null)).Returns(_userContext.Object);
                _userContext.Setup(mock => mock.Find(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(sampleUser);
                var service = new AccountsServices(_mongoDatabase.Object, _mapper, _jwtUtils.Object, _bCryptWrapper.Object);

                //Act & Assert
                await Assert.ThrowsAnyAsync<AppException>(async () => await service.Register(request));
            }
        }

        [Theory]
        [InlineData(false,false)] //user doesn't exist
        [InlineData(true, false)] //user exists, but password is wrong
        [InlineData(true,true)] //user exists and password is correct
        public async Task AccountsServices_Login_ReturnAuthenticateResponseOrException(bool doesUserExist, bool isPasswordValid)
        {
            var request = AccountsMockData.GetSampleLoginRequest();
            var sampleAuthenticateResponse = AccountsMockData.GetSampleAuthenticateResponse();
            User sampleUser;

            if(doesUserExist)
            {
                sampleUser = AccountsMockData.GetSampleUser();
            }
            else
            {
                sampleUser = AccountsMockData.GetSampleNullUser();
            }

            //Arrange
            _mongoDatabase.Setup(mock => mock.GetCollection<User>(It.IsAny<string>(), null)).Returns(_userContext.Object);
            _userContext.Setup(mock => mock.Find(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(sampleUser);
            _bCryptWrapper.Setup(mock => mock.Verify(It.IsAny<string>(),It.IsAny<string>())).Returns(isPasswordValid);
            _jwtUtils.Setup(mock => mock.GenerateToken(It.IsAny<User>())).Returns(sampleAuthenticateResponse.Token);

            var service = new AccountsServices(_mongoDatabase.Object, _mapper, _jwtUtils.Object, _bCryptWrapper.Object);

            //Act & Assert
            var action = async () => await service.Login(request);

            if(doesUserExist && isPasswordValid)
            {
                //Act
                var response = await action.Invoke();
                //Assert
                Assert.NotNull(response);
                Assert.Equal(response.Token, sampleAuthenticateResponse.Token);
            }
            else
            {
                await Assert.ThrowsAsync<AppException>(action);
            }
        }

        [Theory]
        [InlineData(true)] //user was found with the provided userId
        [InlineData(false)] //user was not found with the provided userId
        public async Task AccountsServices_GetUser_ReturnUserOrException(bool doesUserExist)
        {
            User sampleUser;
            if(doesUserExist)
            {
                sampleUser = AccountsMockData.GetSampleUser();
            }
            else
            {
                sampleUser = AccountsMockData.GetSampleNullUser();
            }

            //Arrange
            _mongoDatabase.Setup(mock => mock.GetCollection<User>(It.IsAny<string>(), null)).Returns(_userContext.Object);
            _userContext.Setup(mock => mock.Find(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(sampleUser);
            var service = new AccountsServices(_mongoDatabase.Object, _mapper, _jwtUtils.Object, _bCryptWrapper.Object);

            //Act
            var action = async () => await service.GetUser(1);

            //Assert

            if (doesUserExist)
            {
                var response = await action.Invoke();
                Assert.NotNull(response);
                Assert.Equal(response, sampleUser);
            }
            else
            {
                await Assert.ThrowsAsync<AppException>(action);
            }
            
        }

        [Theory]
        [InlineData(true)] //user was found with the provided email
        [InlineData(false)] //user was not found with the provided email
        public async void AccountsServices_UpdatePassword_ReturnsVoidOrException(bool doesUserExist)
        {
            var sampleUpdatePasswordRequest = AccountsMockData.GetSamplePasswordUpdateRequest();
            User sampleUser;

            if(doesUserExist)
            {
                sampleUser = AccountsMockData.GetSampleUser();
            }
            else
            {
                sampleUser = AccountsMockData.GetSampleNullUser();
            }

            //Arrange
            _mongoDatabase.Setup(mock => mock.GetCollection<User>(It.IsAny<string>(), null)).Returns(_userContext.Object);
            _userContext.Setup(mock => mock.Find(It.IsAny<Expression<Func<User, bool>>>())).ReturnsAsync(sampleUser);
            _bCryptWrapper.Setup(mock => mock.HashPassword(It.IsAny<string>())).Returns(sampleUpdatePasswordRequest.Password);
            _userContext.Setup(mock => mock.UpdateOneAsync(It.IsAny<Expression<Func<User,bool>>>(),It.IsAny<UpdateDefinition<User>>())).Returns(Task.CompletedTask);
            var service = new AccountsServices(_mongoDatabase.Object, _mapper, _jwtUtils.Object, _bCryptWrapper.Object);

            //Act
            var action = async () => await service.UpdatePassword(sampleUpdatePasswordRequest);

            //Assert
            if(doesUserExist)
            {
                await action.Invoke();
                _mongoDatabase.Verify(mock => mock.GetCollection<User>(It.IsAny<string>(), null),Times.Once);
                _userContext.Verify(mock => mock.Find(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
                _bCryptWrapper.Verify(mock => mock.HashPassword(It.IsAny<string>()), Times.Once);
                _userContext.Verify(mock => mock.UpdateOneAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<UpdateDefinition<User>>()), Times.Once);
            }
            else
            {
                await Assert.ThrowsAsync<AppException>(action);
            }

        }

        [Fact]
        public async Task AccoutsServices_UpdateEmail_ReturnsVoidOrException()
        {
            var sampleUpdateEmailRequest = AccountsMockData.GetSampleEmailUpdateRequest();
            var sampleUser = AccountsMockData.GetSampleUser();

            //Arrange
            _mongoDatabase.Setup(mock => mock.GetCollection<User>(It.IsAny<string>(), null)).Returns(_userContext.Object);
            _userContext.Setup(mock => mock.UpdateOneAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<UpdateDefinition<User>>())).Returns(Task.CompletedTask);
            var service = new AccountsServices(_mongoDatabase.Object, _mapper, _jwtUtils.Object, _bCryptWrapper.Object);

            //Act
            await service.UpdateEmail(sampleUser.UserId, sampleUpdateEmailRequest);

            //Assert
            _mongoDatabase.Verify(mock => mock.GetCollection<User>(It.IsAny<string>(), null), Times.Once);
            _userContext.Verify(mock => mock.UpdateOneAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<UpdateDefinition<User>>()), Times.Once);
        }

        [Fact]
        public async Task AccoutsServices_DeleteAccount_ReturnsVoidOrException()
        {
            //Arrange
            _mongoDatabase.Setup(mock => mock.GetCollection<User>(It.IsAny<string>(), null)).Returns(_userContext.Object);
            _userContext.Setup(mock => mock.DeleteOneAsync(It.IsAny<Expression<Func<User, bool>>>())).Returns(Task.CompletedTask);
            var service = new AccountsServices(_mongoDatabase.Object, _mapper, _jwtUtils.Object, _bCryptWrapper.Object);

            //Act
            await service.DeleteAccount(0);

            //Assert
            _mongoDatabase.Verify(mock => mock.GetCollection<User>(It.IsAny<string>(), null), Times.Once);
            _userContext.Verify(mock => mock.DeleteOneAsync(It.IsAny<Expression<Func<User, bool>>>()), Times.Once);
        }
    }
}
