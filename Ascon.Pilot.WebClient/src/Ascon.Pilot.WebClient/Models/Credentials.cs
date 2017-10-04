using Ascon.Pilot.Core;
using System;

namespace Ascon.Pilot.WebClient.Models
{
    public class Credentials
    {
        public Uri ServerUrl { get; private set; }
        public string Username { get; private set; }
        public string ProtectedPassword { get; private set; }
        public Guid Sid { get; private set; }


        public bool UseWindowsAuth
        {
            get { return !string.IsNullOrEmpty(Username) && (Username.Contains("\\") || Username.Contains("@")); }
        }

        public string DatabaseName { get; private set; }

        public static Credentials GetConnectionCredentials(string database, string username, string password, Guid sid)
        {
            var credentials = new Credentials
            {
                ServerUrl = new Uri(ApplicationConst.PilotServerUrl),
                Username = username,
                ProtectedPassword = password.EncryptAes(),
                Sid = sid,
                DatabaseName = database
            };

            return credentials;
        }
    }
}
