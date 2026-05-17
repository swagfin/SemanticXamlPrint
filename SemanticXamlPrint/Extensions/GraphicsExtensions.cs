using SemanticXamlPrint.Parser.Components;
using SemanticXamlPrint.Parser.Layout;
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
        private delegate float ComponentRenderer(Graphics graphics, IXamlComponent component, ComponentDrawingFormatting templateFormatting, float currentX, float currentY, float maxLayoutWidth);

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

        public static float DrawComponent(this Graphics graphics, IXamlComponent component, ComponentDrawingFormatting templateFormatting, float currentX, float currentY, float maxLayoutWidth)
        {
            float boundedMaxLayoutWidth = maxLayoutWidth == 0 ? graphics.VisibleClipBounds.Width : maxLayoutWidth;
            if (component == null) return currentY;
            if (!Renderers.TryGetValue(component.Type, out ComponentRenderer renderer)) return currentY;
            return renderer(graphics, component, templateFormatting, currentX, currentY, boundedMaxLayoutWidth);
        }

        private static float RenderLineBreak(Graphics graphics, IXamlComponent component, ComponentDrawingFormatting templateFormatting, float currentX, float currentY, float maxLayoutWidth)
        {
            return currentY + 3;
        }

        private static float RenderLine(Graphics graphics, IXamlComponent component, ComponentDrawingFormatting templateFormatting, float currentX, float currentY, float maxLayoutWidth)
        {
            LineComponent lineComponent = (LineComponent)component;
            float newY = currentY + 3;
            newY += graphics.DrawlLineAndReturnHeight(lineComponent.Style.ToDashStyle(), currentX, newY, (int)maxLayoutWidth);
            return newY + 3;
        }

        private static float RenderImage(Graphics graphics, IXamlComponent component, ComponentDrawingFormatting templateFormatting, float currentX, float currentY, float maxLayoutWidth)
        {
            ImageComponent imageComponent = (ImageComponent)component;
            string imageSource = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imageComponent.Source ?? "default.png");
            if (!File.Exists(imageSource)) return currentY;
            using (Image image = Image.FromFile(imageSource))
            {
                return currentY + graphics.DrawImageCenteredAndReturnHeight(image, currentX, currentY, imageComponent.Width, imageComponent.Height, maxLayoutWidth);
            }
        }

        private static float RenderQRCode(Graphics graphics, IXamlComponent component, ComponentDrawingFormatting templateFormatting, float currentX, float currentY, float maxLayoutWidth)
        {
            QRCodeComponent qrCodeComponent = (QRCodeComponent)component;
            return currentY + graphics.DrawQRCodeCenteredAndReturnHeight(qrCodeComponent.Text, currentX, currentY, qrCodeComponent.Width, qrCodeComponent.Height, maxLayoutWidth);
        }

        private static float RenderData(Graphics graphics, IXamlComponent component, ComponentDrawingFormatting templateFormatting, float currentX, float currentY, float maxLayoutWidth)
        {
            DataComponent dataComponent = (DataComponent)component;
            ComponentDrawingFormatting format = component.GetSystemDrawingProperties(templateFormatting);
            return currentY + graphics.DrawStringAndReturnHeight(dataComponent.Text, dataComponent.TextWrap, format, currentX, currentY, (int)maxLayoutWidth);
        }

        private static float RenderCells(Graphics graphics, IXamlComponent component, ComponentDrawingFormatting templateFormatting, float currentX, float currentY, float maxLayoutWidth)
        {
            CellsComponent cellsComponent = (CellsComponent)component;
            ComponentDrawingFormatting rowFormat = component.GetSystemDrawingProperties(templateFormatting);
            List<CellComponent> dataRowCells = cellsComponent.Children?.Where(element => element.Type == typeof(CellComponent)).Select(validElement => (CellComponent)validElement).ToList();
            int additionalHeight = 0;
            foreach (CellComponent cell in dataRowCells)
            {
                ComponentDrawingFormatting cellFormat = cell.GetSystemDrawingProperties(rowFormat);
                float x = (cell.X <= 0) ? 0f : cell.X;
                float y = (cell.Y <= 0) ? currentY : cell.Y;
                float z = (cell.Z <= 0) ? (int)maxLayoutWidth : cell.Z;
                int textHeight = graphics.DrawStringAndReturnHeight(cell.Text, cell.TextWrap, cellFormat, x, y, z);
                additionalHeight = (textHeight > additionalHeight) ? textHeight : additionalHeight;
            }
            return currentY + additionalHeight;
        }

        private static float RenderGrid(Graphics graphics, IXamlComponent component, ComponentDrawingFormatting templateFormatting, float currentX, float currentY, float maxLayoutWidth)
        {
            GridComponent gridComponent = (GridComponent)component;
            ComponentDrawingFormatting gridFormat = component.GetSystemDrawingProperties(templateFormatting);
            GridLayoutPlan layoutPlan = GridLayoutPlanner.Build(gridComponent, maxLayoutWidth);

            // layout pass outputs row/column associations; paint pass draws them.
            float yBeforeGrid = currentY;
            float longestColumnY = currentY;
            foreach (GridRowLayoutPlan rowPlan in layoutPlan.Rows)
            {
                float rowStartY = longestColumnY;
                float rowCurrentX = currentX;
                ComponentDrawingFormatting rowFormat = rowPlan.Row.GetSystemDrawingProperties(gridFormat);

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

            SizeF singleLineSize = graphics.MeasureString(text, cellFmt.Font, (int)z, new StringFormat { FormatFlags = StringFormatFlags.NoWrap });
            Rectangle layout = new Rectangle((int)x, (int)y, (int)z, (int)singleLineSize.Height);
            graphics.DrawString(text, cellFmt.Font, cellFmt.Brush, layout, cellFmt.StringFormat);
            return layout.Height;
        }

        public static List<int> GetDivideColumnWidths(this Graphics graphics, string pattern, float maxLayoutWith)
        {
            return ColumnWidthCalculator.Calculate(pattern, maxLayoutWith);
        }

        public static List<int> GetDivideColumnWidths(this Graphics graphics, int columns, float maxLayoutWith)
        {
            return ColumnWidthCalculator.Calculate(columns, maxLayoutWith);
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
            float newWidth = Math.Min(image.Width, maxWidth > 0 ? maxWidth : image.Width);
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
