using Common;
using SixLabors.ImageSharp;
using System.Diagnostics;
using SixLabors.ImageSharp.Processing;

const string filePath = "rastered_image.png";

using var scene = new Scene(1500, 1000)
{
    DiffusedLights = [
        new Light
        {
            Color = new Common.Color(1, 0.9f, 0.8f), // Warm light
            Position = new Vector3(1000, 200, -300),
            Intensity = 0.7f,
        },
        new Light
        {
            Color = new Common.Color(0.8f, 0.8f, 1f), // Cool light
            Position = new Vector3(300, 300, -200),
            Intensity = 0.5f
        }
    ],
    AmbientLight = new Light
    {
        Color = new Common.Color(1, 1, 1),
        Intensity = 0.2f
    },
    BackgroundColor = new Common.Color(0.1f, 0.1f, 0.2f),
    CameraPosition = new Vector3(750, 500, -500)
};

scene.AddPlane(new Vector3(0, 900, 0), new Vector3(0, 0, 0), new Common.Color(0.8f, 0.8f, 0.8f)); // gray floor in the front
scene.AddPlane(new Vector3(0, 0, 1000), new Vector3(0, 0, 90), new Common.Color(0.7f, 0.7f, 0.9f)); // blue wall on the left

scene.AddSphere(new Vector3(1100, 200, 310), 200, new Common.Color(1, 0.9f, 0.2f));
scene.AddCube(new Vector3(1000, 150, 30), 30, new Vector3(80, 10, 30), new Common.Color(1, 0.9f, 0.2f));

scene.AddSphere(new Vector3(700, 400, 350), 80, new Common.Color(1, 0.3f, 0.3f));
scene.AddSphere(new Vector3(850, 450, 300), 70, new Common.Color(0.3f, 1, 0.3f));
scene.AddSphere(new Vector3(750, 550, 400), 90, new Common.Color(0.3f, 0.3f, 1));
scene.AddSphere(new Vector3(600, 500, 250), 60, new Common.Color(1, 1, 0.3f));

scene.AddCube(new Vector3(750, 700, 350), 120, new Vector3(25, 45, 10), new Common.Color(0.8f, 0.4f, 0.8f));

scene.AddSphere(new Vector3(1100, 800, 250), 50, new Common.Color(0, 0.8f, 0.8f));
scene.AddSphere(new Vector3(500, 700, 450), 40, new Common.Color(0.8f, 0.8f, 0));

scene.AddCube(new Vector3(900, 820, 150), 60, new Vector3(0, 30, 45), new Common.Color(0.5f, 0.5f, 1));
scene.AddCube(new Vector3(960, 750, 100), 20, new Vector3(45, 45, 45), new Common.Color(0.5f, 0.5f, 1));
scene.AddCube(new Vector3(1000, 600, 100), 80, new Vector3(45, 60, 15), new Common.Color(0.5f, 1, 0.5f));

static (ITraceableObject? traceableObject, float lambda) FindClosestObject(Ray ray, Scene scene)
{
    ITraceableObject? closestObj = null;
    float closestLambda = float.MaxValue;

    foreach (var obj in scene.TraceableObjects)
    {
        if (obj.Intersect(ray, out var lambda) && lambda < closestLambda)
        {
            closestLambda = lambda;
            closestObj = obj;
        }
    }

    return (closestObj, closestLambda);
}

const float eps = 1e-2f;

var sw = Stopwatch.StartNew();
scene.Bitmap.Mutate(c => c.ProcessPixelRowsAsVector4((row, point) =>
{
    for (int x = 0; x < row.Length; x++)
    {
        // shoot a ray from the camera through the pixel to the scene (perspective projection)
        var pixelPos = new Vector3(x, point.Y, 0);
        var direction = (pixelPos - scene.CameraPosition).Normalize();
        var ray = new Ray(scene.CameraPosition, direction);

        var (closestObj, lambda) = FindClosestObject(ray, scene);

        var pixelColor = scene.BackgroundColor;
        if (closestObj != null)
        {
            var intersectionPoint = ray.Origin + (ray.Direction * lambda);

            // Start with ambient light only
            pixelColor = scene.ComputeAmbientColor(closestObj.Color);

            // Check each light for contribution
            foreach (var light in scene.DiffusedLights)
            {
                // Shadow ray from intersection to this light
                var shadowRayDirection = (light.Position - intersectionPoint).Normalize();
                var shadowRay = new Ray(intersectionPoint + (shadowRayDirection * eps), shadowRayDirection);
                var (shadowObj, shadowLambda) = FindClosestObject(shadowRay, scene);
                var lightDistance = (light.Position - intersectionPoint).Length;

                if (shadowObj == null || shadowLambda > lightDistance)
                {
                    // No shadow for this light, add its contribution
                    var lightDirection = shadowRayDirection; // Already normalized
                    var diffuseFactor = Math.Max(0, lightDirection.ScalarProduct(closestObj.SurfaceNormal(intersectionPoint)));

                    // Add this light's contribution
                    pixelColor = pixelColor + (light.Color * diffuseFactor * light.Intensity * closestObj.Color);
                }
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