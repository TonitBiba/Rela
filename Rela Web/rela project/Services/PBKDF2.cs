using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Rela_project.Services
{
    public class PBKDF2
    {
        private const short SALTSIZE = 20;
        private const int ITERATION = 10000;

        public string password { get; set; }

        public string salteHashedPassword { get; set; }

        public PBKDF2() { }
        public PBKDF2(string password, string salteHashedPassword) {
            this.password = password;
            this.salteHashedPassword = salteHashedPassword;
        }

        Rfc2898DeriveBytes rfc2898DeriveBytes;
        public string hashPassword() {
            byte[] saltByte;
            byte[] passwordByte;
            byte[] saltedHashPassword = new byte[85];
            rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, SALTSIZE, ITERATION, HashAlgorithmName.SHA512);
            saltByte = rfc2898DeriveBytes.Salt;
            passwordByte = rfc2898DeriveBytes.GetBytes(64);
            Buffer.BlockCopy(saltByte, 0, saltedHashPassword, 1, 20);
            Buffer.BlockCopy(passwordByte, 0, saltedHashPassword, 21, 64);
            return Convert.ToBase64String(saltedHashPassword);
        }

        public bool VerifyHashedPassword() {
            byte[] saltedHashPasswordByte = Convert.FromBase64String(salteHashedPassword);
            byte[] saltByte = new byte[20];
            byte[] hashedPasswordByte = new byte[64];
            Buffer.BlockCopy(saltedHashPasswordByte, 1, saltByte, 0, 20);
            Buffer.BlockCopy(saltedHashPasswordByte, 21, hashedPasswordByte, 0, 64);

            rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, saltByte, ITERATION, HashAlgorithmName.SHA512);
            byte[] hashedPasswordByteUser = rfc2898DeriveBytes.GetBytes(64);

            return StructuralComparisons.StructuralEqualityComparer.Equals(hashedPasswordByte, hashedPasswordByteUser);
        }
    }
}