using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ascon.Pilot.Core;
using Ascon.Pilot.WebClient.Server;

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
        TaskModifier GetTaskModifier();
    }
}
