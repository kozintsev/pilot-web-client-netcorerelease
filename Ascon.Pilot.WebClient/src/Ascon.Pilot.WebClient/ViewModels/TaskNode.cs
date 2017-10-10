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
            Name = task.DisplayTitle;
            Initiator = task.Initiator.GetActualName();
            Executor = task.Executor.GetActualName();
        }

        public string Name { get; private set; }
        public string Initiator { get; private set; }
        public string Executor { get; private set; }
    }
}
