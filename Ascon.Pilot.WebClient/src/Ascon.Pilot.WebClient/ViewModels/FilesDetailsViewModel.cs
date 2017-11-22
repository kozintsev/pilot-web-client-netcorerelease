using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ascon.Pilot.WebClient.Models;
using Ascon.Pilot.Core;

namespace Ascon.Pilot.WebClient.ViewModels
{
    public class FilesDetailsViewModel
    {
        private IRepository _repository;
        public FilesDetailsViewModel(Guid objId, long version, IRepository repository, FilesPanelType type)
        {
            IsActual = true;
            Version = version;
            FilesPanelType = type;
            _repository = repository;
            var objs = repository.GetObjects(new[] { objId });
            if (!objs.Any())
                return;

            var types = repository.GetTypes().ToDictionary(x => x.Id, y => y);

            var obj = objs.First();
            var mType = types[obj.TypeId];
            Name = obj.GetTitle(mType);
            Id = objId;
            ObjectTypeId = mType.Id;
            ObjectTypeName = mType.Name;

            File = GetFile(version, obj);
            if (File != null)
            {
                FileId = File.Body.Id;
                FileName = File.Name;
                LastModifiedDate = File.Body.Modified;
                Size = (int)File.Body.Size;
                SizeStr = GetSizeString(Size);
            }
        }

        public string Name { get; private set; }
        public Guid Id { get; private set; }
        public Guid FileId { get; private set; }
        public long Version { get; private set; }
        public DateTime VersionTime { get; private set; }
        public string FileName { get; private set; }
        public string SizeStr { get; private set; }
        public int Size { get; private set; }
        public DateTime LastModifiedDate { get; private set; }
        public int ObjectTypeId { get; private set; }
        public string ObjectTypeName { get; private set; }
        public DFile File { get; private set; }
        public string Extension { get { return Path.GetExtension(FileName); } }
        public bool IsActual { get; private set; }
        public string VersionReason { get; private set; }
        public string Author { get; private set; }

        public FilesPanelType FilesPanelType { get; private set; }

        private DFile GetFile(long version, DObject obj)
        {
            var versionUniversalTime = new DateTime(version).ToUniversalTime();
            if (obj.ActualFileSnapshot.Created.Equals(versionUniversalTime) || version == 0)
            {
                VersionTime = obj.ActualFileSnapshot.Created.ToLocalTime();
                Author = _repository.GetPerson(obj.ActualFileSnapshot.CreatorId).DisplayName;
                return obj.ActualFileSnapshot.Files.FirstOrDefault();
            }
            var snapshot = obj.PreviousFileSnapshots.FirstOrDefault(o => o.Created.Equals(versionUniversalTime));
            if (snapshot != null)
            {
                IsActual = false;
                if (!string.IsNullOrEmpty(snapshot.Reason))
                    VersionReason = string.Format("\"{0}\"",snapshot.Reason);
                VersionTime = snapshot.Created.ToLocalTime();
                Author = _repository.GetPerson(snapshot.CreatorId).DisplayName;
                return snapshot.Files.FirstOrDefault();
            }
            return null;
        }

        private string GetSizeString(int size)
        {
            string[] sizes = {"b", "Kb", "Mb", "Gb"};
            double len = size;
            int order = 0;
            while (len >= 1024 && order + 1 < sizes.Length)
            {
                order++;
                len = len/1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}