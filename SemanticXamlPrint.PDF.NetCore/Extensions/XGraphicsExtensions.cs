using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using SemanticXamlPrint.Parser.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SemanticXamlPrint.PDF.NetCore
{
    public static class XGraphicsExtensions
    {
        public static float DrawComponent(this XGraphics graphics, IXamlComponent component, ComponentXDrawingFormatting TemplateFormatting, float currentX, float currentY, float maxLayoutWidth)
        {
            maxLayoutWidth = maxLayoutWidth == 0 ? (float)graphics.PageSize.Width : maxLayoutWidth;
            //Draw
            if (component.Type == typeof(LineBreakComponent))
            {
                currentY += 3;
            }
            else if (component.Type == typeof(LineComponent))
            {
                LineComponent lineComponent = (LineComponent)component;
                currentY += 3;
                currentY += graphics.DrawlLineAndReturnHeight(lineComponent.Style.ToDashStyle(), currentX, currentY, (int)maxLayoutWidth);
                currentY += 3;
            }
            else if (component.Type == typeof(ImageComponent))
            {
                ImageComponent imageComponent = (ImageComponent)component;
                string imageSource = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imageComponent.Source ?? "default.png");
                if (File.Exists(imageSource))
                {
                    currentY += graphics.DrawImageCenteredAndReturnHeight(XImage.FromFile(imageSource), currentX, currentY, imageComponent.Width, imageComponent.Height, maxLayoutWidth);
                }
            }
            else if (component.Type == typeof(QRCodeComponent))
            {
                QRCodeComponent qRCodeComponent = (QRCodeComponent)component;
                currentY += graphics.DrawQRCodeCenteredAndReturnHeight(qRCodeComponent.Text, currentX, currentY, qRCodeComponent.Width, qRCodeComponent.Height, maxLayoutWidth);
            }
            else if (component.Type == typeof(DataComponent))
            {
                DataComponent dataComponent = (DataComponent)component;
                ComponentXDrawingFormatting fmt = component.GetPdfXDrawingProperties(TemplateFormatting);
                //Draw Data Component
                currentY += graphics.DrawStringAndReturnHeight(dataComponent.Text, dataComponent.TextWrap, fmt, currentX, currentY, (int)maxLayoutWidth);
            }
            else if (component.Type == typeof(CellsComponent))
            {
                CellsComponent dataRowComponent = (CellsComponent)component;
                ComponentXDrawingFormatting rowfmt = component.GetPdfXDrawingProperties(TemplateFormatting);
                //Get all Children of DataRowCells
                List<CellComponent> dataRowCells = dataRowComponent.Children?.Where(element => element.Type == typeof(CellComponent)).Select(validElement => (CellComponent)validElement).ToList();
                int additionalHeight = 0;
                foreach (CellComponent cell in dataRowCells)
                {
                    ComponentXDrawingFormatting cellFmt = cell.GetPdfXDrawingProperties(rowfmt);
                    //Set RowCell Location
                    float x = (cell.X <= 0) ? 0f : cell.X;
                    float y = (cell.Y <= 0) ? currentY : cell.Y;
                    float z = (cell.Z <= 0) ? (int)maxLayoutWidth : cell.Z;
                    //Write String 
                    int textHeight = graphics.DrawStringAndReturnHeight(cell.Text, cell.TextWrap, cellFmt, x, y, z);
                    additionalHeight = (textHeight > additionalHeight) ? textHeight : additionalHeight;
                }
                //Add Line Height
                currentY += additionalHeight;
            }
            else if (component.Type == typeof(GridComponent))
            {
                GridComponent gridComponent = (GridComponent)component;
                ComponentXDrawingFormatting gridfmt = component.GetPdfXDrawingProperties(TemplateFormatting);
                List<int> columnWidths = graphics.GetDivideColumnWidths(gridComponent.ColumnWidths, maxLayoutWidth);
                float y_before_grid = currentY;
                float longest_column_y = currentY;
                //Get Grid Rows
                List<GridRowComponent> gridRows = gridComponent.Children?.Where(element => element.Type == typeof(GridRowComponent)).Select(validElement => (GridRowComponent)validElement).ToList();
                foreach (GridRowComponent row in gridRows)
                {
                    float current_y = longest_column_y;
                    float current_x = currentX;
                    ComponentXDrawingFormatting rowFmt = row.GetPdfXDrawingProperties(gridfmt);
                    for (int colIndex = 0; colIndex < columnWidths.Count; colIndex++)
                    {
                        IXamlComponent componentUnderColumn = row.Children?.FirstOrDefault(x => x.CustomProperties.IsPropertyExistsWithValue("grid.column", colIndex.ToString()));
                        if (componentUnderColumn != null)
                        {
                            float new_y = graphics.DrawComponent(componentUnderColumn, rowFmt, current_x, current_y, columnWidths[colIndex]);
                            longest_column_y = (new_y > longest_column_y) ? new_y : longest_column_y;
                            //Next Column Starting X co-ordinates
                            current_x += columnWidths[colIndex];
                        }
                    }
                }
                //set Highest Column Height
                currentY = longest_column_y;
                //# Check if Drawing Border
                if (!string.IsNullOrEmpty(gridComponent.BorderStyle))
                {
                    graphics.DrawRectangleAndReturnHeight(gridComponent.BorderStyle.ToDashStyle(), currentX, y_before_grid, (int)maxLayoutWidth, (currentY - y_before_grid), gridComponent.BorderWidth);
                    float current_x = currentX;
                    for (int colIndex = 0; colIndex < columnWidths.Count; colIndex++)
                    {
                        graphics.DrawRectangleAndReturnHeight(gridComponent.BorderStyle.ToDashStyle(), current_x, y_before_grid, columnWidths[colIndex], (currentY - y_before_grid), gridComponent.BorderWidth);
                        current_x += columnWidths[colIndex];
                    }
                }
            }
            else
            {
                //unknown Component
            }
            return currentY;
        }

        public static int DrawStringAndReturnHeight(this XGraphics gfx, string text, bool textWrap, ComponentXDrawingFormatting cellFmt, double x, double y, double z)
        {
            text = text ?? string.Empty;
            XFont font = cellFmt.Font;
            XStringFormat stringFormat = cellFmt.StringFormat;
            XBrush brush = cellFmt.Brush;
            //Check wrap
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
            else
            {
                XRect layoutRect = new XRect(x + 2, y, (z - 4), gfx.MeasureString(text, font).Height);
                gfx.DrawString(text, font, brush, layoutRect, stringFormat);
                return (int)layoutRect.Height;
            }
        }


        public static List<int> GetDivideColumnWidths(this XGraphics graphics, string pattern, float maxLayoutWith)
        {
            try
            {
                if (string.IsNullOrEmpty(pattern)) return GetDivideColumnWidths(graphics, 1, maxLayoutWith);
                List<int> columnWidths = new List<int>();
                int total = pattern.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries).Sum(p => Convert.ToInt32(p));
                if (total < 1) return GetDivideColumnWidths(graphics, 1, maxLayoutWith);
                int remainingWidth = (int)maxLayoutWith;
                foreach (string s in pattern.Split('*'))
                {
                    int w = (int)Math.Round((double)remainingWidth / total * Convert.ToInt32(s));
                    columnWidths.Add(w);
                    remainingWidth -= w;
                    total -= Convert.ToInt32(s);
                }
                return columnWidths;
            }
            catch { return GetDivideColumnWidths(graphics, 1, maxLayoutWith); }
        }
        public static List<int> GetDivideColumnWidths(this XGraphics graphics, int columns, float maxLayoutWith)
        {
            columns = columns <= 0 ? 1 : columns;
            int evenColumnWidth = (int)maxLayoutWith / columns;
            List<int> columnWidths = new List<int>();
            for (var i = 0; i < columns; i += 1)
                columnWidths.Add(evenColumnWidth);
            return columnWidths;
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
                    lines.Add(currentLine.ToString().TrimEnd());
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
