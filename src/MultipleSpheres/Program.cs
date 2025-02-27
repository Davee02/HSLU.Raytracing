using Common;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Diagnostics;

const int width = 800;
const int height = 500;
const string filePath = "skia_raster_image.png";

var rayDirection = new Vector3(0, 0, 1);

var spheres = new Sphere[]
{
    new (new Vector3(200, 200, 120), 100, new Common.Color(1, 0, 0)),
    new (new Vector3(300, 200, 100), 50, new Common.Color(0, 1, 0)),
    new (new Vector3(400, 200, 190), 150, new Common.Color(0, 0, 1)),
    new (new Vector3(width, height, 200), 400, new Common.Color(1, 1, 1)),
};

// Create a SkiaSharp bitmap (raster surface)
using var bitmap = new Image<Rgba32>(width, height);
var background = new Common.Color(0, 0, 0);

var sw = Stopwatch.StartNew();
bitmap.ProcessPixelRows(accessor =>
{
    for (int y = 0; y < accessor.Height; y++)
    {
        var pixelRow = accessor.GetRowSpan(y);
        for (int x = 0; x < pixelRow.Length; x++)
        {
            var ray = new Ray(new Vector3(x, y, 0), rayDirection);
            var color = background;

            float? closestLambda = null;
            Sphere closestSphere = default;

            foreach (var sphere in spheres)
            {
                if (sphere.Intersect(ray, out var lambda))
                {
                    if (closestLambda == null || lambda < closestLambda)
                    {
                        closestLambda = lambda;
                        closestSphere = sphere;
                    }
                }
            }

            if (closestLambda != null)
            {
                color = closestSphere.Color;
            }

            pixelRow[x] = color.ToRgba32();
        }
    }
});
sw.Stop();

Console.WriteLine($"Finished rendering in {sw.ElapsedMilliseconds} ms");

await bitmap.SaveAsPngAsync(filePath);

// Open the image with the default image viewer
Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });