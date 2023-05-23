using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt;
namespace backend.Tests.Helpers
{
    public interface IBCryptWrapper
    {
        string HashPassword(string inputKey);
    }

    public class BCryptWrapper : IBCryptWrapper
    {

        public string HashPassword(string inputKey)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(inputKey);
            return hash;
        }
    }
}
