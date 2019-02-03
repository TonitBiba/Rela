using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Rela_project.Services
{
    public class CustomHashPassword : IPasswordHasher
    {
        PBKDF2 pbkdf2;
        public string HashPassword(string password)
        {
            pbkdf2 = new PBKDF2();
            pbkdf2.password = password;
            return pbkdf2.hashPassword();
        }

        public PasswordVerificationResult VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            pbkdf2 = new PBKDF2(providedPassword, hashedPassword);
            if (pbkdf2.VerifyHashedPassword())
                return PasswordVerificationResult.Success;
            else
                return PasswordVerificationResult.Failed;
        }
    }
}