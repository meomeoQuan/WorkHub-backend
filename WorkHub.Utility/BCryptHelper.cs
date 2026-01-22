using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;

namespace WorkHub.Utility
{
    public static class BCryptHelper
    {
        // Encode = Hash password
        public static string Encode(string plainText)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainText);
        }

        // Decode = Verify password (NOT real decoding)
        public static bool Decode(string plainText, string hashedText)
        {
            return BCrypt.Net.BCrypt.Verify(plainText, hashedText);
        }
    }
}
