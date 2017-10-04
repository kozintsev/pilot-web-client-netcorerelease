using Ascon.Pilot.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ascon.Pilot.WebClient.Models
{
    public interface ISearchListener
    {
        void NotifySearchResult(DSearchResult searchResult);
    }
}
