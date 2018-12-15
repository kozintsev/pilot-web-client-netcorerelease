using Ascon.Pilot.DataClasses;

namespace Ascon.Pilot.Web.Models
{
    public interface ISearchListener
    {
        void NotifySearchResult(DSearchResult searchResult);
    }
}
