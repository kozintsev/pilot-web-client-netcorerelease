using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ascon.Pilot.DataClasses;
using Ascon.Pilot.Web.Extensions;
using Ascon.Pilot.Web.Models;

namespace Ascon.Pilot.Web.ViewModels
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

        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Initiator { get; private set; }
        public string Executor { get; private set; }
        public IEnumerable<Attachment> Attachments { get; private set; } = new List<Attachment>();
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

        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string FileExtension { get; private set; }
        public MType Type { get; private set; }
    }
}
