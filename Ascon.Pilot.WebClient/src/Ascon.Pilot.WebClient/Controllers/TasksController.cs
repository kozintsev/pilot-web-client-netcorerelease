using Ascon.Pilot.Core;
using Ascon.Pilot.WebClient.Models;
using Ascon.Pilot.WebClient.ViewModels;
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
            var lookupId = int.Parse(filterId);
            var model = await GetItemsAsync(lookupId);
            return PartialView("TaskList", model);
        }

        private async Task<IEnumerable<TaskNode>> GetItemsAsync(int id)
        {
            var searchDefinition = new DSearchDefinition()
            {
                Id = Guid.NewGuid(),
                MaxResults = 20,
                SearchKind = (SearchKind)id,
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
            var list = objects.Select(o => new TaskNode(o, _context.Repository));
            return list;
        }

    }
}
