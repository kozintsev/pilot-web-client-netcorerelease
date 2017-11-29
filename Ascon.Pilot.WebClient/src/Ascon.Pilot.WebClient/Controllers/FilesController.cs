using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using Ascon.Pilot.Core;
using Ascon.Pilot.Server.Api.Contracts;
using Ascon.Pilot.WebClient.Extensions;
using Ascon.Pilot.WebClient.Models;
using Ascon.Pilot.WebClient.ViewComponents;
using Ascon.Pilot.WebClient.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MuPDF;


namespace Ascon.Pilot.WebClient.Controllers
{
    [Authorize]
    public class FilesController : Controller
    {
        private readonly ILogger<FilesController> _logger;
        private readonly IHostingEnvironment _environment;
        private readonly IContextHolder _contextHolder;
        private readonly MuPdf _mu;
        private static object _lockObj = new object();

        public FilesController(ILogger<FilesController> logger, IHostingEnvironment environment, IContextHolder contextHolder)
        {
            _logger = logger;
            _environment = environment;
            _contextHolder = contextHolder;
            _mu = new MuPdf();
        }

        public IActionResult ChangeFilesPanelType(string returnUrl, FilesPanelType type)
        {
            HttpContext.Session.SetSessionValues(SessionKeys.FilesPanelType, type);
            return Redirect(returnUrl);
        }

        public IActionResult GetBreabcrumbs(Guid id)
        {
            return ViewComponent(typeof(BreadcrumbsViewComponent), id);
        }

        public IActionResult Index(Guid? id, bool isSource = false)
        {
            _contextHolder.GetContext(HttpContext).Build(HttpContext);
            var versionInput = HttpContext.Request.Query["version"].ToString();
            long version;
            long.TryParse(versionInput, out version);

            var model = new UserPositionViewModel();

            id = id ?? DObject.RootId;
            FilesPanelType type = HttpContext.Session.GetSessionValues<FilesPanelType>(SessionKeys.FilesPanelType);
            model.CurrentFolderId = id.Value;
            model.FilesPanelType = type;
            ViewBag.FilesPanelType = type;
            ViewBag.IsSource = isSource;
            
            var context = _contextHolder.GetContext(HttpContext);
            var repo = context.Repository;
            var node = repo.GetObjects(new[] { id.Value }).FirstOrDefault();
            if (node != null)
            {
                if (node.Children?.Any() == false)
                {
                    var nodeType = repo.GetType(node.TypeId);
                    if (nodeType.HasFiles)
                    {
                        model.Version = version;
                        model.IsFile = true;
                    }
                }
            }
            
            return View(model);
        }

        public async Task<IActionResult> GetNodeChilds(Guid id)
        {
            return await Task.Run(() =>
            {
                dynamic[] childNodes;

                var context = _contextHolder.GetContext(HttpContext);
                var repo = context.Repository;
                var node = repo.GetObjects(new[] { id }).First();
                var types = context.Repository.GetTypes().ToDictionary(x => x.Id, y => y);
                var childIds = node.Children?
                                    .Where(x => types[x.TypeId].Children?.Any() == true)
                                    .Select(child => child.ObjectId).ToArray();
                var nodeChilds = repo.GetObjects(childIds);

                childNodes = nodeChilds.Where(t =>
                {
                    var mType = types[t.TypeId];
                    return !mType.IsService;

                }).Select(x =>
                {
                    var mType = types[x.TypeId];


                    var sidePanelItem = new SidePanelItem
                    {
                        DObject = x,
                        Type = mType,
                        SubItems = x.Children.Any(y => types[y.TypeId].Children.Any()) ? new List<SidePanelItem>() : null
                    };

                    return sidePanelItem.GetDynamic(id, types);
                })
                    .ToArray();
                return Json(childNodes);
            });
        }

        public IActionResult SidePanel(Guid? id)
        {
            return ViewComponent(typeof(SidePanelViewComponent), id);
        }

        public IActionResult GetObject(Guid id, bool isSource = false)
        {
            var filesPanelType = HttpContext.Session.GetSessionValues<FilesPanelType>(SessionKeys.FilesPanelType);
            var context = _contextHolder.GetContext(HttpContext);
            var repo = context.Repository;
            var node = repo.GetObjects(new[] { id }).FirstOrDefault();
            if (node != null)
            {
                if (node.Children?.Any() == false)
                {
                    var type = repo.GetType(node.TypeId);
                    if (type.HasFiles)
                    {
                        return ViewComponent(typeof(FileDetailsViewComponent), new { docId = id, panelType = filesPanelType });
                    }
                }
            }

            return ViewComponent(typeof(FilesPanelViewComponent), new { folderId = id, panelType = filesPanelType, onlySource = isSource });
        }

        public IActionResult GetSource(Guid id)
        {
            return GetObject(id, true);
        }

        public IActionResult Preview(Guid id, int size, string name)
        {
            ViewBag.Url = Url.Action("DownloadPdf", new { id, size, name });
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            if (isAjax) return PartialView();
            return View();
        }

        public IActionResult DownloadPdf(Guid id, int size, string name)
        {
            byte[] fileChunk;

            var repository = _contextHolder.GetContext(HttpContext).Repository;
            fileChunk = repository.GetFileChunk(id, 0, size);
            var fileDownloadName = string.IsNullOrWhiteSpace(name) ? id.ToString() : name;
            if (Response.Headers.ContainsKey("Content-Disposition"))
                Response.Headers.Remove("Content-Disposition");
            Response.Headers.Add("Content-Disposition", $"inline; filename={fileDownloadName}");
            return new FileContentResult(fileChunk, "application/pdf");
        }

        public async Task<IActionResult> Download(Guid id, int size, string name)
        {
            return await Task.Run(() =>
            {
                {
                    var repository = _contextHolder.GetContext(HttpContext).Repository;
                    var fileChunk = repository.GetFileChunk(id, 0, size);
                    return new FileContentResult(fileChunk, "application/octet-stream")
                    {
                        FileDownloadName = string.IsNullOrWhiteSpace(name) ? id.ToString() : name
                    };
                }
            });
        }

        public IActionResult DownloadArchive(Guid[] objectsIds)
        {
            if (objectsIds.Length == 0)
                return NotFound();

            byte[] mstData;
            var context = _contextHolder.GetContext(HttpContext);
            var repo = context.Repository;
            var types = repo.GetTypes().ToDictionary(x => x.Id, y => y);
            var objects = repo.GetObjects(objectsIds);

            using (var compressedFileStream = new MemoryStream())
            {
                using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Update, true))
                {
                    AddObjectsToArchive(repo, objects, zipArchive, types, "");
                }
                mstData = compressedFileStream.ToArray();
            }
            return new FileContentResult(mstData, "application/zip") { FileDownloadName = "archive.zip" };
        }

        private void AddObjectsToArchive(IRepository repository, List<DObject> objects, ZipArchive archive, IDictionary<int, MType> types, string currentPath)
        {
            foreach (var obj in objects)
            {
                if (!types[obj.TypeId].Children.Any())
                {
                    var dFile = obj.ActualFileSnapshot.Files.FirstOrDefault(f => Path.GetExtension(f.Name).Equals(".xps") || Path.GetExtension(f.Name).Equals(".pdf"));
                    if (dFile == null)
                        continue;

                    var fileId = dFile.Body.Id;
                    var fileSize = dFile.Body.Size;
                    var fileBody = repository.GetFileChunk(fileId, 0, (int)fileSize);

                    if (archive.Entries.Any(x => x.Name == dFile.Name))
                        dFile.Name += " Conflicted";
                    var zipEntry = archive.CreateEntry(Path.Combine(currentPath, dFile.Name), CompressionLevel.NoCompression);

                    //Get the stream of the attachment
                    using (var originalFileStream = new MemoryStream(fileBody))
                    using (var zipEntryStream = zipEntry.Open())
                    {
                        //Copy the attachment stream to the zip entry stream
                        originalFileStream.CopyTo(zipEntryStream);
                    }
                }
                else
                {
                    var name = obj.GetTitle(types[obj.TypeId]);
                    var directoryPath = Path.Combine(currentPath, name);
                    var objChildrenIds = obj.Children.Select(x => x.ObjectId).ToArray();
                    if (!objChildrenIds.Any())
                        continue;

                    var objChildren = repository.GetObjects(objChildrenIds);
                    AddObjectsToArchive(repository, objChildren, archive, types, directoryPath);
                }
            }
        }

        public IActionResult Thumbnail(Guid id, int size, string extension, int typeId)
        {
            return RedirectToAction("GetTypeIcon", "Home", new { id = typeId });
        }

        public IActionResult Image(Guid id, int size, string extension)
        {
            const string pngContentType = "image/png";
            const string svgContentType = "image/svg+xml";
            var virtualFileResult = File(Url.Content("~/images/file.svg"), svgContentType);

            if (size >= 10 * 1024 * 1024)
                return virtualFileResult;
            var repository = _contextHolder.GetContext(HttpContext).Repository;
            var file = repository.GetFileChunk(id, 0, size);

            try
            {
                if (file != null)
                {
                    if (extension.Contains("xps"))
                    {
                        int page = 1;
                        int dpi = 150;
                        //var RenderType = RenderType.;
                        bool rotateAuto = false;
                        string password = "";

                        var fileName = $"{id}{extension}";
                        var directory = "tmp/";
                        if (!Directory.Exists(directory))
                            Directory.CreateDirectory(directory);
                        using (var fileStream = System.IO.File.Create(directory + fileName))
                            fileStream.Write(file, 0, file.Length);
                        lock (_lockObj)
                        {
                            byte[] thumbnailContent = _mu.RenderFirstPageInBytes(directory + fileName);
                            System.IO.File.Delete(directory + fileName);
                            return File(thumbnailContent, pngContentType, $"{id}.png");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(1, "Unable to generate thumbnail for file", ex);
            }
            return virtualFileResult;
        }

        [HttpPost]
        public ActionResult Rename(Guid idToRename, string newName, Guid renameRootId)
        {

            var api = _contextHolder.GetContext(HttpContext).Repository;
            var objectToRename = api.GetObjects(new[] { idToRename })[0];
            var newObject = objectToRename.Clone();

            /*api.Change(new DChangesetData()
            {
                Changes = new List<DChange>
                {
                    new DChange()
                    {
                        New = newObject,
                        Old = objectToRename
                    }
                }
            });*/
            return RedirectToAction("Index", new { id = renameRootId });
        }

        [HttpPost]
        public ActionResult Remove(Guid idToRemove, Guid removeRootId)
        {
            return RedirectToAction("Index", new { id = removeRootId });
        }

        //[HttpPost]
        //public async Task<RedirectToActionResult> Upload(Guid folderId, IFormFile file)
        //{
        //    try
        //    {
        //        if (file.Length == 0)
        //            throw new ArgumentNullException(nameof(file));

        //        string fileName = file.GetFileName();
        //        var pathToSave = Path.Combine(_environment.WebRootPath, fileName);
        //        await file.SaveAsAsync(pathToSave);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogWarning(1, "Unable to upload file", ex);
        //    }
        //    return RedirectToAction("Index", new { id = folderId });
        //}
    }
}