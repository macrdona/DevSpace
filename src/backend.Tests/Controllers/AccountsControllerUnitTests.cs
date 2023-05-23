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

            //bypassing call (using a mock) and return a sample data
            A.CallTo(() => _contextAccessor.HttpContext.Items["User"]).Returns(UserMockData.GetSampleUser());
        }

        [Fact]
        public void AccountsController_Register_ReturnOk()
        {
            //Arrange
            var sampleAuthenticateResponse = UserMockData.GetSampleAuthenticateResponseModel();
            var registerRequest = UserMockData.GetSampleRegistrationRequestModel();
            var controller = new AccountsController(_accountServices, _contextAccessor);
            A.CallTo(() => _accountServices.Register(registerRequest)).Returns(sampleAuthenticateResponse);

            //Act 
            var result = controller.Registration(registerRequest).Result;

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }

        [Fact]
        public void AccountsController_Login_ReturnsOk()
        {
            //Arrange

            /*Creating fake objects and fake data samples*/
            var sampleAuthenticateResponse = UserMockData.GetSampleAuthenticateResponseModel();
            var loginRequest = UserMockData.GetSampleLoginRequestModel();
            var controller = new AccountsController(_accountServices, _contextAccessor);

            /*bypassing call (using a mock) and return a sample data*/
            A.CallTo(() => _accountServices.Login(loginRequest)).Returns(sampleAuthenticateResponse);

            //Act
            var result = controller.Login(loginRequest).Result;

            //Assert
            result.Should().NotBeNull();
            result.Should().BeOfType(typeof(OkObjectResult));
        }
    }
}