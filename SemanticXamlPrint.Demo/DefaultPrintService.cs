﻿using SemanticXamlPrint.Components;
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
                //  e.Graphics.DrawString("---------------------------------------------------------", fmt.Font, fmt.Brush, 0f, CurrentLinePosition);
                e.Graphics.DrawString(new string(string.IsNullOrEmpty(lineComponent.Style) ? '-' : lineComponent.Style[0], this.Template.MaxWidth), fmt.Font, fmt.Brush, 0f, CurrentLinePosition);
                CurrentLinePosition += (int)(fmt.Font.Size + this.Template.LineSpacing);
            }
            else if (component.Type == typeof(DataComponent))
            {
                DataComponent dataTemplate = (DataComponent)component;
                if (dataTemplate.TextWrap && (int)e.Graphics.MeasureString(dataTemplate.Text, fmt.Font).Width > this.Template.MaxWidth)
                {
                    SizeF size = e.Graphics.MeasureString(dataTemplate.Text, fmt.Font, this.Template.MaxWidth);
                    RectangleF layoutF = new RectangleF(new PointF(0, CurrentLinePosition), size);
                    e.Graphics.DrawString(dataTemplate.Text, fmt.Font, fmt.Brush, layoutF, fmt.StringFormat);
                    CurrentLinePosition += (int)layoutF.Height;
                }
                else
                {
                    Rectangle layout = new Rectangle(0, CurrentLinePosition, this.Template.MaxWidth, (int)fmt.Font.Size + this.Template.LineSpacing);
                    e.Graphics.DrawString(dataTemplate.Text, fmt.Font, fmt.Brush, layout, fmt.StringFormat);
                    CurrentLinePosition += layout.Height + this.Template.LineSpacing;
                }
            }
            else if (component.Type == typeof(GridComponent))
            {
                GridComponent gridComponent = (GridComponent)component;
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
                        ComponentDrawingFormatting childFmt = columnChild.GetSystemDrawingProperties(this.TemplateFormatting);
                        Rectangle layout = new Rectangle((int)x, currentGridLineNo, (int)columnWidth, (int)childFmt.Font.Size + this.Template.LineSpacing);
                        e.Graphics.DrawString(columnChild.Text, childFmt.Font, childFmt.Brush, layout, childFmt.StringFormat);
                        currentGridLineNo += layout.Height + this.Template.LineSpacing;
                        //Update
                        additionalHeight = ((layout.Height + this.Template.LineSpacing) > additionalHeight) ? (layout.Height + this.Template.LineSpacing) : additionalHeight;
                    }
                    //Update Last Max Height
                }
                CurrentLinePosition += additionalHeight + Template.LineSpacing;
                //  DrawComponents(component.Children, e);
            }
            else
            {
                return;
            }
        }



    }
}
