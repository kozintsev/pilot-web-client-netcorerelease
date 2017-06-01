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
            
            var serverApi = HttpContext.GetServerApi();
            var obj = serverApi.GetObjects(new[] { id.Value }).First();

            var mTypes = HttpContext.Session.GetMetatypes();
            var model = new SidePanelViewModel
            {
                ObjectId = id.Value,
                Types = mTypes,
                Items = new List<SidePanelItem>()
            };


            var children = serverApi.GetObjects(obj.Children.Select(x => x.ObjectId).ToArray());
            foreach (var child in children)
            {
                MType type;
                if (mTypes.TryGetValue(child.TypeId, out type))
                {
                    if (type.IsService)
                        continue;
                    
                    model.Items.Add(new SidePanelItem
                        {
                            Type = mTypes[child.TypeId],
                            DObject = child,
                            Selected = child.Id == id
                        });
                }
            }

            return View(model);
        }
    }
}
