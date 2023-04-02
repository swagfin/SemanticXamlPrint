using System;

namespace SemanticXamlPrint.Components
{
    public abstract class XamlComponentCommonProperties
    {
        public string Font { get; set; } = null;
        public string FontStyle { get; set; } = null;
        public int FontSize { get; set; } = 0;
        public string Align { get; set; } = null;

        public bool SetCommonProperties(string propertyName, string value)
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
                    case "align":
                        Align = value;
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
