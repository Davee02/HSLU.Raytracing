using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Common
{
    public struct Scene(int width, int height) : IDisposable
    {
        public int Width { get; set; } = width;

        public int Height { get; set; } = height;

        public List<Sphere> Spheres { get; } = [];

        public List<Plane> Planes { get; } = [];

        public List<Triangle> Triangles { get; } = [];

        public List<ITraceableObject> TraceableObjects { get; } = [];

        public List<Light> DiffusedLights { get; set; } = [];

        public Vector3 CameraPosition { get; set; }

        public Light AmbientLight { get; set; }

        public Color BackgroundColor { get; set; } = new Color(0, 0, 0);

        public Image<Rgba32> Bitmap { get; } = new Image<Rgba32>(width, height);

        public readonly void AddSphere(Vector3 center, float radius, Color color) 
        {
            var sphere = new Sphere(center, radius, color);
            Spheres.Add(sphere);
            TraceableObjects.Add(sphere);
        }

        public readonly void AddPlane(Vector3 position, Vector3 rotationAnglesDegrees, Color color)
        {
            var plane = new Plane(position, rotationAnglesDegrees, color);
            Planes.Add(plane);
            TraceableObjects.Add(plane);
        }


        public readonly void AddTriangle(Vector3 origin, Vector3 v, Vector3 w, Color color)
        {
            AddTriangle(new Triangle(origin, v, w, color));
        }

        public readonly void AddTriangle(Triangle triangle)
        {
            TraceableObjects.Add(triangle);
            Triangles.Add(triangle);
        }


        public readonly void AddRectangle(Vector3 origin, Vector3 w, Vector3 v, Color color)
        {
            var rectangle = new Rectangle(origin, w, v, color);
            AddTriangle(rectangle.Triangle1);
            AddTriangle(rectangle.Triangle2);
        }

        public readonly void AddCube(Vector3 position, float sideLength, Vector3 rotationAnglesDegrees, Color color)
        {
            var cube = new Cube(position, sideLength, rotationAnglesDegrees, color);
            foreach (var triangle in cube.Triangles)
            {
                AddTriangle(triangle);
            }
        }

        public void GenerateRandomSpheres(int num)
        {
            var random = new Random();
            for (int i = 0; i < num; i++)
            {
                var center = new Vector3(random.Next(0, Width), random.Next(0, Height), random.Next(50, 200));
                var radius = random.Next(10, 100);
                var color = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
                AddSphere(center, radius, color);
            }
        }

        public void GenerateRandomCubes(int num)
        {
            var random = new Random();
            for (int i = 0; i < num; i++)
            {
                var center = new Vector3(random.Next(0, Width), random.Next(0, Height), random.Next(50, 200));
                var size = random.Next(10, 100);
                var xRotation = random.Next(0, 180);
                var yRotation = random.Next(0, 180);
                var zRotation = random.Next(0, 180);
                var color = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
                AddCube(center, size, new Vector3(xRotation, yRotation, zRotation), color);
            }
        }

        public Color ComputeDiffusionColor(Vector3 intersectionPoint, Vector3 surfaceNormal, Color objectColor)
        {
            // Start with zero light contribution
            var diffuseLight = new Color(0, 0, 0);

            // Accumulate contributions from all diffused lights
            foreach (var light in DiffusedLights)
            {
                var lightDirection = (light.Position - intersectionPoint).Normalize();
                var diffuseFactor = Math.Max(0, lightDirection.ScalarProduct(surfaceNormal));

                // Add this light's contribution
                diffuseLight += (light.Color * diffuseFactor * light.Intensity * objectColor);
            }

            return diffuseLight;
        }

        public Color ComputeAmbientColor(Color objectColor)
        {
            return AmbientLight.Color * AmbientLight.Intensity * objectColor;
        }

        public void Dispose()
        {
            Bitmap?.Dispose();
        }
    }
}
