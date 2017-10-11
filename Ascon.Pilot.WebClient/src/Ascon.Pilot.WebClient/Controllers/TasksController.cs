using Ascon.Pilot.Core;
using Ascon.Pilot.WebClient.Models;
using Ascon.Pilot.WebClient.ViewModels;
using Ascon.Pilot.WebClient.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ascon.Pilot.WebClient.Controllers
{
    /// <summary>
    /// Контроллер задач
    /// </summary>
    [Authorize]
    public class TasksController : Controller
    {
        private IContext _context;

        public TasksController(IContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Открытие представление Index
        /// </summary>
        /// <returns>представление Index</returns>
        // GET: /<controller>/
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GetTasks(string filterId)
        {
            var id = int.Parse(filterId);
            FilterId = id;
            MaxResults = 100;
            var items = await GetItemsAsync(id, MaxResults);
            Tasks = items.Select(t => t.Id).ToList();
            return PartialView("TaskList", items);
        }

        [HttpPost]
        public async Task<ActionResult> GetNextTasks()
        {
            var items = await GetItemsAsync(FilterId, MaxResults += 100);
            var tasks = new List<TaskNode>();
            var ids = Tasks;
            foreach (var item in items)
            {
                if (ids.Contains(item.Id))
                    continue;

                tasks.Add(item);
                ids.Add(item.Id);
            }
            Tasks = ids;
            return PartialView("TaskList", tasks);
        }

        private int MaxResults
        {
            get
            {
                return HttpContext.Session.GetSessionValues<int>("max_results");
            }
            set
            {
                HttpContext.Session.SetSessionValues<int>("max_results", value);
            }
        }

        private int FilterId
        {
            get
            {
                return HttpContext.Session.GetSessionValues<int>("filter_id");
            }
            set
            {
                HttpContext.Session.SetSessionValues("filter_id", value);
            }
        }

        private IList<Guid> Tasks
        {
            get
            {
                return HttpContext.Session.GetSessionValues<IList<Guid>>("tasks");
            }
            set
            {
                HttpContext.Session.SetSessionValues("tasks", value);
            }
        }

        private async Task<IEnumerable<TaskNode>> GetItemsAsync(int id, int results)
        {
            var searchDefinition = new DSearchDefinition()
            {
                Id = Guid.NewGuid(),
                MaxResults = results,
                SearchKind = (SearchKind)id,
                Ascending = false,
                SortFieldName = SystemAttributes.TASK_DATE_OF_ASSIGNMENT
            };

            var repo = _context.Repository;
            var searchResults = await repo.Search(searchDefinition);
            if (searchResults.Found == null)
            {
                return new List<TaskNode>();
            }

            var objects = repo.GetObjects(searchResults.Found.ToArray());
            var list = objects.Select(o => new TaskNode(o, _context.Repository));
            return list;
        }

    }
}
