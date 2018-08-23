using System;
using System.Collections.Concurrent;
using Ascon.Pilot.Core;
using Ascon.Pilot.Server.Api.Contracts;

namespace Ascon.Pilot.Server.Api
{
    public class NullableServerCallback : IServerCallback
    {
        private readonly ConcurrentQueue<Action<IServerCallback>> _scheduled = new ConcurrentQueue<Action<IServerCallback>>();
        private IServerCallback _callback;

        public IServerCallback Callback
        {
            get { return _callback; }
            set
            {
                _callback = value;
                if (_callback == null)
                    return;

                Action<IServerCallback> action;
                while (_scheduled.TryDequeue(out action))
                {
                    action(value);
                }
            }
        }

        public void NotifyChangeset(DChangeset changeset)
        {
            var callback = Callback;
            if (callback != null)
                callback.NotifyChangeset(changeset);
            else
                _scheduled.Enqueue(c => { c.NotifyChangeset(changeset); });
        }

        public void NotifyOrganisationUnitChangeset(OrganisationUnitChangeset changeset)
        {
            var callback = Callback;
            if (callback != null)
                callback.NotifyOrganisationUnitChangeset(changeset);
            else
                _scheduled.Enqueue(c => { c.NotifyOrganisationUnitChangeset(changeset); });
        }

        public void NotifyPersonChangeset(PersonChangeset changeset)
        {
            var callback = Callback;
            if (callback != null)
                callback.NotifyPersonChangeset(changeset);
            else
                _scheduled.Enqueue(c => { c.NotifyPersonChangeset(changeset); });
        }

        public void NotifyDMetadataChangeset(DMetadataChangeset changeset)
        {
            var callback = Callback;
            if (callback != null)
                callback.NotifyDMetadataChangeset(changeset);
            else
                _scheduled.Enqueue(c => { c.NotifyDMetadataChangeset(changeset); });
        }

        public void NotifySearchResult(DSearchResult searchResult)
        {
            var callback = Callback;
            if (callback != null)
                callback.NotifySearchResult(searchResult);
            else
                _scheduled.Enqueue(c => { c.NotifySearchResult(searchResult); });
        }

        public void NotifyGeometrySearchResult(DGeometrySearchResult searchResult)
        {
            var callback = Callback;
            if (callback != null)
                callback.NotifyGeometrySearchResult(searchResult);
            else
                _scheduled.Enqueue(c => { c.NotifyGeometrySearchResult(searchResult); });
        }

        public void NotifyDNotificationChangeset(DNotificationChangeset changeset)
        {
            var callback = Callback;
            if (callback != null)
                callback.NotifyDNotificationChangeset(changeset);
            else
                _scheduled.Enqueue(c => { c.NotifyDNotificationChangeset(changeset); });
        }
    }

    class NullServerUpdateCallback : IServerUpdateCallback
    {
        public IServerUpdateCallback Callback { get; set; }

        public void NotifyUploadToServerError(string error)
        {
            var callback = Callback;
            callback?.NotifyUploadToServerError(error);
        }

        public void NotifyDownloadFromServerError(string messageError)
        {
            var callback = Callback;
            callback?.NotifyDownloadFromServerError(messageError);
        }
    }

    public class NullableMessagesCallback : IMessageCallback
    {
        public IMessageCallback Callback { get; set; }

        public void NotifyMessageCreated(NotifiableDMessage message)
        {
            Callback?.NotifyMessageCreated(message);
        }

        public void NotifyTypingMessage(Guid chatId, int personId)
        {
            Callback?.NotifyTypingMessage(chatId, personId);
        }

        public void CreateNotification(DNotification notification)
        {
            Callback?.CreateNotification(notification);
        }
    }
}
