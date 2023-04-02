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
                case "uniformdatagrid":
                    component = new UniformDataGridComponent();
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
                default:
                    throw new Exception($"Invalid node name {node.Name}");
            }

            // parse attributes
            var attributes = node.Attributes;
            if (attributes != null)
            {
                foreach (XmlAttribute attribute in attributes)
                {
                    _ = component.TrySetProperty(attribute.Name?.ToLower()?.Trim(), attribute.Value);
                }
            }

            // parse child nodes
            var childNodes = node.ChildNodes;
            if (childNodes != null)
            {
                foreach (XmlNode childNode in childNodes)
                {
                    var childObject = CreateComponentFromXml(childNode);
                    component.AddChild(childObject);
                }
            }

            return component;
        }
    }
}
