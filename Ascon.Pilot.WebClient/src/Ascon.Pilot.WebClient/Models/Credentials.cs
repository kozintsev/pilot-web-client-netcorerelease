using Ascon.Pilot.Core;
using System;

namespace Ascon.Pilot.WebClient.Models
{
    public class Credentials
    {
        public Uri ServerUrl { get; private set; }
        public string Username { get; private set; }
        public string ProtectedPassword { get; private set; }

        public bool UseWindowsAuth
        {
            get { return !string.IsNullOrEmpty(Username) && (Username.Contains("\\") || Username.Contains("@")); }
        }

        public string DatabaseName { get; private set; }

        public static Credentials GetConnectionCredentials(string database, string username, string password)
        {
            var credentials = new Credentials
            {
                ServerUrl = new Uri(ApplicationConst.PilotServerUrl),
                Username = username,
                ProtectedPassword = password.EncryptAes(),
                DatabaseName = database
            };

            return credentials;
        }
    }
}
