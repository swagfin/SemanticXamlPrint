using SemanticXamlPrint.Parser.Components;
using System;
using System.Drawing;
using System.Drawing.Printing;

namespace SemanticXamlPrint
{
    public static class DefaultXamlGraphics
    {
        private static int CurrentChildIndex { get; set; } = 0;
        private static bool RequestedMorePage { get; set; } = false;
        public static float DrawXamlComponent(this PrintPageEventArgs eventArgs, IXamlComponent xamlComponent, float yPositionDraw = 0)
        {
            if (xamlComponent == null) return yPositionDraw;
            if (xamlComponent.Type != typeof(TemplateComponent)) throw new Exception($"Root Component must be that of a [{nameof(TemplateComponent)}] but currently is: [{xamlComponent.Name}]");
            TemplateComponent template = (TemplateComponent)xamlComponent;
            ComponentDrawingFormatting TemplateFormatting = template.GetSystemDrawingProperties(Defaults.Formatting) ?? throw new Exception("Default template properties are missing");
            float _currentLineY = yPositionDraw + (float)template.MarginTop;
            double pageHeight = eventArgs.PageSettings.PaperSize.Height - template.MarginBottom;
            //Draw Root Component Children
            CurrentChildIndex = RequestedMorePage ? CurrentChildIndex : 0;
            for (int i = CurrentChildIndex; i < template?.Children?.Count; i++)
            {
                if (_currentLineY > pageHeight)
                {
                    eventArgs.HasMorePages = true;
                    RequestedMorePage = true;
                    return yPositionDraw;
                }
                else
                {
                    eventArgs.HasMorePages = false;
                    RequestedMorePage = false;
                }
                _currentLineY = eventArgs.Graphics.DrawComponent(template?.Children[i], TemplateFormatting, template.MarginLeft, _currentLineY, eventArgs.Graphics.VisibleClipBounds.Width - (template.MarginLeft + template.MarginRight));
                CurrentChildIndex++;
            }
            return _currentLineY;
        }

        [Obsolete("This method is obsolete, please use DrawXamlComponent an Extension of: PrintPageEventArgs")]
        public static float DrawXamlComponent(this Graphics graphics, IXamlComponent xamlComponent, float yPositionDraw = 0) => throw new Exception("method is obsolete, please use DrawXamlComponent an Extension of: PrintPageEventArgs");
    }
}
