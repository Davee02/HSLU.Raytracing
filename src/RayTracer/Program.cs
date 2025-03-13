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
        Position = new Vector3(-300, 200, 0),
    },
    AmbientLight = new Light
    {
        Color = new Common.Color(1, 1, 1),
        Intensity = 0.1f
    },
    BackgroundColor = new Common.Color(0, 0, 0),
};
scene.GenerateRandomSpheres(10);
//scene.AddTriangle(new Triangle(new Vector3(500, 500, 100), new Vector3(500, 600, 100), new Vector3(600, 500, 100), new Common.Color(1, 0, 0)));
//scene.AddTriangle(new Triangle(new Vector3(0, 0, 500), new Vector3(100, 0, 100), new Vector3(0, 100, 100), new Common.Color(1, 1, 0)));
//scene.AddRectangle(new Rectangle(new Vector3(100, 100, 100), new Vector3(100, 0, 100), new Vector3(0, 100, 0), new Common.Color(0, 1, 0)));
scene.AddCube(new Cube(new Vector3(1000, 1000, 150), 100, new Common.Color(0, 1, 0)));

static (Sphere? sphere, float lambda) FindClosestSphere(Ray ray, Scene scene)
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

static (Triangle? triangle, float lambda) FindClosestTriangle(Ray ray, Scene scene)
{
    Triangle? closestTriangle = null;
    float closestLambda = float.MaxValue;
    foreach (var triangle in scene.Triangles)
    {
        if (triangle.Intersect(ray, out var lambda) && lambda < closestLambda)
        {
            closestLambda = lambda;
            closestTriangle = triangle;
        }
    }
    return (closestTriangle, closestLambda);
}

static Common.Color ComputeLightingForSpheres(Ray ray, Sphere sphere, float closestLambda, Scene scene)
{
    var intersectionPoint = ray.Origin + (ray.Direction * closestLambda);
    var lightDirection = (scene.DiffusedLight.Position - intersectionPoint).Normalize();
    var surfaceNormal = (intersectionPoint - sphere.Center).Normalize();
    var diffuseFactor = Math.Max(0, lightDirection.ScalarProduct(surfaceNormal));

    var diffuseLight = scene.DiffusedLight.Color * diffuseFactor * sphere.Color;
    var ambientLight = scene.AmbientLight.Color * scene.AmbientLight.Intensity * sphere.Color;

    return diffuseLight + ambientLight;
}

static Common.Color ComputeLightingForTriangles(Ray ray, Triangle triangle, float closestLambda, Scene scene)
{
    var intersectionPoint = ray.Origin + (ray.Direction * closestLambda);
    var lightDirection = (scene.DiffusedLight.Position - intersectionPoint).Normalize();
    var surfaceNormal = (triangle.P2 - triangle.P1)
    .CrossProduct(triangle.P3 - triangle.P1)
    .Normalize();
    var diffuseFactor = Math.Max(0, lightDirection.ScalarProduct(surfaceNormal));

    var diffuseLight = scene.DiffusedLight.Color * diffuseFactor * triangle.Color;
    var ambientLight = scene.AmbientLight.Color * scene.AmbientLight.Intensity * triangle.Color;

    return diffuseLight + ambientLight;
}

var cameraPos = new Vector3(scene.Width / 2, scene.Height / 2, -1000);

var sw = Stopwatch.StartNew();
scene.Bitmap.Mutate(c => c.ProcessPixelRowsAsVector4((row, point) =>
{
    for (int x = 0; x < row.Length; x++)
    {
        var pixelPos = new Vector3(x, point.Y, 0);
        var direction = (pixelPos - cameraPos).Normalize();
        var ray = new Ray(cameraPos, direction);

        //var ray = new Ray(new Vector3(x, point.Y, 0), rayDirection);
        var (closestSphere, closestSphereLambda) = FindClosestSphere(ray, scene);
        var (closestTriangle, closestTriangleLambda) = FindClosestTriangle(ray, scene);
        var color = scene.BackgroundColor;

        if (closestTriangle != null && closestTriangleLambda < closestSphereLambda) // check if the closest intersection is a triangle 
        {
            color = ComputeLightingForTriangles(ray, closestTriangle.Value, closestTriangleLambda, scene);
        }
        else if (closestSphere != null)
        {
            color = ComputeLightingForSpheres(ray, closestSphere.Value, closestSphereLambda, scene);
        }

        row[x] = color.ToVector4();
    }
}));
sw.Stop();

Console.WriteLine($"Finished rendering in {sw.ElapsedMilliseconds} ms");

await scene.Bitmap.SaveAsPngAsync(filePath);

// Open the image with the default image viewer
Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });