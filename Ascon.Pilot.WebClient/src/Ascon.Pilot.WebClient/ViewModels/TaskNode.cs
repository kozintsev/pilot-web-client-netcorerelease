using Ascon.Pilot.Core;
using Ascon.Pilot.WebClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ascon.Pilot.WebClient.ViewModels
{
    public class TaskNode
    {
        public TaskNode(DObject obj)
        {
            var task = new DTask(obj);
            Name = task.Title;
        }

        public string Name { get; set; }
    }
}
