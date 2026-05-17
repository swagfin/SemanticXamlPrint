using SemanticXamlPrint.Parser.Components;
using SemanticXamlPrint.Parser.Factories;
using System.Xml;

namespace SemanticXamlPrint.Parser.Extensions
{
    public static class XmlNodeExtensions
    {
        /// <summary>
        /// This will Create an IXamlComponent Object from Xml Node
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static IXamlComponent CreateComponentFromXml(this XmlNode node)
        {
            IXamlComponent component = XamlComponentFactory.Create(node.Name, node.Value);
            if (component == null) return null;

            // parse attributes
            if (node?.Attributes != null)
            {
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    _ = component.TrySetProperty(attribute.Name?.ToLower()?.Trim(), attribute.Value);
                }
            }

            // parse child nodes
            if (node?.ChildNodes != null)
            {
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    IXamlComponent childObject = childNode.CreateComponentFromXml();
                    if (childObject != null)
                    {
                        if (childObject.Type == typeof(TemplateComponent)) throw new System.Exception($"{nameof(TemplateComponent)} can NOT be a child class");
                        component.AddChild(childObject);
                    }
                }
            }

            return component;
        }
    }
}
