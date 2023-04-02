using SemanticXamlPrint.Components;
using SemanticXamlPrint.Demo.SystemDrawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
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
            if (this.Template.MaxWidth <= 0) this.Template.MaxWidth = 300;
            if (this.Template.LineSpacing <= 0) this.Template.LineSpacing = 2;
            if (this.Template.MarginTop <= 0) this.Template.MarginTop = 2;
            //Set Star
            //Print Config
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
            List<IXamlComponent> templateComponents = Template?.Children;
            for (int i = 0; i < templateComponents.Count; i++)
            {
                DrawComponent(templateComponents[i], e);
            }
        }

        private void DrawComponent(IXamlComponent component, PrintPageEventArgs e)
        {
            //Get Styling for Component
            ComponentDrawingFormatting fmt = component.GetSystemDrawingProperties(this.TemplateFormatting);
            if (component.Type == typeof(LineComponent))
            {
                e.Graphics.DrawString("---------------------------------------------------------", fmt.Font, fmt.Brush, 0f, CurrentLinePosition);
                CurrentLinePosition += Template.LineSpacing;
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
            //#Eventually Also Children
            for (int i = 0; i < component?.Children?.Count; i++)
            {
                DrawComponent(component.Children[i], e);
            }
        }

    }
}
