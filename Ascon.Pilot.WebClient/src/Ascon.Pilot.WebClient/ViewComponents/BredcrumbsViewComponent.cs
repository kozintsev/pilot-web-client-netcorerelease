using System;
using System.Collections.Generic;
using Ascon.Pilot.Core;
using Ascon.Pilot.WebClient.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Ascon.Pilot.WebClient.Models;

namespace Ascon.Pilot.WebClient.ViewComponents
{
    public class BreadcrumbsViewComponent : ViewComponent
    {
        private IContextHolder _contextHolder;

        public BreadcrumbsViewComponent(IContextHolder contextHolder)
        {
            _contextHolder = contextHolder;
        }

        public IViewComponentResult Invoke(Guid id)
        {
            Queue<KeyValuePair<string, string>> result = new Queue<KeyValuePair<string, string>>();
            try
            {
                var types = HttpContext.Session.GetMetatypes();
                var serverApi = _contextHolder.GetContext(HttpContext).ServerApi;

                Guid parentId = id;
                bool isSource = ViewBag.IsSource ?? false;
                while (parentId != DObject.RootId)
                {
                    var obj = serverApi.GetObjects(new[] { parentId })[0];
                    var mType = types[obj.TypeId];
                    result.Enqueue(new KeyValuePair<string, string>(Url.Action("Index", "Files", new { id = obj.Id, isSource }), obj.GetTitle(mType)));
                    if (mType.IsMountable)
                        isSource = false;
                    parentId = obj.ParentId;
                }
                result.Enqueue(new KeyValuePair<string, string>(Url.Action("Index", "Files", new { id = DObject.RootId }), "Начало"));
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
            return View(result);
        }
    }
}