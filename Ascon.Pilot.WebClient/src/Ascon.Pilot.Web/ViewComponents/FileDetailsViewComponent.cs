using System;
using System.Threading.Tasks;
using Ascon.Pilot.Web.Models;
using Ascon.Pilot.Web.Models.Store;
using Ascon.Pilot.Web.ViewModels;
using DocumentRender;
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
        private readonly IDocumentRender _render;
        private readonly IStore _store;

        public FileDetailsViewComponent(IContextHolder contextHolder, IDocumentRender render, IStore store)
        {
            _contextHolder = contextHolder;
            _render = render;
            _store = store;
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
                        ViewBag.Repository = _contextHolder.GetContext(HttpContext).Repository;
                        ViewBag.DocumentRender = _render;
                        ViewBag.Store = _store;

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
