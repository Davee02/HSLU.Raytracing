using Common;
using SixLabors.ImageSharp;
using System.Diagnostics;
using SixLabors.ImageSharp.Processing;
using Rectangle = Common.Rectangle;

const string filePath = "rastered_image.png";
var rayDirection = new Vector3(0, 0, 1);

using var scene = new Scene(1500, 1000, 1000)
{
    DiffusedLight = new Light
    {
        Color = new Common.Color(1, 1, 1),
        Position = new Vector3(750, -500, -1000),
    },
    AmbientLight = new Light
    {
        Color = new Common.Color(1, 1, 1),
        Intensity = 0.1f
    },
    BackgroundColor = new Common.Color(0, 0, 0),
};
scene.GenerateRandomSpheres(10);
scene.GenerateRandomCubes(10);
scene.AddTriangle(new Triangle(new Vector3(500, 500, 100), new Vector3(100, 0, 0), new Vector3(0, 100, 0), new Common.Color(1, 0, 0)));
scene.AddRectangle(new Rectangle(new Vector3(300, 300, 100), new Vector3(100, 0, 0), new Vector3(0, 200, 0), new Common.Color(0, 1, 0)));
scene.AddCube(new Cube(new Vector3(700, 700, 100), 100, new Vector3(30, 30, 30), new Common.Color(0, 0, 1)));

static (ITraceableObject? traceableObject, float lambda) FindClosestObject(Ray ray, Scene scene)
{
    ITraceableObject? closestObj = null;
    float closestLambda = float.MaxValue;

    foreach (var oby in scene.TraceableObjects)
    {
        if (oby.Intersect(ray, out var lambda) && lambda < closestLambda)
        {
            closestLambda = lambda;
            closestObj = oby;
        }
    }

    return (closestObj, closestLambda);
}

var cameraPos = new Vector3(scene.Width / 2, scene.Height / 2, -1000);

var sw = Stopwatch.StartNew();
scene.Bitmap.Mutate(c => c.ProcessPixelRowsAsVector4((row, point) =>
{
    for (int x = 0; x < row.Length; x++)
    {
        //var pixelPos = new Vector3(x, point.Y, 0);
        //var direction = (pixelPos - cameraPos).Normalize();
        //var ray = new Ray(cameraPos, direction);
        var ray = new Ray(new Vector3(x, point.Y, 0), rayDirection);

        var (closestObj, lambda) = FindClosestObject(ray, scene);

        var pixelColor = scene.BackgroundColor;    
        if (closestObj != null)
        {
            var intersectionPoint = ray.Origin + (ray.Direction * lambda);
            pixelColor = scene.ComputeLighting(intersectionPoint, closestObj.SurfaceNormal(intersectionPoint), closestObj.Color);
        }

        row[x] = pixelColor.ToVector4();
    }
}));
sw.Stop();

Console.WriteLine($"Finished rendering in {sw.ElapsedMilliseconds} ms");

await scene.Bitmap.SaveAsPngAsync(filePath);

// Open the image with the default image viewer
Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });