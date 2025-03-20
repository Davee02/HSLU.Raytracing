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
        Position = new Vector3(1500, 600, -500)
    },
    AmbientLight = new Light
    {
        Color = new Common.Color(1, 1, 1),
        Intensity = 0.3f
    },
    BackgroundColor = new Common.Color(0, 0, 0),
};
//scene.AddPlane(new Plane(new Vector3(300, 900, 0), new Vector3(0, 0, 0), new Common.Color(1, 0, 0)));
//scene.AddPlane(new Plane(new Vector3(300, 900, 500), new Vector3(90, 0, 0), new Common.Color(0, 1f, 0)));
//scene.GenerateRandomSpheres(10);
//scene.GenerateRandomCubes(10);
//scene.AddTriangle(new Triangle(new Vector3(500, 500, 100), new Vector3(100, 0, 0), new Vector3(0, 100, 0), new Common.Color(1, 0, 0)));
//scene.AddRectangle(new Rectangle(new Vector3(300, 300, 100), new Vector3(100, 0, 0), new Vector3(0, 200, 0), new Common.Color(0, 1, 0)));
//scene.AddCube(new Cube(new Vector3(600, 700, 150), 100, new Vector3(30, 30, 30), new Common.Color(0, 0, 1)));
scene.AddSphere(new Sphere(new Vector3(600, 300, 300), 100, new Common.Color(1, 1, 0)));
scene.AddSphere(new Sphere(new Vector3(800, 300, 200), 50, new Common.Color(1, 0, 0)));
//scene.AddSphere(new Sphere(new Vector3(1000, 300, 200), 100, new Common.Color(0, 1, 1)));

static (ITraceableObject? traceableObject, float lambda) FindClosestObject(Ray ray, Scene scene, ITraceableObject? excludeObj = null)
{
    ITraceableObject? closestObj = null;
    float closestLambda = float.MaxValue;

    foreach (var obj in scene.TraceableObjects)
    {
        if (obj == excludeObj) continue; // skip the originating object

        if (obj.Intersect(ray, out var lambda) && lambda < closestLambda)
        {
            closestLambda = lambda;
            closestObj = obj;
        }
    }

    return (closestObj, closestLambda);
}

var cameraPos = new Vector3(scene.Width / 2, scene.Height / 2, -500);

const float eps = 0.1f;

var sw = Stopwatch.StartNew();
scene.Bitmap.Mutate(c => c.ProcessPixelRowsAsVector4((row, point) =>
{
    for (int x = 0; x < row.Length; x++)
    {
        // shoot a ray from the camera through the pixel to the scene (perspective projection)
        var pixelPos = new Vector3(x, point.Y, 0);
        var direction = (pixelPos - cameraPos).Normalize();
        var ray = new Ray(cameraPos, direction);

        var (closestObj, lambda) = FindClosestObject(ray, scene);

        var pixelColor = scene.BackgroundColor;    
        if (closestObj != null)
        {
            var intersectionPoint = ray.Origin + (ray.Direction * lambda);

            // shoot a ray from the intersection point to the light source
            var shadowRayDirection = (scene.DiffusedLight.Position - intersectionPoint).Normalize();
            var shadowRay = new Ray(intersectionPoint + (shadowRayDirection * eps), shadowRayDirection);
            var (shadowObj, shadowLambda) = FindClosestObject(shadowRay, scene);
            var lightDistance = (scene.DiffusedLight.Position - intersectionPoint).Length;

            if (shadowObj == null || shadowLambda * shadowRayDirection.Length > lightDistance)
            {
                // no object is blocking the light source, so no shadow is cast
                pixelColor = scene.ComputeDiffusionColor(intersectionPoint, closestObj.SurfaceNormal(intersectionPoint), closestObj.Color);
            }
            else
            {
                pixelColor = scene.ComputeAmbientColor(closestObj.Color);
            }
        }

        row[x] = pixelColor.ToVector4();
    }
}));
sw.Stop();

Console.WriteLine($"Finished rendering in {sw.ElapsedMilliseconds} ms");

await scene.Bitmap.SaveAsPngAsync(filePath);

// Open the image with the default image viewer
Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });