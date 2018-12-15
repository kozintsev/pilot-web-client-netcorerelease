using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Ascon.Pilot.Common;
using Ascon.Pilot.DataClasses;
using Ascon.Pilot.Web.Models.Numerators;
using Ascon.Pilot.Web.Utils;

namespace Ascon.Pilot.Web.Extensions
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

        public static string GetTitle(this DObject obj, MType type)
        {
            if (type.IsProjectFileOrFolder())
            {
                DValue name;
                if (obj.Attributes.TryGetValue(SystemAttributes.PROJECT_ITEM_NAME, out name))
                    return name;
                return "unnamed";
            }
            return GetObjectTitle(obj, type);
        }

        public static string GetNumeratorAttributeText(INObject obj, string attributeValue, NCounterDescriptionList description)
        {
            if (string.IsNullOrEmpty(attributeValue))
                return null;

            bool deferred = false;
            string format = attributeValue;
            if (attributeValue.StartsWith(NumeratorDescription.Deferred))
            {
                deferred = true;
                format = attributeValue.Substring(NumeratorDescription.Deferred.Length);
            }
            if (description.DeferredRegistration)
            {
                deferred = true;
            }

            return new NumeratorFormatter(new NumeratorKeywordProviderAggregator(new INumeratorKeywordProvider[]
            {
                new CurrentDateProvider(deferred),
                new AttributeKeywordProvider(),
                new UnknownProvider(description.DraftText)
            })).Format(obj, format);
        }

        private static string GetObjectTitle(DObject obj, MType type)
        {
            if (type.Id == MType.SMART_FOLDER_ID && obj.Attributes.ContainsKey(SystemAttributes.SMART_FOLDER_TITLE))
            {
                return obj.Attributes[SystemAttributes.SMART_FOLDER_TITLE].ToString();
            }

            if (obj.Id == DObject.RootId)
                return type.Title;

            var sb = new StringBuilder();
            var attibutes = AttributeFormatter.Format(type, obj.Attributes);

            foreach (var displayableAttr in type.GetDisplayAttributes())
            {
                var attributeText = GetAttributeText(obj, attibutes, displayableAttr);
                if (sb.Length != 0 && !string.IsNullOrEmpty(attributeText))
                {
                    sb.Append(Constants.PROJECT_TITLE_ATTRIBUTES_DELIMITER);
                }
                sb.Append(attributeText);
            }
            return sb.ToString();
        }

        private static IEnumerable<MAttribute> GetDisplayAttributes(this MType type)
        {
            return type.Attributes.Where(d => d.ShowInTree).OrderBy(d => d.DisplaySortOrder);
        }

        private static string GetAttributeText(DObject obj, IReadOnlyDictionary<string, DValue> attributes, MAttribute attr)
        {
            DValue value;
            attributes.TryGetValue(attr.Name, out value);
            var strValue = value?.Value?.ToString();
            if (attr.Type == MAttrType.Numerator)
            {
                try
                {
                    return GetNumeratorAttributeText(obj, strValue, attr.ParsedConfiguration().CounterDescriptions);
                }
                catch (FormatException)
                {
                    return string.Empty;
                }
            }
            return strValue;
        }

    }
}
