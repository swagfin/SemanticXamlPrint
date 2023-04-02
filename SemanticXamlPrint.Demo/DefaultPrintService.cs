using SemanticXamlPrint.Components;
using SemanticXamlPrint.Demo.SystemDrawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;

namespace SemanticXamlPrint.Demo
{
    public class DefaultPrintService
    {
        private TemplateComponent Template;
        private PrintDocument printDocument;
        private int CurrentLinePosition { get; set; }
        public ComponentDrawingFormatting TemplateFormatting { get; set; }

        public void Print(IXamlComponent xamlComponent, string printerName = null)
        {
            if (xamlComponent == null) return;
            if (xamlComponent.Type != typeof(TemplateComponent)) throw new Exception($"Root Component must be that of a [{nameof(TemplateComponent)}] but currently is: [{Template.Name}]");
            this.Template = (TemplateComponent)xamlComponent;
            this.TemplateFormatting = this.Template.GetSystemDrawingProperties(Defaults.Formatting) ?? throw new Exception("Default template properties are missing");
            if (this.Template.MaxWidth <= 50) this.Template.MaxWidth = 290;//Default Thermal Size
            if (this.Template.LineSpacing < 0) this.Template.LineSpacing = 2;
            if (this.Template.MarginTop < 0) this.Template.MarginTop = 20;
            //Set Star
            this.printDocument = new PrintDocument();
            printDocument.PrintPage += PrintTemplatePage;
            if (!string.IsNullOrWhiteSpace(printerName))
                printDocument.PrinterSettings.PrinterName = printerName;
            this.printDocument.Print();
        }

        private void PrintTemplatePage(object sender, PrintPageEventArgs e)
        {
            //Set Starting Poistition
            CurrentLinePosition = this.Template.MarginTop;
            //Skip Drawing Template and Instead Draw Template' Childres
            DrawComponents(Template?.Children, e);
        }
        private void DrawComponents(List<IXamlComponent> components, PrintPageEventArgs e)
        {
            for (int i = 0; i < components?.Count; i++)
                DrawComponent(components[i], e);
        }
        private void DrawComponent(IXamlComponent component, PrintPageEventArgs e)
        {
            //Get Styling for Component
            ComponentDrawingFormatting fmt = component.GetSystemDrawingProperties(this.TemplateFormatting);
            if (component.Type == typeof(LineBreakComponent))
            {
                CurrentLinePosition += Template.LineSpacing;
            }
            else if (component.Type == typeof(LineComponent))
            {
                LineComponent lineComponent = (LineComponent)component;
                e.Graphics.DrawString(new string(string.IsNullOrEmpty(lineComponent.Style) ? '-' : lineComponent.Style[0], this.Template.MaxWidth), fmt.Font, fmt.Brush, 0f, CurrentLinePosition);
                CurrentLinePosition += (int)(fmt.Font.Size + this.Template.LineSpacing);
            }
            else if (component.Type == typeof(DataComponent))
            {
                DataComponent dataComponent = (DataComponent)component;
                if (dataComponent.TextWrap && (int)e.Graphics.MeasureString(dataComponent.Text, fmt.Font).Width > this.Template.MaxWidth)
                {
                    SizeF size = e.Graphics.MeasureString(dataComponent.Text, fmt.Font, this.Template.MaxWidth);
                    RectangleF layoutF = new RectangleF(new PointF(0, CurrentLinePosition), size);
                    e.Graphics.DrawString(dataComponent.Text, fmt.Font, fmt.Brush, layoutF, fmt.StringFormat);
                    CurrentLinePosition += (int)layoutF.Height;
                }
                else
                {
                    Rectangle layout = new Rectangle(0, CurrentLinePosition, this.Template.MaxWidth, (int)fmt.Font.Size + this.Template.LineSpacing);
                    e.Graphics.DrawString(dataComponent.Text, fmt.Font, fmt.Brush, layout, fmt.StringFormat);
                    CurrentLinePosition += layout.Height + this.Template.LineSpacing;
                }
            }
            else if (component.Type == typeof(DataRowComponent))
            {
                DataRowComponent dataRowComponent = (DataRowComponent)component;
                //Get all Children of DataRowCells
                List<DataRowCellComponent> dataRowCells = dataRowComponent.Children?.Where(element => element.Type == typeof(DataRowCellComponent))
                                                                               .Select(validElement => (DataRowCellComponent)validElement)
                                                                               .ToList();
                int additionalHeight = 0;
                foreach (DataRowCellComponent cell in dataRowCells)
                {
                    ComponentDrawingFormatting cellFmt = cell.GetSystemDrawingProperties(fmt);
                    //Set RowCell Location
                    float x = (cell.X <= 0) ? 0f : cell.X;
                    float y = (cell.Y <= 0) ? CurrentLinePosition : cell.Y;
                    float z = (cell.Z <= 0) ? this.Template.MaxWidth : cell.Z;
                    //Determine Wrap
                    //# e.Graphics.DrawString("Item Description", font2, Brushes.Black, 0f, (float)currentLinePosition);
                    if (cell.TextWrap && (int)e.Graphics.MeasureString(cell.Text, cellFmt.Font).Width > z)
                    {
                        SizeF size = e.Graphics.MeasureString(cell.Text, cellFmt.Font, (int)z);
                        RectangleF layoutF = new RectangleF(new PointF(x, y), size);
                        e.Graphics.DrawString(cell.Text, cellFmt.Font, cellFmt.Brush, layoutF, cellFmt.StringFormat);
                        additionalHeight = ((int)layoutF.Height > additionalHeight) ? (int)layoutF.Height : additionalHeight;
                    }
                    else
                    {
                        Rectangle layout = new Rectangle((int)x, (int)y, (int)z, (int)cellFmt.Font.Size + this.Template.LineSpacing);
                        e.Graphics.DrawString(cell.Text, cellFmt.Font, cellFmt.Brush, layout, cellFmt.StringFormat);
                        additionalHeight = ((layout.Height + this.Template.LineSpacing) > additionalHeight) ? (layout.Height + this.Template.LineSpacing) : additionalHeight;
                    }
                }
                //Add Line Height
                CurrentLinePosition += additionalHeight + this.Template.LineSpacing;

            }
            else if (component.Type == typeof(UniformDataGridComponent))
            {
                UniformDataGridComponent gridComponent = (UniformDataGridComponent)component;
                List<DataComponent> gridChildren = gridComponent.Children?.Where(element => element.Type == typeof(DataComponent))
                                                                                .Select(validElement => (DataComponent)validElement)
                                                                                .ToList();
                //Calculate Even Column Width
                float columnWidth = e.Graphics.VisibleClipBounds.Width / gridComponent.Columns;
                int additionalHeight = 0;
                for (int columnIndex = 0; columnIndex < gridComponent.Columns; columnIndex++)
                {
                    // Calculate the x-coordinate of the starting point of the column
                    float x = columnIndex * columnWidth;
                    //Get All Items on Row
                    int currentGridLineNo = CurrentLinePosition;
                    //Get Only Childrens for Column || Object Memory Manipulation
                    List<DataComponent> columnChildrens = gridChildren?.Where(child => ((child.CustomProperties.TryGetValue("grid.column", out string valIndex) && int.TryParse(valIndex, out int setIndex)) ? setIndex : 0) == columnIndex)?.ToList();
                    foreach (DataComponent columnChild in columnChildrens)
                    {
                        ComponentDrawingFormatting childFmt = columnChild.GetSystemDrawingProperties(fmt);
                        Rectangle layout = new Rectangle((int)x, currentGridLineNo, (int)columnWidth, (int)childFmt.Font.Size + this.Template.LineSpacing);
                        e.Graphics.DrawString(columnChild.Text, childFmt.Font, childFmt.Brush, layout, childFmt.StringFormat);
                        currentGridLineNo += layout.Height + this.Template.LineSpacing;
                        //Update
                        additionalHeight = ((layout.Height + this.Template.LineSpacing) > additionalHeight) ? (layout.Height + this.Template.LineSpacing) : additionalHeight;
                    }
                    //Update Last Max Height
                }
                CurrentLinePosition += additionalHeight + Template.LineSpacing;
            }
            else
            {
                return;
            }
        }



    }
}
