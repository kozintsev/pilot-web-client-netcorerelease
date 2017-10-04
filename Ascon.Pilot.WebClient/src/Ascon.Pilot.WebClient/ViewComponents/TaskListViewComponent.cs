using Ascon.Pilot.WebClient.Extensions;
using Ascon.Pilot.WebClient.ViewModels;
using Ascon.Pilot.Core;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ascon.Pilot.WebClient.Models;

namespace Ascon.Pilot.WebClient.ViewComponents
{
    public class TaskListViewComponent : ViewComponent
    {
        private TaskCompletionSource<IEnumerable<TaskNode>> _tcs;
        private IContext _context;

        public TaskListViewComponent(IContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int id)
        {
            var items = await GetItemsAsync(id);
            return View(items);
        }

        public void Notify(DSearchResult result)
        {
            if (result.Found == null)
            {
                _tcs?.SetResult(new List<TaskNode>());
                return;
            }

            //var objects = _repository?.GetObjects(result.Found.ToArray());
            //var list = objects.Select(o => new TaskNode(o));
            //_tcs?.SetResult(list);
        }

        private async Task<IEnumerable<TaskNode>> GetItemsAsync(int id)
        {
            //TODO DI!!!
            //_tcs = new TaskCompletionSource<IEnumerable<TaskNode>>();
            //if (_repository == null)
            //  _repository = HttpContext.GetServerApi();

            var searchDefinition = new DSearchDefinition()
            {
                Id = Guid.NewGuid(),
                MaxResults = 10,
                SearchKind = SearchKind.AllTasks,
                Ascending = true,
                SortFieldName = SystemAttributes.TASK_DATE_OF_ASSIGNMENT
            };

            var repo = _context.Repository;
            var searchResults = await repo.Search(searchDefinition);
            if (searchResults.Found == null)
            {
                return new List<TaskNode>();
            }

            var objects = repo.GetObjects(searchResults.Found.ToArray());
            var list = objects.Select(o => new TaskNode(o));
            return list;
        }
    }
}
