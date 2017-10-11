using Ascon.Pilot.Core;
using Ascon.Pilot.WebClient.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ascon.Pilot.WebClient.Models
{
    public class DTask
    {
        private readonly Guid _id;
        private DPerson _initiator;
        private DPerson _executor;
        private DPerson _person;
        private readonly DObject _source;
        private DOrganisationUnit _executorPosition;
        private DOrganisationUnit _initiatorPosition;
        private readonly IRepository _repository;

        public DTask(DObject obj, IRepository repository)
        {
            _repository = repository;
            _id = obj.Id;
            _source = obj;
        }

        public Guid Id
        {
            get { return _id; }
        }

        public Guid ParentId
        {
            get { return _source.ParentId; }
        }

        public DPerson Initiator
        {
            get
            {
                if (_initiator != null)
                    return _initiator;

                return InitiatorPosition != null ? _repository.GetPersonOnOrganisationUnit(InitiatorPosition.Id) : _initiator;
            }
        }

        public DPerson Executor
        {
            get
            {
                if (_executor != null)
                    return _executor;

                return ExecutorPosition != null ? _repository.GetPersonOnOrganisationUnit(ExecutorPosition.Id) : _executor;
            }
        }

        public DOrganisationUnit ExecutorPosition
        {
            get
            {
                if (_executorPosition == null)
                {
                    DValue executorPositionId;
                    if (_source.Attributes.TryGetValue(SystemAttributes.TASK_EXECUTOR_POSITION, out executorPositionId))
                    {
                        int pos;
                        if (int.TryParse(executorPositionId.ToString(), out pos))
                        {
                            _executorPosition = _repository.GetOrganisationUnit(pos);
                        }
                    }
                }
                return _executorPosition;
            }
        }

        public DOrganisationUnit InitiatorPosition
        {
            get
            {
                if (_initiatorPosition == null)
                {
                    DValue initiatorPositionId;
                    if (_source.Attributes.TryGetValue(SystemAttributes.TASK_INITIATOR_POSITION, out initiatorPositionId))
                    {
                        int pos;
                        if (int.TryParse(initiatorPositionId.ToString(), out pos))
                        {
                            _initiatorPosition = _repository.GetOrganisationUnit(pos);
                        }
                    }
                }
                return _initiatorPosition;
            }
        }

        public string ExecutorRole
        {
            get
            {
                DValue role;
                _source.Attributes.TryGetValue(SystemAttributes.TASK_EXECUTOR_ROLE, out role);
                return role.StrValue;
            }
        }

        //public DPerson Creator
        //{
        //    get { return _person ?? (_person = _repository.GetPerson(_source.CreatorId)); }
        //}

        public DateTime Created
        {
            get { return _source.Created.ToLocalTime(); }
        }

        //public IList<Guid> Children
        //{
        //    get
        //    {
        //        var filter = new TasksTypeFilter(() => _repository);
        //        return new ReadOnlyCollection<Guid>(_source.Children.Filter(filter).ToList());
        //    }
        //}

        //public ReadOnlyCollection<Guid> InitiatorAttachments
        //{
        //    get { return new ReadOnlyCollection<Guid>(_source.Relations.Where(x => x.Type == RelationType.TaskInitiatorAttachments).Select(x => x.TargetId).ToList()); }
        //}

        //public ReadOnlyCollection<Guid> ExecutorAttachments
        //{
        //    get { return new ReadOnlyCollection<Guid>(_source.Relations.Where(x => x.Type == RelationType.TaskExecutorAttachments).Select(x => x.TargetId).ToList()); }
        //}

        //public Guid Chat
        //{
        //    get
        //    {
        //        var filter = new TaskChatTypeFilter(() => _repository);
        //        return _source.Children.Filter(filter).FirstOrDefault();
        //    }
        //}

        //public ReadOnlyCollection<NFile> Files
        //{
        //    get { return _source.ActualFileSnapshot.Files; }
        //}

        public DObject Source
        {
            get
            {
                if (_source == null)
                    return new DObject { Id = _id };
                return _source;
            }
        }

        public string Title
        {
            get
            {
                DValue title;
                if (_source.Attributes.TryGetValue(SystemAttributes.TASK_TITLE, out title))
                {
                    return title == null ? string.Empty : title.ToString();
                }

                return string.Empty;
            }
        }

        public string DisplayTitle
        {
            get
            {
                if (string.IsNullOrEmpty(Title))
                {
                    return Description.LimitCharacters(150);
                }

                return Title;
            }
        }

        public TaskKind Kind
        {
            get
            {
                DValue result;
                if (_source.Attributes.TryGetValue(SystemAttributes.TASK_KIND, out result))
                {
                    return (TaskKind)Int32.Parse(result.ToString());
                }

                return TaskKind.Task;
            }
        }

        public string Description
        {
            get
            {
                DValue description;
                if (_source.Attributes.TryGetValue(SystemAttributes.TASK_DESCRIPTION, out description))
                {
                    return description == null ? string.Empty : description.ToString();
                }

                return string.Empty;
            }
        }

        public DateTime DeadlineDate
        {
            get
            {
                DValue deadLine;
                if (_source.Attributes.TryGetValue(SystemAttributes.TASK_DEADLINE_DATE, out deadLine))
                {
                    if (deadLine.DateValue != null)
                        return ((DateTime)deadLine).ToLocalTime();
                }

                return DateTime.MaxValue;
            }
        }

        public DateTime DateOfAssignment
        {
            get
            {
                DValue date;
                if (_source.Attributes.TryGetValue(SystemAttributes.TASK_DATE_OF_ASSIGNMENT, out date))
                {
                    if (date.DateValue != null)
                        return ((DateTime)date).ToLocalTime();
                }

                return Created;
            }
        }

        public DateTime? DateOfCompletion
        {
            get
            {
                DValue date;
                if (_source.Attributes.TryGetValue(SystemAttributes.TASK_DATE_OF_COMPLETION, out date))
                {
                    return date.DateValue?.ToLocalTime();
                }

                return null;
            }
        }

        public DateTime? DateOfStart
        {
            get
            {
                DValue date;
                if (_source.Attributes.TryGetValue(SystemAttributes.TASK_DATE_OF_START, out date))
                {
                    return date.DateValue?.ToLocalTime();
                }

                return null;
            }
        }

        public DateTime? DateOfRevokation
        {
            get
            {
                DValue date;
                if (_source.Attributes.TryGetValue(SystemAttributes.TASK_DATE_OF_REVOKATION, out date))
                {
                    return date.DateValue?.ToLocalTime();
                }

                return null;
            }
        }

        public TaskState State
        {
            get
            {
                DValue state;
                if (_source.Attributes.TryGetValue(SystemAttributes.TASK_STATE, out state))
                {
                    return (TaskState)Enum.Parse(typeof(TaskState), state.ToString());
                }

                return TaskState.None;
            }
        }


        public bool IsVersion
        {
            get
            {
                DValue result;
                if (_source.Attributes.TryGetValue(SystemAttributes.TASK_IS_VERSION, out result))
                {
                    return Convert.ToBoolean(result.IntValue);
                }

                return false;
            }
        }
    }

    public static class TaskExtensions
    {
        public static string GetTitle(this DTask task)
        {
            if (string.IsNullOrEmpty(task.Title))
            {
                return task.Description.LimitCharacters(150);
            }

            return task.Title;
        }

        public static bool IsTaskExecutor(this DTask task, DPerson user)
        {
            var executorPosition = task.ExecutorPosition;
            return user.Positions.Any(x => x.Position == executorPosition.Id);
        }

        public static bool IsTaskInitiator(this DTask task, DPerson user)
        {
            var executorPosition = task.InitiatorPosition;
            return user.Positions.Any(x => x.Position == executorPosition.Id);
        }

        public static string GetInitiatorDisplayName(this DTask task, IRepository repository)
        {
            string name;
            if (task.Initiator == null)
            {
                name = repository.GetOrganisationUnit(task.InitiatorPosition.Id).Title;
            }
            else
            {
                name = IsTaskInitiator(task, repository.CurrentPerson()) ? "Вы" : task.Initiator.GetActualName();
            }

            return name;
        }

        public static string GetExecutorDisplayName(this DTask task, IRepository repository)
        {
            string name;
            if (task.Executor == null)
            {
                name = repository.GetOrganisationUnit(task.ExecutorPosition.Id).Title;
            }
            else
            {
                name = IsTaskExecutor(task, repository.CurrentPerson()) ? "Вы" : task.Executor.GetActualName();
            }

            return name;
        }
    }
}
