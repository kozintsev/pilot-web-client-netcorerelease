using System;
using System.Linq;
using Ascon.Pilot.Core;

namespace Ascon.Pilot.WebClient.Extensions
{
    public static class DObjectExtensions
    {
        public static bool IsVisible(this DObject node)
        {
            return !node.IsForbidden();
        }

        public static bool IsForbidden(this DObject obj)
        {
            return obj.TypeId == 0 && obj.Id != DObject.RootId && !obj.Access.Any() && obj.Attributes.Count == 0 && obj.StateInfo.State != ObjectState.DeletedPermanently;
        }
    }
}
