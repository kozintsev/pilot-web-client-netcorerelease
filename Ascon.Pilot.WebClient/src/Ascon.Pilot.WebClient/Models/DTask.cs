using Ascon.Pilot.Core;
using Ascon.Pilot.WebClient.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Ascon.Pilot.WebClient.Models
{
    public class DTask
    {
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
            Id = obj.Id;
            _source = obj;
        }

        public Guid Id { get; }

        public Guid ParentId => _source.ParentId;

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
                if (_executorPosition != null) return _executorPosition;
                if (!_source.Attributes.TryGetValue(SystemAttributes.TASK_EXECUTOR_POSITION, out var executorPositionId)
                ) return _executorPosition;
                if (int.TryParse(executorPositionId.ToString(), out var pos))
                {
                    _executorPosition = _repository.GetOrganisationUnit(pos);
                }
                return _executorPosition;
            }
        }

        public DOrganisationUnit InitiatorPosition
        {
            get
            {
                if (_initiatorPosition != null) return _initiatorPosition;
                if (!_source.Attributes.TryGetValue(SystemAttributes.TASK_INITIATOR_POSITION,
                    out var initiatorPositionId)) return _initiatorPosition;
                if (int.TryParse(initiatorPositionId.ToString(), out var pos))
                {
                    _initiatorPosition = _repository.GetOrganisationUnit(pos);
                }
                return _initiatorPosition;
            }
        }

        public string ExecutorRole
        {
            get
            {
                _source.Attributes.TryGetValue(SystemAttributes.TASK_EXECUTOR_ROLE, out var role);
                return role?.StrValue;
            }
        }

        //public DPerson Creator
        //{
        //    get { return _person ?? (_person = _repository.GetPerson(_source.CreatorId)); }
        //}

        public DateTime Created => _source.Created;

        //public IList<Guid> Children
        //{
        //    get
        //    {
        //        var filter = new TasksTypeFilter(() => _repository);
        //        return new ReadOnlyCollection<Guid>(_source.Children.Filter(filter).ToList());
        //    }
        //}

        public ReadOnlyCollection<Guid> InitiatorAttachments
        {
            get { return new ReadOnlyCollection<Guid>(_source.Relations.Where(x => x.Type == RelationType.TaskInitiatorAttachments).Select(x => x.TargetId).ToList()); }
        }

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

        public DObject Source => _source ?? new DObject { Id = Id };

        public string Title
        {
            get
            {
                if (_source.Attributes.TryGetValue(SystemAttributes.TASK_TITLE, out var title))
                {
                    return title == null ? string.Empty : title.ToString();
                }

                return string.Empty;
            }
        }

        public string DisplayTitle => string.IsNullOrEmpty(Title) ? Description.LimitCharacters(150) : Title;

        public TaskKind Kind
        {
            get
            {
                if (_source.Attributes.TryGetValue(SystemAttributes.TASK_KIND, out var result))
                {
                    return (TaskKind)int.Parse(result.ToString());
                }

                return TaskKind.Task;
            }
        }

        public string Description
        {
            get
            {
                if (_source.Attributes.TryGetValue(SystemAttributes.TASK_DESCRIPTION, out var description))
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
                if (!_source.Attributes.TryGetValue(SystemAttributes.TASK_DEADLINE_DATE, out var deadLine))
                    return DateTime.MaxValue;
                return deadLine.DateValue != null ? ((DateTime)deadLine).ToLocalTime() : DateTime.MaxValue;
            }
        }

        public DateTime DateOfAssignment
        {
            get
            {
                if (!_source.Attributes.TryGetValue(SystemAttributes.TASK_DATE_OF_ASSIGNMENT, out var date)) return Created;
                return date.DateValue != null ? ((DateTime)date).ToLocalTime() : Created;
            }
        }

        public DateTime? DateOfCompletion => _source.Attributes.TryGetValue(SystemAttributes.TASK_DATE_OF_COMPLETION, out var date) ? date.DateValue?.ToLocalTime() : null;

        public DateTime? DateOfStart => _source.Attributes.TryGetValue(SystemAttributes.TASK_DATE_OF_START, out var date) ? date.DateValue?.ToLocalTime() : null;

        public DateTime? DateOfRevokation => _source.Attributes.TryGetValue(SystemAttributes.TASK_DATE_OF_REVOKATION, out var date) ? date.DateValue?.ToLocalTime() : null;

        public TaskState State
        {
            get
            {
                if (_source.Attributes.TryGetValue(SystemAttributes.TASK_STATE, out var state))
                {
                    return (TaskState)Enum.Parse(typeof(TaskState), state.ToString());
                }

                return TaskState.None;
            }
        }


        public bool IsVersion => _source.Attributes.TryGetValue(SystemAttributes.TASK_IS_VERSION, out var result) && Convert.ToBoolean(result.IntValue);
    }

    public static class TaskExtensions
    {
        public static string GetTitle(this DTask task)
        {
            return string.IsNullOrEmpty(task.Title) ? task.Description.LimitCharacters(150) : task.Title;
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
