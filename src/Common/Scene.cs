using Common.Objects;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Numerics;
using System.Text;

namespace Common
{
    public struct Scene(Vector2 imageSize) : IDisposable
    {
        private BVH.BVH? bvh = null;

        public RenderSettings RenderSettings { get; set; } = new();

        public Vector2 ImageSize { get; } = imageSize;

        public List<Sphere> Spheres { get; } = [];

        public List<Objects.Plane> Planes { get; } = [];

        public List<Triangle> Triangles { get; } = [];

        public List<ITraceableObject> TraceableObjects { get; } = [];

        public List<Light> DiffusedLights { get; set; } = [];

        public Camera Camera { get; set; }

        public Light AmbientLight { get; set; }

        public Color BackgroundColor { get; set; } = new Color(0, 0, 0);

        public Image<Rgba32> Bitmap { get; } = new Image<Rgba32>((int)imageSize.X, (int)imageSize.Y);

        public readonly BVH.BVH? BVH => bvh;

        public void BuildBVH()
        {
            bvh = new BVH.BVH(TraceableObjects);
        }

        public readonly void AddSphere(Vector3 center, float radius, Material color) 
        {
            var sphere = new Sphere(center, radius, color);
            Spheres.Add(sphere);
            TraceableObjects.Add(sphere);
        }

        public readonly void AddPlane(Vector3 position, Vector3 rotationAnglesDegrees, Material material)
        {
            var plane = new Objects.Plane(position, rotationAnglesDegrees, material);
            Planes.Add(plane);
            TraceableObjects.Add(plane);
        }

        public readonly void AddTriangle(Vector3 origin, Vector3 v, Vector3 w, Material material)
        {
            AddTriangle(new Triangle(origin, v, w, material));
        }

        public readonly void AddTriangle(Triangle triangle)
        {
            TraceableObjects.Add(triangle);
            Triangles.Add(triangle);
        }

        public readonly void AddTriangles(IEnumerable<Triangle> triangles)
        {
            foreach(var triangle in triangles)
            {
                TraceableObjects.Add(triangle);
                Triangles.Add(triangle);
            }
        }


        public readonly void AddRectangle(Vector3 position, Vector3 normal, Vector3 up, float width, float height, float thickness, Material material)
        {
            var rectangle = new Objects.Rectangle(position, normal, up, width, height, thickness, material);
            AddRectangle(rectangle);
        }

        public readonly void AddRectangle(Objects.Rectangle rectangle)
        {
            foreach (var triangle in rectangle.Triangles)
            {
                AddTriangle(triangle);
            }
        }

        public readonly void AddCube(Vector3 position, float sideLength, Vector3 rotationAnglesDegrees, Material material)
        {
            var cube = new Cube(position, sideLength, rotationAnglesDegrees, material);
            foreach (var triangle in cube.Triangles)
            {
                AddTriangle(triangle);
            }
        }

        public Color ComputeAmbientColor(Material objectMaterial)
        {
            return AmbientLight.Color * AmbientLight.AttenuationC * objectMaterial.DiffuseColor;
        }

        public void Dispose()
        {
            Bitmap?.Dispose();
        }

        public readonly string PrintInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Image resolution: {ImageSize.X}x{ImageSize.Y}");
            sb.AppendLine($"Number of spheres: {Spheres.Count}");
            sb.AppendLine($"Number of planes: {Planes.Count}");
            sb.AppendLine($"Number of triangles: {Triangles.Count}");
            return sb.ToString();
        }

        public void Render()
        {
            Console.WriteLine(PrintInfo());

            Console.WriteLine("Building BVH...");
            BuildBVH();
            Console.WriteLine($"BVH built with depth {BVH.Depth}");

            int totalRows = Bitmap.Height / RenderSettings.LineSkipStep;

            using var progressBar = new EnhancedProgressBar(
                totalRows,
                $"Rendering {ImageSize.X}x{ImageSize.Y} image",
                80,
                true);

#if DEBUG
    int rowsCompleted = 0;

    for (int y = 0; y < Bitmap.Height; y++)
    {
        if (y % RenderSettings.LineSkipStep != 0)
        {
            continue;
        }

        for (int x = 0; x < Bitmap.Width; x++)
        {
            var rays = Camera.GetRaysForPixel(x, y).ToArray();
            var ray = rays[0];

            var pixelColor = Tracer.TraceRay(ray, this, 0, RenderSettings.MaxRecursionDepth);
            Bitmap[x, y] = pixelColor.ToRgba32();
        }
        
        rowsCompleted++;
        progressBar.Update(rowsCompleted);
    }
#else
            var thisScene = this; // needed for the lambda function

            // Use thread-safe counter for parallel processing
            int completedRows = 0;
            object lockObj = new();

            Bitmap.Mutate(c => c.ProcessPixelRowsAsVector4((row, point) =>
            {
                if (point.Y % thisScene.RenderSettings.LineSkipStep != 0)
                {
                    return;
                }

                for (int x = 0; x < row.Length; x++)
                {
                    var rays = thisScene.Camera.GetRaysForPixel(x, point.Y).ToArray();

                    var pixelColor = Color.Black;
                    foreach (var ray in rays)
                    {
                        // Calculate the color for each ray
                        pixelColor += Tracer.TraceRay(ray, thisScene, 0, thisScene.RenderSettings.MaxRecursionDepth);
                        row[x] += pixelColor.ToVector4();
                    }

                    pixelColor /= rays.Length;

                    row[x] = pixelColor.ToVector4();
                }

                // Update progress in a thread-safe way
                if (point.Y % thisScene.RenderSettings.LineSkipStep == 0)
                {
                    int newCompletedRows = Interlocked.Increment(ref completedRows);
                    progressBar.Update(newCompletedRows);
                }
            }));
#endif
        }
    }
}
