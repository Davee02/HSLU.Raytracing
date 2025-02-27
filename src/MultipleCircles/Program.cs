using Common;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Diagnostics;
using SixLabors.ImageSharp.Processing;

const int width = 800;
const int height = 500;
const int radius = 100;
const string filePath = "skia_raster_image.png";

var circle1 = new Circle(new Vector2(200, 200), radius, new Common.Color(1, 0, 0));
var circle2 = new Circle(new Vector2(300, 300), radius, new Common.Color(0, 1, 0));
var circle3 = new Circle(new Vector2(350, 200), radius, new Common.Color(0, 0, 1));

// Create a SkiaSharp bitmap (raster surface)
using var bitmap = new Image<Rgba32>(width, height);
var background = new Common.Color(0, 0, 0);

var sw = Stopwatch.StartNew();
bitmap.Mutate(c => c.ProcessPixelRowsAsVector4((row, point) =>
{
    var y = point.Y;

    for (int x = 0; x < row.Length; x++)
    {
        var location = new Vector2(x, y);
        var color = new Common.Color(background.R, background.G, background.B);

        if (circle1.ContainsPixel(location))
        {
            color += circle1.Color;
        }
        if (circle2.ContainsPixel(location))
        {
            color += circle2.Color;
        }
        if (circle3.ContainsPixel(location))
        {
            color += circle3.Color;
        }

        row[x] = color.ToVector4();
    }
}));
sw.Stop();

Console.WriteLine($"Finished rendering in {sw.ElapsedMilliseconds} ms");

await bitmap.SaveAsPngAsync(filePath);

// Open the image with the default image viewer
Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });