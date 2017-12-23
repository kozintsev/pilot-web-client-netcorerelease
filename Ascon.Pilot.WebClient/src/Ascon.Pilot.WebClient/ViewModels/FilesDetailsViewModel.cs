﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ascon.Pilot.WebClient.Models;
using Ascon.Pilot.Core;

namespace Ascon.Pilot.WebClient.ViewModels
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

            Snapshots = new List<DFilesSnapshot> {obj.ActualFileSnapshot};
            foreach (var previousFileSnapshot in obj.PreviousFileSnapshots)
            {
                Snapshots.Add(previousFileSnapshot);
            }
        }

        public string Name { get; }
        public Guid Id { get; }
        public Guid FileId { get; }
        public long Version { get; }
        public DateTime VersionTime { get; private set; }
        public string FileName { get; }
        public string SizeStr { get; }
        public int Size { get; }
        public DateTime LastModifiedDate { get; }
        public int ObjectTypeId { get; }
        public string ObjectTypeName { get; }
        public string ObjectTypeTitle { get; }
        public DFile File { get; }
        public string Extension => Path.GetExtension(FileName);
        public bool IsActual { get; private set; }
        public string VersionReason { get; private set; }
        public string Author { get; private set; }
        public SortedList<string, string> Attributes { get; }
        public List<DFilesSnapshot> Snapshots { get; }

        public FilesPanelType FilesPanelType { get; }

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
                    VersionTime = obj.ActualFileSnapshot.Created.ToLocalTime();
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
                    VersionTime = snapshot.Created.ToLocalTime();
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



        private static IEnumerable<MAttribute> GetVisibleAttributes(MType type)
        {
            return type.Attributes.OrderBy(u => u.DisplaySortOrder).Where(a => a.IsService == false).ToArray();
        }
    }
}