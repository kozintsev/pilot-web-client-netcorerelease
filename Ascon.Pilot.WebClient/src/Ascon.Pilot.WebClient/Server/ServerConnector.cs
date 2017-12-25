using System.Linq;
using Ascon.Pilot.Server.Api;
using Ascon.Pilot.Server.Api.Contracts;
using Ascon.Pilot.WebClient.Models;

namespace Ascon.Pilot.WebClient.Server
{
    public interface IServerConnector
    {
        IServerApi ServerApi { get; }
        IFileArchiveApi FileArchiveApi { get; }
        void Connect();
        void Disconnect();
        int PersonId { get; }
    }

    public class ServerConnector : IServerConnector
    {
        private readonly HttpPilotClient _client;
        private ServerCallback _serverCallback;

        public int PersonId { get; private set; }
        public IServerApi ServerApi { get; private set; }
        public IFileArchiveApi FileArchiveApi { get; private set; }

        public ServerConnector()
        {
            _client = new HttpPilotClient();
        }

        public void Connect()
        {
            _client.Connect("");

            _serverCallback = new ServerCallback();

            ServerApi = _client.GetServerApi(_serverCallback);
            ServerApi.OpenDatabase("11", "11", "11", false);

            var people = ServerApi.LoadPeople();
            var person = people.FirstOrDefault(p => !p.IsDeleted && p.Login == "");
            if (person != null)
                PersonId = person.Id;

            FileArchiveApi = _client.GetFileArchiveApi("");
        }

        public void Disconnect()
        {
            _client?.Disconnect();
        }
    }
}

