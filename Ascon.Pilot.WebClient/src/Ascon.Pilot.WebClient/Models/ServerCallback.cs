using Ascon.Pilot.Server.Api.Contracts;
using Ascon.Pilot.Core;

namespace Ascon.Pilot.WebClient.Models
{
    internal interface IRemoteStorageListener
    {
        void Notify(DSearchResult result);
    }

    internal class ServerCallback : IServerCallback
    {
        private IRemoteStorageListener _listener;
        
        public void SetCallbackListener(IRemoteStorageListener listener)
        {
            _listener = listener;
        }

        public void NotifyChangeset(DChangeset changeset)
        {
        }

        public void NotifyDMetadataChangeset(DMetadataChangeset changeset)
        {
        }

        public void NotifyDNotificationChangeset(DNotificationChangeset changeset)
        {
        }

        public void NotifyGeometrySearchResult(DGeometrySearchResult searchResult)
        {
        }

        public void NotifyOrganisationUnitChangeset(OrganisationUnitChangeset changeset)
        {
        }

        public void NotifyPersonChangeset(PersonChangeset changeset)
        {
        }

        public void NotifySearchResult(DSearchResult searchResult)
        {
            _listener?.Notify(searchResult);
        }
    }
}
