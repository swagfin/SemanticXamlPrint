using SemanticXamlPrint.Parser.Components;
using System;
using System.Collections.Generic;

namespace SemanticXamlPrint.Parser.Factories
{
    internal static class XamlComponentFactory
    {
        private static readonly Dictionary<string, Func<IXamlComponent>> ComponentFactories = new Dictionary<string, Func<IXamlComponent>>(StringComparer.OrdinalIgnoreCase)
        {
            { "template", () => new TemplateComponent() },
            { "image", () => new ImageComponent() },
            { "grid", () => new GridComponent() },
            { "gridrow", () => new GridRowComponent() },
            { "cells", () => new CellsComponent() },
            { "cell", () => new CellComponent() },
            { "qrcode", () => new QRCodeComponent() },
            { "data", () => new DataComponent() },
            { "line", () => new LineComponent() },
            { "linebreak", () => new LineBreakComponent() }
        };

        public static IXamlComponent Create(string nodeName, string nodeValue)
        {
            if (string.Equals(nodeName, "#text", StringComparison.OrdinalIgnoreCase))
            {
                return new TextBlockComponent(nodeValue);
            }
            if (string.Equals(nodeName, "#comment", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }
            if (ComponentFactories.TryGetValue(nodeName?.Trim() ?? string.Empty, out Func<IXamlComponent> factory))
            {
                return factory();
            }
            throw new Exception($"Invalid node name {nodeName}");
        }
    }
}
