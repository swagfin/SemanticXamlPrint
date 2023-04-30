using SemanticXamlPrint.Components;
using System;
using System.Drawing;

namespace SemanticXamlPrint
{
    public static class DefaultXamlGraphics
    {
        public static void DrawXamlComponent(this Graphics graphics, IXamlComponent xamlComponent)
        {
            if (xamlComponent == null) return;
            if (xamlComponent.Type != typeof(TemplateComponent)) throw new Exception($"Root Component must be that of a [{nameof(TemplateComponent)}] but currently is: [{xamlComponent.Name}]");
            TemplateComponent Template = (TemplateComponent)xamlComponent;
            ComponentDrawingFormatting TemplateFormatting = Template.GetSystemDrawingProperties(Defaults.Formatting) ?? throw new Exception("Default template properties are missing");
            if (Template.MaxWidth <= 50) Template.MaxWidth = 290;
            if (Template.MarginTop < 0) Template.MarginTop = 20;
            float _currentLineY = Template.MarginTop;
            //Draw Root Component Children
            for (int i = 0; i < Template?.Children?.Count; i++)
            {
                _currentLineY = graphics.DrawComponent(Template?.Children[i], TemplateFormatting, 0, _currentLineY, graphics.VisibleClipBounds.Width);
            }
        }
    }
}
