using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using ProtoBuf;

namespace Ascon.Pilot.Core
{
    [ProtoContract]
    public class DChangeset
    {
        [ProtoMember(1)]
        public long Id { get; set; }

        [ProtoMember(2)]
        public List<Guid> Changed { get; private set; }

        [ProtoMember(3)]
        public Guid Identity { get; set; }

        public DChangeset()
        {
            Changed = new List<Guid>();
        }

        public DChangeset(long id, Guid identity, IEnumerable<Guid> changed) 
        {
            Id = id;
            Identity = identity;
            Changed = new List<Guid>(changed);
        }

        public DChangeset(long id, Guid identity, params Guid[] changed)
        {
            Id = id;
            Identity = identity;
            Changed = new List<Guid>(changed);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DChangeset)obj);
        }

        protected bool Equals(DChangeset other)
        {
            return Id == other.Id && Changed.SequenceEqual(other.Changed) && Identity.Equals(other.Identity);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                foreach (var id in Changed)
                {
                    hashCode = (hashCode * 397) ^ id.GetHashCode();
                }
                hashCode = (hashCode * 397) ^ Identity.GetHashCode();
                return hashCode;
            }
        }
    }

    [ProtoContract]
    public class DDatabaseInfo
    {
        [ProtoMember(1)]
        public long MetadataVersion { get; set; }

        [ProtoMember(2)]
        public DChangeset LastChangeset { get; set; }

        [ProtoMember(3)]
        public DPerson Person { get; set; }

        [ProtoMember(4)]
        public Guid DatabaseId { get; set; }

        [ProtoMember(5)]
        public long DatabaseVersion { get; set; }
    }

    [ProtoContract]
    public class DDatabaseImportInfo
    {
        [ProtoMember(1)]
        public DMetadata Metadata { get; set; }

        [ProtoMember(2)]
        public DObject RootObject { get; set; }

        [ProtoMember(3)]
        public DObject TasksRootObject { get; set; }
    }

    [ProtoContract]
    public class DMetadata
    {
        [ProtoMember(1)]
        public long Version { get; set; }

        [ProtoMember(2)]
        public List<MType> Types { get; private set; }

        public DMetadata()
        {
            Types = new List<MType>();
        }

        public DMetadata Clone()
        {
            var clone = new DMetadata
            {
                Version = Version
            };

            foreach (var type in Types)
                clone.Types.Add(type.Clone());

            return clone;
        }
    }

    [ProtoContract]
    public class DValue
    {
        public object Value { get; set; }

        [ProtoMember(1)]
        public String StrValue
        {
            get { return Value as String; }
            set { Value = value; }
        }

        [ProtoMember(2)]
        public long? IntValue
        {
            get { return Value as long?; }
            set { Value = value; }
        }

        [ProtoMember(3)]
        public double? DoubleValue
        {
            get { return Value as double?; }
            set { Value = value; }
        }

        [ProtoMember(4)]
        public DateTime? DateValue
        {
            get { return Value as DateTime?; }
            set { Value = value; }
        }

        [ProtoMember(5)]
        public string[] ArrayValue
        {
            get { return Value as string[]; }
            set { Value = value; }
        }

        [ProtoMember(6)]
        public decimal? DecimalValue
        {
            get { return Value as decimal?; }
            set { Value = value; }
        }

        public override int GetHashCode()
        {
            if (Value == null)
                return 0;
            return Value.GetHashCode();
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (other is DValue)
                return Equals(other as DValue);
            return Equals(Value, other);
        }

        public bool Equals(DValue other)
        {
            if (ReferenceEquals(this, other))
                return true;
            if (other == null)
                return false;
            return Equals(Value, other.Value);
        }

        public static implicit operator DValue(long value)
        {
            return new DValue { Value = value };
        }

        public static implicit operator DValue(double value)
        {
            return new DValue { Value = value };
        }

        public static implicit operator DValue(String value)
        {
            return new DValue { Value = value };
        }

        public static implicit operator DValue(String[] value)
        {
            return new DValue { Value = value };
        }

        public static implicit operator DValue(DateTime value)
        {
            return new DValue { Value = value };
        }


        public static implicit operator DValue(decimal value)
        {
            return new DValue { Value = value };
        }

        public static implicit operator long(DValue value)
        {
            return (long)value.Value;
        }

        public static implicit operator double(DValue value)
        {
            return (double)value.Value;
        }

        public static implicit operator String(DValue value)
        {
            return (string) value?.Value;
        }

        public static implicit operator String[](DValue value)
        {
            return (String[])value?.Value;
        }

        public static implicit operator DateTime(DValue value)
        {
            return (DateTime)value.Value;
        }

        public static implicit operator decimal(DValue value)
        {
            return (decimal)value.Value;
        }

        public bool IsArray
        {
            get { return Value != null && Value.GetType().IsArray; }
        }

        public DValue Clone()
        {
            return new DValue { Value = Value };
        }

        public override string ToString()
        {
            if (StrValue != null)
                return StrValue;
            if (IntValue != null)
                return IntValue.ToString();
            if (DoubleValue != null)
                return DoubleValue.ToString();
            if (DateValue != null)
                return DateValue.Value.ToShortDateString();
            if (DecimalValue != null)
                return DecimalValue.Value.ToString(CultureInfo.InvariantCulture);
            if (ArrayValue != null)
                return string.Join(", ", ArrayValue);
            return string.Empty;
        }

        public static DValue GetDValue(object value)
        {
            if (value == null)
                return new DValue();
            if (value is int)
                return (int)value;
            if (value is long)
                return (long)value;
            if (value is double)
                return (double)value;
            if (value is string)
                return (string)value;
            if (value is DateTime)
                return (DateTime)value;
            if (value is decimal)
                return (decimal) value;
            throw new Exception(String.Format("Error convertion attribute value [{0}] to DValue", value));
        }
    }

    [Obsolete("Use DChild instead")]
    [ProtoContract(EnumPassthru = true)]
    public enum LinkType
    {
        Tree = 1,
        Files = 2,
        Tasks = 3,
        Messages = 4,
    }

    [ProtoContract(EnumPassthru = true)]
    public enum RelationType
    {
        SourceFiles = 1,
        TaskInitiatorAttachments = 2,
        TaskExecutorAttachments = 3,
        MessageAttachments = 4,
        Custom = 5
    }

    [Obsolete("Use DChild instead")]
    public static class LinkTypes
    {
        private static IEnumerable<LinkType> AllCache;

        public static IEnumerable<LinkType> All
        {
            get { return AllCache ?? (AllCache = Enum.GetValues(typeof(LinkType)).Cast<LinkType>()); }
        }
    }

    public static class RelationTypes
    {
        private static IEnumerable<RelationType> AllCache;

        public static IEnumerable<RelationType> All
        {
            get { return AllCache ?? (AllCache = Enum.GetValues(typeof(RelationType)).Cast<RelationType>()); }
        }
    }

    [ProtoContract(EnumPassthru = true)]
    public enum ObjectState : byte
    {
        Alive,
        InRecycleBin,
        DeletedPermanently,
        Frozen,
        LockRequested,
        LockAccepted
    }

    [ProtoContract]
    [DebuggerDisplay("{Id}")]
    public class DObject
    {
        public static readonly Guid GlobalRootId = new Guid("2EA0EA87-69CB-400C-8909-59F1322814DA");
        public static readonly Guid RootId = new Guid("00000001-0001-0001-0001-000000000001");
        public static readonly Guid TaskRootId = new Guid("ADB79734-723C-4FF8-8DA8-8F52E140FBF4");
        public static readonly Guid ExtensionRootId = new Guid("E6519D37-1984-407E-96A0-1CD371F68F16");
        public static readonly Guid ReportRootId = new Guid("7DAB217D-6E06-4C2C-AB77-B5EC9361415D");
        public static readonly Guid TaskTemplateRootId = new Guid("DB4AB44C-B3D1-4F5A-B049-13CDF0C2EFE7");
        public static readonly Guid DocumentTemplateRootId = new Guid("109DA1F5-7E1A-4DF4-95BD-1FD5AA023DD6");

        public static IEnumerable<Guid> RootsIds()
        {
            yield return RootId;
            yield return TaskRootId;
            yield return ExtensionRootId;
            yield return ReportRootId;
            yield return TaskTemplateRootId;
        }
        
        [ProtoMember(1)]
        public Guid Id { get; set; }

        [ProtoMember(2)]
        public int TypeId { get; set; }

        [ProtoMember(3)]
        public Guid ParentId { get; set; }

        [ProtoMember(5)]
        public long LastChange { get; set; }

        [ProtoMember(7)]
        public int CreatorId { get; set; }

        [ProtoMember(9)]
        public SortedList<string, DValue> Attributes { get; private set; }

#pragma warning disable 618
        [ProtoMember(10)]
        [Obsolete("Use the ActualFileSnapshot property instead")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public List<DFile> Files { get; private set; }
#pragma warning restore 618

        [ProtoMember(11)]
        public List<DFilesSnapshot> PreviousFileSnapshots { get; private set; }

        [ProtoMember(12)]
        public DateTime Created { get; set; }

        [ProtoMember(14)]
        public HashSet<AccessRecord> Access { get; private set; }

        [ProtoMember(15), Obsolete("Use the Access.SecretParentId instead")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsSecret { get; set; }

        [ProtoMember(16), Obsolete("Use the State property instead")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsDeleted { get; set; }

        [ProtoMember(17), Obsolete("Use the State property instead")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public bool IsInRecycleBin { get; set; }

#pragma warning disable 618
        [ProtoMember(18)]
        [Obsolete("Use the Children property instead")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public ChildrenCollection OldChildren { get; private set; }
#pragma warning restore 618

        [ProtoMember(19)]
        [Obsolete("Use Relations instead")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public RelationsCollection OldRelations { get; private set; }

        [ProtoMember(20)]
        public HashSet<int> Subscribers { get; private set; }

        [ProtoMember(21)]
        public List<DChild> Children { get; private set; }

        [ProtoMember(22)]
        [Obsolete("Use the State property instead")]
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public DLockInfo LockInfo { get; private set; }

        [ProtoMember(23)]
        public DFilesSnapshot ActualFileSnapshot { get; private set; }

        [ProtoMember(24)]
        public DStateInfo StateInfo { get; private set; }

        [ProtoMember(25)]
        public SecretInfo SecretInfo { get; set; }

        [ProtoMember(26)]
        public List<Guid> Context { get; private set; }

        [ProtoMember(27)]
        public List<DRelation> Relations { get; private set; }

        public DObject()
        {
            Id = Guid.Empty;
            ParentId = Guid.Empty;
            Attributes = new SortedList<string, DValue>();
            ActualFileSnapshot = new DFilesSnapshot();
            PreviousFileSnapshots = new List<DFilesSnapshot>();
            Access = new HashSet<AccessRecord>();
#pragma warning disable 618
            Files = new List<DFile>();
            OldChildren = new ChildrenCollection();
            OldRelations = new RelationsCollection();
#pragma warning restore 618
            Relations = new List<DRelation>();
            Subscribers = new HashSet<int>();
            Children = new List<DChild>();
            StateInfo = new DStateInfo();
            Context = new List<Guid>();
        }

        public void AssignTo(DObject other)
        {
            other.Id = Id;
            other.TypeId = TypeId;
            other.CreatorId = CreatorId;
            other.ParentId = ParentId;
            other.LastChange = LastChange;
            other.Created = Created;

            other.Access.Clear();
            foreach (var item in Access)
                other.Access.Add(item);
            other.SecretInfo = SecretInfo;

            other.Attributes.Clear();
            foreach (var item in Attributes)
                other.Attributes.Add(item.Key, item.Value.Clone());

            ActualFileSnapshot.AssignTo(other.ActualFileSnapshot);

            other.PreviousFileSnapshots.Clear();
            foreach (var item in PreviousFileSnapshots)
                other.PreviousFileSnapshots.Add(item.Clone());

#pragma warning disable 618
            other.IsSecret = IsSecret;
            other.Files.Clear();
            foreach (var dFile in Files)
            {
                other.Files.Add(dFile.Clone());
            }
            OldChildren.AssignTo(other.OldChildren);
            other.IsDeleted = IsDeleted;
            other.IsInRecycleBin = IsInRecycleBin;
            OldRelations.AssignTo(other.OldRelations);
#pragma warning restore 618

            other.Relations.Clear();
            other.Relations.AddRange(Relations.Select(x => x.Clone()));

            other.Subscribers.Clear();
            foreach (var item in Subscribers)
                other.Subscribers.Add(item);

            other.Children.Clear();
            foreach (var child in Children)
                other.Children.Add(child.Clone());

            StateInfo.AssignTo(other.StateInfo);

            other.Context.Clear();
            other.Context.AddRange(Context);
        }

        public DObject Clone()
        {
            var result = new DObject();
            AssignTo(result);
            return result;
        }
    }

    [ProtoContract]
    [DebuggerDisplay("Id: {ObjectId} Type: {TypeId}")]
    public class DChild
    {
        [ProtoMember(1)]
        public int TypeId { get; set; }

        [ProtoMember(2)]
        public Guid ObjectId { get; set; }

        public DChild Clone()
        {
            return new DChild
            {
                TypeId = TypeId,
                ObjectId = ObjectId
            };
        }

        protected bool Equals(DChild other)
        {
            return TypeId == other.TypeId && ObjectId.Equals(other.ObjectId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DChild) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (TypeId*397) ^ ObjectId.GetHashCode();
            }
        }
    }

    [ProtoContract]
    [Obsolete("Use List<DChild> instead")]
    [DebuggerDisplay("Children: {Tree.Count}, Files: {Files.Count}, Tasks: {Tasks.Count}, Messages: {Messages.Count}")]
    public class ChildrenCollection
    {
        private readonly List<Guid> _tree = new List<Guid>();
        private readonly List<Guid> _files = new List<Guid>();
        private readonly List<Guid> _tasks = new List<Guid>();
        private readonly List<Guid> _messages = new List<Guid>();

        [ProtoMember(1)]
        public List<Guid> Tree {
            get { return _tree; }
        }

        [ProtoMember(2)]
        public List<Guid> Files
        {
            get { return _files; }
        }

        [ProtoMember(3)]
        public List<Guid> Tasks
        {
            get { return _tasks; }
        }

        [ProtoMember(4)]
        public List<Guid> Messages
        {
            get { return _messages; }
        }

        public List<Guid> this[LinkType linkType]
        {
            get
            {
                switch (linkType)
                {
                    case LinkType.Tree:
                        return _tree;
                    case LinkType.Files:
                        return _files;
                    case LinkType.Tasks:
                        return _tasks;
                    case LinkType.Messages:
                        return _messages;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public void Clear()
        {
            foreach (var collection in Collections)
            {
                collection.Clear();
            }
        }

        public IEnumerable<Guid> All()
        {
           foreach (var guid in _tree)
                yield return guid;
            foreach (var guid in _files)
                yield return guid;
            foreach (var guid in _tasks)
                yield return guid;
            foreach (var guid in _messages)
                yield return guid;
        }

        private IEnumerable<List<Guid>> Collections
        {
            get
            {
                yield return _tree;
                yield return _files;
                yield return _tasks;
                yield return _messages;
            }
        }

        public void Remove(Guid id)
        {
            foreach (var collection in Collections)
            {
                collection.Remove(id);
            }
        }

        public bool Contains(Guid id)
        {
            return Collections.Any(collection => collection.Contains(id));
        }

        public void AssignTo(ChildrenCollection other)
        {
            other.Clear();
            foreach (var linkType in LinkTypes.All)
            {
                other[linkType].AddRange(this[linkType]);
            }
        }

        public bool SequenceEqual(ChildrenCollection other)
        {
            return LinkTypes.All.All(x => this[x].SequenceEqual(other[x]));
        }
    }

    [ProtoContract]
    [DebuggerDisplay("{Type} {TargetId} {Name}")]
    public class DRelation
    {
        [ProtoMember(1)]
        public Guid Id { get; set; }

        [ProtoMember(2)]
        public Guid TargetId { get; set; }

        [ProtoMember(3)]
        public RelationType Type { get; set; }

        [ProtoMember(4)]
        public string Name { get; set; }

        [ProtoMember(5)]
        public DateTime VersionId { get; set; }

        protected bool Equals(DRelation other)
        {
            return Id.Equals(other.Id) && TargetId.Equals(other.TargetId) && Type == other.Type && string.Equals(Name, other.Name) && VersionId == other.VersionId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((DRelation) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Id.GetHashCode();
                hashCode = (hashCode * 397) ^ TargetId.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) Type;
                hashCode = (hashCode * 397) ^ (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ VersionId.GetHashCode();
                return hashCode;
            }
        }

        public DRelation Clone()
        {
            return new DRelation
            {
                Id = Id,
                TargetId = TargetId,
                Type = Type,
                Name = Name,
                VersionId = VersionId
            };
        }
    }

    [ProtoContract]
    [Obsolete("Use DRelation instead")]
    [DebuggerDisplay("Relations: {SourceFiles.Count}, InitiatorAttach: {TaskInitiatorAttachments.Count}, ExecutorAttach: {TaskExecutorAttachments.Count}")]
    public class RelationsCollection
    {
        private readonly List<Guid> _sourceFiles = new List<Guid>();
        private readonly List<Guid> _taskInitiatorAttachments = new List<Guid>();
        private readonly List<Guid> _taskExecutorAttachments = new List<Guid>();
        private readonly List<Guid> _messageAttachments = new List<Guid>();
        
        [ProtoMember(1)]
        public List<Guid> SourceFiles
        {
            get { return _sourceFiles; }
        }

        [ProtoMember(2)]
        public List<Guid> TaskInitiatorAttachments
        {
            get { return _taskInitiatorAttachments; }
        }

        [ProtoMember(3)]
        public List<Guid> TaskExecutorAttachments
        {
            get { return _taskExecutorAttachments; }
        }

        [ProtoMember(4)]
        public List<Guid> MessageAttachments {
            get { return _messageAttachments; }
        }

        public List<Guid> this[RelationType linkType]
        {
            get
            {
                switch (linkType)
                {
                    case RelationType.SourceFiles:
                        return _sourceFiles;
                    case RelationType.TaskExecutorAttachments:
                        return _taskExecutorAttachments;
                    case RelationType.TaskInitiatorAttachments:
                        return _taskInitiatorAttachments;
                    case RelationType.MessageAttachments:
                        return _messageAttachments;
                    case RelationType.Custom:
                        return new List<Guid>();
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        public void Clear()
        {
            _sourceFiles.Clear();
            _taskInitiatorAttachments.Clear();
            _taskExecutorAttachments.Clear();
            _messageAttachments.Clear();
        }

        public void AssignTo(RelationsCollection other)
        {
            other.Clear();
            foreach (var relationType in RelationTypes.All)
            {
                other[relationType].AddRange(this[relationType]);
            }
        }

        public bool SequenceEqual(RelationsCollection other)
        {
            return RelationTypes.All.All(x => this[x].SequenceEqual(other[x]));
        }

        public bool Any()
        {
            return RelationTypes.All.Any(x => this[x].Any());
        }
    }

    [ProtoContract]
    [DebuggerDisplay("{OrgUnitId} - {Access}")]
    public struct AccessRecord
    {
        public static readonly AccessRecord Empty = new AccessRecord();
        public const int RECORD_OWNER_UNDEFINED = 0;

        [ProtoMember(1)]
        public int OrgUnitId { get; set; }

        [ProtoMember(2)]
        public Access Access { get; set; }

        [ProtoMember(3)]
        public int RecordOwnerPosition { get; set; }

        [ProtoMember(4)]
        public Guid InheritanceSource { get; set; }

        public AccessRecord(int organizationalUnitId, int recordOwnerPosition, Guid inheritanceSource, Access access) : this()
        {
            OrgUnitId = organizationalUnitId;
            Access = access;
            RecordOwnerPosition = recordOwnerPosition;
            InheritanceSource = inheritanceSource;
        }

        public bool Equals(AccessRecord other)
        {
            return OrgUnitId == other.OrgUnitId && Access.Equals(other.Access) && RecordOwnerPosition == other.RecordOwnerPosition && InheritanceSource == other.InheritanceSource;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is AccessRecord && Equals((AccessRecord)obj);
        }

        public static bool operator ==(AccessRecord record1, AccessRecord record2)
        {
            return record1.Equals(record2);
        }

        public static bool operator !=(AccessRecord record1, AccessRecord record2)
        {
            return !record1.Equals(record2);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = OrgUnitId;
                hashCode = (hashCode * 397) ^ Access.GetHashCode();
                hashCode = (hashCode * 397) ^ RecordOwnerPosition.GetHashCode();
                hashCode = (hashCode * 397) ^ InheritanceSource.GetHashCode();
                return hashCode;
            }
        }

        public bool IsTwinTo(AccessRecord other)
        {
            return this.OrgUnitId == other.OrgUnitId
                   && this.RecordOwnerPosition == other.RecordOwnerPosition
                   && Equals(this.Access, other.Access);
        }

        public override string ToString()
        {
            return $"AccessRecord : {OrgUnitId}, OwnerPosition: {RecordOwnerPosition}, InheritanceSource : {InheritanceSource}, Access:{Access}";
        }
    }

    [ProtoContract]
    [DebuggerDisplay("AccessLevel : {AccessLevel}, IsInheritable : {IsInheritable}, ValidThrough:{ValidThrough}")]
    public struct Access
    {
        private DateTime _validThrough;
        
        [ProtoMember(1)]
        public AccessLevel AccessLevel { get; set; }

        [ProtoMember(2)]
        public DateTime ValidThrough
        {
            get
            {
                if (_validThrough.Equals(DateTime.MinValue))
                    _validThrough = DateTime.MaxValue;
                return _validThrough;
            }
            set
            {
                _validThrough = value;
            }
        }

        [ProtoMember(3)]
        public bool IsInheritable { get; set; }

        public Access(AccessLevel accessLevel, DateTime validThrough, bool isInheritable) : this()
        {
            AccessLevel = accessLevel;
            ValidThrough = validThrough;
            IsInheritable = isInheritable;
        }

        public bool Equals(Access other)
        {
            return AccessLevel == other.AccessLevel &&
                   IsInheritable.Equals(other.IsInheritable) &&
                   ValidThrough.Equals(other.ValidThrough);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is Access && Equals((Access) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int) AccessLevel;
                hashCode = (hashCode * 397) ^ ValidThrough.GetHashCode();
                hashCode = (hashCode * 397) ^ IsInheritable.GetHashCode();
                return hashCode;
            }
        }

        public override string ToString()
        {
            return $"Access : {AccessLevel}, IsInheritable: {IsInheritable}, ValidThrough : {ValidThrough}";
        }
    }

    [Flags]
    [ProtoContract(EnumPassthru = true)]
    public enum AccessLevel : byte
    {
        None = 0,
        Create = 1 << 0,
        Edit = 1 << 1,
        View = 1 << 2,
        Freeze = 1 << 3,
        Agreement = 1 << 4,

        ViewCreate = View | Create,
        ViewEdit = View | Create | Edit,
        ViewEditAgrement = ViewEdit | Agreement,
        Full = View | Create | Edit | Freeze | Agreement
    }

    [ProtoContract]
    [DebuggerDisplay("IsSecret = {IsSecret}")]
    public struct SecretInfo
    {
        public static readonly SecretInfo Public = new SecretInfo();

        public SecretInfo(Guid secretParentId, int secretChangedBy, DateTime secretChangedTimestamp)
        {
            SecretParentId = secretParentId;
            SecretChangedBy = secretChangedBy;
            SecretChangedTimestamp = secretChangedTimestamp;
        }

        [ProtoMember(1)]
        public Guid SecretParentId { get; set; }

        [ProtoMember(2)]
        public int SecretChangedBy { get; set; }

        [ProtoMember(3)]
        public DateTime SecretChangedTimestamp { get; set; }

        public bool IsSecret => SecretParentId != Guid.Empty;

        private bool Equals(SecretInfo other)
        {
            return
                SecretParentId.Equals(other.SecretParentId) &&
                SecretChangedBy == other.SecretChangedBy &&
                SecretChangedTimestamp.Equals(other.SecretChangedTimestamp);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SecretInfo)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = SecretParentId.GetHashCode();
                hashCode = (hashCode * 397) ^ SecretChangedBy;
                hashCode = (hashCode * 397) ^ SecretChangedTimestamp.GetHashCode();
                return hashCode;
            }
        }

        public static bool operator ==(SecretInfo a, SecretInfo b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(SecretInfo a, SecretInfo b)
        {
            return !a.Equals(b);
        }
    }

    [ProtoContract]
    public class DFilesSnapshot
    {
        [ProtoMember(1)]
        public DateTime Created { get; set; }

        [ProtoMember(2)]
        public int CreatorId { get; set; }

        [ProtoMember(3)]
        public string Reason { get; set; }
        
        [ProtoMember(4)]
        public List<DFile> Files { get; private set; }

        public bool IsEmpty
        {
            get
            {
                // ориентируемся на дату из-за наличия такого кода: newObject.ActualFileSnapshot.Files.Clear();
                return (Created == DateTime.MinValue);
            }
        }

        public DFilesSnapshot()
        {
            Reason = String.Empty;
            Files = new List<DFile>();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Created.GetHashCode();
                hashCode = (hashCode*397) ^ CreatorId;
                hashCode = (hashCode*397) ^ (Reason != null ? Reason.GetHashCode() : 0);
                foreach (var file in Files)
                {
                    hashCode = (hashCode * 397) ^ file.GetHashCode();    
                }
                return hashCode;
            }
        }

        public override bool Equals(object other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (other is DFilesSnapshot)
                return Equals(other as DFilesSnapshot);

            return Equals(this, other);
        }

        public bool Equals(DFilesSnapshot other)
        {
            if (ReferenceEquals(this, other))
                return true;

            if (other == null)
                return false;

            if (Created != other.Created)
                return false;

            if (CreatorId != other.CreatorId)
                return false;

            if (Reason != other.Reason)
                return false;

            if (!Files.SequenceEqual(other.Files))
                return false;

            return true;
        }

        public DFilesSnapshot Clone()
        {
            var result = new DFilesSnapshot();
            AssignTo(result);
            return result;
        }

        public void AssignTo(DFilesSnapshot other)
        {
            other.Created = Created;
            other.CreatorId = CreatorId;
            other.Reason = Reason;

            foreach (var file in Files)
                other.Files.Add(file.Clone());
        }

        public void AddFile(DFile file, int personId, DateTime? created = null)
        {
            Init(personId, created);
            Files.Add(file);
        }

        public void AddFiles(IEnumerable<DFile> files, int personId, DateTime? created = null)
        {
            Init(personId, created);
            Files.AddRange(files);
        }

        private void Init(int personId, DateTime? created)
        {
            if (IsEmpty)
            {
                CreatorId = personId;
                Created = created ?? DateTime.UtcNow;
            }
        }
    }

    [ProtoContract(EnumPassthru = true)]
    public enum TypeKind
    {
        User = 0,
        System = 1,
        [Obsolete("Use the MType.IsService property instead")]
        Service = 2
    }

    [ProtoContract]
    [DebuggerDisplay("{Title}")]
    public class MType
    {
        public const int ROOT_ID = 0;
        public const int SMART_FOLDER_ID = 1;

        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public string Name { get; set; }

        [ProtoMember(3)]
        public string Title { get; set; }

        [ProtoMember(4)]
        public byte[] Icon { get; set; }

        [ProtoMember(5)]
        public int Sort { get; set; }

        [ProtoMember(7)]
        public bool HasFiles { get; set; }

        [ProtoMember(8)]
        public List<int> Children { get; private set; }

        [ProtoMember(9)]
        public List<MAttribute> Attributes { get; private set; }

        [ProtoMember(10)]
        public bool IsDeleted { get; set; }

        [ProtoMember(11)]
        public TypeKind Kind { get; set; }

        [ProtoMember(12)]
        public bool IsMountable { get; set; }

        [ProtoMember(13)]
        public bool IsService { get; set; }

        [ProtoMember(14)]
        public bool IsProject { get; set; }

        public MType()
        {
            Name = String.Empty;
            Title = String.Empty;
            Children = new List<int>();
            Attributes = new List<MAttribute>();
        }

        public MType Clone()
        {
            var clone = new MType
            {
                HasFiles = HasFiles,
                Icon = Icon,
                Id = Id,
                Name = Name,
                Title = Title,
                IsDeleted = IsDeleted,
                IsMountable = IsMountable,
                Kind = Kind,
                Sort = Sort,
                IsService = IsService,
                IsProject = IsProject
            };
            
            foreach (var attribute in Attributes)
                clone.Attributes.Add(attribute.Clone());

            foreach (var child in Children)
                clone.Children.Add(child);

            return clone;
        }
    }

    [ProtoContract(EnumPassthru = true)]
    public enum MAttrType 
    {
        Integer = 0, 
        Double = 1, 
        DateTime = 2, 
        String = 3,
        Decimal = 4,
        Numerator = 5,
        Array = 6
        //Don't forget to update SDK's AttributeType
    }

    [ProtoContract]
    public class MAttribute
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public string Title { get; set; }
        
        [ProtoMember(3)]
        public bool Obligatory { get; set; }
        
        [ProtoMember(6)]
        public MAttrType Type { get; set; }

        [ProtoMember(7)]
        public int DisplayHeight { get; set; }

        [ProtoMember(8)]
        public bool ShowInTree { get; set; }

        [ProtoMember(9)]
        public int DisplaySortOrder { get; set; }

        [ProtoMember(10)]
        public bool IsService { get; set; }

        [ProtoMember(11)]
        public string Configuration { get; set; }

        public MAttribute()
        {
            Name = String.Empty;
            Title = String.Empty;
            DisplayHeight = 1;
        }

        protected bool Equals(MAttribute other)
        {
            return string.Equals(Name, other.Name) 
                && string.Equals(Title, other.Title) 
                && Obligatory.Equals(other.Obligatory) 
                && DisplaySortOrder == other.DisplaySortOrder
                && ShowInTree == other.ShowInTree
                && Type == other.Type 
                && IsService == other.IsService
                && DisplayHeight == other.DisplayHeight
                && Configuration == other.Configuration;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MAttribute) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Title != null ? Title.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Configuration != null ? Configuration.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Obligatory.GetHashCode();
                hashCode = (hashCode * 397) ^ DisplaySortOrder.GetHashCode();
                hashCode = (hashCode * 397) ^ ShowInTree.GetHashCode();
                hashCode = (hashCode * 397) ^ (int) Type;
                hashCode = (hashCode * 397) ^ IsService.GetHashCode();
                hashCode = (hashCode * 397) ^ DisplayHeight;
                return hashCode;
            }
        }

        public MAttribute Clone()
        {
            var clone = new MAttribute
            {
                DisplayHeight = DisplayHeight,
                DisplaySortOrder = DisplaySortOrder,
                IsService = IsService,
                Name = Name,
                Obligatory = Obligatory,
                ShowInTree = ShowInTree,
                Configuration = Configuration,
                Title = Title,
                Type = Type
            };
            return clone;
        }
    }

    [ProtoContract(EnumPassthru = true)]
    public enum ActivityStatus
    {
        Active = 1, Vacation = 2, Dismissed = 3
    }

    [ProtoContract]
    public class AdUser
    {
        [ProtoMember(1)]
        public string Sid { get; set; }
        [ProtoMember(2)]
        public string Name { get; set; }
        [ProtoMember(3)]
        public string DisplayName { get; set; }
        [ProtoMember(4)]
        public List<string> OrgUnits { get; private set; }

        public AdUser()
        {
            OrgUnits = new List<string>();
        }
    }

    [ProtoContract]
    public class DPerson
    {
        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public string Login { get; set; }

        [ProtoMember(6)]
        public ActivityStatus Status { get; set; }

        [ProtoMember(8)]
        public string DisplayName { get; set; }

        [ProtoMember(9)]
        public List<MPosition> Positions { get; private set; }

        [ProtoMember(10)]
        public string Comment { get; set; }

        [ProtoMember(11)]
        public string Sid { get; set; }

        [ProtoMember(12)]
        public bool IsDeleted { get; set; }

        [ProtoMember(13)]
        public bool IsAdmin { get; set; }

        public DPerson()
        {
            Login = String.Empty;
            DisplayName = String.Empty;
            Status = ActivityStatus.Active;
            Positions = new List<MPosition>();
            Comment = String.Empty;
            Sid = String.Empty;
            IsDeleted = false;
            IsAdmin = false;
        }

        public DPerson Clone()
        {
            var result = new DPerson();
            AssignTo(result);
            return result;
        }

        private void AssignTo(DPerson other)
        {
            other.Id = Id;
            other.DisplayName = DisplayName;
            other.Login = Login;
            other.Sid = Sid;
            other.Status = Status;
            other.Comment = Comment;
            other.IsDeleted = IsDeleted;
            other.IsAdmin = IsAdmin;
            
            foreach (var position in Positions)
                other.Positions.Add(position);
        }
    }

    [ProtoContract]
    public class MPosition
    {
        [ProtoMember(1)]
        public int Order { get; private set; }

        [ProtoMember(2)]
        public int Position { get; private set; }

        public MPosition(int order, int position)
        {
            Order = order;
            Position = position;
        }

        public MPosition()
        {
            Order = 0;
            Position = 0;
        }
    }

    [ProtoContract]
    public class DOrganisationUnit
    {
        public const int ROOT_ID = 0;

        [ProtoMember(1)]
        public int Id { get; set; }

        [ProtoMember(2)]
        public string Title { get; set; }

        [ProtoMember(3)]
        public bool IsPosition { get; set; }

        [ProtoMember(4)]
        public List<int> Children { get; private set; }

        [ProtoMember(5)]
        public bool IsDeleted { get; set; }

        [ProtoMember(6)]
        public bool IsBoss { get; set; }

        public DOrganisationUnit()
        {
            Title = String.Empty;
            IsDeleted = false;
            Children = new List<int>();
        }

        public DOrganisationUnit Clone()
        {
            var result = new DOrganisationUnit();
            AssignTo(result);
            return result;
        }

        private void AssignTo(DOrganisationUnit other)
        {
            other.Id = Id;
            other.Title = Title;
            other.IsPosition = IsPosition;
            other.IsDeleted = IsDeleted;
            other.IsBoss = IsBoss;

            foreach (var child in Children)
                other.Children.Add(child);
        }
    }

    [ProtoContract]
    public class DOrganisationUnitChangesetData
    {
        [ProtoMember(1)]
        public Guid Id { get; private set; }

        [ProtoMember(2)]
        public DOrganisationUnit Updated { get; set; }

        [ProtoMember(3)]
        public DOrganisationUnit Created { get; set; }

        public DOrganisationUnitChangesetData()
        {
            Id = Guid.NewGuid();
        }
    }

    [ProtoContract]
    public class DPersonOnPositionData
    {
        [ProtoMember(1)]
        public Guid Id { get; private set; }

        [ProtoMember(2)]
        public int PositionId { get; set; }

        [ProtoMember(3)]
        public List<int> People { get; private set; }

        public DPersonOnPositionData()
        {
            Id = Guid.NewGuid();
            People = new List<int>();
        }
    }

    [ProtoContract]
#if DEBUG
    [DebuggerDisplay("{DebugView}")]
#endif
    public class DChange
    {
        [ProtoMember(1)]
        public DObject Old { get; set; }

        [ProtoMember(2)]
        public DObject New { get; set; }
    }

    [ProtoContract]
    public class DChangesetData
    {
        [ProtoMember(1)]
		public long Id { get; set; }

        [ProtoMember(2)]
		public Guid Identity { get; set; }

        [ProtoMember(3)]
        public List<DChange> Changes { get; private set; }

        [ProtoMember(4)]
        public List<Guid> NewFileBodies { get; private set; }

        public DChangesetData()
		{
            Identity = Guid.Empty;
            Changes = new List<DChange>();
            NewFileBodies = new List<Guid>();
		}
    }

    [ProtoContract]
    public class DFileBody
    {
        [ProtoMember(1)]
        public Guid Id { get; set; }

        [ProtoMember(2)]
        public long Size { get; set; }

        [ProtoMember(3)]
        public string Md5 { get; set; }

        [ProtoMember(4)]
        public DateTime Modified { get; set; }

        [ProtoMember(5)]
        public DateTime Created { get; set; }

        [ProtoMember(6)]
        public DateTime Accessed { get; set; }

        protected bool Equals(DFileBody other)
        {
            return Id.Equals(other.Id) && Size == other.Size && string.Equals(Md5, other.Md5) && Modified.Equals(other.Modified) && Created.Equals(other.Created) && Accessed.Equals(other.Accessed);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DFileBody) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Id.GetHashCode();
                hashCode = (hashCode*397) ^ Size.GetHashCode();
                hashCode = (hashCode*397) ^ (Md5 != null ? Md5.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ Modified.GetHashCode();
                hashCode = (hashCode*397) ^ Created.GetHashCode();
                hashCode = (hashCode*397) ^ Accessed.GetHashCode();
                return hashCode;
            }
        }
    }

    [ProtoContract]
    public class DSignature
    {
        [ProtoMember(1)]
        public Guid Id { get; set; }

        [ProtoMember(2)]
        public Guid DatabaseId { get; set; }

        [ProtoMember(3)]
        public int PositionId { get; set; }      

        [ProtoMember(4)]
        public string Role { get; set; }

        [ProtoMember(5)]
        public string Sign { get; set; }

        [ProtoMember(6)]
        public string RequestedSigner { get; set; }

        protected bool Equals(DSignature other)
        {
            return Id == other.Id
                && DatabaseId.Equals(other.DatabaseId) 
                && PositionId == other.PositionId 
                && string.Equals(RequestedSigner, other.RequestedSigner) 
                && string.Equals(Role, other.Role) && Equals(Sign, other.Sign);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DSignature) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = DatabaseId.GetHashCode();
                hashCode = (hashCode*397) ^ Id.GetHashCode();
                hashCode = (hashCode*397) ^ PositionId;
                hashCode = (hashCode*397) ^ (RequestedSigner != null ? RequestedSigner.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Role != null ? Role.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Sign != null ? Sign.GetHashCode() : 0);
                return hashCode;
            }
        }

        public DSignature Clone()
        {
            return new DSignature
            {
                Id = Id,
                DatabaseId = DatabaseId,
                PositionId = PositionId,
                Role = Role,
                Sign = Sign,
                RequestedSigner = RequestedSigner
            };
        }
    }

    [ProtoContract]
    [DebuggerDisplay("{Body.Id}")]
    public class DFile
    {
        public DFile()
        {
            Signatures = new List<DSignature>();
        }

        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public DFileBody Body { get; set; }

        [ProtoMember(3)]
        public List<DSignature> Signatures { get; private set; }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (Name != null ? Name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Body != null ? Body.GetHashCode() : 0);
                foreach (var signature in Signatures)
                {
                    hashCode = (hashCode * 397) ^ signature.GetHashCode();   
                }
                return hashCode;
            }
        }

        public bool EqualsWithoutSignatures(DFile other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (!string.Equals(Name, other.Name))
                return false;
            return Body.Equals(other.Body);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) 
                return false;
            if (ReferenceEquals(this, obj)) 
                return true;
            if (obj.GetType() != GetType()) 
                return false;
            return Equals((DFile)obj);
        }

        protected bool Equals(DFile other)
        {
            if (!string.Equals(Name, other.Name))
                return false;
            if (!Body.Equals(other.Body))
                return false;
            if (Signatures.Count != other.Signatures.Count)
                return false;
            if (!Signatures.SequenceEqual(other.Signatures))
                return false;

            return true;
        }

        public DFile Clone()
        {
            var result = new DFile();
            AssignTo(result);
            return result;
        }

        private void AssignTo(DFile other)
        {
            other.Name = Name;
                        
            other.Body = Body != null ? new DFileBody
            {
                Id = Body.Id,
                Md5 = Body.Md5,
                Size = Body.Size,
                Modified = Body.Modified,
                Created = Body.Created,
                Accessed = Body.Accessed
            } : null;

            other.Signatures.Clear();
            foreach (var signature in Signatures)
            {
                other.Signatures.Add(signature.Clone());
            }
        }
    }

    [ProtoContract]
    public class AdminDatabaseInfo
    {
        [ProtoMember(1)]
        public string Name { get; set; }

        [ProtoMember(2)]
        public long DatabaseSize { get; set; }

        [ProtoMember(3)]
        public long Version { get; set; }

        [ProtoMember(4)]
        public string Language { get; set; }

        [ProtoMember(5)]
        public DatabaseState State { get; set; }

        [ProtoMember(6)]
        public string InitializingError { get; set; }

        [ProtoMember(7)]
        public Guid Id { get; set; }

        [ProtoMember(8)]
        public string DatabaseLocation { get; set; }

        [ProtoMember(9)]
        public string FileArchiveLocation { get; set; }
    }

    [ProtoContract]
    public class DServerAdministrator
    {
        public DServerAdministrator()
        {
            Login = string.Empty;
            DisplayName = string.Empty;
            Sid = string.Empty;
            Id = Guid.Empty;
        }

        [ProtoMember(1)]
        public string Login { get; set; }

        [ProtoMember(2)]
        public string DisplayName { get; set; }

        [ProtoMember(3)]
        public string Sid { get; set; }

        [ProtoMember(4)]
        public Guid Id { get; set; }

        public DServerAdministrator Clone()
        {
            var copy = new DServerAdministrator
            {
                Login = Login,
                DisplayName = DisplayName,
                Sid = Sid,
                Id = Id
            };

            return copy;
        }
    }

    [ProtoContract(EnumPassthru = true)]
    public enum FileSystemNodeKind : byte
    {
        File = 0,
        Folder = 1,
        Disk = 2,
        LocalComputer = 3,
        NetworkComputer = 4,
        NetworkNode = 5
    }

    [ProtoContract]
    public class FileSystemNode
    {
        [ProtoMember(1)]
        public FileSystemNodeKind Kind { get; set; }
        [ProtoMember(2)]
        public string Name { get; set; }
        [ProtoMember(3)]
        public string Path { get; set; }
        [ProtoMember(4)]
        public bool HasChildren { get; set; }
        [ProtoMember(5)]
        public bool HasWritePermission { get; set; }
        [ProtoMember(6)]
        public bool CanBeRenamedOrDeleted { get; set; }
    }

    [ProtoContract(EnumPassthru = true)]
    public enum DatabaseState
    {
        Launched = 1,
        Stopped = 2,
        Broken = 3,
        MarkedForDeletion = 4,
        Importing = 5,
        Updating = 6
    }

    [ProtoContract]
    public class DatabaseChangeset
    {
        [ProtoMember(1)]
        public String Changed { get; private set; }

        public DatabaseChangeset()
        {
            Changed = string.Empty;
        }

        public DatabaseChangeset(string databaseName)
        {
            Changed = databaseName;
        }
    }

    [ProtoContract]
    public class PersonChangeset
    {
        [ProtoMember(1)]
        public IEnumerable<DPerson> Changed { get; private set; }

        public PersonChangeset()
        {
        }

        public PersonChangeset(IEnumerable<DPerson> persons)
        {
            Changed = persons;
        }
    }

    [ProtoContract]
    public class OrganisationUnitChangeset
    {
        [ProtoMember(1)]
        public IEnumerable<DOrganisationUnit> Changed { get; private set; }

        public OrganisationUnitChangeset()
        {
        }

        public OrganisationUnitChangeset(IEnumerable<DOrganisationUnit> organisationUnits)
        {
            Changed = organisationUnits;
        }
    }

    [ProtoContract]
    public class DMetadataChangeset
    {
        [ProtoMember(1)]
        public long Version { get; private set; }

        public DMetadataChangeset()
        {
        }

        public DMetadataChangeset(long version)
        {
            Version = version;
        }
    }

    [ProtoContract(EnumPassthru = true)]
    public enum SearchKind
    {
        FullText,
        CustomFullText,
        CustomTopText,
        RecycleBin,
        PilotStorageRecycleBin,
        AllTasks,
        ActualTasks,
        AssignedTasks,
        RecivedTasks,
        OutdatedTasks,
        CompletedTasks,
        RevokedTasks,
        AuditorTasks,
        Custom,
        FileContent
    }

    [ProtoContract]
    public class DSearchDefinition
    {
        [ProtoMember(1)]
        public Guid Id { get; set; }

        [ProtoMember(2)]
        public string SearchString { get; set; }

        [ProtoMember(3), Obsolete("Use the DObject.CONTEXT field intead")]
        public Guid ContextObjectId { get; set; }
        
        [ProtoMember(4)]
        public SearchKind SearchKind { get; set; }

        [ProtoMember(5)]
        public int MaxResults { get; set; }

        [ProtoMember(6)]
        public string SortFieldName { get; set; }

        [ProtoMember(7)]
        public bool Ascending { get; set; }
        
        public DSearchDefinition()
        {
            Id = Guid.NewGuid();
            MaxResults = Constants.MAX_ITEMS_LOAD_PER_PAGE;
            SortFieldName = string.Empty;
        }
    }

    [ProtoContract]
    public class DSearchResult
    {
        [ProtoMember(1)]
        public Guid SearchDefinitionId { get; set; }

        [ProtoMember(2)]
        public IEnumerable<Guid> Found { get; set; }

        [ProtoMember(3)]
        public long TotalResults { get; set; }
    }

    [ProtoContract]
    public class DGeometrySearchDefinition
    {
        [ProtoMember(1)]
        public Guid Id { get; set; }

        [ProtoMember(2)]
        public string SearchPattern { get; set; }

        [ProtoMember(3)]
        public int AngularTolerance { get; set; }

        [ProtoMember(4)]
        public bool AllowRotate { get; set; }

        [ProtoMember(5)]
        public int PageIndex { get; set; }

        [ProtoMember(6)]
        public int PageSize { get; set; }
    }
    
    [ProtoContract]
    public class DGeometrySearchResultInfo
    {
        [ProtoMember(1)]
        public string ThumbName { get; set; }

        [ProtoMember(2)]
        public byte[] Thumbnail { get; set; }

        [ProtoMember(3)]
        public Guid ObjectId { get; set; }

        [ProtoMember(4)]
        public int Total { get; set; }

        [ProtoMember(5)]
        public double X { get; set; }

        [ProtoMember(6)]
        public double Y { get; set; }

        [ProtoMember(7)]
        public double LeftUpX { get; set; }

        [ProtoMember(8)]
        public double LeftUpY { get; set; }
    }

    [ProtoContract]
    public class DGeometrySearchResult
    {
        [ProtoMember(1)]
        public Guid SearchDefinitionId { get; set; }

        [ProtoMember(2)]
        public IEnumerable<DGeometrySearchResultInfo> Found { get; set; }
    }

    [ProtoContract]
    public class DNotificationChangeset
    {
        [ProtoMember(1)]
        public IEnumerable<DNotification> Changed { get; set; }

        public DNotificationChangeset()
        {
            
        }

        public DNotificationChangeset(IEnumerable<DNotification> notifications)
        {
            Changed = notifications;
        }
    }

    [ProtoContract]
    public class DNotification
    {
        [ProtoMember(8)]
        public Guid Id { get; set; }
        
        [ProtoMember(1)]
        public Guid ObjectId { get; set; }

        [ProtoMember(2)]
        public string Title { get; set; }

        [ProtoMember(3)]
        public int? InitiatedByPersonId { get; set; }

        [ProtoMember(4)]
        public DateTime DateTime { get; set; }

        [ProtoMember(5)]
        public int TypeId { get; set; }

        [ProtoMember(6)]
        public NotificationChangeKind ChangeKind { get; set; }

        [ProtoMember(7)]
        public NotificationSourceKind SourceKind { get; set; }

        [ProtoMember(9)]
        public DateTime VersionTime { get; set; }

        [ProtoMember(10)]
        public string NameOfChangedFile { get; set; }
    }

    [ProtoContract]
    public class DSettingsCollection
    {
        [ProtoMember(1)]
        public Dictionary<string, string> Values { get; private set; }

        public DSettingsCollection()
        {
            Values = new Dictionary<string, string>();
        }
    }

    [ProtoContract]
    public class DSettings
    {
        [ProtoMember(1)]
        public Dictionary<int, DSettingsCollection> PersonalSettings { get; private set; }

        [ProtoMember(2)]
        public Dictionary<int, DSettingsCollection> CommonSettings { get; private set; }

        public DSettings()
        {
            PersonalSettings = new Dictionary<int, DSettingsCollection>();
            CommonSettings = new Dictionary<int, DSettingsCollection>();
        }
    }

    [ProtoContract]
    public class DSettingsChange
    {
        public const int UNDEFINED = -1;

        public DSettingsChange()
        {
            OrgUnitId = UNDEFINED;
            PersonId = UNDEFINED;
        }

        [ProtoMember(1)]
        public Guid Identity { get; set; }

        [ProtoMember(2)]
        public int OrgUnitId { get; set; }

        [ProtoMember(3)]
        public int PersonId { get; set; }

        [ProtoMember(4)]
        public string Key { get; set; }

        [ProtoMember(5)]
        public string Value { get; set; }
    }

    [ProtoContract]
    public class DReservation
    {
        [ProtoMember(1)]
        public Guid DatabaseId { get; set; }

        [ProtoMember(2)]
        public int ProductId { get; set; }

        [ProtoMember(3)]
        public int OrganisationUnitId { get; set; }

        [ProtoMember(4)]
        public int Amount { get; set; }

        [ProtoMember(5)]
        public string OrganisationUnitTitle { get; set; }

        [ProtoMember(6)]
        public bool IsLastLeaf { get; set; }

        public override bool Equals(object obj)
        {
            var reservation = obj as DReservation;
            if (reservation == null)
                return false;
            if (reservation == this)
                return true;

            return DatabaseId.Equals(reservation.DatabaseId) &&
                   ProductId == reservation.ProductId &&
                   OrganisationUnitId == reservation.OrganisationUnitId;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = DatabaseId.GetHashCode();
                hashCode = (hashCode*397) ^ ProductId;
                hashCode = (hashCode*397) ^ OrganisationUnitId;
                return hashCode;
            }
        }
    }
    
    [ProtoContract]
    public class DActiveSession
    {
        [ProtoMember(1)]
        public Guid SessionId { get; set; }

        [ProtoMember(2)]
        public int ProductId { get; set; }

        [ProtoMember(3)]
        public string UserName { get; set; }

        [ProtoMember(4)]
        public string Ip { get; set; }

        [ProtoMember(5)]
        public string HostName { get; set; }

        [ProtoMember(6)]
        public Guid DatabaseId { get; set; }
        
        public override bool Equals(object other)
        {
            var session = other as DActiveSession;
            if (session == null)
                return false;
            return SessionId.Equals(session.SessionId);

        }

        public override int GetHashCode()
        {
            return SessionId.GetHashCode();
        }
    }
    
    [ProtoContract]
    public class DAnnotationCorrespondenceHistory
    {
        [ProtoMember(1)]
        public Guid AnnotationId { get; set; }

        [ProtoMember(2)]
        public IEnumerable<Guid> CorrespondenceIds { get; set; }
    }

    [ProtoContract(EnumPassthru = true)]
    public enum CounterResetPeriod
    {
        None,
        Monthly,
        Quarterly,
        Yearly
    }

    [ProtoContract]
    public class DCounter
    {
        public DCounter()
        {
        }

        public DCounter(string name)
        {
            Name = name;
        }
        
        [ProtoMember(1)]
        public long Value { get; set; }

        [ProtoMember(2)]
        public CounterResetPeriod ResetPeriod { get; set; }

        [ProtoMember(3)]
        public DateTime NextResetTime { get; set; }

        [ProtoMember(4)]
        public string Name { get; set; }
    }

    [ProtoContract(EnumPassthru = true)]
    public enum LockState
    {
        None,
        Requested,
        Accepted
    }

    [ProtoContract]
    [DebuggerDisplay("State: {State} PersonId: {PersonId} Date: {Date}")]
    public class DLockInfo
    {
        [ProtoMember(1)]
        public LockState State { get; set; }

        [ProtoMember(2)]
        public DateTime Date { get; set; }
        
        [ProtoMember(3)]
        public int PersonId { get; set; }

        protected bool Equals(DLockInfo other)
        {
            return State == other.State && Date.Equals(other.Date) && PersonId == other.PersonId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DLockInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = State.GetHashCode();
                hashCode = (hashCode*397) ^ Date.GetHashCode();
                hashCode = (hashCode*397) ^ PersonId;
                return hashCode;
            }
        }

        public void AssignTo(DLockInfo lockInfo)
        {
            lockInfo.State = State;
            lockInfo.Date = Date;
            lockInfo.PersonId = PersonId;
        }

        public void Lock(int personId, DateTime date)
        {
            State = LockState.Requested;
            PersonId = personId;
            Date = date;
        }

        public void Unlock()
        {
            State = LockState.None;
            Date = default(DateTime);
            PersonId = default(int);
        }
    }

    [ProtoContract]
    [DebuggerDisplay("State: {State} PersonId: {PersonId} Date: {Date}")]
    public class DStateInfo
    {
        public DStateInfo()
        {
            State = ObjectState.Alive;
        }

        [ProtoMember(1)]
        public ObjectState State { get; set; }

        [ProtoMember(2)]
        public DateTime Date { get; set; }

        [ProtoMember(3)]
        public int PersonId { get; set; }

        [ProtoMember(4)]
        public int PositionId { get; set; }

        protected bool Equals(DStateInfo other)
        {
            return State == other.State && Date.Equals(other.Date) && PersonId == other.PersonId && PositionId == other.PositionId;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DStateInfo)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = State.GetHashCode();
                hashCode = (hashCode * 397) ^ Date.GetHashCode();
                hashCode = (hashCode * 397) ^ PersonId;
                hashCode = (hashCode * 397) ^ PositionId;
                return hashCode;
            }
        }

        public void AssignTo(DStateInfo stateInfo)
        {
            stateInfo.State = State;
            stateInfo.Date = Date;
            stateInfo.PersonId = PersonId;
            stateInfo.PositionId = PositionId;
        }

        public bool IsLockStateNone()
        {
            return State != ObjectState.LockAccepted && State != ObjectState.LockRequested;
        }

        public void SetState(ObjectState state, int personId, int positionId, DateTime date)
        {
            State = state;
            PersonId = personId;
            PositionId = positionId;
            Date = date;
        }

        public void SetState(ObjectState state, int personId, int positionId)
        {
            SetState(state, personId, positionId, DateTime.UtcNow);
        }

        public void SetState(DStateInfo stateInfo)
        {
            SetState(stateInfo.State, stateInfo.PersonId, stateInfo.PositionId, stateInfo.Date);
        }
    }
}
