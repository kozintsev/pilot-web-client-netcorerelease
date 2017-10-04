using Ascon.Pilot.Server.Api;
using Ascon.Pilot.Server.Api.Contracts;
using Ascon.Pilot.WebClient.Extensions;
using Microsoft.AspNetCore.Http;
using System;

namespace Ascon.Pilot.WebClient.Models
{
    public interface IContext : IDisposable
    {
        void Build(HttpContext http);
        IRepository Repository { get; }
        IServerApi ServerApi { get; }
    }

    class Context : IContext
    {
        private Repository _repository;
        private HttpPilotClient _client;
        private ServerCallback _serverCallback;
        bool _isInitialized;

        public void Build(HttpContext http)
        {
            if (_isInitialized)
                return;

            _client = http.GetClient();
            if (_client == null)
                _client = new HttpPilotClient();

            _serverCallback = new ServerCallback();
            ServerApi = http.GetServerApi(_serverCallback);
            _repository = new Repository(ServerApi);
            _serverCallback.SetCallbackListener(_repository);
            _isInitialized = true;
        }

        public IRepository Repository
        {
            get { return _repository; }
        }

        public IServerApi ServerApi
        {
            get; private set;
        }

        //TODO create in this class
        public void SetHttpClient(HttpPilotClient client)
        {
            _client = client;
        }

        public void Dispose()
        {
        }
    }
}
