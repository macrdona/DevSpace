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
using backend.Helpers.Wrappers;
using backend.Helpers;
using Moq;
using System.Linq.Expressions;

namespace backend.Tests.Services
{
    public class AccountsServicesUnitTests
    {
        private readonly IMongoCollectionWrapper<User> _userContext;
        private readonly IMapper _mapper;
        private readonly IJwtUtils _jwtUtils;
        public AccountsServicesUnitTests()
        {
            _mapper = A.Fake<IMapper>();
            _jwtUtils = A.Fake<IJwtUtils>();
            _userContext = A.Fake<IMongoCollectionWrapper<User>>();
        }

        [Theory]
        [MemberData(nameof(UserMockData.GetSampleRegistrationRequestModel), MemberType = typeof(UserMockData))]
        public void AccountsServices_Register_ReturnAuthenticateResponse(RegistrationRequest request, User sampleUser, AuthenticateResponse sampleAuthenticateResponse)
        {
            //Arrange
            var findFluentWrapper = A.Fake<IFindFluentWrapper<User>>();
            var bCryptWrapper = A.Fake<IBCryptWrapper>();
            var service = new AccountsServices(_userContext, _mapper, _jwtUtils);

            A.CallTo(() => _userContext.Find(x => x.Username == request.Username, null)).Returns(null);
            A.CallTo(() => findFluentWrapper.FirstOrDefaultAsync()).Returns(Task.FromResult<User>(null));
            A.CallTo(() => _mapper.Map<User>(request)).Returns(sampleUser);
            A.CallTo(() => _userContext.CountDocumentsAsync(x => x.Username == sampleUser.Username,null,default)).Returns(sampleUser.UserId+1);
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

        [Theory]
        [MemberData(nameof(UserMockData.GetSampleRegistrationRequestModel), MemberType = typeof(UserMockData))]
        public void AccountsServices_Register_ThrowsAppException(RegistrationRequest request, User sampleUser, AuthenticateResponse sampleAuthenticateResponse)
        {
            //Arrange
            //var findFluentWrapper = A.Fake<IFindFluentWrapper<User>>();
            var i = A.Fake<AccountsServices>();
            var service = new AccountsServices(_userContext, _mapper, _jwtUtils);

            A.CallTo(() => i.FindUser(x => x.Username == request.Username)).Returns(Task.FromResult(sampleUser));
            //A.CallTo(() => findFluentWrapper.FirstOrDefaultAsync()).Returns(sampleUser);

            //Act
            //Assert

            Assert.ThrowsAsync<AppException>(() => service.Register(request));
        }
    }
}
