using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Restaurant_Management_System.Common
{
    public class PasswordBase64
    {
        public static string EncryptPassword(string text)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(text);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string DecryptPassword(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}