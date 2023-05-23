using backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.Tests.Data
{
    internal class UserMockData
    {
        public static User GetSampleUser()
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

        public static AuthenticateResponse GetSampleAuthenticateResponseModel()
        {
            var authenticateResponse = new AuthenticateResponse()
            {
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJ1c2VybmFtZSI6InN0cmluZyIsImZpcnN0Tm" +
                "FtZSI6InN0cmluZyIsImxhc3ROYW1lIjoic3RyaW5nIiwibmJmIjoxNjg0Nzc4NTM2LCJleHAiOjE2ODUzODMzMzYsImlh" +
                "dCI6MTY4NDc3ODUzNn0.Mc5XhBFju-bm8o6zOnGewYhMioHkdKnGgK1UNXKQV6Y"
            };

            return authenticateResponse;
        }

        public static RegistrationRequest GetSampleRegistrationRequestModel()
        {
            var sampleUser = GetSampleUser();
            var registerRequest = new RegistrationRequest()
            {
                Username = sampleUser.Username,
                Password = sampleUser.Password,
                FirstName = sampleUser.FirstName,
                LastName = sampleUser.LastName,
                Email = sampleUser.Email,
            };

            return registerRequest;
        }

        public static LoginRequest GetSampleLoginRequestModel()
        {
            var sampleUser = GetSampleUser();
            var loginRequest = new LoginRequest() { Username = sampleUser.Username, Password = sampleUser.Password };

            return loginRequest;
        }
    }
}
