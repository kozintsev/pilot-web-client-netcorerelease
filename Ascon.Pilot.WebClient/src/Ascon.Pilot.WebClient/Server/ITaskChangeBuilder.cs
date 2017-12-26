using System;
using System.Collections.Generic;
using Ascon.Pilot.Core;
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
}
