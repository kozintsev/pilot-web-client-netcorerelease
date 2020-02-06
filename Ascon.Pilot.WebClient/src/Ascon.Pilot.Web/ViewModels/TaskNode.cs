using System;
using Ascon.Pilot.DataClasses;
using Ascon.Pilot.Web.Models;

namespace Ascon.Pilot.Web.ViewModels
{
    public class TaskNode
    {
        public TaskNode(DObject obj, IRepository repository)
        {
            var task = new DTask(obj, repository);
            Id = task.Id;
            Title = task.DisplayTitle;
            Initiator = task.GetInitiatorDisplayName(repository);
            Executor = task.GetExecutorDisplayName(repository);
            State = task.State;
            Kind = task.Kind;
            DateOfAssignment = task.DateOfAssignment;
            DeadlineDate = task.DeadlineDate;
            DateOfStart = task.DateOfStart;
        }

        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Initiator { get; private set; }
        public string Executor { get; private set; }
        public TaskState State { get; private set; }
        public TaskKind Kind { get; private set; }
        public DateTimeOffset DateOfAssignment { get; private set; }
        public DateTimeOffset DeadlineDate { get; private set; }
        public DateTimeOffset? DateOfStart { get; private set; }
    }
}
