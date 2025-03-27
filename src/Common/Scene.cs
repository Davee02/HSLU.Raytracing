using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using System.Numerics;

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
    }
}
