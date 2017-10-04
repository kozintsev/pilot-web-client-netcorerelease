using Ascon.Pilot.Core;
using Ascon.Pilot.Server.Api.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ascon.Pilot.WebClient.Models
{
    public interface IRepository
    {
        Task<DSearchResult> Search(DSearchDefinition searchDefinition);
        List<DObject> GetObjects(Guid[] ids);
    }

    class Repository : IRepository, IRemoteStorageListener
    {
        private IServerApi _serverApi;
        private TaskCompletionSource<DSearchResult> _searchCompletionSource;

        public Repository(IServerApi serverApi)
        {
            _serverApi = serverApi;
        }

        public Task<DSearchResult> Search(DSearchDefinition searchDefinition)
        {
            _searchCompletionSource = new TaskCompletionSource<DSearchResult>();
            _serverApi.AddSearch(searchDefinition);
            return _searchCompletionSource.Task;
        }

        public List<DObject> GetObjects(Guid[] ids)
        {
            throw new NotImplementedException();
        }

        public void Notify(DSearchResult result)
        {
            _searchCompletionSource.SetResult(result);
        }
    }
}
