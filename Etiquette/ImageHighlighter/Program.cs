using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Advanced;
using System;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Usage: <inputImagePath> <outputImagePath> <targetColorHex> <extension>");
            return;
        }

        string inputImagePath = args[0];
        string outputImagePath = args[1];
        string targetColorHex = args[2];
        string fileExtension = args[3];

        // Ensure the output directory exists
        if (!Directory.Exists(outputImagePath))
        {
            Directory.CreateDirectory(outputImagePath);
        }

        // Parse the target color from hex
        Rgba32 targetColor = ParseColorFromHex(targetColorHex);
        Rgba32 replacementColor = new Rgba32(255, 0, 0, 255); // Bright red

        // Process all image files in the input folder with the specified extension
        string[] imageFiles = Directory.GetFiles(inputImagePath, "*" + fileExtension);

        foreach (string imagePath in imageFiles)
        {
            string fileName = Path.GetFileName(imagePath);
            string outputFilePath = Path.Combine(outputImagePath, fileName);

            Console.WriteLine($"Processing {fileName}...");
            ReplaceColorInImage(imagePath, outputFilePath, targetColor, replacementColor);
        }

        Console.WriteLine("Colour replacement complete for all files!");

   
    }

    static Rgba32 ParseColorFromHex(string hex)
    {
        if (hex.StartsWith("#")) hex = hex.Substring(1);

        byte r = Convert.ToByte(hex.Substring(0, 2), 16);
        byte g = Convert.ToByte(hex.Substring(2, 2), 16);
        byte b = Convert.ToByte(hex.Substring(4, 2), 16);

        return new Rgba32(r, g, b);
    }

    static void ReplaceColorInImage(string inputImagePath, string outputImagePath, Rgba32 targetColor, Rgba32 replacementColor)
    {
        using (Image<Rgba32> image = Image.Load<Rgba32>(inputImagePath))
        {
            // Iterate over each pixel
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    // Get the pixel at the current position
                    Rgba32 pixel = image[x, y];

                    // Check if the pixel is similar to the target color
                    if (AreColorsSimilar(pixel, targetColor))
                    {
                        // Replace the pixel with the replacement color
                        image[x, y] = replacementColor;
                    }
                }
            }

            // Save the modified image
            image.Save(outputImagePath);
        }
    }

    static bool AreColorsSimilar(Rgba32 color1, Rgba32 color2, int tolerance = 20)
    {
        return Math.Abs(color1.R - color2.R) <= tolerance &&
               Math.Abs(color1.G - color2.G) <= tolerance &&
               Math.Abs(color1.B - color2.B) <= tolerance;
    }
}