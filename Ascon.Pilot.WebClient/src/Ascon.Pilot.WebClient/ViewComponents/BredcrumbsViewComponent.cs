using System;
using System.Collections.Generic;
using Ascon.Pilot.Core;
using Ascon.Pilot.WebClient.Extensions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Ascon.Pilot.WebClient.ViewComponents
{
    public class BreadcrumbsViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(Guid? id)
        {
            return await Task.Run(() => Breadcrumbs(id));
        }
        public IViewComponentResult Breadcrumbs(Guid? id)
        {
            id = id ?? DObject.RootId;
            var types = HttpContext.Session.GetMetatypes();

            var serverApi = HttpContext.GetServerApi();
            Queue<KeyValuePair<string, string>> result = new Queue<KeyValuePair<string, string>>();
            Guid parentId = id.Value;
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
            return View(result);
        }
        //public IViewComponentResult Invoke(Guid? id)
        //{
        //    id = id ?? DObject.RootId;
        //    var types = HttpContext.Session.GetMetatypes();

        //    var serverApi = HttpContext.GetServerApi();
        //    Queue<KeyValuePair<string, string>> result = new Queue<KeyValuePair<string, string>>();
        //    Guid parentId = id.Value;
        //    bool isSource = ViewBag.IsSource ?? false;
        //    while (parentId != DObject.RootId)
        //    {
        //        var obj = serverApi.GetObjects(new[] { parentId })[0];
        //        var mType = types[obj.TypeId];
        //        result.Enqueue(new KeyValuePair<string, string>(Url.Action("Index", "Files", new { id = obj.Id, isSource }), obj.GetTitle(mType)));
        //        if (mType.IsMountable)
        //            isSource = false;
        //        parentId = obj.ParentId;
        //    }
        //    result.Enqueue(new KeyValuePair<string, string>(Url.Action("Index", "Files", new { id = DObject.RootId }), "Начало"));
        //    return View(result);
        //}
    }
}