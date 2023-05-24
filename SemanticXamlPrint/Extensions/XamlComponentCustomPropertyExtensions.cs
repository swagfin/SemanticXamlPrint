using SemanticXamlPrint.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SemanticXamlPrint
{
    public static class XamlComponentCustomPropertyExtensions
    {
        private static XamlComponentCustomProperty GetProperty(this List<XamlComponentCustomProperty> customProperties, string property) => customProperties?.FirstOrDefault(x => x.Key?.ToLower() == property?.ToLower());

        public static bool IsPropertyExists(this List<XamlComponentCustomProperty> customProperties, string property)
        {
            return GetProperty(customProperties, property) != null;
        }
        public static bool IsPropertyExistsWithValue(this List<XamlComponentCustomProperty> customProperties, string property, string value)
        {
            return customProperties?.FirstOrDefault(x => x.Key?.ToLower() == property?.ToLower() && x.Value.Equals(value, StringComparison.OrdinalIgnoreCase)) != null;
        }
        public static void AddCustomProperty(this List<XamlComponentCustomProperty> customProperties, string property, string value)
        {
            if (customProperties.IsPropertyExists(property) || string.IsNullOrEmpty(property)) return;
            customProperties?.Add(new XamlComponentCustomProperty(property?.ToLower()?.Trim(), value));
        }
        public static T GetPropertyValue<T>(this List<XamlComponentCustomProperty> customProperties, string property) where T : IConvertible
        {
            try
            {
                XamlComponentCustomProperty propertyObj = GetProperty(customProperties, property);
                return (propertyObj == null) ? default : (T)Convert.ChangeType(propertyObj.Value.Trim(), typeof(T));
            }
            catch { return default; }
        }
    }
}
