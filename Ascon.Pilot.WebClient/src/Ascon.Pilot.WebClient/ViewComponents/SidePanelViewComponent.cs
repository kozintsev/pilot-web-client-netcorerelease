using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Ascon.Pilot.Core;
using Ascon.Pilot.WebClient.Extensions;
using Ascon.Pilot.WebClient.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Ascon.Pilot.WebClient.ViewComponents
{
    /// <summary>
    /// Компонент - боковая панель.
    /// </summary>
    public class SidePanelViewComponent : ViewComponent
    {
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
            var model = new SidePanelViewModel();
            try
            {
                var serverApi = HttpContext.GetServerApi();
                var obj = serverApi.GetObjects(new[] { id.Value }).First();

                var mTypes = HttpContext.Session.GetMetatypes();
                model.ObjectId = id.Value;
                model.Types = mTypes;
                model.Items = new List<SidePanelItem>();

                var children = serverApi.GetObjects(obj.Children.Where(t => mTypes[t.TypeId].IsService == false).Select(x => x.ObjectId).ToArray());
                Guid reqId = id.Value;
                model.Items = GetListSidePanel(children, mTypes, reqId);

                while (reqId != DObject.RootId)
                {
                    if (obj.Id == DObject.RootId)
                        break;
                    obj = serverApi.GetObjects(new[] { obj.ParentId }).First();
                    children = serverApi.GetObjects(obj.Children.Where(t => mTypes[t.TypeId].IsService == false).Select(x => x.ObjectId).ToArray());
                    var subtree = model.Items;
                    model.Items = GetListSidePanel(children, mTypes, reqId, subtree);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return View(model);
        }

        static List<SidePanelItem> GetListSidePanel(List<DObject> list, IDictionary<int, MType> mtype, Guid id, List<SidePanelItem> subitem = null)
        {
            List<SidePanelItem> md = new List<SidePanelItem>();
            foreach (var child in list)
            {
                md.Add(new SidePanelItem
                {
                    Type = mtype[child.TypeId],
                    DObject = child,
                    SubItems = subitem,
                    Selected = child.Id == id
                });
            }
            return md;
        }
    }
}
