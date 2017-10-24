using Ascon.Pilot.Core;
using Ascon.Pilot.Server.Api;
using Ascon.Pilot.Server.Api.Contracts;
using Ascon.Pilot.Transport;
using Ascon.Pilot.WebClient.Extensions;
using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;

namespace Ascon.Pilot.WebClient.Models
{
    public interface IContext : IDisposable
    {
        void Build(HttpContext http);
        IRepository Repository { get; }
        IServerApi ServerApi { get; }
        DDatabaseInfo Connect(Credentials credentials);
        bool IsInitialized { get; }
    }

    class Context : IContext
    {
        private Repository _repository;
        private HttpPilotClient _client;
        private readonly ServerCallback _serverCallback;

        public Context()
        {
            _serverCallback = new ServerCallback();
        }

        public DDatabaseInfo Connect(Credentials credentials)
        {
            _client = new HttpPilotClient();
            _client.Connect(ApplicationConst.PilotServerUrl);
            ServerApi = _client.GetServerApi(_serverCallback);
            var dbInfo = ServerApi.OpenDatabase(credentials.DatabaseName, credentials.Username, credentials.ProtectedPassword, credentials.UseWindowsAuth);
            _repository = new Repository(ServerApi, _serverCallback);
            _repository.Initialize(credentials.Username);
            IsInitialized = true;
            return dbInfo;
        }

        public void Build(HttpContext context)
        {
            if (IsInitialized)
                return;

            //if (ServerApi == null)
            //{
                var dbName = context.User.FindFirstValue(ClaimTypes.Surname);
                var login = context.User.FindFirstValue(ClaimTypes.Name);
                var protectedPassword = context.User.FindFirstValue(ClaimTypes.UserData);
                var clientIdString = context.User.FindFirstValue(ClaimTypes.Sid);
                var creds = Credentials.GetConnectionCredentials(dbName, login, protectedPassword.DecryptAes());
                Connect(creds);
                //return;
            //}

            //_repository = new Repository(ServerApi, _serverCallback);
            //var login = context.User.FindFirstValue(ClaimTypes.Name);
            //_repository.Initialize(login);
            //IsInitialized = true;
        }

        public IRepository Repository
        {
            get { return _repository; }
        }

        public IServerApi ServerApi
        {
            get; private set;
        }

        public bool IsInitialized
        {
            get; private set;
        }

        public void Dispose()
        {
            _client?.Disconnect();
            _client?.Dispose();
        }
    }
}
