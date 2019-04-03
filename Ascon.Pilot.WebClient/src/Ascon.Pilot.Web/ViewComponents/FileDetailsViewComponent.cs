using System;
using System.Threading.Tasks;
using Ascon.Pilot.Web.Models;
using Ascon.Pilot.Web.ViewModels;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace Ascon.Pilot.Web.ViewComponents
{
    /// <summary>
    /// Компнент - панель управления файлом.
    /// </summary>
    public class FileDetailsViewComponent : ViewComponent
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(FileDetailsViewComponent));
        private readonly IContextHolder _contextHolder;

        public FileDetailsViewComponent(IContextHolder contextHolder)
        {
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
                        _logger.Error(ex);
                        throw new Exception(ex.Message);

                    }
                }
            });
        }
    }
}
