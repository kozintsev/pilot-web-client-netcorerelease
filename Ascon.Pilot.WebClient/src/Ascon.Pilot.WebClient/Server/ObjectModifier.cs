﻿using System;
using System.Linq;
using Ascon.Pilot.Core;

namespace Ascon.Pilot.WebClient.Server
{
    public interface IObjectModifier
    {
        DChange EditObject(Guid objectId);
        DChange AddFile(DChange id, DFile file, string fileName);
        void Apply(DChangesetData changesetData);
    }

    public class ObjectModifier : IObjectModifier
    {
        private readonly IServerConnector _connector;
        private readonly int _personId;

        public ObjectModifier(IServerConnector connector, int personId)
        {
            _connector = connector;
            _personId = personId;
        }

        public DChange EditObject(Guid objectId)
        {
            var objects = _connector.ServerApi.GetObjects(new[] { objectId });
            var old = objects.First();
            var changed = old.Clone();
            var change = new DChange { Old = old, New = changed };
            return change;
        }

        public DChange AddFile(DChange change, DFile file, string fileName)
        {
            if (change.Old.ActualFileSnapshot.Files.Count > 0)
                change = CreateSnapshot(change);
            change.New.ActualFileSnapshot.AddFile(file, _personId);
            return change;
        }

        public void Apply(DChangesetData changesetData)
        {
            _connector.ServerApi.Change(changesetData);
        }

        private DChange CreateSnapshot(DChange change)
        {
            if (change.New.ActualFileSnapshot.IsEmpty)
            {
                change.New.PreviousFileSnapshots.Add(new DFilesSnapshot
                {
                    CreatorId = change.New.CreatorId,
                    Created = change.New.Created
                });
            }
            else
            {
                change.New.PreviousFileSnapshots.Add(change.New.ActualFileSnapshot.Clone());
            }

            change.New.ActualFileSnapshot.Created = GetEffectiveCreated(change);
            change.New.ActualFileSnapshot.Reason = "Rvt updated";
            change.New.ActualFileSnapshot.CreatorId = _personId;
            change.New.ActualFileSnapshot.Files.Clear();
            return change;
        }

        private static DateTime GetEffectiveCreated(DChange change)
        {
            var now = DateTime.UtcNow;

            if (!change.New.PreviousFileSnapshots.Any())
                return now;

            var maxCreated = change.New.PreviousFileSnapshots.Max(x => x.Created);
            return maxCreated < now ? now : maxCreated.AddMilliseconds(1);
        }
    }
}
