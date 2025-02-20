using SkiaSharp;

const int width = 800;
const int height = 500;
const string filePath = "skia_raster_image.png";

// Create a SkiaSharp bitmap (raster surface)
var bitmap = new SKBitmap(width, height);

// Get the pixel buffer
for (int y = 0; y < height; y++)
{
    for (int x = 0; x < width; x++)
    {
        var color = SKColor.Parse("#FF0000");

        // Set the pixel
        bitmap.SetPixel(x, y, color);
    }
}

// Save the image as PNG
using (var fs = new FileStream(filePath, FileMode.Create))
{
    bitmap.Encode(fs, SKEncodedImageFormat.Png, 100);
}

Console.WriteLine($"Image saved to {filePath}");