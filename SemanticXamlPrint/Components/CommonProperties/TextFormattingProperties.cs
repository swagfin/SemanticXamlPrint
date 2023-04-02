using System;

namespace SemanticXamlPrint.Components.CommonProperties
{
    public abstract class TextFormattingProperties
    {
        public string Font { get; set; } = "Calibri";
        public string FontStyle { get; set; } = "Regular";
        public int FontSize { get; set; } = 12;

        public bool SetStylingProperties(string propertyName, string value)
        {
            try
            {
                switch (propertyName)
                {
                    case "font":
                        Font = value;
                        break;
                    case "fontstyle":
                        FontStyle = value;
                        break;
                    case "fontsize":
                        FontSize = Convert.ToInt32(value);
                        break;
                    default:
                        return false;
                }
                return true;
            }
            catch { return false; }
        }
    }
}
