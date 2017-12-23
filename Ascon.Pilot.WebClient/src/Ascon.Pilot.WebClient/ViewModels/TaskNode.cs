using Ascon.Pilot.Core;
using Ascon.Pilot.WebClient.Models;
using System;

namespace Ascon.Pilot.WebClient.ViewModels
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

        public Guid Id { get; }
        public string Title { get; }
        public string Initiator { get; }
        public string Executor { get; }
        public TaskState State { get; }
        public TaskKind Kind { get; }
        public DateTime DateOfAssignment { get; }
        public DateTime DeadlineDate { get; }
        public DateTime? DateOfStart { get; }
    }
}
