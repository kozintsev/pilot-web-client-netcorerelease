using System;
using System.Collections.Generic;
using System.Linq;
using Ascon.Pilot.Core;
using Ascon.Pilot.Server.Api.Contracts;
using Ascon.Pilot.WebClient.Models;

namespace Ascon.Pilot.WebClient.Server
{
    public class TaskModifier
    {
        private readonly IServerApi _backend;
        private readonly IRepository _repository;
        private readonly Dictionary<Guid, ITaskChangeBuilder> _builders = new Dictionary<Guid, ITaskChangeBuilder>();
        private const int TaskTypeId = 6;

        public TaskModifier(IRepository repository)
        {
            if (repository == null)
                throw new ArgumentNullException(nameof(repository));
            _repository = repository;
            _backend = repository.GetServerApi();
        }

        public ITaskChangeBuilder EditWorkFlow(Guid sourceId)
        {
            return InnerEdit(sourceId);
        }

        public DObject NewStage(Guid id, Guid parentWorkflowId, long order)
        {
            var stage = CreateNewObject(id, parentWorkflowId, TaskTypeId).SetStageOrder(order);
            return stage.GetNewTaskObject();
        }

        public ITaskChangeBuilder EditStage(Guid sourceId)
        {
            return InnerEdit(sourceId);
        }
        

        public ITaskChangeBuilder DeleteStage(Guid workflowId, Guid stageId)
        {
            return Edit(workflowId).RemoveTaskChild(stageId);
        }
        

        public ITaskChangeBuilder UpdateTask(Guid taskId, int executorPosition)
        {
            UpdateState(taskId, executorPosition);
            return Edit(taskId).SetExecutor(executorPosition);
        }

        public void Apply()
        {
            var changes = _builders.Select(x => x.Value.Change).ToArray();

            if (!changes.Any())
                throw new InvalidOperationException("There are no changes to apply");

            //Пустое изменение
            CheckTaskChangesApply(changes);

            var changesetData = new DChangesetData {Identity = Guid.NewGuid()};

            //Добавим изменение (создание) текущего объекта
            changesetData.Changes.AddRange(changes);

            _backend.Change(changesetData);

            _builders.Clear();
        }

        private void UpdateState(Guid taskId, int executorPosition)
        {
            var task = GetActualObject(taskId);
            var oldExecutorPosition = task.GetExecutorPosition();

            var oldState = (TaskState)(long)task.Attributes["TaskState B65D6C5B-7D8E-4055-852F-D1AAB060CD22"]; 

            var isExecutorChanged = oldExecutorPosition != executorPosition;
            if (isExecutorChanged && oldState != TaskState.Revoked)
            {
                Edit(taskId).SetState(TaskState.Assigned);
                return;
            }

            if (isExecutorChanged)
                InnerEdit(taskId).SetState(TaskState.Revoked);
        }

        private ITaskChangeBuilder CreateNewObject(Guid id, Guid parentId, int typeId)
        {
            var obj = new DObject
            {
                Id = id,
                ParentId = parentId,
                TypeId = typeId,
                CreatorId = _repository.CurrentPerson().Id,
                Created = DateTime.UtcNow
            };

            InnerEdit(parentId).AddChild(obj.Id, typeId);
            return CreateChangeBuilder(obj.Id, null, obj).SetParent(parentId);
        }

        //При изменении этого метода не забыть поправить TaskHistoryChangeHelper.PrepareHistoryChanges
        //автоматический переход задания в состояние "Выпонено" при согласовании документов
        private void CreateTaskVersion(Guid sourceId)
        {
            var newId = Guid.NewGuid();
            var source = GetActualObject(sourceId);
            var newObj = source.Clone();
            newObj.Access.Clear();
            newObj.Children.Clear();
            newObj.Id = newId;
            newObj.Created = DateTime.UtcNow;
            newObj.ClearTaskVersions();
            newObj.CreatorId = source.CreatorId;

            // добавим текущей таске в дети клона
            InnerEdit(source.Id).AddTaskChild(newId);

            //создадим change для клона
            //клону добавим папу - текущую таску
            CreateChangeBuilder(newObj.Id, null, newObj).SetParent(source.Id).SetIsVersion(true);
        }

        public ITaskChangeBuilder Edit(Guid id)
        {
            CreateTaskVersion(id);
            return InnerEdit(id);
        }

        public ITaskChangeBuilder InnerEdit(Guid id)
        {
            ITaskChangeBuilder builder;
            if (_builders.TryGetValue(id, out builder)) return builder;
            var obj = _backend.GetObjects(new[] { id }).First(); 
            var changed = obj.Clone();
            builder = CreateChangeBuilder(obj.Id, obj, changed);
            return builder;
        }

        private DObject GetActualObject(Guid id)
        {
            ITaskChangeBuilder builder;
            return _builders.TryGetValue(id, out builder) ? builder.GetNewTaskObject() : _backend.GetObjects(new[] { id }).First();
        }

        private ITaskChangeBuilder CreateChangeBuilder(Guid builderId, DObject old, DObject @new)
        {
            var change = new DChange { Old = old, New = @new };
            var builder = new TaskChangeBuilder(change, this, _backend);
            _builders[builderId] = builder;
            return builder;
        }

        private void CheckTaskChangesApply(IEnumerable<DChange> changes)
        {
            var changesList = changes.ToList();
            if (changesList.Count != 2)
                return;

            var task = changesList[0].New;
            var version = changesList[1].New;
            if (!IsTaskChanged(task, version))
                throw new InvalidOperationException("There are no changes to apply");
        }

        private bool IsTaskChanged(DObject task, DObject version)
        {
            task.Attributes.Remove(SystemAttributes.TASK_IS_VERSION);

            version.Attributes.Remove(SystemAttributes.TASK_IS_VERSION);

            if (!task.Attributes.SequenceEqual(version.Attributes))
                return true;


            var taskAttachments = task.Relations.Where(x => x.Type == RelationType.TaskInitiatorAttachments ||
                                                            x.Type == RelationType.TaskExecutorAttachments);
            var versionAttachments = version.Relations.Where(x => x.Type == RelationType.TaskInitiatorAttachments ||
                                                                  x.Type == RelationType.TaskExecutorAttachments);

            return !taskAttachments.SequenceEqualIgnoreOrder(versionAttachments);
        }

        public void SetExecutionDepartment(Guid taskId, int orgUnitPosition)
        {
            InnerEdit(taskId).SetAttribute(SystemAttributes.TASK_EXECUTION_DEPARTMENT, orgUnitPosition);
        }
    }
}
