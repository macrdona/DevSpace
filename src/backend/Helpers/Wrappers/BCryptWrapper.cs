using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt;
using BCrypt.Net;

namespace backend.Helpers.Wrappers
{
    public interface IBCryptWrapper
    {
        string HashPassword(string inputKey);
        bool Verify(string text, string hash);
    }

    public class BCryptWrapper : IBCryptWrapper
    {

        public string HashPassword(string inputKey)
        {
            var hash = BCrypt.Net.BCrypt.HashPassword(inputKey);
            return hash;
        }

        public bool Verify(string text, string hash)
        {
            var isValid = BCrypt.Net.BCrypt.Verify(text, hash);
            return isValid;
        }
    }
}
