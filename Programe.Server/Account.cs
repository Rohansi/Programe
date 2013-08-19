using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Programe.Server
{
    public class Account
    {
        public string Username;
        public byte[] Password;
        public byte[] Salt;

        private Account() { }

        public void Save()
        {
            if (!ValidUsername(Username))
                throw new Exception("Invalid username");

            var file = AccountFilename(Username);
            File.WriteAllText(file, JsonConvert.SerializeObject(this));
        }

        public static Account Login(string username, string password, out string error)
        {
            if (!ValidUsername(username))
            {
                error = "Invalid username";
                return null;
            }

            if (password.Length < 6)
            {
                error = "Password too short";
                return null;
            }

            var file = AccountFilename(username);
            if (!File.Exists(file))
            {
                error = "Account does not exist";
                return null;
            }

            var account = JsonConvert.DeserializeObject<Account>(File.ReadAllText(file));
            var hashedPassword = HashPassword(password, account.Salt);

            if (!hashedPassword.SequenceEqual(account.Password))
            {
                error = "Incorrect password";
                return null;
            }

            error = null;
            return account;
        }

        public static bool Register(string username, string password, out string error)
        {
            if (!ValidUsername(username))
            {
                error = "Invalid username";
                return false;
            }

            if (password.Length < 6)
            {
                error = "Password too short";
                return false;
            }

            var file = AccountFilename(username);
            if (File.Exists(file))
            {
                error = "Account already exists";
                return false;
            }

            var salt = GenerateSalt();
            var hashedPassword = HashPassword(password, salt);

            var account = new Account();
            account.Username = username;
            account.Password = hashedPassword;
            account.Salt = salt;
            account.Save();

            error = null;
            return true;
        }

        private static bool ValidUsername(string username)
        {
            return !(username.Length < 3 || username.Length > 10 || !username.All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)));
        }

        private static string AccountFilename(string username)
        {
            return string.Format("accounts/{0}.json", username.ToLower());
        }

        #region Password Utility Functions
        public static byte[] HashPassword(string password, byte[] salt)
        {
            if (salt == null || salt.Length != 16)
                throw new Exception("Bad salt");

            var h = new Rfc2898DeriveBytes(password, salt, 1000);
            return h.GetBytes(128);
        }

        private static Random random = new Random();
        public static byte[] GenerateSalt()
        {
            var salt = new byte[16];
            random.NextBytes(salt);
            return salt;
        }
        #endregion
    }
}
