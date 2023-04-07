using SemanticXamlPrint.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace SemanticXamlPrint
{
    public static class DefaultXamlGraphics
    {
        private static TemplateComponent Template;
        private static int CurrentLineY { get; set; }
        public static ComponentDrawingFormatting TemplateFormatting { get; set; }

        public static void Draw(this Graphics graphics, IXamlComponent xamlComponent)
        {
            if (xamlComponent == null) return;
            if (xamlComponent.Type != typeof(TemplateComponent)) throw new Exception($"Root Component must be that of a [{nameof(TemplateComponent)}] but currently is: [{Template.Name}]");
            Template = (TemplateComponent)xamlComponent;
            TemplateFormatting = Template.GetSystemDrawingProperties(Defaults.Formatting) ?? throw new Exception("Default template properties are missing");
            if (Template.MaxWidth <= 50) Template.MaxWidth = 290;
            if (Template.MarginTop < 0) Template.MarginTop = 20;
        }

        private static void PrintTemplatePage(object sender, Graphics graphics)
        {
            CurrentLineY = Template.MarginTop;
            DrawComponents(Template?.Children, graphics);
        }
        private static void DrawComponents(List<IXamlComponent> components, Graphics graphics)
        {
            for (int i = 0; i < components?.Count; i++)
                DrawComponent(components[i], graphics);
        }
        private static void DrawComponent(IXamlComponent component, Graphics graphics)
        {
            if (component.Type == typeof(LineBreakComponent))
            {
                CurrentLineY += 3;
            }
            else if (component.Type == typeof(LineComponent))
            {
                LineComponent lineComponent = (LineComponent)component;
                CurrentLineY += 3;
                CurrentLineY += graphics.DrawlLineAndReturnHeight(lineComponent.Style.ToDashStyle(), 0, CurrentLineY, (int)graphics.VisibleClipBounds.Width);
                CurrentLineY += 3;
            }
            else if (component.Type == typeof(ImageComponent))
            {
                ImageComponent imageComponent = (ImageComponent)component;
                string imageSource = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, imageComponent.Source ?? "default.png");
                if (File.Exists(imageSource))
                {
                    CurrentLineY += graphics.DrawImageCenteredAndReturnHeight(Image.FromFile(imageSource), 0, CurrentLineY, imageComponent.Width, imageComponent.Height);
                }
            }
            else if (component.Type == typeof(DataComponent))
            {
                DataComponent dataComponent = (DataComponent)component;
                ComponentDrawingFormatting fmt = component.GetSystemDrawingProperties(TemplateFormatting);
                //Draw Data Component
                CurrentLineY += graphics.DrawStringAndReturnHeight(dataComponent.Text, dataComponent.TextWrap, fmt, 0, CurrentLineY, (int)graphics.VisibleClipBounds.Width);
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
                    float y = (cell.Y <= 0) ? CurrentLineY : cell.Y;
                    float z = (cell.Z <= 0) ? (int)graphics.VisibleClipBounds.Width : cell.Z;
                    //Write String 
                    int textHeight = graphics.DrawStringAndReturnHeight(cell.Text, cell.TextWrap, cellFmt, x, y, z);
                    additionalHeight = (textHeight > additionalHeight) ? textHeight : additionalHeight;
                }
                //Add Line Height
                CurrentLineY += additionalHeight;
            }
            else if (component.Type == typeof(GridComponent))
            {
                GridComponent gridComponent = (GridComponent)component;
                ComponentDrawingFormatting gridfmt = component.GetSystemDrawingProperties(TemplateFormatting);
                List<int> columnWidths = graphics.GetDivideColumnWidths(gridComponent.ColumnWidths);
                int y_before_grid = CurrentLineY;
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
                            int textHeight = graphics.DrawStringAndReturnHeight(columnComponent.Text, columnComponent.TextWrap, childFmt, lastXPosition, CurrentLineY, columnWidths[colIndex]);
                            additionalHeight = (textHeight > additionalHeight) ? textHeight : additionalHeight;
                            lastXPosition += columnWidths[colIndex];
                        }
                    }
                    CurrentLineY += additionalHeight;
                }
                //#Check if Drawing Border
                if (!string.IsNullOrEmpty(gridComponent.BorderStyle))
                {
                    graphics.DrawRectangleAndReturnHeight(gridComponent.BorderStyle.ToDashStyle(), 0, y_before_grid, (int)graphics.VisibleClipBounds.Width, CurrentLineY - y_before_grid);
                    lastXPosition = 0;
                    for (int colIndex = 0; colIndex < columnWidths.Count; colIndex++)
                    {
                        graphics.DrawRectangleAndReturnHeight(gridComponent.BorderStyle.ToDashStyle(), lastXPosition, y_before_grid, columnWidths[colIndex], CurrentLineY - y_before_grid);
                        lastXPosition += columnWidths[colIndex];
                    }
                }
            }
            else
            {
                return;
            }
        }
    }
}
