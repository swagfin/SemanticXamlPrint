using PdfSharp.Drawing;
using PdfSharp.Pdf;
using SemanticXamlPrint.Parser;
using SemanticXamlPrint.Parser.Components;
using SemanticXamlPrint.PDF;
using System;
using System.Drawing.Printing;
using System.IO;
namespace SemanticXamlPrint.Demo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("TESTING XAML PRINT");
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
            try
            {
                //Get Template Contents
                byte[] xamlFileBytes = File.ReadAllBytes("custom.excessgrid.template");
                //Use Default Parser 
                IXamlComponent xamlComponent = DefaultXamlParser.Parse(xamlFileBytes);

                //####  SYSTEM DRAWING #####
                PrintDocument printDocument = new PrintDocument();
                printDocument.PrintPage += (obj, eventAgs) =>
                {
                    //Use Xaml Draw Extension to Print
                    eventAgs.DrawXamlComponent(xamlComponent);
                };
                printDocument.Print();
                //####  SYSTEM DRAWING #####


                //####  PDF SHARP #####
                using (PdfDocument document = new PdfDocument())
                {
                    // Add a page to the document
                    PdfPage page = document.AddPage();
                    // Create a graphics object for the page
                    XGraphics xgraphics = XGraphics.FromPdfPage(page);
                    //Use Xaml Draw Extension to Generate PDF
                    xgraphics.DrawXamlComponent(xamlComponent);
                    // Save the PDF document to a file
                    document.Save("output.pdf");
                }
                //####  PDF SHARP #####
            }
            catch (Exception ex)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message.ToString());
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.White;
            }

            Console.ReadLine();
        }

    }
}
