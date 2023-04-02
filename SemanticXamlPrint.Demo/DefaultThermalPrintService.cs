using SemanticXamlPrint.Components;
using SemanticXamlPrint.Demo.SystemDrawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
namespace SemanticXamlPrint.Demo
{
    public class DefaultThermalPrintService
    {
        private TemplateComponent Template;
        private PrintDocument printDocument;
        private readonly int DefaultPaperWidth = 270;
        private readonly int DefaultLineHeight = 14;
        private int CurrentLinePosition { get; set; } = 20;
        public ComponentDrawingFormatting TemplateFormatting { get; set; }

        public void Print(IXamlComponent xamlComponent, string printerName = null)
        {
            if (xamlComponent == null) return;
            if (xamlComponent.Type != typeof(TemplateComponent)) throw new Exception($"Root Component must be that of a [{nameof(TemplateComponent)}] but currently is: [{Template.Name}]");
            this.Template = (TemplateComponent)xamlComponent;
            this.TemplateFormatting = this.Template.GetSystemDrawingProperties(Defaults.Formatting) ?? throw new Exception("Default template properties are missing");
            //Print Config
            this.printDocument = new PrintDocument();
            printDocument.PrintPage += PrintTemplatePage;
            if (!string.IsNullOrWhiteSpace(printerName))
                printDocument.PrinterSettings.PrinterName = printerName;
            this.printDocument.Print();
        }

        private void PrintTemplatePage(object sender, PrintPageEventArgs e)
        {
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
                CurrentLinePosition += DefaultLineHeight;
            }
            else if (component.Type == typeof(DataComponent))
            {
                DataComponent dataTemplate = (DataComponent)component;

                Rectangle r = new Rectangle(0, CurrentLinePosition, DefaultPaperWidth, DefaultLineHeight);
                e.Graphics.DrawString(dataTemplate.Text, fmt.Font, fmt.Brush, r, fmt.StringFormat);
                CurrentLinePosition += DefaultLineHeight;
            }
            //#Eventually Also Children
            for (int i = 0; i < component?.Children?.Count; i++)
            {
                DrawComponent(component.Children[i], e);
            }
        }

    }
}
