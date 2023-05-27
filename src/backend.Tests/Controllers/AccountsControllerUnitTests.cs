using backend.Models;
using backend.Services;
using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Http;
using backend.Controllers;
using Microsoft.AspNetCore.Mvc;
using backend.Tests.Data;

namespace backend.Tests.Controllers
{
    public class AccountsControllerUnitTests
    {
        private readonly Mock<IAccountServices> _accountServices;
        private readonly Mock<IHttpContextAccessor> _contextAccessor;

        public AccountsControllerUnitTests()
        {
            _accountServices = new Mock<IAccountServices>();
            _contextAccessor = new Mock<IHttpContextAccessor>();
        }

        [Theory]
        [MemberData(nameof(AccountsMockData.GetSampleRegistrationRequestModel), MemberType = typeof(AccountsMockData))]
        public async Task AccountsController_Register_ReturnOk(RegistrationRequest request, User sampleUser, AuthenticateResponse sampleAuthenticateResponse)
        {
            //Arrange

            /*bypassing call (using a mock) and return a sample data*/
            _contextAccessor.Setup(mock => mock.HttpContext.Items["User"]).Returns(sampleUser);
            _accountServices.Setup(mock => mock.Register(request)).ReturnsAsync(sampleAuthenticateResponse);
            var controller = new AccountsController(_accountServices.Object, _contextAccessor.Object);
            

            //Act 
            var result = await controller.Registration(request);

            //Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [MemberData(nameof(AccountsMockData.GetSampleLoginRequestModel), MemberType = typeof(AccountsMockData))]
        public async Task AccountsController_Login_ReturnsOk(LoginRequest request, User sampleUser, AuthenticateResponse sampleAuthenticateResponse)
        {
            //Arrange

            /*bypassing call (using a mock) and return a sample data*/
            _contextAccessor.Setup(mock => mock.HttpContext.Items["User"]).Returns(sampleUser);
            _accountServices.Setup(mock => mock.Login(request)).ReturnsAsync(sampleAuthenticateResponse);
            var controller = new AccountsController(_accountServices.Object, _contextAccessor.Object);

            //Act
            var result = await controller.Login(request);

            //Assert
            Assert.NotNull(result);
            Assert.IsType<OkObjectResult>(result);
        }
    }
}