using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using SemanticXamlPrint.Parser.Components;
using SemanticXamlPrint.Parser.Layout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SemanticXamlPrint.PDF
{
    public static class XGraphicsExtensions
    {
        private delegate float ComponentRenderer(XGraphics graphics, IXamlComponent component, ComponentXDrawingFormatting templateFormatting, float currentX, float currentY, float maxLayoutWidth);

        private static readonly Dictionary<Type, ComponentRenderer> Renderers = new Dictionary<Type, ComponentRenderer>
        {
            { typeof(LineBreakComponent), RenderLineBreak },
            { typeof(LineComponent), RenderLine },
            { typeof(ImageComponent), RenderImage },
            { typeof(QRCodeComponent), RenderQRCode },
            { typeof(DataComponent), RenderData },
            { typeof(CellsComponent), RenderCells },
            { typeof(GridComponent), RenderGrid }
        };

        public static float DrawComponent(this XGraphics graphics, IXamlComponent component, ComponentXDrawingFormatting templateFormatting, float currentX, float currentY, float maxLayoutWidth)
        {
            float boundedMaxLayoutWidth = maxLayoutWidth == 0 ? (float)graphics.PageSize.Width : maxLayoutWidth;
            if (component == null) return currentY;
            if (!Renderers.TryGetValue(component.Type, out ComponentRenderer renderer)) return currentY;
            return renderer(graphics, component, templateFormatting, currentX, currentY, boundedMaxLayoutWidth);
        }

        private static float RenderLineBreak(XGraphics graphics, IXamlComponent component, ComponentXDrawingFormatting templateFormatting, float currentX, float currentY, float maxLayoutWidth)
        {
            return currentY + 3;
        }

        private static float RenderLine(XGraphics graphics, IXamlComponent component, ComponentXDrawingFormatting templateFormatting, float currentX, float currentY, float maxLayoutWidth)
        {
            LineComponent lineComponent = (LineComponent)component;
            float newY = currentY + 3;
            newY += graphics.DrawlLineAndReturnHeight(lineComponent.Style.ToDashStyle(), currentX, newY, (int)maxLayoutWidth);
            return newY + 3;
        }

        private static float RenderImage(XGraphics graphics, IXamlComponent component, ComponentXDrawingFormatting templateFormatting, float currentX, float currentY, float maxLayoutWidth)
        {
            ImageComponent imageComponent = (ImageComponent)component;
            string imageSource = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imageComponent.Source ?? "default.png");
            if (!File.Exists(imageSource)) return currentY;
            using (XImage image = XImage.FromFile(imageSource))
            {
                return currentY + graphics.DrawImageCenteredAndReturnHeight(image, currentX, currentY, imageComponent.Width, imageComponent.Height, maxLayoutWidth);
            }
        }

        private static float RenderQRCode(XGraphics graphics, IXamlComponent component, ComponentXDrawingFormatting templateFormatting, float currentX, float currentY, float maxLayoutWidth)
        {
            QRCodeComponent qrCodeComponent = (QRCodeComponent)component;
            return currentY + graphics.DrawQRCodeCenteredAndReturnHeight(qrCodeComponent.Text, currentX, currentY, qrCodeComponent.Width, qrCodeComponent.Height, maxLayoutWidth);
        }

        private static float RenderData(XGraphics graphics, IXamlComponent component, ComponentXDrawingFormatting templateFormatting, float currentX, float currentY, float maxLayoutWidth)
        {
            DataComponent dataComponent = (DataComponent)component;
            ComponentXDrawingFormatting format = component.GetPdfXDrawingProperties(templateFormatting);
            return currentY + graphics.DrawStringAndReturnHeight(dataComponent.Text, dataComponent.TextWrap, format, currentX, currentY, (int)maxLayoutWidth);
        }

        private static float RenderCells(XGraphics graphics, IXamlComponent component, ComponentXDrawingFormatting templateFormatting, float currentX, float currentY, float maxLayoutWidth)
        {
            CellsComponent cellsComponent = (CellsComponent)component;
            ComponentXDrawingFormatting rowFormat = component.GetPdfXDrawingProperties(templateFormatting);
            List<CellComponent> dataRowCells = cellsComponent.Children?.Where(element => element.Type == typeof(CellComponent)).Select(validElement => (CellComponent)validElement).ToList();
            int additionalHeight = 0;
            foreach (CellComponent cell in dataRowCells)
            {
                ComponentXDrawingFormatting cellFormat = cell.GetPdfXDrawingProperties(rowFormat);
                float x = (cell.X <= 0) ? 0f : cell.X;
                float y = (cell.Y <= 0) ? currentY : cell.Y;
                float z = (cell.Z <= 0) ? (int)maxLayoutWidth : cell.Z;
                int textHeight = graphics.DrawStringAndReturnHeight(cell.Text, cell.TextWrap, cellFormat, x, y, z);
                additionalHeight = (textHeight > additionalHeight) ? textHeight : additionalHeight;
            }
            return currentY + additionalHeight;
        }

        private static float RenderGrid(XGraphics graphics, IXamlComponent component, ComponentXDrawingFormatting templateFormatting, float currentX, float currentY, float maxLayoutWidth)
        {
            GridComponent gridComponent = (GridComponent)component;
            ComponentXDrawingFormatting gridFormat = component.GetPdfXDrawingProperties(templateFormatting);
            GridLayoutPlan layoutPlan = GridLayoutPlanner.Build(gridComponent, maxLayoutWidth);

            float yBeforeGrid = currentY;
            float longestColumnY = currentY;
            foreach (GridRowLayoutPlan rowPlan in layoutPlan.Rows)
            {
                float rowStartY = longestColumnY;
                float rowCurrentX = currentX;
                ComponentXDrawingFormatting rowFormat = rowPlan.Row.GetPdfXDrawingProperties(gridFormat);

                for (int columnIndex = 0; columnIndex < layoutPlan.ColumnWidths.Count; columnIndex++)
                {
                    IXamlComponent componentUnderColumn = rowPlan.ComponentsByColumn[columnIndex];
                    if (componentUnderColumn != null)
                    {
                        float newY = graphics.DrawComponent(componentUnderColumn, rowFormat, rowCurrentX, rowStartY, layoutPlan.ColumnWidths[columnIndex]);
                        longestColumnY = (newY > longestColumnY) ? newY : longestColumnY;
                    }
                    rowCurrentX += layoutPlan.ColumnWidths[columnIndex];
                }
            }

            float currentGridY = longestColumnY;
            if (!string.IsNullOrEmpty(gridComponent.BorderStyle))
            {
                graphics.DrawRectangleAndReturnHeight(gridComponent.BorderStyle.ToDashStyle(), currentX, yBeforeGrid, (int)maxLayoutWidth, (currentGridY - yBeforeGrid), gridComponent.BorderWidth);
                float borderCurrentX = currentX;
                for (int columnIndex = 0; columnIndex < layoutPlan.ColumnWidths.Count; columnIndex++)
                {
                    graphics.DrawRectangleAndReturnHeight(gridComponent.BorderStyle.ToDashStyle(), borderCurrentX, yBeforeGrid, layoutPlan.ColumnWidths[columnIndex], (currentGridY - yBeforeGrid), gridComponent.BorderWidth);
                    borderCurrentX += layoutPlan.ColumnWidths[columnIndex];
                }
            }
            return currentGridY;
        }

        public static int DrawStringAndReturnHeight(this XGraphics gfx, string text, bool textWrap, ComponentXDrawingFormatting cellFmt, double x, double y, double z)
        {
            text = text ?? string.Empty;
            XFont font = cellFmt.Font;
            XStringFormat stringFormat = cellFmt.StringFormat;
            XBrush brush = cellFmt.Brush;
            if (textWrap && gfx.MeasureString(text, font).Width > z)
            {
                string[] lines = SplitTextIntoLines(gfx, text, font, z);
                double lineHeight = font.GetHeight();
                double totalHeight = lines.Length * lineHeight;
                XTextFormatter textFormatter = new XTextFormatter(gfx);
                foreach (string line in lines)
                {
                    textFormatter.DrawString(line, font, brush, new XRect(x + 2, y, (z - 2), totalHeight), stringFormat);
                    y += lineHeight;
                }
                return (int)totalHeight;
            }

            XRect layoutRect = new XRect(x + 2, y, (z - 4), gfx.MeasureString(text, font).Height);
            gfx.DrawString(text, font, brush, layoutRect, stringFormat);
            return (int)layoutRect.Height;
        }

        public static List<int> GetDivideColumnWidths(this XGraphics graphics, string pattern, float maxLayoutWith)
        {
            return ColumnWidthCalculator.Calculate(pattern, maxLayoutWith);
        }

        public static List<int> GetDivideColumnWidths(this XGraphics graphics, int columns, float maxLayoutWith)
        {
            return ColumnWidthCalculator.Calculate(columns, maxLayoutWith);
        }

        public static int DrawlLineAndReturnHeight(this XGraphics graphics, XDashStyle dashStyle, float x, float y, float z)
        {
            XPen pen = new XPen(XColors.Black)
            {
                DashStyle = dashStyle
            };
            graphics.DrawLine(pen, x, y, z, y);
            return 1;
        }

        public static int DrawImageCenteredAndReturnHeight(this XGraphics graphics, XImage image, double x, double y, double maxWidth, double maxHeight, double maxLayoutWidth)
        {
            double newWidth = Math.Min(image.PixelWidth, maxWidth > 0 ? maxWidth : image.PixelWidth);
            double newHeight = Math.Min(image.PixelHeight, maxHeight > 0 ? maxHeight : image.PixelHeight);
            double centeredX = x + (maxLayoutWidth - newWidth) / 2;
            graphics.DrawImage(image, centeredX > 0 ? centeredX : x, y, newWidth, newHeight);
            return (int)newHeight;
        }

        public static int DrawRectangleAndReturnHeight(this XGraphics gfx, XDashStyle dashStyle, double x, double y, double width, double height, double lineWidth = 0.3)
        {
            XPen pen = new XPen(XColors.Black, lineWidth)
            {
                DashStyle = dashStyle
            };
            gfx.DrawRectangle(pen, x, y, width, height);
            return (int)height;
        }

        private static string[] SplitTextIntoLines(XGraphics gfx, string text, XFont font, double maxWidth)
        {
            string[] words = text?.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> lines = new List<string>();
            StringBuilder currentLine = new StringBuilder();
            foreach (string word in words)
            {
                if (gfx.MeasureString(string.Format("{0}{1} ", currentLine, word), font).Width <= maxWidth)
                {
                    currentLine.Append(string.Format("{0} ", word));
                }
                else
                {
                    if (currentLine.Length > 0)
                    {
                        lines.Add(currentLine.ToString().TrimEnd());
                    }
                    currentLine.Clear();
                    currentLine.Append(string.Format("{0} ", word));
                }
            }
            if (currentLine.Length > 0)
            {
                lines.Add(currentLine.ToString().TrimEnd());
            }
            return lines.ToArray();
        }
    }
}
