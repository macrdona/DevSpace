using backend.Models;
using backend.Services;
using Xunit;
using FluentAssertions;
using FakeItEasy;
using Microsoft.AspNetCore.Http;
using backend.Controllers;
using Microsoft.AspNetCore.Mvc;
using backend.Tests.Data;

namespace backend.Tests.Controllers
{
    public class AccountsControllerUnitTests
    {
        private readonly IAccountServices _accountServices;
        private readonly IHttpContextAccessor _contextAccessor;

        public AccountsControllerUnitTests()
        {
            _accountServices = A.Fake<IAccountServices>();
            _contextAccessor = A.Fake<IHttpContextAccessor>();
        }

        [Theory]
        [MemberData(nameof(UserMockData.GetSampleRegistrationRequestModel), MemberType = typeof(UserMockData))]
        public void AccountsController_Register_ReturnOk(RegistrationRequest request, User sampleUser, AuthenticateResponse sampleAuthenticateResponse)
        {
            //Arrange

            /*bypassing call (using a mock) and return a sample data*/
            A.CallTo(() => _contextAccessor.HttpContext.Items["User"]).Returns(sampleUser);

            var controller = new AccountsController(_accountServices, _contextAccessor);
            A.CallTo(() => _accountServices.Register(request)).Returns(sampleAuthenticateResponse);

            //Act 
            var result = controller.Registration(request).Result;

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Theory]
        [MemberData(nameof(UserMockData.GetSampleLoginRequestModel), MemberType = typeof(UserMockData))]
        public void AccountsController_Login_ReturnsOk(LoginRequest request, User sampleUser, AuthenticateResponse sampleAuthenticateResponse)
        {
            //Arrange

            /*bypassing call (using a mock) and return a sample data*/
            A.CallTo(() => _contextAccessor.HttpContext.Items["User"]).Returns(sampleUser);

            /*Creating fake objects and fake data samples*/
            var controller = new AccountsController(_accountServices, _contextAccessor);

            /*bypassing call (using a mock) and return a sample data*/
            A.CallTo(() => _accountServices.Login(request)).Returns(sampleAuthenticateResponse);

            //Act
            var result = controller.Login(request).Result;

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }
    }
}