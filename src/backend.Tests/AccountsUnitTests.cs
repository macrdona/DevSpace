using backend.Models;
using backend.Services;
using MongoDB.Driver;
using Moq;
using System.Diagnostics;
using System.Linq.Expressions;
using Xunit;
using FluentAssertions;
using FakeItEasy;
using Microsoft.Extensions.Options;
using backend.Helpers;
using AutoMapper;
using backend.Authorization;
using Autofac.Extras.Moq;

namespace backend.Tests
{
    public class AccountsUnitTests
    {
        [Fact]
        public async Task AccountServices_GetUser_ReturnsUserObject()
        {
            using (var mock = AutoMock.GetLoose())
            {
                int id = 20;
                mock.Mock<IMongoCollection<User>>()
                     .Setup(c => c.Find(It.IsAny<Expression<Func<User,bool>>>(),null))
                     .Returns(GetSampleUser());

                var _accountService = mock.Create<AccountsServices>();

                var expected = GetSampleUser();

                var actual = _accountService.GetUser(id);

                Assert.True(actual != null);
                Assert.Equal(expected, actual);
            }
        }

        private async Task<User> GetSampleUser()
        {
            var _user = new User()
            {
                UserId = 20,
                Username = "string20",
                FirstName = "string20",
                LastName = "string20",
                Email = "string20@gmail.com",
                Password = "string20"
            };

            return _user;
        }
    }
}