using System;
using System.Threading.Tasks;
using Ascon.Pilot.Web.Controllers;
using Ascon.Pilot.Web.Models;
using Ascon.Pilot.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Ascon.Pilot.Web.ViewComponents
{
    /// <summary>
    /// Компнент - панель управления файлом.
    /// </summary>
    public class FileDetailsViewComponent : ViewComponent
    {
        private readonly ILogger<FilesController> _logger;
        private readonly IContextHolder _contextHolder;

        public FileDetailsViewComponent(ILogger<FilesController> logger, IContextHolder contextHolder)
        {
            _logger = logger;
            _contextHolder = contextHolder;
        }

        /// <summary>
        /// Вызвать компонент панели файлов
        /// </summary>
        /// <param name="docId">Идентификатор текущего документа</param>
        /// <param name="panelType">Тип отображения панели</param>
        /// <param name="version">Время версии</param>
        /// <returns>Представление панели управения файлом для каталога с идентификатором Id и итпом отбражения Type.</returns>
        public async Task<IViewComponentResult> InvokeAsync(Guid docId, FilesPanelType panelType, long version)
        {
            return await Task.Run(() =>
            {
                {
                    try
                    {
                        var model = new FilesDetailsViewModel(docId, version, _contextHolder.GetContext(HttpContext).Repository, panelType);
                        return View("FileDetails", model);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ex.Message);
                    }
                }
            });
        }
    }
}
