using System;
using System.IO;
using ImageMagick;

namespace PdfToImages
{
    class Program
    {
        static void Main(string[] args)
        {
            // Check if the user provided the PDF file path
            if (args.Length == 0)
            {
                Console.WriteLine("Usage: PdfToImages <path to PDF>");
                return;
            }

            string pdfPath = args[0];

            // Verify if the file exists
            if (!File.Exists(pdfPath))
            {
                Console.WriteLine("PDF file does not exist.");
                return;
            }

            try
            {
                // Convert the PDF to images
                ConvertPdfToImages(pdfPath, 300);
                Console.WriteLine("PDF successfully converted to images.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// Converts a PDF document into a series of images.
        /// </summary>
        /// <param name="pdfPath">The path to the PDF file.</param>
        /// <param name="dpi">The resolution (DPI) of the output images.</param>
        private static void ConvertPdfToImages(string pdfPath, int dpi)
        {
            // Load the PDF document
            using (var pdfDocument = new MagickImageCollection())
            {
                // Read the PDF into the collection (each page becomes an image)
                pdfDocument.Read(pdfPath, new MagickReadSettings()
                {
                    Density = new Density(dpi) // Set the DPI for the images
                });

                // Loop through each page of the PDF
                for (int i = 0; i < pdfDocument.Count; i++)
                {
                    // Get the page as an image
                    using (var image = pdfDocument[i])
                    {
                        // Create a file name for the image
                        string imageFileName = Path.Combine(Path.GetDirectoryName(pdfPath),
                            Path.GetFileNameWithoutExtension(pdfPath) + $"_Page-{i + 1:000}.png");

                        // Save the image as PNG
                        image.Write(imageFileName, MagickFormat.Png);
                        Console.WriteLine($"Page {i + 1} saved as {imageFileName}");
                    }
                }
            }
        }
    }
}