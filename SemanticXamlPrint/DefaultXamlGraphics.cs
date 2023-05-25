using SemanticXamlPrint.Parser.Components;
using System;
using System.Drawing;

namespace SemanticXamlPrint
{
    public static class DefaultXamlGraphics
    {
        public static float DrawXamlComponent(this Graphics graphics, IXamlComponent xamlComponent, float yPositionDraw = 0)
        {
            if (xamlComponent == null) return yPositionDraw;
            if (xamlComponent.Type != typeof(TemplateComponent)) throw new Exception($"Root Component must be that of a [{nameof(TemplateComponent)}] but currently is: [{xamlComponent.Name}]");
            TemplateComponent Template = (TemplateComponent)xamlComponent;
            ComponentDrawingFormatting TemplateFormatting = Template.GetSystemDrawingProperties(Defaults.Formatting) ?? throw new Exception("Default template properties are missing");
            float _currentLineY = yPositionDraw + Template.MarginTop;
            //Draw Root Component Children
            for (int i = 0; i < Template?.Children?.Count; i++)
                _currentLineY = graphics.DrawComponent(Template?.Children[i], TemplateFormatting, 0, _currentLineY, graphics.VisibleClipBounds.Width);
            return _currentLineY;
        }
    }
}
