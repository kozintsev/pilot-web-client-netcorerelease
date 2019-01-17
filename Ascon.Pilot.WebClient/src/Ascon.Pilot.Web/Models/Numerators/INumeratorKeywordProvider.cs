using Ascon.Pilot.DataClasses;

namespace Ascon.Pilot.Web.Models.Numerators
{
    public interface INumeratorKeywordProvider
    {
        object GetValue(INObject obj, string keyword);
    }
}