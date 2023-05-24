using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System;

namespace SemanticXamlPrint.PDF.Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Printing PDF");

            PdfDocument document = new PdfDocument();

            // Add a page to the document
            PdfPage page = document.AddPage();

            // Create a graphics object for the page
            XGraphics gfx = XGraphics.FromPdfPage(page);

            // Create a font
            XFont font = new XFont("Arial", 12, XFontStyle.Regular);

            // Create a brush for text color
            XBrush brush = XBrushes.Black;

            // Set the position and size for drawing the text
            double x = 50;
            double y = 50;
            double z = 200;

            // Example text
            string text = "Hello, World!";

            // Draw the text on the page using the extension method
            gfx.DrawStringAndReturnHeight(text, true, new ComponentXDrawingFormatting { Font = font, Brush = brush }, x, y, z);

            // Save the PDF document to a file
            document.Save("output.pdf");

            // Close the PDF document
            document.Close();


            Console.ReadLine();
        }
    }
}
