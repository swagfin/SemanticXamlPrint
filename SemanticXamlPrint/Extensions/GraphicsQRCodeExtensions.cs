using QRCoder;
using System;
using System.Drawing;

namespace SemanticXamlPrint
{
    internal static class GraphicsQRCodeExtensions
    {
        public static int DrawQRCodeCenteredAndReturnHeight(this Graphics graphics, string text, float x, float y, float maxWidth, float maxHeight, float maxLayoutWith)
        {
            //Generate QR Code
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(text ?? "unspecified", QRCodeGenerator.ECCLevel.Q))
            using (QRCode qrCode = new QRCode(qrCodeData))
            {
                Bitmap qrCodeImage = qrCode.GetGraphic(10);
                //Draw Image
                float newWidth = Math.Min(qrCodeImage.Height, maxWidth > 0 ? maxWidth : qrCodeImage.Width);
                float newHeight = Math.Min(qrCodeImage.Height, maxHeight > 0 ? maxHeight : qrCodeImage.Height);
                float centeredX = x + (maxLayoutWith - newWidth) / 2;
                graphics.DrawImage(qrCodeImage, centeredX > 0 ? centeredX : x, y, newWidth, newHeight);
                return (int)newHeight;
            }
        }
    }
}
