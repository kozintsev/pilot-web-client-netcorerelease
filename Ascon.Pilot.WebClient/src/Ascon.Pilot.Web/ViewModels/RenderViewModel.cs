using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Ascon.Pilot.DataClasses;
using Ascon.Pilot.Web.Models;
using Ascon.Pilot.Web.Models.Store;
using DocumentRender;
using DocumentRender.DocumentConverter;
using log4net;
using Microsoft.AspNetCore.Mvc;

namespace Ascon.Pilot.Web.ViewModels
{
    public class RenderViewModel
    {
        private readonly DFile _file;
        private readonly IRepository _repository;
        private readonly IDocumentRender _documentRender;
        private readonly IStore _store;
        private readonly ILog _logger = LogManager.GetLogger(typeof(RenderViewModel));

        public RenderViewModel(DFile file, IRepository repository, IDocumentRender documentRender, IStore store)
        {
            _file = file;
            _repository = repository;
            _documentRender = documentRender;
            _store = store;
        }

        public Guid FileId => _file.Body.Id;

        public int GetPages()
        {
            return GetFilePages(_file);
        }

        public int GetFilePages(DFile file)
        {
            if (file.Body.Size >= 10 * 1024 * 1024)
                return 0;

            try
            {
                var pages = _store.GetFilePageCount(file.Body.Id);
                if (pages != 0)
                    return pages;
                
                var fileContent = _repository.GetFileChunk(_file.Body.Id, 0, (int)_file.Body.Size);
                var count = _documentRender.RenderPages(fileContent, _store.GetImagesStorageDirectory(file.Body.Id));
                return count;
            }
            catch (RenderToolNotFoundException ex)
            {
                _logger.Error(ex);
                //todo show error ui
            }
            catch (Exception ex)
            {
                _logger.Error("Unable to generate page for file", ex);
            }

            return 0;
        }

        private FileContentResult File(byte[] fileContents, string contentType, string fileDownloadName)
        {
            var fileContentResult = new FileContentResult(fileContents, contentType)
            {
                FileDownloadName = fileDownloadName
            };
            return fileContentResult;
        }
    }
}
