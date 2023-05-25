using SemanticXamlPrint.Parser.Components;
using System;
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
        /// <exception cref="Exception"></exception>
        public static IXamlComponent CreateComponentFromXml(this XmlNode node)
        {
            IXamlComponent component;
            switch (node.Name?.ToLower()?.Trim())
            {
                case "template":
                    component = new TemplateComponent();
                    break;
                case "image":
                    component = new ImageComponent();
                    break;
                case "grid":
                    component = new GridComponent();
                    break;
                case "gridrow":
                    component = new GridRowComponent();
                    break;
                case "cells":
                    component = new CellsComponent();
                    break;
                case "cell":
                    component = new CellComponent();
                    break;
                case "qrcode":
                    component = new QRCodeComponent();
                    break;
                case "data":
                    component = new DataComponent();
                    break;
                case "line":
                    component = new LineComponent();
                    break;
                case "linebreak":
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
                    IXamlComponent childObject = childNode.CreateComponentFromXml();
                    if (childObject != null)
                    {
                        if (childObject.Type == typeof(TemplateComponent)) throw new Exception($"{nameof(TemplateComponent)} can NOT be a child class");
                        component.AddChild(childObject);
                    }
                }
            return component;
        }
    }
}
