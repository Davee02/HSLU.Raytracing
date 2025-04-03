using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Numerics;
using System.Text;

namespace Common
{
    public struct Scene(Vector2 imageSize) : IDisposable
    {
        public Vector2 ImageSize { get; } = imageSize;

        public List<Sphere> Spheres { get; } = [];

        public List<Plane> Planes { get; } = [];

        public List<Triangle> Triangles { get; } = [];

        public List<ITraceableObject> TraceableObjects { get; } = [];

        public List<Light> DiffusedLights { get; set; } = [];

        public Camera Camera { get; set; }

        public Light AmbientLight { get; set; }

        public Color BackgroundColor { get; set; } = new Color(0, 0, 0);

        public Image<Rgba32> Bitmap { get; } = new Image<Rgba32>((int)imageSize.X, (int)imageSize.Y);

        public readonly void AddSphere(Vector3 center, float radius, Material color) 
        {
            var sphere = new Sphere(center, radius, color);
            Spheres.Add(sphere);
            TraceableObjects.Add(sphere);
        }

        public readonly void AddPlane(Vector3 position, Vector3 rotationAnglesDegrees, Material material)
        {
            var plane = new Plane(position, rotationAnglesDegrees, material);
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


        public readonly void AddRectangle(Vector3 origin, Vector3 w, Vector3 v, Material material)
        {
            var rectangle = new Rectangle(origin, w, v, material);
            AddTriangle(rectangle.Triangle1);
            AddTriangle(rectangle.Triangle2);
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
            return AmbientLight.Color * AmbientLight.Intensity * objectMaterial.Color;
        }

        public void Dispose()
        {
            Bitmap?.Dispose();
        }

        public string PrintInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Image resolution: {ImageSize.X}x{ImageSize.Y}");
            sb.AppendLine($"Number of spheres: {Spheres.Count}");
            sb.AppendLine($"Number of planes: {Planes.Count}");
            sb.AppendLine($"Number of triangles: {Triangles.Count}");
            return sb.ToString();
        }
    }
}
