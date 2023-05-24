using backend.Models;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace backend.Tests.Data
{
    public class UserMockData : IEnumerable<object[]>
    {
        public static IEnumerable<object[]> GetSampleRegistrationRequestModel()
        {
            yield return new object[] {

                new RegistrationRequest()
                {
                    Username = "string3",
                    FirstName = "string3",
                    LastName = "string3",
                    Email = "string3@gmail.com",
                    Password = "string3"
                },
                new User()
                {
                    UserId = 3,
                    Username = "string3",
                    FirstName = "string3",
                    LastName = "string3",
                    Email = "string3@gmail.com",
                    Password = "string3"
                },
                new AuthenticateResponse()
                {
                    Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJ1c2VybmFtZSI6InN0cmluZyIsImZpcnN0Tm" +
                    "FtZSI6InN0cmluZyIsImxhc3ROYW1lIjoic3RyaW5nIiwibmJmIjoxNjg0Nzc4NTM2LCJleHAiOjE2ODUzODMzMzYsImlh" +
                    "dCI6MTY4NDc3ODUzNn0.Mc5XhBFju-bm8o6zOnGewYhMioHkdKnGgK1UNXKQV6Y"
                }
            };
        }

        public static IEnumerable<object[]> GetSampleLoginRequestModel()
        {
            yield return new object[] { 
                new LoginRequest() { Username = "string1", Password = "string1" },
                new User()
                {
                    UserId = 1,
                    Username = "string1",
                    FirstName = "string1",
                    LastName = "string1",
                    Email = "string1@gmail.com",
                    Password = "string1"
                },
                new AuthenticateResponse()
                {
                    Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpZCI6IjEiLCJ1c2VybmFtZSI6InN0cmluZyIsImZpcnN0Tm" +
                    "FtZSI6InN0cmluZyIsImxhc3ROYW1lIjoic3RyaW5nIiwibmJmIjoxNjg0Nzc4NTM2LCJleHAiOjE2ODUzODMzMzYsImlh" +
                    "dCI6MTY4NDc3ODUzNn0.Mc5XhBFju-bm8o6zOnGewYhMioHkdKnGgK1UNXKQV6Y"
                }
            };
        }

        public IEnumerator<object[]> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
