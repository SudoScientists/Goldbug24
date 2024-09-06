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
    }

    static void ProcessImage(string inputImagePath, string outputImagePath, Rgba32 targetColor, Rgba32 replacementColor, bool isEven)
    {
        using (Image<Rgba32> image = Image.Load<Rgba32>(inputImagePath))
        {

            
            // Save the modified image
            image.Save(outputImagePath);
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
    
}