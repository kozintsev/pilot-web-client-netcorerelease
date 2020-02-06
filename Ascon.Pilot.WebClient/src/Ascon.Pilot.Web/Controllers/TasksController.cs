﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ascon.Pilot.DataClasses;
using Ascon.Pilot.Web.Extensions;
using Ascon.Pilot.Web.Models;
using Ascon.Pilot.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ascon.Pilot.Web.Controllers
{
    /// <summary>
    /// Контроллер задач
    /// </summary>
    [Authorize]
    public class TasksController : Controller
    {
        private IContextHolder _contextHolder;

        public TasksController(IContextHolder contextHolder)
        {
            _contextHolder = contextHolder;
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

        [HttpPost]
        public async Task<ActionResult> GetTaskDetails(string taskId)
        {
            return await Task<ActionResult>.Factory.StartNew(() =>
            {
                var id = Guid.Parse(taskId);
                var model = new TaskDetailsViewModel(id, _contextHolder.GetContext(HttpContext).Repository);
                return PartialView("TaskDetails", model);
            });
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

        private List<MType> GetMTypeTask(IRepository repo)
        {
            var types = repo.GetTypes();
            var list = new List<MType>();
            foreach (var type in types)
            {
                if (type.Name == "task_simple")
                {
                    list.Add(type);

                }

                if (type.Name == "workflow_approval")
                {
                    list.Add(type);
                }
            }
            return list;
        }

        private async Task<IEnumerable<TaskNode>> GetItemsAsync(int id, int results)
        {
            var context = _contextHolder.GetContext(HttpContext);

            var repo = _contextHolder.GetContext(HttpContext).Repository;

            var list1 = GetMTypeTask(repo);

            var searchDefinition = new DSearchDefinition
            {
                Id = Guid.NewGuid(),
                Request =
                {
                    MaxResults = results,
                    SearchKind = (SearchKind)id,
                    SortDefinitions =
                    {
                        new DSortDefinition {
                            Ascending = false,
                            FieldName = SystemAttributes.TASK_DATE_OF_ASSIGNMENT

                        }
                    }
                }
            };


            var searchResults = await repo.Search(searchDefinition);
            

            var searchDefinition2 = new DSearchDefinition
            {
                Id = Guid.NewGuid(),
                Request =
                {
                    MaxResults = results,
                    SearchKind = SearchKind.Custom,
                    SearchString = "+DObject.State.State:(&#32;0 OR &#32;3) +DObject.TypeId:(&#32;4 OR &#32;5 OR &#32;17 OR &#32;10 OR &#32;16 OR &#32;19 OR &#32;15 OR &#32;11 OR &#32;14 OR &#32;9 OR &#32;12 OR &#32;8 OR &#32;6 OR &#32;7 OR &#32;13 OR &#32;35 OR &#32;18 OR &#32;36 OR &#32;37 OR &#32;38 OR &#32;39 OR &#32;40 OR &#32;41 OR &#32;42 OR &#32;43 OR &#32;44 OR &#32;45 OR &#32;46 OR &#32;47 OR &#32;48 OR &#32;49 OR &#32;50 OR &#32;51 OR &#32;52 OR &#32;53 OR &#32;54 OR &#32;55 OR &#32;56 OR &#32;57 OR &#32;58 OR &#32;59 OR &#32;60 OR &#32;61 OR &#32;62 OR &#32;63 OR &#32;64) +DObject.TypeId:(&#32;60 OR &#32;57 OR &#32;56 OR &#32;58 OR &#32;59)",
                    SortDefinitions =
                    {
                        new DSortDefinition()
                        {
                            FieldName = "DObject.Created",
                            Ascending = false
                        }
                    }
                }
            };

            var searchResults2 = await repo.Search(searchDefinition2);

            if (searchResults.Found == null && searchResults2.Found == null)
            {
                return new List<TaskNode>();
            }

            var objects2 = repo.GetObjects(searchResults2.Found.ToArray())
                .Where(x => x.ParentId == Guid.Empty).ToList();


            if (searchResults.Found != null)
            {
                var objects = repo.GetObjects(searchResults.Found.ToArray());


                foreach (var item in objects)
                {
                    if (objects2.Any(x => x.Id == item.Id))
                    {
                        objects2.Remove(objects2.First(y => y.Id == item.Id));
                    }
                }

                objects2.AddRange(objects);
            }
    

            var list = objects2.Select(o => new TaskNode(o, repo));
            return list;
        }

    }
}
