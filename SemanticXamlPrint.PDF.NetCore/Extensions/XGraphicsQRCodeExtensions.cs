using PdfSharpCore.Drawing;

namespace SemanticXamlPrint.PDF.NetCore
{
    internal static class XGraphicsQRCodeExtensions
    {
        public static int DrawQRCodeCenteredAndReturnHeight(this XGraphics graphics, string text, float x, float y, float maxWidth, float maxHeight, float maxLayoutWith)
        {
            return graphics.DrawStringAndReturnHeight("QR CODE is not supported on .NET Core", true, XDefaults.Formatting, x, y, maxLayoutWith);
        }
    }
}
