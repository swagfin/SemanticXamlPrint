using SemanticXamlPrint.Parser.Components;
using SemanticXamlPrint.Parser.Layout;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;

namespace SemanticXamlPrint
{
    public static class DefaultXamlGraphics
    {
        private static readonly Dictionary<IXamlComponent, PrintJobState> PrintStates = new Dictionary<IXamlComponent, PrintJobState>();

        private class PrintJobState
        {
            public int CurrentChildIndex { get; set; }
            public bool RequestedMorePage { get; set; }
        }

        private static PrintJobState GetPrintState(IXamlComponent xamlComponent)
        {
            if (xamlComponent == null) return new PrintJobState();
            if (PrintStates.TryGetValue(xamlComponent, out PrintJobState state)) return state;
            PrintJobState newState = new PrintJobState();
            PrintStates[xamlComponent] = newState;
            return newState;
        }

        private static void RemovePrintState(IXamlComponent xamlComponent)
        {
            if (xamlComponent != null)
            {
                _ = PrintStates.Remove(xamlComponent);
            }
        }

        public static float DrawXamlComponent(this PrintPageEventArgs eventArgs, IXamlComponent xamlComponent, float yPositionDraw = 0)
        {
            if (xamlComponent == null) return yPositionDraw;
            if (xamlComponent.Type != typeof(TemplateComponent)) throw new Exception($"Root Component must be that of a [{nameof(TemplateComponent)}] but currently is: [{xamlComponent.Name}]");
            TemplateComponent template = (TemplateComponent)xamlComponent;
            ComponentDrawingFormatting TemplateFormatting = template.GetSystemDrawingProperties(Defaults.Formatting) ?? throw new Exception("Default template properties are missing");
            float _currentLineY = yPositionDraw + (float)template.MarginTop;
            double pageHeight = eventArgs.PageSettings.PaperSize.Height - template.MarginBottom;
            RenderLayoutContext layoutContext = new RenderLayoutContext
            {
                PageBottomY = (float)pageHeight
            };
            PrintJobState state = GetPrintState(xamlComponent);
            //Draw Root Component Children
            state.CurrentChildIndex = state.RequestedMorePage ? state.CurrentChildIndex : 0;
            for (int i = state.CurrentChildIndex; i < template?.Children?.Count; i++)
            {
                if (_currentLineY > pageHeight)
                {
                    eventArgs.HasMorePages = true;
                    state.RequestedMorePage = true;
                    return yPositionDraw;
                }
                else
                {
                    eventArgs.HasMorePages = false;
                    state.RequestedMorePage = false;
                }
                _currentLineY = eventArgs.Graphics.DrawComponent(template?.Children[i], TemplateFormatting, template.MarginLeft, _currentLineY, eventArgs.Graphics.VisibleClipBounds.Width - (template.MarginLeft + template.MarginRight), layoutContext);
                state.CurrentChildIndex++;
            }
            RemovePrintState(xamlComponent);
            return _currentLineY;
        }

        [Obsolete("This method is obsolete, please use DrawXamlComponent an Extension of: PrintPageEventArgs")]
        public static float DrawXamlComponent(this Graphics graphics, IXamlComponent xamlComponent, float yPositionDraw = 0) => throw new Exception("method is obsolete, please use DrawXamlComponent an Extension of: PrintPageEventArgs");
    }
}
