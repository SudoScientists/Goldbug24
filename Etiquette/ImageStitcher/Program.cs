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
            Console.WriteLine("Usage: <inputImageFolderPath> <outputImagePath> <extension>");
            return;
        }

        string inputImagePath = args[0];
        string outputImagePath = args[1];
        string fileExtension = args[2];

        // Ensure the output directory exists
        if (!Directory.Exists(outputImagePath))
        {
            Directory.CreateDirectory(outputImagePath);
        }

        // Parse the target color from hex
        Rgba32 searchColour = new Rgba32(128, 0, 128, 255); //PURPLE
        Rgba32 replacementColour = new Rgba32(255, 0, 0, 255); //RED

        // Create the synthetic (composite) image
        // final image is 1332px (444 pages each page contributing 3 pixels from the border) x ~2100 (mean average height)
        Image<Rgba32> compositeImage = new(1332, 2100, new Rgba32(255,255,255,255));

        // Process all image files in the input folder with the specified extension
        string[] imageFiles = Directory.GetFiles(inputImagePath, "*" + fileExtension);

        foreach (string imagePath in imageFiles)
        {
            string fileName = Path.GetFileName(imagePath);

            int pageNum = int.Parse(fileName.Substring(fileName.IndexOf("-")+1, 3));

            Console.WriteLine($"Page {pageNum} Processing {fileName}...");

            ProcessImage(compositeImage, imagePath, searchColour, replacementColour, pageNum);
        }
        
        //SUPER LAZY Hardcode
        compositeImage.Save(@"D:\Dropbox\GoldBug\Etiquette\final.png");
        Console.WriteLine("Stitch Complete");
    }

    static void ProcessImage(Image<Rgba32> compositeImage, string inputImagePath, Rgba32 targetColor, Rgba32 replacementColor, int pageNum)
    {
        // Determine even or odd - ODD use RHS, EVEN use LHS
        bool isEven = pageNum%2 == 0;

        int sliceWidth = 3;
        int xStitchLocation = (pageNum -1)*sliceWidth; //1st [0]->[2] 3px; 2nd [3]->[5]; 3rd [6]->[8] == (pagenum - 1)*3 ergo pg 444 [1329]-[1331]

        using (Image<Rgba32> image = Image.Load<Rgba32>(inputImagePath))
        {
            for (int x = isEven? 0 : image.Width - sliceWidth; x < (isEven ? sliceWidth : image.Width); x++)
            {
                for (int y = 0; y < 2100; y++)
                {
                    Rgba32 sourcePixel = image[x,y];
                    compositeImage[xStitchLocation, y] = sourcePixel;
                }

                Console.WriteLine($"X:{x} -> cX:{xStitchLocation} ");
                xStitchLocation++; 
            }
        }
    }    
}