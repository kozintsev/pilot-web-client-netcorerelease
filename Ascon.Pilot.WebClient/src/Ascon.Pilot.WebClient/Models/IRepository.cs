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
        DPerson GetPersonOnOrganisationUnit(int id);
        DPerson GetPerson(int id);
        DOrganisationUnit GetOrganisationUnit(int id);
        DPerson CurrentPerson();
        MType GetType(int id);
        IEnumerable<MType> GetTypes();
        byte[] GetFileChunk(Guid id, long pos, int count);
    }

    internal class Repository : IRepository, IRemoteStorageListener
    {
        private readonly IServerApi _serverApi;
        private Dictionary<int, DPerson> _persons = new Dictionary<int, DPerson>();
        private Dictionary<int, DOrganisationUnit> _organisationUnits = new Dictionary<int, DOrganisationUnit>();
        private TaskCompletionSource<DSearchResult> _searchCompletionSource;
        private DPerson _person;
        private List<MType> _types;

        public Repository(IServerApi serverApi, ServerCallback serverCallback)
        {
            _serverApi = serverApi;
            serverCallback.SetCallbackListener(this);
        }

        public void Initialize(string currentLogin)
        {
            _persons = _serverApi.LoadPeople().ToDictionary(x => x.Id, y => y);
            _organisationUnits = _serverApi.LoadOrganisationUnits().ToDictionary(x => x.Id, y => y);
            _person = _persons.First(p => p.Value.Login.Equals(currentLogin, StringComparison.OrdinalIgnoreCase)).Value;
            _types = _serverApi.GetMetadata(0).Types;
        }

        public Task<DSearchResult> Search(DSearchDefinition searchDefinition)
        {
            _searchCompletionSource = new TaskCompletionSource<DSearchResult>();
            _serverApi.AddSearch(searchDefinition);
            return _searchCompletionSource.Task;
        }

        public List<DObject> GetObjects(Guid[] ids)
        {
            return _serverApi.GetObjects(ids);
        }

        public DPerson GetPersonOnOrganisationUnit(int id)
        {
            return _persons.Values.Where(p => p.Positions.Select(m => m.Position).Contains(id))
                .OrderBy(o => o.Positions.First(x => x.Position == id).Order)
                .FirstOrDefault();
        }

        public DPerson GetPerson(int id)
        {
            return _persons.Values.FirstOrDefault(p => p.Id.Equals(id));
        }

        public DOrganisationUnit GetOrganisationUnit(int id)
        {
            DOrganisationUnit result;
            return !_organisationUnits.TryGetValue(id, out result) ? new DOrganisationUnit() : result;
        }

        public MType GetType(int id)
        {
            return _types.FirstOrDefault(t => t.Id == id);
        }

        public DPerson CurrentPerson()
        {
            return _person;
        }

        public void Notify(DSearchResult result)
        {
            try
            {
                _searchCompletionSource.SetResult(result);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        public IEnumerable<MType> GetTypes()
        {
            return _types;
        }

        public byte[] GetFileChunk(Guid id, long pos, int count)
        {
            return _serverApi.GetFileChunk(id, pos, count);
        }
    }
}
