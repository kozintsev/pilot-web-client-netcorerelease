using System.Linq;
using Ascon.Pilot.WebClient.Extensions;
using Ascon.Pilot.WebClient.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ascon.Pilot.WebClient.Controllers
{
    /// <summary>
    /// Контроллер модели Home
    /// </summary>
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IContextHolder _contextHolder;

        public HomeController(IContextHolder contextHolder)
        {
            _contextHolder = contextHolder;
        }

        /// <summary>
        /// Представление Index
        /// </summary>
        /// <returns>представление Index</returns>
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Files");
        }
        /// <summary>
        /// Представление Types
        /// </summary>
        /// <returns>представление Types</returns>
        public IActionResult Types()
        {
            var context = _contextHolder.GetContext(HttpContext);
            var types = context.Repository.GetTypes();
            return View(types);
        }

        /// <summary>
        /// Установление типа иконок файлов для папки id
        /// </summary>
        /// <param name="id">уникальный идетификатор папки</param>
        /// <returns>представления иконок файлов для разных типов файлов</returns>
        public IActionResult GetTypeIcon(int id)
        {
            const string svgContentType = "image/svg+xml";
            var context = _contextHolder.GetContext(HttpContext);
            var types = context.Repository.GetTypes().ToList();
            var type = types.FirstOrDefault(t => t.Id == id);
            if (type != null)
            {
                if (type.Icon != null)
                    return File(type.Icon, svgContentType);
            }
            
            return File(Url.Content("~/images/file.svg"), svgContentType);
        }
        /// <summary>
        /// Сообщения об ошибках
        /// </summary>
        /// <param name="message">Текст сообщения об ошибке</param>
        /// <returns>Представление сообщения об ошибке</returns>
        [AllowAnonymous]
        public IActionResult Error(string message)
        {
            return View(message);
        }
    }
}
