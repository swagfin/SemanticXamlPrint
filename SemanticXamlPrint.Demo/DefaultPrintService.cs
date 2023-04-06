using SemanticXamlPrint.Components;
using SemanticXamlPrint.Demo.Extensions;
using SemanticXamlPrint.Demo.SystemDrawing;
using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;

namespace SemanticXamlPrint.Demo
{
    public class DefaultPrintService
    {
        private TemplateComponent Template;
        private PrintDocument printDocument;
        private int CurrentLineY { get; set; }
        public ComponentDrawingFormatting TemplateFormatting { get; set; }

        public void Print(IXamlComponent xamlComponent, string printerName = null)
        {
            if (xamlComponent == null) return;
            if (xamlComponent.Type != typeof(TemplateComponent)) throw new Exception($"Root Component must be that of a [{nameof(TemplateComponent)}] but currently is: [{Template.Name}]");
            this.Template = (TemplateComponent)xamlComponent;
            this.TemplateFormatting = this.Template.GetSystemDrawingProperties(Defaults.Formatting) ?? throw new Exception("Default template properties are missing");
            if (this.Template.MaxWidth <= 50) this.Template.MaxWidth = 290;
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
            CurrentLineY = this.Template.MarginTop;
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
                CurrentLineY += Template.LineSpacing;
            }
            else if (component.Type == typeof(LineComponent))
            {
                LineComponent lineComponent = (LineComponent)component;
                string lineContent = new string(string.IsNullOrEmpty(lineComponent.Style) ? '-' : lineComponent.Style[0], (int)(e.Graphics.VisibleClipBounds.Width / 2));
                CurrentLineY += e.Graphics.DrawStringAndReturnHeight(lineContent, false, fmt, 0, CurrentLineY, (int)e.Graphics.VisibleClipBounds.Width);
            }
            else if (component.Type == typeof(DataComponent))
            {
                DataComponent dataComponent = (DataComponent)component;
                CurrentLineY += e.Graphics.DrawStringAndReturnHeight(dataComponent.Text, dataComponent.TextWrap, fmt, 0, CurrentLineY, (int)e.Graphics.VisibleClipBounds.Width);
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
                    float y = (cell.Y <= 0) ? CurrentLineY : cell.Y;
                    float z = (cell.Z <= 0) ? (int)e.Graphics.VisibleClipBounds.Width : cell.Z;
                    //Write String 
                    int textHeight = e.Graphics.DrawStringAndReturnHeight(cell.Text, cell.TextWrap, cellFmt, x, y, z);
                    additionalHeight = (textHeight > additionalHeight) ? textHeight : additionalHeight;
                }
                //Add Line Height
                CurrentLineY += additionalHeight;
            }
            else if (component.Type == typeof(GridComponent))
            {
                GridComponent gridComponent = (GridComponent)component;
                List<DataComponent> gridChildren = gridComponent.Children?.Where(element => element.Type == typeof(DataComponent))
                                                                                .Select(validElement => (DataComponent)validElement)
                                                                                .ToList();
                //Divide Column Widths
                List<int> columnWidths = e.Graphics.GetDivideColumnWidths(gridComponent.ColumnWidths, gridComponent.Columns);
                //Calculate Even Column Width
                int additionalHeight = 0;
                float lastXPosition = 0;
                for (int columnIndex = 0; columnIndex < gridComponent.Columns; columnIndex++)
                {
                    List<DataComponent> columnChildrens = gridChildren?.Where(child => child.CustomProperties.IsPropertyExistsWithValue("grid.column", columnIndex.ToString()))?.ToList();
                    foreach (DataComponent dataComponent in columnChildrens)
                    {
                        ComponentDrawingFormatting childFmt = dataComponent.GetSystemDrawingProperties(fmt);
                        int textHeight = e.Graphics.DrawStringAndReturnHeight(dataComponent.Text, dataComponent.TextWrap, childFmt, lastXPosition, CurrentLineY, columnWidths[columnIndex]);
                        additionalHeight = (textHeight > additionalHeight) ? textHeight : additionalHeight;
                    }
                    lastXPosition += columnWidths[columnIndex];
                }
                CurrentLineY += additionalHeight;
            }
            else
            {
                return;
            }
        }
    }
}
