using System.Diagnostics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("Usage: <inputImagePath> <outputImagePath> <targetColourHex> <extension>");
            return;
        }

        string inputImagePath = args[0];
        string outputImagePath = args[1];
        string targetColourHex = args[2];
        string fileExtension = args[3];

        // Ensure the output directory exists
        if (!Directory.Exists(outputImagePath))
        {
            Directory.CreateDirectory(outputImagePath);
        }

        // Parse the target color from hex
        Rgba32 targetColour = ParseColorFromHex(targetColourHex);
        Rgba32 replacementColour = new Rgba32(255, 0, 0, 255); // Bright red

        // Process all image files in the input folder with the specified extension
        string[] imageFiles = Directory.GetFiles(inputImagePath, "*" + fileExtension);

        foreach (string imagePath in imageFiles)
        {
            string fileName = Path.GetFileName(imagePath);
            string outputFilePath = Path.Combine(outputImagePath, fileName);

            //Determine even or odd page from filename
            int pageNum = int.Parse(fileName.Substring(fileName.IndexOf("-")+1, 3));
            bool isEven = pageNum%2 == 0;


            Console.WriteLine($"Page {pageNum} even {isEven} Processing {fileName}...");
            ProcessImage(imagePath, outputFilePath, targetColour, replacementColour, isEven);
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

    /// <summary>
    /// Replaces the target colour in a file with a new colour, used to identify fold points
    /// </summary>
    /// <param name="inputImagePath"></param>
    /// <param name="outputImagePath"></param>
    /// <param name="targetColor"></param>
    /// <param name="replacementColor"></param>
    static void ProcessImage(string inputImagePath, string outputImagePath, Rgba32 targetColor, Rgba32 replacementColor, bool isEven)
    {
        using (Image<Rgba32> image = Image.Load<Rgba32>(inputImagePath))
        {
            CullBody(image, isEven);
            HighlightFolds(targetColor, replacementColor, image);
            SimplifyImage(image);
            ConnectFolds(image, isEven, 2, 5);
            
            // Save the modified image
            image.Save(outputImagePath);
        }

        static void CullBody(Image<Rgba32> image, bool isEven)
        {
            //If the page is EVEN preserve the LHS
            //If the page is ODD preserve the RHS

            Rgba32 blackColour = new Rgba32(0, 0, 0);

            int cullBand = 5; // 5 pixels being saved

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    if (isEven)
                    {
                        if (x > cullBand)
                        {
                            image[x,y] = blackColour;
                        }
                    }
                    else
                    {
                        if (x < (image.Width - cullBand))
                        {
                            image[x,y] = blackColour;
                        }
                    }
                }
            }
        }

        static void HighlightFolds(Rgba32 targetColor, Rgba32 replacementColor, Image<Rgba32> image)
        {
            // Iterate over each pixel
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    // Get the pixel at the current position
                    Rgba32 pixel = image[x, y];

                    // Check if the pixel is similar to the target color
                    if (AreColoursSimilar(pixel, targetColor))
                    {
                        // Replace the pixel with the replacement color
                        image[x, y] = replacementColor;
                    }
                }
            }
        }

        static void SimplifyImage(Image<Rgba32> image)
        {
            Rgba32 redColour = new Rgba32(255, 0, 0, 255);
            Rgba32 blackColour = new Rgba32(0, 0, 0);


            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    // Get the pixel at the current position
                    Rgba32 pixel = image[x, y];

                    // Check if the pixel is similar to the target color
                    image[x, y] = AreColoursSimilar(pixel, redColour, 1) ? redColour : blackColour;
                }
            }
        }
    
        static void ConnectFolds(Image<Rgba32> image, bool isLeft, int tolerance, int margin)
        {
            Rgba32 purpleColour = new Rgba32(128, 0, 128, 255);
            Rgba32 redColour = new Rgba32(255, 0, 0, 255);
            Rgba32 blackColour = new Rgba32(0, 0, 0);

            bool fillerMode = false;
            int flipTolerance = tolerance;

            for (int y = 0; y < image.Height; y++)
            {
                for (int x = isLeft ? 0 : image.Width - margin; x < (isLeft ? margin : image.Width); x++) // only calc the LHS / RHS respectively
                {
                    // Get the pixel at the current position
                    Rgba32 pixel = image[x, y];

                    if (AreColoursSimilar(pixel, redColour, 1) == true)
                    {
                        flipTolerance--;
                    }

                    if (flipTolerance == 0)
                    {
                        fillerMode = !fillerMode;
                        flipTolerance = tolerance;
                    }

                    if (fillerMode)
                    {
                        image[x,y] = purpleColour;
                    }
                    // Check if the pixel is similar to the target color
                   // image[x, y] = AreColoursSimilar(pixel, redColour, 1) ? purpleColour : blackColour;

                }
            }
        }
    }

    static bool AreColoursSimilar(Rgba32 color1, Rgba32 color2, int tolerance = 20)
    {
        return Math.Abs(color1.R - color2.R) <= tolerance &&
               Math.Abs(color1.G - color2.G) <= tolerance &&
               Math.Abs(color1.B - color2.B) <= tolerance;
    }
}