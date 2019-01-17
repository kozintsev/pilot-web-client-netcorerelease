using System;
using System.Threading.Tasks;
using Ascon.Pilot.Web.Models;
using Ascon.Pilot.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Ascon.Pilot.Web.Controllers
{
    public class TaskDetailsController : Controller
    {
        private readonly IContextHolder _contextHolder;

        public TaskDetailsController(IContextHolder contextHolder)
        {
            _contextHolder = contextHolder;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return await Task<ActionResult>.Factory.StartNew(() =>
            {
                var taskId = HttpContext.Request.Query["id"].ToString();
                if (string.IsNullOrEmpty(taskId))
                    return View("TaskDetails");

                var id = Guid.Parse(taskId);
                var model = new TaskDetailsViewModel(id, _contextHolder.GetContext(HttpContext).Repository);
                return View("TaskDetails", model);
            });
        }
    }
}
