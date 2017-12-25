using System;
using System.Collections.Generic;
using System.Linq;
using Ascon.Pilot.Core;
using Ascon.Pilot.Server.Api.Contracts;
using Ascon.Pilot.WebClient.Models;

namespace Ascon.Pilot.WebClient.Server
{
    public interface ITaskChangeBuilder
    {
        ITaskChangeBuilder SetAttribute(string name, DValue value);
        ITaskChangeBuilder SetState(TaskState state);
        ITaskChangeBuilder SetAttributes(IEnumerable<KeyValuePair<string, DValue>> attributes);
        ITaskChangeBuilder RemoveAttribute(string name);
        ITaskChangeBuilder RemoveTaskChild(Guid childId);
        ITaskChangeBuilder SetExecutor(int positionId);
        ITaskChangeBuilder SetTitle(string title);
        ITaskChangeBuilder SetDescription(string description);
        ITaskChangeBuilder SetStageOrder(long order);
        ITaskChangeBuilder AddChild(Guid childId, int typeId);
        ITaskChangeBuilder AddTaskChild(Guid childId);
        ITaskChangeBuilder SetIsVersion(bool isVersion);
        ITaskChangeBuilder SetParent(Guid id);
        DObject GetNewTaskObject();
        IEnumerable<Guid> NewFileIds { get; }
        DChange Change { get; }
    }

    public class TaskChangeBuilder : ITaskChangeBuilder
    {
        private readonly DChange _change;
        private readonly TaskModifier _modifier;
        private readonly IServerApi _backend;
        private readonly List<Guid> _newFileIds = new List<Guid>();

        public TaskChangeBuilder(DChange change, TaskModifier modifier, IServerApi backend)
        {
            _change = change;
            _modifier = modifier;
            _backend = backend;
        }

        public ITaskChangeBuilder SetAttribute(string name, DValue value)
        {
            _change.New.Attributes[name] = value;
            return this;
        }

        public ITaskChangeBuilder SetAttributes(IEnumerable<KeyValuePair<string, DValue>> attributes)
        {
            foreach (var attribute in attributes)
                _change.New.Attributes[attribute.Key] = attribute.Value;
            return this;
        }

        public ITaskChangeBuilder RemoveAttribute(string name)
        {
            _change.New.Attributes.Remove(name);
            return this;
        }

        public ITaskChangeBuilder SetTitle(string title)
        {
            if (title == null)
                title = string.Empty;

            SetAttribute(SystemAttributes.TASK_TITLE, title);
            return this;
        }

        public ITaskChangeBuilder SetDescription(string description)
        {
            if (description == null)
                description = string.Empty;

            SetAttribute(SystemAttributes.TASK_DESCRIPTION, description);
            return this;
        }

        public ITaskChangeBuilder SetState(TaskState state)
        {
            SetAttribute(SystemAttributes.TASK_STATE, (int)state);
            if (state == TaskState.OnValidation)
                SetDateOfCompletion(DateTime.UtcNow);
            if (state == TaskState.InProgress)
                SetDateOfStart(DateTime.UtcNow);
            if (state == TaskState.Revoked)
                SetDateOfRevokation(DateTime.UtcNow);
            if (state == TaskState.Assigned)
            {
                SetDateOfStart(null);
                SetDateOfRevokation(null);
            }
            return this;
        }

        public ITaskChangeBuilder SetExecutor(int positionId)
        {
            SetAttribute(SystemAttributes.TASK_EXECUTOR_POSITION, positionId);
            return this;
        }

        public ITaskChangeBuilder SetInitiator(int positionId)
        {
            SetAttribute(SystemAttributes.TASK_INITIATOR_POSITION, positionId);
            return this;
        }

        public ITaskChangeBuilder SetParent(Guid id)
        {
            if (_change.Old != null)
                _change.Old.ParentId = Guid.NewGuid();
            _change.New.ParentId = id;
            return this;
        }

        public ITaskChangeBuilder SetIsVersion(bool isVersion)
        {
            SetAttribute(SystemAttributes.TASK_IS_VERSION, Convert.ToInt32(isVersion));
            return this;
        }

        public ITaskChangeBuilder AddChild(Guid childId, int typeId)
        {
            if (!_change.New.Children.Exists(ch => ch.ObjectId == childId))
                _change.New.Children.Add(new DChild { ObjectId = childId, TypeId = typeId });

            return this;
        }

        public ITaskChangeBuilder AddTaskChild(Guid childId)
        {
            if (!_change.New.Children.Exists(ch => ch.ObjectId == childId))
            {
                var objs = _backend.GetObjects(new[] {DObject.TaskRootId });
                var typeId = _backend.GetObjects(new[] {DObject.TaskRootId}).First().TypeId;
                //var typeId = 1; //GetType(SystemTypes.TASK).Id;
                _change.New.Children.Add(new DChild { ObjectId = childId, TypeId = typeId });
            }
            return this;
        }

        public ITaskChangeBuilder AddRelation(DRelation relation)
        {
            _change.New.Relations.Add(relation);
            return this;
        }

        public ITaskChangeBuilder RemoveRelationById(Guid relationId)
        {
            _change.New.Relations.RemoveAll(x => x.Id == relationId);
            return this;
        }

        public ITaskChangeBuilder ClearRelations(RelationType relationType)
        {
            _change.New.Relations.RemoveAll(x => x.Type == relationType);
            return this;
        }

        public ITaskChangeBuilder SetStageOrder(long order)
        {
            SetAttribute(SystemAttributes.TASK_STAGE_ORDER, order);
            return this;
        }

        public ITaskChangeBuilder RemoveTaskChild(Guid childId)
        {
            var item = new DChild {ObjectId = childId};
            if (_change.Old != null && !_change.Old.Children.Exists(ch => ch.ObjectId == childId))
            {
                var typeId = GetType(SystemTypes.TASK).Id;
                _change.Old.Children.Add(new DChild { ObjectId = childId, TypeId = typeId });
            }
            _change.New.Children.Remove(item);
            return this;
        }

        public ITaskChangeBuilder SetValidationPhase(bool withValidation)
        {
            SetAttribute(SystemAttributes.TASK_WITH_VALIDATION_PHASE, withValidation.ToString());
            return this;
        }

        public ITaskChangeBuilder SetDateOfAssignment(DateTime dateTime)
        {
            SetAttribute(SystemAttributes.TASK_DATE_OF_ASSIGNMENT, dateTime);
            return this;
        }

        public ITaskChangeBuilder SetDateOfCompletion(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                RemoveAttribute(SystemAttributes.TASK_DATE_OF_COMPLETION);
                return this;
            }

            SetAttribute(SystemAttributes.TASK_DATE_OF_COMPLETION, dateTime);
            return this;
        }

        public ITaskChangeBuilder SetDateOfStart(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                RemoveAttribute(SystemAttributes.TASK_DATE_OF_START);
                return this;
            }

            SetAttribute(SystemAttributes.TASK_DATE_OF_START, dateTime);
            return this;
        }

        public ITaskChangeBuilder SetDateOfRevokation(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                RemoveAttribute(SystemAttributes.TASK_DATE_OF_REVOKATION);
                return this;
            }

            SetAttribute(SystemAttributes.TASK_DATE_OF_REVOKATION, dateTime);
            return this;
        }

        public ITaskChangeBuilder SetIsCheckedSignMeAs(bool isCheckedSignMeAs)
        {
            SetAttribute(SystemAttributes.TASK_IS_CHECKED_SIGN_ME_AS, Convert.ToInt32(isCheckedSignMeAs));
            return this;
        }

        public ITaskChangeBuilder SetSelectedRole(string selectedRole)
        {
            SetAttribute(SystemAttributes.TASK_SELECTED_ROLE, selectedRole);
            return this;
        }

        public ITaskChangeBuilder SetAuditors(IEnumerable<int> auditors)
        {
            if (auditors == null)
                return this;

            var auditrsArray = auditors.Select(a => a.ToString()).ToArray();
            SetAttribute(SystemAttributes.TASK_AUDITORS, auditrsArray);
            return this;
        }

        public ITaskChangeBuilder SetExecutorRole(string role)
        {
            if (role == null)
                return this;

            SetAttribute(SystemAttributes.TASK_EXECUTOR_ROLE, role);
            return this;
        }

        public ITaskChangeBuilder SetMessageText(string messageText)
        {
            SetAttribute(SystemAttributes.TASK_MESSAGE_TEXT_ATTRIBUTE_NAME, messageText);
            return this;
        }

        public ITaskChangeBuilder SetTags(IEnumerable<string> tags)
        {
            if (tags == null)
                return this;

            SetAttribute(SystemAttributes.TAGS, tags.ToArray());
            return this;
        }

        public IEnumerable<Guid> NewFileIds => _newFileIds;

        public DObject GetNewTaskObject()
        {
            return new DObject();
        }

        public DChange Change => new DChange();

        private MType GetType(string name)
        {
            //return _backend.GetTypes().FirstOrDefault(x => x.Name == name) ?? null;
            return null;
        }
    }
}
