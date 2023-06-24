using SemanticXamlPrint.Parser.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;

namespace SemanticXamlPrint
{
    internal static class GraphicsExtensions
    {
        public static float DrawComponent(this Graphics graphics, IXamlComponent component, ComponentDrawingFormatting TemplateFormatting, float currentX, float currentY, float maxLayoutWidth)
        {
            maxLayoutWidth = maxLayoutWidth == 0 ? graphics.VisibleClipBounds.Width : maxLayoutWidth;
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
                    currentY += graphics.DrawImageCenteredAndReturnHeight(Image.FromFile(imageSource), currentX, currentY, imageComponent.Width, imageComponent.Height, maxLayoutWidth);
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
                ComponentDrawingFormatting fmt = component.GetSystemDrawingProperties(TemplateFormatting);
                //Draw Data Component
                currentY += graphics.DrawStringAndReturnHeight(dataComponent.Text, dataComponent.TextWrap, fmt, currentX, currentY, (int)maxLayoutWidth);
            }
            else if (component.Type == typeof(CellsComponent))
            {
                CellsComponent dataRowComponent = (CellsComponent)component;
                ComponentDrawingFormatting rowfmt = component.GetSystemDrawingProperties(TemplateFormatting);
                //Get all Children of DataRowCells
                List<CellComponent> dataRowCells = dataRowComponent.Children?.Where(element => element.Type == typeof(CellComponent)).Select(validElement => (CellComponent)validElement).ToList();
                int additionalHeight = 0;
                foreach (CellComponent cell in dataRowCells)
                {
                    ComponentDrawingFormatting cellFmt = cell.GetSystemDrawingProperties(rowfmt);
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
                ComponentDrawingFormatting gridfmt = component.GetSystemDrawingProperties(TemplateFormatting);
                List<int> columnWidths = graphics.GetDivideColumnWidths(gridComponent.ColumnWidths, maxLayoutWidth);
                float y_before_grid = currentY;
                float longest_column_y = currentY;
                //Get Grid Rows
                List<GridRowComponent> gridRows = gridComponent.Children?.Where(element => element.Type == typeof(GridRowComponent)).Select(validElement => (GridRowComponent)validElement).ToList();
                foreach (GridRowComponent row in gridRows)
                {
                    float current_y = longest_column_y;
                    float current_x = currentX;
                    ComponentDrawingFormatting rowFmt = row.GetSystemDrawingProperties(gridfmt);
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

        public static int DrawStringAndReturnHeight(this Graphics graphics, string text, bool textWrap, ComponentDrawingFormatting cellFmt, float x, float y, float z)
        {
            text = text ?? string.Empty;
            if (textWrap && (int)graphics.MeasureString(text, cellFmt.Font).Width > z)
            {
                SizeF size = graphics.MeasureString(text, cellFmt.Font, (int)z);
                RectangleF layoutF = new RectangleF(new PointF(x, y), size);
                graphics.DrawString(text, cellFmt.Font, cellFmt.Brush, layoutF, cellFmt.StringFormat);
                return (int)layoutF.Height;
            }
            else
            {
                SizeF size = graphics.MeasureString(text, cellFmt.Font, (int)z, new StringFormat { FormatFlags = StringFormatFlags.NoWrap });
                Rectangle layout = new Rectangle((int)x, (int)y, (int)z, (int)size.Height);
                graphics.DrawString(text, cellFmt.Font, cellFmt.Brush, layout, cellFmt.StringFormat);
                return layout.Height;
            }
        }


        public static List<int> GetDivideColumnWidths(this Graphics graphics, string pattern, float maxLayoutWith)
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
        public static List<int> GetDivideColumnWidths(this Graphics graphics, int columns, float maxLayoutWith)
        {
            columns = columns <= 0 ? 1 : columns;
            int evenColumnWidth = (int)maxLayoutWith / columns;
            List<int> columnWidths = new List<int>();
            for (var i = 0; i < columns; i += 1)
                columnWidths.Add(evenColumnWidth);
            return columnWidths;
        }

        public static int DrawlLineAndReturnHeight(this Graphics graphics, DashStyle dashStyle, float x, float y, float z)
        {
            using (Pen pen = new Pen(Color.Black) { DashStyle = dashStyle })
            {
                graphics.DrawLine(pen, x, y, z, y);
            }
            return 1;
        }
        public static int DrawImageCenteredAndReturnHeight(this Graphics graphics, Image image, float x, float y, float maxWidth, float maxHeight, float maxLayoutWith)
        {

            float newWidth = Math.Min(image.Height, maxWidth > 0 ? maxWidth : image.Width);
            float newHeight = Math.Min(image.Height, maxHeight > 0 ? maxHeight : image.Height);
            float centeredX = x + (maxLayoutWith - newWidth) / 2;
            graphics.DrawImage(image, centeredX > 0 ? centeredX : x, y, newWidth, newHeight);
            return (int)newHeight;
        }
        public static int DrawRectangleAndReturnHeight(this Graphics graphics, DashStyle dashStyle, float x, float y, float z, float height, double lineWidth = 0.3)
        {
            using (Pen pen = new Pen(Color.Black, (float)lineWidth) { DashStyle = dashStyle })
            {
                graphics.DrawRectangle(pen, x, y, z, height);
            }
            return (int)height;
        }
    }
}
