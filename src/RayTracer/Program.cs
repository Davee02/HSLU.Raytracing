using Common;
using SixLabors.ImageSharp;
using System.Diagnostics;
using SixLabors.ImageSharp.Processing;
using System.Numerics;

const string filePath = "rastered_image.png";

using var scene = new Scene(1500, 1000)
{
    DiffusedLights = 
    [
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
        },
        new Light
        {
            Color = new Common.Color(1, 0, 0), // Red light
            Position = new Vector3(750, -1000, 200),
            Intensity = 0.2f
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

scene.AddPlane(new Vector3(0, 900, 0), new Vector3(0, 0, 0), new Material(new Common.Color(0.8f, 0.8f, 0.8f), 0.1f, 20)); // gray floor in the front
scene.AddPlane(new Vector3(0, 0, 1000), new Vector3(0, 0, 90), new Material(new Common.Color(0.7f, 0.7f, 0.9f), 0.1f, 20)); // blue wall on the left

scene.AddSphere(new Vector3(1100, 200, 310), 200, new Material(new Common.Color(1, 0.9f, 0.2f), 1f, 20));
scene.AddCube(new Vector3(1000, 150, 30), 30, new Vector3(80, 10, 30), new Material(new Common.Color(1, 0.9f, 0.2f), 0.1f, 20));

scene.AddSphere(new Vector3(700, 400, 350), 80, new Material(new Common.Color(1, 0.3f, 0.3f), 0.1f, 20));
scene.AddSphere(new Vector3(850, 450, 300), 70, new Material(new Common.Color(0.3f, 1, 0.3f), 0.1f, 20));
scene.AddSphere(new Vector3(750, 550, 400), 90, new Material(new Common.Color(0.3f, 0.3f, 1), 0.1f, 20));
scene.AddSphere(new Vector3(600, 500, 250), 60, new Material(new Common.Color(1, 1, 0.3f), 0.1f, 20));

scene.AddCube(new Vector3(750, 700, 350), 120, new Vector3(25, 45, 10), new Material(new Common.Color(0.8f, 0.4f, 0.8f), 0.1f, 20));

scene.AddSphere(new Vector3(1100, 800, 250), 10, new Material(new Common.Color(0, 0.8f, 0.8f), 0.1f, 20));
scene.AddSphere(new Vector3(550, 850, 250), 40, new Material(new Common.Color(0.8f, 0.8f, 0), 0.1f, 20));

scene.AddCube(new Vector3(900, 820, 150), 60, new Vector3(0, 30, 45), new Material(new Common.Color(0.5f, 0.5f, 1), 0.1f, 20));
scene.AddCube(new Vector3(960, 750, 100), 20, new Vector3(45, 45, 45), new Material(new Common.Color(0.5f, 0.5f, 1), 0.1f, 20));
scene.AddCube(new Vector3(1000, 600, 100), 80, new Vector3(45, 60, 15), new Material(new Common.Color(0.5f, 1, 0.5f), 0.1f, 20));

static Hit? FindClosestObject(Ray ray, Scene scene)
{
    Hit hit = default;
    Hit? closestHit = default;
    float closestLambda = float.MaxValue;

    foreach (var obj in scene.TraceableObjects)
    {
        if (obj.TryIntersect(ray, ref hit) && hit.Lambda < closestLambda)
        {
            closestHit = hit;
            closestLambda = hit.Lambda;
        }
    }

    return closestHit;
}

static Common.Color TraceRay(Ray ray, Scene scene, int depth, int maxDepth)
{
    // Base case: if we've reached max recursion depth, return background color
    if (depth > maxDepth)
    {
        return scene.BackgroundColor;
    }

    var closestHit = FindClosestObject(ray, scene);
    if (!closestHit.HasValue)
    {
        return scene.BackgroundColor;
    }

    var hit = closestHit.Value;

    // Calculate reflection ray
    var reflectionRay = new Ray(hit.Position, Vector3.Normalize(Vector3.Reflect(ray.Direction, hit.Normal)));

    // Start with ambient light only
    var pixelColor = scene.ComputeAmbientColor(hit.Material);

    // Accumulate contributions from all diffused lights
    var contributingLights = new List<(Light light, float diffuseFactor, float specularFactor)>();
    foreach (var light in scene.DiffusedLights)
    {
        // Shadow ray from intersection to this light
        var lightDirection = Vector3.Normalize(light.Position - hit.Position);
        var shadowRay = new Ray(hit.Position, lightDirection);

        var closestShadowHit = FindClosestObject(shadowRay, scene);
        var lightDistance = (light.Position - hit.Position).Length();

        if (!closestShadowHit.HasValue || closestShadowHit.Value.Lambda > lightDistance)
        {
            // No shadow for this light, add its contribution
            var diffuseFactor = Vector3.Dot(lightDirection, hit.Normal);

            if (diffuseFactor > 0)
            {
                var specularFactor = Vector3.Dot(reflectionRay.Direction, shadowRay.Direction);
                specularFactor = MathF.Max(0, specularFactor);
                specularFactor = MathF.Pow(specularFactor, hit.Material.Shininess);

                contributingLights.Add((light, diffuseFactor, specularFactor));
            }
        }
    }

    if (contributingLights.Count > 0)
    {
        // Apply normalization factor
        var lightFactor = 1f / contributingLights.Count; // normalize by dividing by the number of contributing light sources
        foreach (var (light, diffuseFactor, specularFactor) in contributingLights)
        {
            var diffuseContribution = hit.Material.Color * diffuseFactor;
            var specularContribution = light.Color * specularFactor;

            pixelColor += light.Color * light.Intensity * (diffuseContribution + specularContribution);
        }
    }

    // Calculate reflection color (if material is reflective)
    if (hit.Material.Reflectivity > 0)
    {
        var reflectionColor = TraceRay(reflectionRay, scene, depth + 1, maxDepth);
        // Mix the reflection color with the direct illumination color based on reflectivity
        pixelColor = pixelColor * (1 - hit.Material.Reflectivity) + reflectionColor * hit.Material.Reflectivity;
    }

    return pixelColor;
}

var lineSkipStep = 1;
var maxRecursionDepth = 3;
var sw = Stopwatch.StartNew();
scene.Bitmap.Mutate(c => c.ProcessPixelRowsAsVector4((row, point) =>
{
    if (point.Y % lineSkipStep != 0)
    {
        return;
    }

    for (int x = 0; x < row.Length; x++)
    {
        // shoot a ray from the camera through the pixel to the scene (perspective projection)
        var pixelPos = new Vector3(x, point.Y, 0);
        var direction = Vector3.Normalize(pixelPos - scene.CameraPosition);
        var ray = new Ray(scene.CameraPosition, direction);

        // Use the recursive TraceRay function instead of direct color calculation
        var pixelColor = TraceRay(ray, scene, 0, maxRecursionDepth);

        row[x] = pixelColor.ToVector4();
    }
}));
sw.Stop();

Console.WriteLine($"Finished rendering in {sw.ElapsedMilliseconds} ms");

await scene.Bitmap.SaveAsPngAsync(filePath);

// Open the image with the default image viewer
Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });