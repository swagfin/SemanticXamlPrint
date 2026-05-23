using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using SemanticXamlPrint.Parser.Components;
using SemanticXamlPrint.Parser.Layout;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SemanticXamlPrint.PDF.NetCore
{
    public static class DefaultXamlXGraphics
    {
        public static float DrawXamlComponent(this PdfDocument document, IXamlComponent xamlComponent, float yPositionDraw = 0, PdfSharpCore.PageOrientation pageOrientation = PdfSharpCore.PageOrientation.Portrait)
        {
            if (xamlComponent == null) return yPositionDraw;
            if (xamlComponent.Type != typeof(TemplateComponent)) throw new Exception($"Root Component must be that of a [{nameof(TemplateComponent)}] but currently is: [{xamlComponent.Name}]");
            TemplateComponent template = (TemplateComponent)xamlComponent;
            ComponentXDrawingFormatting templateFormatting = template.GetPdfXDrawingProperties(XDefaults.Formatting) ?? throw new Exception("Default template properties are missing");
            float currentLineY = yPositionDraw + template.MarginTop;

            PdfPage page = document.AddPage();
            page.Orientation = pageOrientation;

            List<IXamlComponent> footerComponents = template.Children.Where(IsFooterComponent).ToList();
            List<IXamlComponent> bodyComponents = template.Children.Where(component => !IsFooterComponent(component)).ToList();
            bool repeatFooter = template.RepeatFooter && footerComponents.Count > 0;

            float footerHeight = 0;
            if (repeatFooter)
            {
                using (XGraphics measureGraphics = XGraphics.FromPdfPage(page))
                {
                    double measurePageBottom = page.Height.Point - template.MarginBottom;
                    footerHeight = MeasureComponentBlockHeight(measureGraphics, footerComponents, templateFormatting, template.MarginLeft, (float)measurePageBottom, (float)measureGraphics.PageSize.Width - (template.MarginLeft + template.MarginRight));
                }
            }

            for (int index = 0; index < bodyComponents.Count; index++)
            {
                IXamlComponent currentComponent = bodyComponents[index];
                double pageBottom = page.Height.Point - template.MarginBottom;
                float pageBottomForBody = (float)pageBottom - (repeatFooter ? footerHeight : 0);

                if (currentComponent.Type == typeof(GridComponent) && ((GridComponent)currentComponent).RepeatHeaderOnPageBreak)
                {
                    currentLineY = DrawGridWithRepeatingHeader(document, ref page, pageOrientation, (GridComponent)currentComponent, templateFormatting, template, currentLineY, yPositionDraw + template.MarginTop, pageBottomForBody, (float)(template.MarginLeft + template.MarginRight), repeatFooter, footerComponents, footerHeight);
                    continue;
                }

                if (currentLineY > pageBottomForBody)
                {
                    if (repeatFooter)
                    {
                        DrawFooterOnPage(page, footerComponents, templateFormatting, template, footerHeight);
                    }
                    page = document.AddPage();
                    page.Orientation = pageOrientation;
                    currentLineY = yPositionDraw + template.MarginTop;
                    pageBottom = page.Height.Point - template.MarginBottom;
                    pageBottomForBody = (float)pageBottom - (repeatFooter ? footerHeight : 0);
                }

                using (XGraphics xgraphics = XGraphics.FromPdfPage(page))
                {
                    RenderLayoutContext layoutContext = new RenderLayoutContext { PageBottomY = pageBottomForBody };
                    currentLineY = xgraphics.DrawComponent(currentComponent, templateFormatting, template.MarginLeft, currentLineY, (float)xgraphics.PageSize.Width - (template.MarginLeft + template.MarginRight), layoutContext);
                }
            }

            if (repeatFooter)
            {
                DrawFooterOnPage(page, footerComponents, templateFormatting, template, footerHeight);
            }

            return currentLineY;
        }

        private static float DrawGridWithRepeatingHeader(PdfDocument document, ref PdfPage page, PdfSharpCore.PageOrientation pageOrientation, GridComponent sourceGrid, ComponentXDrawingFormatting templateFormatting, TemplateComponent template, float currentY, float topStartY, float pageBottomForBody, float horizontalMargins, bool repeatFooter, List<IXamlComponent> footerComponents, float footerHeight)
        {
            List<GridRowComponent> allRows = sourceGrid.Children.Where(element => element.Type == typeof(GridRowComponent)).Select(validElement => (GridRowComponent)validElement).ToList();
            int headerCount = Math.Min(sourceGrid.HeaderRows, allRows.Count);
            List<GridRowComponent> headerRows = allRows.Take(headerCount).ToList();
            List<GridRowComponent> dataRows = allRows.Skip(headerCount).ToList();

            bool drawHeaders = headerRows.Count > 0;

            using (XGraphics xgraphics = XGraphics.FromPdfPage(page))
            {
                if (drawHeaders)
                {
                    RenderLayoutContext layoutContext = new RenderLayoutContext { PageBottomY = pageBottomForBody };
                    GridComponent headerGrid = CreateGridSlice(sourceGrid, headerRows);
                    currentY = xgraphics.DrawComponent(headerGrid, templateFormatting, template.MarginLeft, currentY, (float)xgraphics.PageSize.Width - horizontalMargins, layoutContext);
                }
            }

            foreach (GridRowComponent row in dataRows)
            {
                if (currentY > pageBottomForBody)
                {
                    if (repeatFooter)
                    {
                        DrawFooterOnPage(page, footerComponents, templateFormatting, template, footerHeight);
                    }
                    page = document.AddPage();
                    page.Orientation = pageOrientation;
                    currentY = topStartY;

                    if (drawHeaders)
                    {
                        using (XGraphics headerGraphics = XGraphics.FromPdfPage(page))
                        {
                            RenderLayoutContext headerContext = new RenderLayoutContext { PageBottomY = pageBottomForBody };
                            GridComponent headerGrid = CreateGridSlice(sourceGrid, headerRows);
                            currentY = headerGraphics.DrawComponent(headerGrid, templateFormatting, template.MarginLeft, currentY, (float)headerGraphics.PageSize.Width - horizontalMargins, headerContext);
                        }
                    }
                }

                using (XGraphics rowGraphics = XGraphics.FromPdfPage(page))
                {
                    RenderLayoutContext rowContext = new RenderLayoutContext { PageBottomY = pageBottomForBody };
                    GridComponent rowGrid = CreateGridSlice(sourceGrid, new List<GridRowComponent> { row });
                    currentY = rowGraphics.DrawComponent(rowGrid, templateFormatting, template.MarginLeft, currentY, (float)rowGraphics.PageSize.Width - horizontalMargins, rowContext);
                }
            }

            return currentY;
        }

        private static GridComponent CreateGridSlice(GridComponent source, List<GridRowComponent> rows)
        {
            GridComponent grid = new GridComponent
            {
                ColumnWidths = source.ColumnWidths,
                BorderStyle = source.BorderStyle,
                BorderWidth = source.BorderWidth,
                MinHeight = 0,
                HeightMode = null,
                BottomReserve = 0,
                RepeatHeaderOnPageBreak = false,
                HeaderRows = source.HeaderRows
            };
            foreach (GridRowComponent row in rows)
            {
                grid.AddChild(row);
            }
            return grid;
        }

        private static float MeasureComponentBlockHeight(XGraphics graphics, List<IXamlComponent> components, ComponentXDrawingFormatting templateFormatting, float marginLeft, float pageBottomY, float maxLayoutWidth)
        {
            float y = 0;
            RenderLayoutContext layoutContext = new RenderLayoutContext { PageBottomY = pageBottomY };
            foreach (IXamlComponent component in components)
            {
                y = graphics.DrawComponent(component, templateFormatting, marginLeft, y, maxLayoutWidth, layoutContext);
            }
            return y;
        }

        private static void DrawFooterOnPage(PdfPage page, List<IXamlComponent> footerComponents, ComponentXDrawingFormatting templateFormatting, TemplateComponent template, float footerHeight)
        {
            using (XGraphics graphics = XGraphics.FromPdfPage(page))
            {
                float maxLayoutWidth = (float)graphics.PageSize.Width - (template.MarginLeft + template.MarginRight);
                float startY = (float)(page.Height.Point - template.MarginBottom - footerHeight);
                RenderLayoutContext layoutContext = new RenderLayoutContext { PageBottomY = (float)(page.Height.Point - template.MarginBottom) };
                float y = startY;
                foreach (IXamlComponent component in footerComponents)
                {
                    y = graphics.DrawComponent(component, templateFormatting, template.MarginLeft, y, maxLayoutWidth, layoutContext);
                }
            }
        }

        private static bool IsFooterComponent(IXamlComponent component)
        {
            return component?.CustomProperties?.IsPropertyExistsWithValue("page.footer", "true") == true;
        }
    }
}
