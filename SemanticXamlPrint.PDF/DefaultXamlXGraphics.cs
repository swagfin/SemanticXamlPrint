using PdfSharp.Drawing;
using SemanticXamlPrint.Components;
using System;

namespace SemanticXamlPrint.PDF
{
    public static class DefaultXamlXGraphics
    {
        public static float DrawXamlComponent(this XGraphics graphics, IXamlComponent xamlComponent, float yPositionDraw = 0)
        {
            if (xamlComponent == null) return yPositionDraw;
            if (xamlComponent.Type != typeof(TemplateComponent)) throw new Exception($"Root Component must be that of a [{nameof(TemplateComponent)}] but currently is: [{xamlComponent.Name}]");
            TemplateComponent Template = (TemplateComponent)xamlComponent;
            ComponentXDrawingFormatting TemplateFormatting = Template.GetPdfXDrawingProperties(Defaults.Formatting) ?? throw new Exception("Default template properties are missing");
            float _currentLineY = yPositionDraw + Template.MarginTop;
            //Draw Root Component Children
            for (int i = 0; i < Template?.Children?.Count; i++)
                _currentLineY = graphics.DrawComponent(Template?.Children[i], TemplateFormatting, 0, _currentLineY, (float)graphics.PageSize.Width);
            return _currentLineY;
        }
    }
}
