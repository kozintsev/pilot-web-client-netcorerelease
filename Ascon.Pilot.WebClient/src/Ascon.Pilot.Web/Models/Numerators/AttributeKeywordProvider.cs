using Ascon.Pilot.DataClasses;

namespace Ascon.Pilot.Web.Models.Numerators
{
    public class AttributeKeywordProvider : INumeratorKeywordProvider
    {
        public object GetValue(INObject obj, string keyword)
        {
            DValue value;
            return obj.Attributes.TryGetValue(keyword, out value) ? value.Value : null;
        }
    }
}
