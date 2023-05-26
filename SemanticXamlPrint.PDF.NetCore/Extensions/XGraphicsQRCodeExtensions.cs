using PdfSharpCore.Drawing;
using QRCoder;
using System;
using System.IO;

namespace SemanticXamlPrint.PDF.NetCore
{
    internal static class XGraphicsQRCodeExtensions
    {
        public static int DrawQRCodeCenteredAndReturnHeight(this XGraphics graphics, string text, float x, float y, float maxWidth, float maxHeight, float maxLayoutWith)
        {
            //Generate QR Code
            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(text ?? "unspecified", QRCodeGenerator.ECCLevel.Q))
            using (QRCode qrCode = new QRCode(qrCodeData))
            {
                using (MemoryStream stream = new MemoryStream())
                using (SixLabors.ImageSharp.Image qrCodeImage = qrCode.GetGraphic(10))
                {
                    qrCodeImage.Save(stream, new SixLabors.ImageSharp.Formats.Png.PngEncoder());
                    byte[] streamBytes = stream.ToArray();
                    float newWidth = Math.Min(qrCodeImage.Height, maxWidth > 0 ? maxWidth : qrCodeImage.Width);
                    float newHeight = Math.Min(qrCodeImage.Height, maxHeight > 0 ? maxHeight : qrCodeImage.Height);
                    float centeredX = x + (maxLayoutWith - newWidth) / 2;
                    graphics.DrawImage(XImage.FromStream(() => { return new MemoryStream(streamBytes); }), centeredX > 0 ? centeredX : x, y, newWidth, newHeight);
                    return (int)newHeight;
                }
            }
        }
    }
}
