using Ascon.Pilot.Core;
using Ascon.Pilot.WebClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ascon.Pilot.WebClient.Server;

namespace Ascon.Pilot.WebClient.ViewModels
{
    public class TaskDetailsViewModel
    {
        private readonly IRepository _repository;
        private readonly Guid _taskId;

        public TaskDetailsViewModel(Guid taskId, IRepository repository)
        {
            _repository = repository;
            _taskId = taskId;
            IsChangeState = false;
            var objs = repository.GetObjects(new[] { taskId });
            if (!objs.Any())
                return;

            var task = new DTask(objs.First(), repository);
            Title = task.Title;
            Description = task.Description;
            Initiator = task.GetInitiatorDisplayName(repository);
            Executor = task.GetExecutorDisplayName(repository);
            if (task.InitiatorAttachments.Any())
            {
                Attachments = repository.GetObjects(task.InitiatorAttachments.ToArray()).Select(a => new Attachment(a, repository));
            }

            NumberState = (int) task.State;

            if (task.State == TaskState.Assigned)
            {
                IsChangeState = true;
                State = "Приступить к выполнению задания";
            }
            if (task.State == TaskState.InProgress)
            {
                IsChangeState = true;
                State = "Завершить работу по заданию";
            }

            if (Initiator == "Вы")
            {
                IsChangeState = true;
                State = "Отозвать задание";
            }

            if (task.State == TaskState.Revoked)
            {
                IsChangeState = true;
                State = "Отозвать задание";
            }

            if (task.State == TaskState.OnValidation && Initiator == "Вы")
            {
                IsChangeState = false;
                State = "Вернуть задание на доработку";
            }

            if (task.State == TaskState.Completed && Initiator == "Вы")
            {
                IsChangeState = false;
                State = "Подтвердить выполненние задания";
            }
        }

        public string Title { get; }
        public string Description { get; }
        public string Initiator { get; }
        public string Executor { get; }
        public string State { get; }
        public int NumberState { get; }
        public bool IsChangeState { get; }

        public IEnumerable<Attachment> Attachments { get; } = new List<Attachment>();

        public bool SetState(int oldState, int newState)
        {
            var modifier = new TaskModifier(_repository.GetServerApi());
            modifier.Edit(_taskId).SetState((TaskState) newState);
            modifier.Apply();
            return true;
        }
    }

    public class Attachment
    {
        public Attachment(DObject obj, IRepository repository)
        {
            Id = obj.Id;
            var type = repository.GetType(obj.TypeId);
            Title = obj.GetTitle(type);
            Type = type;
            if (Type.IsProjectFile())
            {
                FileExtension = Path.GetExtension(Title);
            }
        }

        public Guid Id { get; }
        public string Title { get; }
        public string FileExtension { get; }
        public MType Type { get; }
    }
}
