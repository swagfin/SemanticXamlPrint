using SemanticXamlPrint.Components;
using System;
using System.Xml;

namespace SemanticXamlPrint.Parsers
{
    public class DefaultXamlParser
    {
        public IXamlComponent Parse(string xamlFilePath)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(xamlFilePath);
            var rootNode = xmlDocument.DocumentElement;
            return CreateComponentFromXml(rootNode);
        }

        private IXamlComponent CreateComponentFromXml(XmlNode node)
        {
            IXamlComponent component;
            switch (node.Name?.ToLower()?.Trim())
            {
                case "template":
                    component = new TemplateComponent();
                    break;
                case "grid":
                    component = new GridComponent();
                    break;
                case "datarow":
                    component = new DataRowComponent();
                    break;
                case "datarowcell":
                    component = new DataRowCellComponent();
                    break;
                case "data":
                    component = new DataComponent();
                    break;
                case "line":
                    component = new LineComponent();
                    break;
                case "br":
                    component = new LineBreakComponent();
                    break;
                case "#text":
                    component = new TextBlockComponent(node.Value);
                    break;
                case "#comment":
                    return null;
                default:
                    throw new Exception($"Invalid node name {node.Name}");
            }
            // parse attributes
            if (node?.Attributes != null)
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    _ = component.TrySetProperty(attribute.Name?.ToLower()?.Trim(), attribute.Value);
                }
            // parse child nodes
            if (node?.ChildNodes != null)
                foreach (XmlNode childNode in node.ChildNodes)
                {
                    IXamlComponent childObject = CreateComponentFromXml(childNode);
                    if (childObject != null)
                        component.AddChild(childObject);
                }
            return component;
        }
    }
}
