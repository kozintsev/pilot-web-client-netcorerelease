using Ascon.Pilot.Core;
using Ascon.Pilot.WebClient.Models;
using Ascon.Pilot.WebClient.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ascon.Pilot.WebClient.ViewModels
{
    public class TaskNode
    {
        public TaskNode(DObject obj, IRepository repository)
        {
            var task = new DTask(obj, repository);
            Id = task.Id;
            Name = task.DisplayTitle;
            Initiator = task.Initiator.GetActualName();
            Executor = task.Executor.GetActualName();
            State = task.State;
            Kind = task.Kind;
            DateOfAssignment = task.DateOfAssignment;
            DeadlineDate = task.DeadlineDate;
            DateOfStart = task.DateOfStart;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Initiator { get; private set; }
        public string Executor { get; private set; }
        public TaskState State { get; private set; }
        public TaskKind Kind { get; private set; }
        public DateTime DateOfAssignment { get; private set; }
        public DateTime DeadlineDate { get; private set; }
        public DateTime? DateOfStart { get; private set; }
    }
}
