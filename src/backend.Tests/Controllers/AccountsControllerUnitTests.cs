using backend.Models;
using backend.Services;
using Xunit;
using FluentAssertions;
using Moq;
using Microsoft.AspNetCore.Http;
using backend.Controllers;
using Microsoft.AspNetCore.Mvc;
using backend.Tests.Data;
using System.Web.Http.Results;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
        [InlineData(true)]
        [InlineData(false)]
        public async Task AccountsController_Register_ReturnOkorBadRequest(bool isModelStateValid)
        {
            //Arrange
            RegistrationRequest request;
            var sampleUser = AccountsMockData.GetSampleUser();
            var sampleAuthenticateResponse = AccountsMockData.GetSampleAuthenticateResponse();

            if(isModelStateValid)
            {
                request = AccountsMockData.GetSampleRegistrationRequest();
            }
            else
            {
                request = new RegistrationRequest();
            }

            /*bypassing call (using a mock) and return a sample data*/
            _contextAccessor.Setup(mock => mock.HttpContext.Items["User"]).Returns(sampleUser);
            _accountServices.Setup(mock => mock.Register(request)).ReturnsAsync(sampleAuthenticateResponse);
            var controller = new AccountsController(_accountServices.Object, _contextAccessor.Object);

            //Act 
            var action = async () => await controller.Registration(request);

            //Assert
            if (isModelStateValid)
            {
                var result = await action.Invoke();
                Assert.NotNull(result);
                Assert.IsType<OkObjectResult>(result);
            }
            else
            {
                controller.ModelState.AddModelError("Username", "Required");
                var result = await action.Invoke();
                Assert.IsType<BadRequestObjectResult>(result);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task AccountsController_Login_ReturnsOkOrBadRequest(bool isModelStateValid)
        {
            //Arrange
            LoginRequest request;
            var sampleUser = AccountsMockData.GetSampleUser();
            var sampleAuthenticateResponse = AccountsMockData.GetSampleAuthenticateResponse();

            if (isModelStateValid)
            {
                request = AccountsMockData.GetSampleLoginRequest();
            }
            else
            {
                request = new LoginRequest();
            }

            /*bypassing call (using a mock) and return a sample data*/
            _contextAccessor.Setup(mock => mock.HttpContext.Items["User"]).Returns(sampleUser);
            _accountServices.Setup(mock => mock.Login(request)).ReturnsAsync(sampleAuthenticateResponse);
            var controller = new AccountsController(_accountServices.Object, _contextAccessor.Object);

            //Act
            var action = async () => await controller.Login(request);

            //Assert
            if(isModelStateValid)
            {
                var result = await action.Invoke();
                Assert.NotNull(result);
                Assert.IsType<OkObjectResult>(result);
            }
            else
            {
                controller.ModelState.AddModelError("Username", "Required");
                var result = await action.Invoke();
                Assert.IsType<BadRequestObjectResult>(result);
            }
            
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task AccountsController_UpdatePassword_ReturnsOkOrBadRequest(bool isModelStateValid)
        {
            //Arrange
            PasswordUpdateRequest request;
            var sampleUser = AccountsMockData.GetSampleUser();

            if (isModelStateValid)
            {
                request = AccountsMockData.GetSamplePasswordUpdateRequest();
            }
            else
            {
                request = new PasswordUpdateRequest();
            }

            /*bypassing call (using a mock) and return a sample data*/
            _contextAccessor.Setup(mock => mock.HttpContext.Items["User"]).Returns(sampleUser);
            _accountServices.Setup(mock => mock.UpdatePassword(request)).Returns(Task.CompletedTask);
            var controller = new AccountsController(_accountServices.Object, _contextAccessor.Object);

            //Act
            var action = async () => await controller.UpdatePassword(request);

            //Assert
            if (isModelStateValid)
            {
                var result = await action.Invoke();
                Assert.NotNull(result);
                Assert.IsType<OkObjectResult>(result);
            }
            else
            {
                controller.ModelState.AddModelError("Email", "Required");
                var result = await action.Invoke();
                Assert.IsType<BadRequestObjectResult>(result);
            }
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task AccountsController_UpdateEmail_ReturnsOkOrBadRequest(bool isModelStateValid)
        {
            //Arrange
            EmailUpdateRequest request;
            var sampleUser = AccountsMockData.GetSampleUser();

            if (isModelStateValid)
            {
                request = AccountsMockData.GetSampleEmailUpdateRequest();
            }
            else
            {
                request = new EmailUpdateRequest();
            }

            /*bypassing call (using a mock) and return a sample data*/
            _contextAccessor.Setup(mock => mock.HttpContext.Items["User"]).Returns(sampleUser);
            _accountServices.Setup(mock => mock.UpdateEmail(It.IsAny<int>(),request)).Returns(Task.CompletedTask);
            var controller = new AccountsController(_accountServices.Object, _contextAccessor.Object);

            //Act
            var action = async () => await controller.UpdateEmail(request);

            //Assert
            if (isModelStateValid)
            {
                var result = await action.Invoke();
                Assert.NotNull(result);
                Assert.IsType<OkObjectResult>(result);
            }
            else
            {
                controller.ModelState.AddModelError("Email", "Required");
                var result = await action.Invoke();
                Assert.IsType<BadRequestObjectResult>(result);
            }
        }

        [Fact]
        public async Task AccountsController_DeleteAccount_ReturnsOkOrBadRequest()
        {
            //Arrange
            var sampleUser = AccountsMockData.GetSampleUser();

            /*bypassing call (using a mock) and return a sample data*/
            _contextAccessor.Setup(mock => mock.HttpContext.Items["User"]).Returns(sampleUser);
            _accountServices.Setup(mock => mock.DeleteAccount(It.IsAny<int>())).Returns(Task.CompletedTask);
            var controller = new AccountsController(_accountServices.Object, _contextAccessor.Object);

            //Act
            var result = await controller.DeleteAccount();

            //Assert
            Assert.IsType<OkObjectResult>(result);
        }
    }
}