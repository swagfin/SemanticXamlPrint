using System;
using System.Collections.Generic;
using System.Linq;

namespace SemanticXamlPrint.Parser.Components
{
    public static class XamlComponentCustomPropertyExtensions
    {
        private static string Normalize(string value) => value?.Trim() ?? string.Empty;

        public static Dictionary<string, string> ToPropertyMap(this List<XamlComponentCustomProperty> customProperties)
        {
            Dictionary<string, string> propertyMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (customProperties == null) return propertyMap;
            foreach (XamlComponentCustomProperty property in customProperties)
            {
                string key = Normalize(property?.Key);
                if (string.IsNullOrEmpty(key)) continue;
                if (!propertyMap.ContainsKey(key))
                {
                    propertyMap[key] = property?.Value;
                }
            }
            return propertyMap;
        }

        private static XamlComponentCustomProperty GetProperty(this List<XamlComponentCustomProperty> customProperties, string property)
        {
            string propertyKey = Normalize(property);
            return customProperties?.FirstOrDefault(x => string.Equals(Normalize(x.Key), propertyKey, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsPropertyExists(this List<XamlComponentCustomProperty> customProperties, string property)
        {
            return GetProperty(customProperties, property) != null;
        }

        public static bool IsPropertyExistsWithValue(this List<XamlComponentCustomProperty> customProperties, string property, string value)
        {
            string propertyKey = Normalize(property);
            string propertyValue = Normalize(value);
            return customProperties?.FirstOrDefault(x => string.Equals(Normalize(x.Key), propertyKey, StringComparison.OrdinalIgnoreCase) && string.Equals(Normalize(x.Value), propertyValue, StringComparison.OrdinalIgnoreCase)) != null;
        }

        public static void AddCustomProperty(this List<XamlComponentCustomProperty> customProperties, string property, string value)
        {
            string propertyKey = Normalize(property);
            if (customProperties.IsPropertyExists(propertyKey) || string.IsNullOrEmpty(propertyKey)) return;
            customProperties?.Add(new XamlComponentCustomProperty(propertyKey, value));
        }

        public static T GetPropertyValue<T>(this List<XamlComponentCustomProperty> customProperties, string property) where T : IConvertible
        {
            try
            {
                XamlComponentCustomProperty propertyObj = GetProperty(customProperties, property);
                return (propertyObj == null) ? default : (T)Convert.ChangeType(propertyObj.Value?.Trim(), typeof(T));
            }
            catch { return default; }
        }
    }
}
