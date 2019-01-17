﻿using System;
using System.Collections.Generic;
using System.Linq;
using Ascon.Pilot.DataClasses;
using Ascon.Pilot.Web.Extensions;
using Ascon.Pilot.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ascon.Pilot.Web.ViewComponents
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
                var context = _contextHolder.GetContext(HttpContext);
                var types = context.Repository.GetTypes().ToDictionary(x => x.Id, y => y);
                var repository = _contextHolder.GetContext(HttpContext).Repository;

                Guid parentId = id;
                bool isSource = ViewBag.IsSource ?? false;
                while (parentId != DObject.RootId)
                {
                    var obj = repository.GetObjects(new[] { parentId })[0];
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