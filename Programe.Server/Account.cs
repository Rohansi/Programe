using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace Programe.Server
{
    public class Account
    {
        private const string InvalidUsernameMessage = "Usernames must be 3 to 10 characters long and may only contain letters, digits and spaces.";
        private const string InvalidPasswordMessage = "Passwords must be at least 6 characters long.";

        public string Username;
        public byte[] Password;
        public byte[] Salt;

        private Account() { }

        public void Save()
        {
            if (!ValidUsername(Username))
                throw new Exception(InvalidUsernameMessage);

            var file = AccountFilename(Username);
            File.WriteAllText(file, JsonConvert.SerializeObject(this));
        }

        public static Account Login(string username, string password, out string message)
        {
            if (!ValidUsername(username))
            {
                message = InvalidUsernameMessage;
                return null;
            }

            if (password.Length < 6)
            {
                message = InvalidPasswordMessage;
                return null;
            }

            var file = AccountFilename(username);
            if (!File.Exists(file))
            {
                message = string.Format("Account '{0}' does not exist.", username);
                return null;
            }

            var account = JsonConvert.DeserializeObject<Account>(File.ReadAllText(file));
            var hashedPassword = HashPassword(password, account.Salt);

            if (!hashedPassword.SequenceEqual(account.Password))
            {
                message = "Incorrect username or password.";
                return null;
            }

            message = string.Format("Logged in as {0}.", account.Username);
            return account;
        }

        public static Account Register(string username, string password, out string message)
        {
            if (!ValidUsername(username))
            {
                message = InvalidUsernameMessage;
                return null;
            }

            if (password.Length < 6)
            {
                message = InvalidPasswordMessage;
                return null;
            }

            var file = AccountFilename(username);
            if (File.Exists(file))
            {
                message = "An account with that username already exists.";
                return null;
            }

            var salt = GenerateSalt();
            var hashedPassword = HashPassword(password, salt);

            var account = new Account();
            account.Username = username;
            account.Password = hashedPassword;
            account.Salt = salt;
            account.Save();

            message = "Your account has been created and you have been automatically logged in.";
            return account;
        }

        private static bool ValidUsername(string username)
        {
            username = username.Trim();
            return !(username.Length < 3 || username.Length > 10 || !username.All(c => char.IsLetterOrDigit(c) || c == ' '));
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
