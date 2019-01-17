using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ascon.Pilot.DataClasses;
using Ascon.Pilot.Web.Extensions;
using Ascon.Pilot.Web.Models;
using Ascon.Pilot.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Ascon.Pilot.Web.ViewComponents
{
    /// <summary>
    /// Компонент - боковая панель.
    /// </summary>
    public class SidePanelViewComponent : ViewComponent
    {
        private readonly IContextHolder _contextHolder;

        public SidePanelViewComponent(IContextHolder contextHolder)
        {
            _contextHolder = contextHolder;
        }

        /// <summary>
        /// Вызов боковой панели.
        /// </summary>
        /// <param name="id">Id папки.</param>
        /// <returns>Боковая панель для папки с идентификатором Id.</returns>
        public async Task<IViewComponentResult> InvokeAsync(Guid? id)
        {
            return await Task.Run(() => GetSidePanel(id)) ;
        }


        /// <summary>
        /// Отображение боковой панели.
        /// </summary>
        /// <param name="id">Уникальный Id папки, для которой запрашивается представление.</param>
        /// <returns>Представление боковой панели для папки с идентификатором Id.</returns>
        public IViewComponentResult GetSidePanel(Guid? id)
        {
            id = id ?? DObject.RootId;

            var context = _contextHolder.GetContext(HttpContext);
            var repo = context.Repository;
            var rootObject = repo.GetObjects(new[] { id.Value }).First();
            var bRootObject = repo.GetObjects(new[] { DObject.RootId }).First();
            var mTypes = repo.GetTypes().ToDictionary(x => x.Id, y => y);
            var model = new SidePanelViewModel
            {
                ObjectId = id.Value,
                Types = mTypes,
                Items = new List<SidePanelItem>()
            };

            var prevId = rootObject.Id;
            var parentId = rootObject.Id;
            do
            {
                var parentObject = repo.GetObjects(new[] { parentId }).First();
                var parentChildsIds = parentObject.Children
                                        .Where(x => mTypes[x.TypeId].IsService == false)
                                        .Select(x => x.ObjectId).ToArray();
                if (parentChildsIds.Length != 0)
                {
                    var parentChilds = repo.GetObjects(parentChildsIds);
                    var visibleChildren = parentChilds.Where(x => x.IsVisible()).ToList();
                    var subtree = model.Items;
                    model.Items = new List<SidePanelItem>();
                    foreach (var parentChild in visibleChildren)
                    {
                        if (parentChild.IsForbidden())
                            continue;

                        model.Items.Add(new SidePanelItem
                        {
                            Type = mTypes[parentChild.TypeId],
                            DObject = parentChild,
                            SubItems = parentChild.Id == prevId ? subtree : null,
                            Selected = parentChild.Id == id
                        });
                    }
                }

                prevId = parentId;
                parentId = parentObject.ParentId;
            } while (parentId != bRootObject.ParentId);
            return View(model);
        }
    }
}
