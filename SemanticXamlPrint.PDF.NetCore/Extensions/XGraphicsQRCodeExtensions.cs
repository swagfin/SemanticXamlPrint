using PdfSharpCore.Drawing;
using System;
using ZXing;
using ZXing.QrCode;
using ZXing.QrCode.Internal;

namespace SemanticXamlPrint.PDF.NetCore
{
    internal static class XGraphicsQRCodeExtensions
    {
        public static int DrawQRCodeCenteredAndReturnHeight(this XGraphics graphics, string text, float x, float y, float maxWidth, float maxHeight, float maxLayoutWith)
        {
            // Generate QR code image using ZXing.Net
            BarcodeWriter<XBitmapImage> writer = new BarcodeWriter<XBitmapImage>
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    ErrorCorrection = ErrorCorrectionLevel.Q,
                    Width = 10,
                    Height = 10
                }
            };
            XBitmapImage qrCodeImage = writer.Write(text);
            float newWidth = Math.Min(qrCodeImage.PixelWidth, maxWidth > 0 ? maxWidth : qrCodeImage.PixelWidth);
            float newHeight = Math.Min(qrCodeImage.PixelHeight, maxHeight > 0 ? maxHeight : qrCodeImage.PixelHeight);
            float centeredX = x + (maxLayoutWith - newWidth) / 2;
            graphics.DrawImage(qrCodeImage, centeredX > 0 ? centeredX : x, y, newWidth, newHeight);
            return (int)newHeight;
        }
    }
}
