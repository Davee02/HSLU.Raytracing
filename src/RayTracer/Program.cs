using Common;
using SixLabors.ImageSharp;
using System.Diagnostics;
using SixLabors.ImageSharp.Processing;

const string filePath = "rastered_image.png";
var rayDirection = new Vector3(0, 0, 1);

using var scene = new Scene(1500, 1000, 1000)
{
    DiffusedLight = new Light
    {
        Color = new Common.Color(1, 1, 1),
        Position = new Vector3(-1000, -1000, -1000),
    },
    AmbientLight = new Light
    {
        Color = new Common.Color(1, 1, 1),
        Intensity = 0.1f
    },
    BackgroundColor = new Common.Color(0, 0, 0),
};
scene.GenerateRandomSpheres(1000);

static (Sphere?, float) FindClosestSphere(Ray ray, Scene scene)
{
    Sphere? closestSphere = null;
    float closestLambda = float.MaxValue;

    foreach (var sphere in scene.Spheres)
    {
        if (sphere.Intersect(ray, out var lambda) && lambda < closestLambda)
        {
            closestLambda = lambda;
            closestSphere = sphere;
        }
    }

    return (closestSphere, closestLambda);
}

static Common.Color ComputeLighting(Ray ray, Sphere sphere, float closestLambda, Scene scene)
{
    var intersectionPoint = ray.Origin + (ray.Direction * closestLambda);
    var lightDirection = (scene.DiffusedLight.Position - intersectionPoint).Normalize();
    var surfaceNormal = (intersectionPoint - sphere.Center).Normalize();
    var diffuseFactor = Math.Max(0, lightDirection.ScalarProduct(surfaceNormal));

    var diffuseLight = scene.DiffusedLight.Color * diffuseFactor * sphere.Color;
    var ambientLight = scene.AmbientLight.Color * scene.AmbientLight.Intensity * sphere.Color;

    return diffuseLight + ambientLight;
}

var sw = Stopwatch.StartNew();
scene.Bitmap.Mutate(c => c.ProcessPixelRowsAsVector4((row, point) =>
{
    for (int x = 0; x < row.Length; x++)
    {
        var ray = new Ray(new Vector3(x, point.Y, 0), rayDirection);
        var (closestSphere, closestLambda) = FindClosestSphere(ray, scene);
        var color = scene.BackgroundColor;

        if (closestSphere != null) // Check if a sphere was hit
        {
            color = ComputeLighting(ray, closestSphere.Value, closestLambda, scene);
        }

        row[x] = color.ToVector4();
    }
}));
sw.Stop();

Console.WriteLine($"Finished rendering in {sw.ElapsedMilliseconds} ms");

await scene.Bitmap.SaveAsPngAsync(filePath);

// Open the image with the default image viewer
Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });