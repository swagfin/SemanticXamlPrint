using PdfSharp.Drawing;
using PdfSharp.Pdf;
using SemanticXamlPrint.Parser.Components;
using System;

namespace SemanticXamlPrint.PDF
{
    public static class DefaultXamlXGraphics
    {
        public static float DrawXamlComponent(this PdfDocument document, IXamlComponent xamlComponent, float yPositionDraw = 0, PdfSharp.PageOrientation pageOrientation = PdfSharp.PageOrientation.Portrait)
        {
            if (xamlComponent == null) return yPositionDraw;
            if (xamlComponent.Type != typeof(TemplateComponent)) throw new Exception($"Root Component must be that of a [{nameof(TemplateComponent)}] but currently is: [{xamlComponent.Name}]");
            TemplateComponent template = (TemplateComponent)xamlComponent;
            ComponentXDrawingFormatting TemplateFormatting = template.GetPdfXDrawingProperties(XDefaults.Formatting) ?? throw new Exception("Default template properties are missing");
            float _currentLineY = yPositionDraw + (float)template.MarginTop;
            //create default Page
            // Add a page to the document
            PdfPage page = document.AddPage();
            page.Orientation = pageOrientation;
            //Draw Root Component Children
            for (int i = 0; i < template?.Children?.Count; i++)
            {
                double pageHeight = page.Height.Point - template.MarginBottom;
                if (_currentLineY > pageHeight)
                {
                    page = document.AddPage();
                    page.Orientation = pageOrientation;
                    _currentLineY = yPositionDraw + (float)template.MarginTop;
                }
                //Draw Component
                using (XGraphics xgraphics = XGraphics.FromPdfPage(page))
                    _currentLineY = xgraphics.DrawComponent(template?.Children[i], TemplateFormatting, template.MarginLeft, _currentLineY, (float)xgraphics.PageSize.Width - (template.MarginLeft + template.MarginRight));
            }
            return _currentLineY;
        }
    }
}
