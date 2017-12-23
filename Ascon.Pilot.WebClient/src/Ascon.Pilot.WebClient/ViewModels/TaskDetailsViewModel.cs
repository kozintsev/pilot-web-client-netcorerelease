using Ascon.Pilot.Core;
using Ascon.Pilot.WebClient.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Ascon.Pilot.WebClient.ViewModels
{
    public class TaskDetailsViewModel
    {
        public TaskDetailsViewModel(Guid taskId, IRepository repository)
        {
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
        }

        public string Title { get; }
        public string Description { get; }
        public string Initiator { get; }
        public string Executor { get; }
        public IEnumerable<Attachment> Attachments { get; } = new List<Attachment>();
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
