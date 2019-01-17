using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ascon.Pilot.DataClasses;
using Ascon.Pilot.Web.Extensions;
using Ascon.Pilot.Web.Models;

namespace Ascon.Pilot.Web.ViewModels
{
    public class FilesDetailsViewModel
    {
        private readonly IRepository _repository;

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
            ObjectTypeTitle = mType.Title;
            File = GetFile(version, obj);
            if (File != null)
            {
                FileId = File.Body.Id;
                FileName = File.Name;
                LastModifiedDate = File.Body.Modified;
                Size = (int)File.Body.Size;
                SizeStr = GetSizeString(Size);
            }

            Attributes = new SortedList<string, string>();
            var visibleAttrs = GetVisibleAttributes(mType);
            foreach (var attr in visibleAttrs)
            {
                var value = GetAttrValue(obj, attr.Name);
                if (!string.IsNullOrEmpty(value))
                    Attributes.Add(attr.Title, value);
            }

            Snapshots = new List<DFilesSnapshot>();
            Snapshots.Add(obj.ActualFileSnapshot);
            foreach (var previousFileSnapshot in obj.PreviousFileSnapshots.ToArray().Reverse())
            {
                Snapshots.Add(previousFileSnapshot);
            }
        }

        public string Name { get; private set; }
        public Guid Id { get; private set; }
        public Guid FileId { get; private set; }
        public long Version { get; private set; }
        public DateTimeOffset VersionTime { get; private set; }
        public string FileName { get; private set; }
        public string SizeStr { get; private set; }
        public int Size { get; private set; }
        public DateTime LastModifiedDate { get; private set; }
        public int ObjectTypeId { get; private set; }
        public string ObjectTypeName { get; private set; }
        public string ObjectTypeTitle { get; private set; }
        public DFile File { get; private set; }
        public string Extension { get { return Path.GetExtension(FileName); } }
        public bool IsActual { get; private set; }
        public string VersionReason { get; private set; }
        public string Author { get; private set; }
        public SortedList<string, string> Attributes { get; private set; }
        public List<DFilesSnapshot> Snapshots { get; private set; }

        public FilesPanelType FilesPanelType { get; private set; }

        private static string GetAttrValue(DObject obj, string attrName)
        {
            DValue value;
            if (!obj.Attributes.TryGetValue(attrName, out value))
                return null;
            return value.StrValue;
        }

        private DFile GetFile(long version, DObject obj)
        {
            var versionUniversalTime = new DateTime(version);
            if (obj.ActualFileSnapshot.Created.Equals(versionUniversalTime) || version == 0)
            {
                var file = obj.ActualFileSnapshot.Files.FirstOrDefault();
                if (file != null)
                {
                    VersionTime = obj.ActualFileSnapshot.Created;
                    Author = _repository.GetPerson(obj.ActualFileSnapshot.CreatorId).DisplayName;
                }
                return file;
            }
            var snapshot = obj.PreviousFileSnapshots.FirstOrDefault(o => o.Created.Equals(versionUniversalTime));
            if (snapshot != null)
            {
                IsActual = false;
                var file = snapshot.Files.FirstOrDefault();
                if (file != null)
                {
                    if (!string.IsNullOrEmpty(snapshot.Reason))
                        VersionReason = string.Format("\"{0}\"", snapshot.Reason);
                    VersionTime = snapshot.Created;
                    Author = GetPersonDisplayName(snapshot.CreatorId);
                }
                return file;
            }
            return null;
        }

        public string GetPersonDisplayName(int personId)
        {
           return _repository.GetPerson(personId).DisplayName;
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



        private MAttribute[] GetVisibleAttributes(MType type)
        {
            return type.Attributes.OrderBy(u => u.DisplaySortOrder).Where(a => a.IsService == false).ToArray();
        }
    }
}