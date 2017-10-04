using Ascon.Pilot.WebClient.Extensions;
using Ascon.Pilot.WebClient.ViewModels;
using Ascon.Pilot.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ascon.Pilot.WebClient.Models;
using Ascon.Pilot.Server.Api.Contracts;

namespace Ascon.Pilot.WebClient.ViewComponents
{
    public class TaskListViewComponent : ViewComponent, IListener
    {
        private ServerCallback _callback = new ServerCallback();
        private TaskCompletionSource<IEnumerable<TaskNode>> _tcs;
        private IServerApi _serverApi;

        public TaskListViewComponent()
        {
            
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var items = await GetItemsAsync(id);
            return View(items);
        }

        public void Notify(DSearchResult result)
        {
            _callback.Unsubscribe();
            var objects = _serverApi?.GetObjects(result.Found.ToArray());
            var list = objects.Select(o => new TaskNode(o));
            _tcs?.SetResult(list);
        }

        private Task<IEnumerable<TaskNode>> GetItemsAsync(int id)
        {
            _callback.Subscribe(this);
            //TODO DI!!!
            _tcs = new TaskCompletionSource<IEnumerable<TaskNode>>();
            _serverApi = HttpContext.GetServerApi(_callback);
            var searchDefinition = new DSearchDefinition()
            {
                Id = Guid.NewGuid(),
                MaxResults = 10,
                SearchKind = SearchKind.AllTasks,
                Ascending = true,
                SortFieldName = SystemAttributes.TASK_DATE_OF_ASSIGNMENT
            };

            _serverApi.AddSearch(searchDefinition);
            return _tcs.Task;
        }
    }
}
