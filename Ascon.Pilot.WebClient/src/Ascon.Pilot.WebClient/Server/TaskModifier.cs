using System;
using System.Collections.Generic;
using System.Linq;
using Ascon.Pilot.Core;

namespace Ascon.Pilot.WebClient.Server
{
    public interface ITaskModifier
    {
        void Apply(DChangesetData changesetData);
        DChange EditStage(DObject stage);
        DChange EditTask(DObject task);
        DChange EditTaskById(Guid taskId);
        DChange EditWorkflow(DObject workflow);
        DObject NewAgreementTask(DObject workflow, DObject stage, int executorPosition);
        DObject NewStage(DObject workflow);
        IEnumerable<DObject> NewTask(string title, string description, IEnumerable<int> executors, DateTime deadLine);
        IEnumerable<DObject> NewTask(string title, string description, IEnumerable<int> executors, DateTime deadLine, IEnumerable<Guid> attachments);
        DObject NewTask(DObject workflow, DObject stage, int executorPosition);
        DObject NewWorkflow();
    }

    public class TaskModifier : ITaskModifier
    {
        private readonly IServerConnector _connector;
        private readonly int _personId;

        public TaskModifier(IServerConnector connector, int personId)
        {
            _connector = connector;
            _personId = personId;
        }

        public void Apply(DChangesetData changesetData)
        {
            _connector.ServerApi.Change(changesetData);
        }

        public DChange EditStage(DObject stage)
        {
            throw new NotImplementedException();
        }

        public DChange EditTask(DObject task)
        {
            var changed = task.Clone();
            var change = new DChange { Old = task, New = changed };
            return change;
        }

        public DChange EditTaskById(Guid taskId)
        {
            throw new NotImplementedException();
        }

        public DChange EditWorkflow(DObject workflow)
        {
            throw new NotImplementedException();
        }

        public DObject NewAgreementTask(DObject workflow, DObject stage, int executorPosition)
        {
            throw new NotImplementedException();
        }

        public DObject NewStage(DObject workflow)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DObject> NewTask(string title, string description, IEnumerable<int> executors, DateTime deadLine)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DObject> NewTask(string title, string description, IEnumerable<int> executors, DateTime deadLine,
            IEnumerable<Guid> attachments)
        {
            throw new NotImplementedException();
        }

        public DObject NewTask(DObject workflow, DObject stage, int executorPosition)
        {
            throw new NotImplementedException();
        }

        public DObject NewWorkflow()
        {
            throw new NotImplementedException();
        }
    }
}
