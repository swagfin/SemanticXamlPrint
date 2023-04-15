using SemanticXamlPrint.Components;
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
        public static void DrawComponent(this Graphics graphics, IXamlComponent component, ComponentDrawingFormatting TemplateFormatting, ref float currentLineY)
        {
            if (component.Type == typeof(LineBreakComponent))
            {
                currentLineY += 3;
            }
            else if (component.Type == typeof(LineComponent))
            {
                LineComponent lineComponent = (LineComponent)component;
                currentLineY += 3;
                currentLineY += graphics.DrawlLineAndReturnHeight(lineComponent.Style.ToDashStyle(), 0, currentLineY, (int)graphics.VisibleClipBounds.Width);
                currentLineY += 3;
            }
            else if (component.Type == typeof(ImageComponent))
            {
                ImageComponent imageComponent = (ImageComponent)component;
                string imageSource = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imageComponent.Source ?? "default.png");
                if (File.Exists(imageSource))
                {
                    currentLineY += graphics.DrawImageCenteredAndReturnHeight(Image.FromFile(imageSource), 0, currentLineY, imageComponent.Width, imageComponent.Height);
                }
            }
            else if (component.Type == typeof(QRCodeComponent))
            {
                QRCodeComponent qRCodeComponent = (QRCodeComponent)component;
                currentLineY += graphics.DrawQRCodeCenteredAndReturnHeight(qRCodeComponent.Text, 0, currentLineY, qRCodeComponent.Width, qRCodeComponent.Height);
            }
            else if (component.Type == typeof(DataComponent))
            {
                DataComponent dataComponent = (DataComponent)component;
                ComponentDrawingFormatting fmt = component.GetSystemDrawingProperties(TemplateFormatting);
                //Draw Data Component
                currentLineY += graphics.DrawStringAndReturnHeight(dataComponent.Text, dataComponent.TextWrap, fmt, 0, currentLineY, (int)graphics.VisibleClipBounds.Width);
            }
            else if (component.Type == typeof(DataRowComponent))
            {
                DataRowComponent dataRowComponent = (DataRowComponent)component;
                ComponentDrawingFormatting rowfmt = component.GetSystemDrawingProperties(TemplateFormatting);
                //Get all Children of DataRowCells
                List<DataRowCellComponent> dataRowCells = dataRowComponent.Children?.Where(element => element.Type == typeof(DataRowCellComponent)).Select(validElement => (DataRowCellComponent)validElement).ToList();
                int additionalHeight = 0;
                foreach (DataRowCellComponent cell in dataRowCells)
                {
                    ComponentDrawingFormatting cellFmt = cell.GetSystemDrawingProperties(rowfmt);
                    //Set RowCell Location
                    float x = (cell.X <= 0) ? 0f : cell.X;
                    float y = (cell.Y <= 0) ? currentLineY : cell.Y;
                    float z = (cell.Z <= 0) ? (int)graphics.VisibleClipBounds.Width : cell.Z;
                    //Write String 
                    int textHeight = graphics.DrawStringAndReturnHeight(cell.Text, cell.TextWrap, cellFmt, x, y, z);
                    additionalHeight = (textHeight > additionalHeight) ? textHeight : additionalHeight;
                }
                //Add Line Height
                currentLineY += additionalHeight;
            }
            else if (component.Type == typeof(GridComponent))
            {
                GridComponent gridComponent = (GridComponent)component;
                ComponentDrawingFormatting gridfmt = component.GetSystemDrawingProperties(TemplateFormatting);
                List<int> columnWidths = graphics.GetDivideColumnWidths(gridComponent.ColumnWidths);
                float y_before_grid = currentLineY;
                int additionalHeight = 0;
                float lastXPosition = 0;
                //Get Grid Rows
                List<GridRowComponent> gridRows = gridComponent.Children?.Where(element => element.Type == typeof(GridRowComponent)).Select(validElement => (GridRowComponent)validElement).ToList();
                foreach (GridRowComponent row in gridRows)
                {
                    additionalHeight = 0;
                    lastXPosition = 0;
                    ComponentDrawingFormatting rowFmt = row.GetSystemDrawingProperties(gridfmt);
                    List<DataComponent> rowChildren = row.Children?.Where(element => element.Type == typeof(DataComponent)).Select(validElement => (DataComponent)validElement).ToList();
                    for (int colIndex = 0; colIndex < columnWidths.Count; colIndex++)
                    {
                        DataComponent columnComponent = rowChildren?.FirstOrDefault(x => x.CustomProperties.IsPropertyExistsWithValue("grid.column", colIndex.ToString()));
                        if (columnComponent != null)
                        {
                            ComponentDrawingFormatting childFmt = columnComponent.GetSystemDrawingProperties(rowFmt);
                            int textHeight = graphics.DrawStringAndReturnHeight(columnComponent.Text, columnComponent.TextWrap, childFmt, lastXPosition, currentLineY, columnWidths[colIndex]);
                            additionalHeight = (textHeight > additionalHeight) ? textHeight : additionalHeight;
                            lastXPosition += columnWidths[colIndex];
                        }
                    }
                    currentLineY += additionalHeight;
                }
                //#Check if Drawing Border
                if (!string.IsNullOrEmpty(gridComponent.BorderStyle))
                {
                    graphics.DrawRectangleAndReturnHeight(gridComponent.BorderStyle.ToDashStyle(), 0, y_before_grid, (int)graphics.VisibleClipBounds.Width, currentLineY - y_before_grid);
                    lastXPosition = 0;
                    for (int colIndex = 0; colIndex < columnWidths.Count; colIndex++)
                    {
                        graphics.DrawRectangleAndReturnHeight(gridComponent.BorderStyle.ToDashStyle(), lastXPosition, y_before_grid, columnWidths[colIndex], currentLineY - y_before_grid);
                        lastXPosition += columnWidths[colIndex];
                    }
                }
            }
            else
            {
                return;
            }
        }

        public static int DrawStringAndReturnHeight(this Graphics graphics, string text, bool textWrap, ComponentDrawingFormatting cellFmt, float x, float y, float z)
        {
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


        public static List<int> GetDivideColumnWidths(this Graphics graphics, string pattern)
        {
            try
            {
                if (string.IsNullOrEmpty(pattern)) return GetDivideColumnWidths(graphics, 1);
                List<int> columnWidths = new List<int>();
                int total = pattern.Split(new char[] { '*' }, StringSplitOptions.RemoveEmptyEntries).Sum(p => Convert.ToInt32(p));
                if (total < 1) return GetDivideColumnWidths(graphics, 1);
                int remainingWidth = (int)graphics.VisibleClipBounds.Width;
                foreach (string s in pattern.Split('*'))
                {
                    int w = (int)Math.Round((double)remainingWidth / total * Convert.ToInt32(s));
                    columnWidths.Add(w);
                    remainingWidth -= w;
                    total -= Convert.ToInt32(s);
                }
                return columnWidths;
            }
            catch { return GetDivideColumnWidths(graphics, 1); }
        }
        public static List<int> GetDivideColumnWidths(this Graphics graphics, int columns)
        {
            columns = columns <= 0 ? 1 : columns;
            int evenColumnWidth = (int)graphics.VisibleClipBounds.Width / columns;
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
        public static int DrawImageCenteredAndReturnHeight(this Graphics graphics, Image image, float x, float y, float maxWidth = 0, float maxHeight = 0)
        {

            float newWidth = Math.Min(image.Height, maxWidth > 0 ? maxWidth : image.Width);
            float newHeight = Math.Min(image.Height, maxHeight > 0 ? maxHeight : image.Height);
            float centeredX = x + (graphics.VisibleClipBounds.Width - newWidth) / 2;
            graphics.DrawImage(image, centeredX > 0 ? centeredX : x, y, newWidth, newHeight);
            return (int)newHeight;
        }
        public static int DrawRectangleAndReturnHeight(this Graphics graphics, DashStyle dashStyle, float x, float y, float z, float height)
        {

            using (Pen pen = new Pen(Color.Black) { DashStyle = dashStyle })
            {
                graphics.DrawRectangle(pen, x, y, z, height);
            }
            return (int)height;
        }
    }
}
