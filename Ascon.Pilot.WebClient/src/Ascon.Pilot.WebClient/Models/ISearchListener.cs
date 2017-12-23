using Ascon.Pilot.Core;

namespace Ascon.Pilot.WebClient.Models
{
    public interface ISearchListener
    {
        void NotifySearchResult(DSearchResult searchResult);
    }
}
