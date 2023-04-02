using SemanticXamlPrint.Components;
using System;
using System.Drawing;
using System.Drawing.Printing;
namespace SemanticXamlPrint.Demo
{
    public class DefaultPrintService
    {
        private TemplateComponent Template;
        private PrintDocument printDocument;
        private readonly int DefaultPaperWidth = 270;
        private readonly int DefaultLineHeight = 14;

        public Font DefaultFont { get; private set; }
        private int CurrentLinePosition { get; set; } = 20;

        public void Print(IXamlComponent xamlComponent, string printerName = null)
        {
            if (xamlComponent == null) return;
            if (xamlComponent.Type != typeof(TemplateComponent)) throw new Exception($"Root Component must be that of a [{nameof(Components.TemplateComponent)}] but currently is: [{Template.Name}]");
            this.Template = (TemplateComponent)xamlComponent;
            //Print Config
            this.printDocument = new PrintDocument();
            printDocument.PrintPage += PrintTemplatePage;
            if (!string.IsNullOrWhiteSpace(printerName))
                printDocument.PrinterSettings.PrinterName = printerName;
            this.printDocument.Print();
        }

        private void PrintTemplatePage(object sender, PrintPageEventArgs e)
        {
            this.DefaultFont = new Font(Template.Font, Template.FontSize, FontStyle.Bold);
            this.CurrentLinePosition = 20;
            foreach (IXamlComponent template in Template?.Children)
            {
                DrawComponent(template, e);
            }
        }

        private void DrawComponent(IXamlComponent component, PrintPageEventArgs e)
        {
            if (component.Type == typeof(LineComponent))
            {
                e.Graphics.DrawString("---------------------------------------------------------", DefaultFont, Brushes.Black, 0f, CurrentLinePosition);
                CurrentLinePosition += DefaultLineHeight;
            }
            else if (component.Type == typeof(DataComponent))
            {
                DataComponent dataTemplate = (DataComponent)component;

                Rectangle r = new Rectangle(0, CurrentLinePosition, DefaultPaperWidth, DefaultLineHeight);
                e.Graphics.DrawString(dataTemplate.Text, DefaultFont, Brushes.Black, r, StringAlign.Center);
                CurrentLinePosition += DefaultLineHeight;
            }

            //#Eventually Also Children
            foreach (IXamlComponent template in component?.Children)
            {
                DrawComponent(template, e);
            }
        }
    }
}
