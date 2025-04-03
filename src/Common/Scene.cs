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


        public readonly void AddRectangle(Vector3 position, Vector3 normal, Vector3 up, float width, float height, Material material)
        {
            var rectangle = new Rectangle(position, normal, up, width, height, material);
            TraceableObjects.Add(rectangle);
        }

        public readonly void AddRectangle(Rectangle rectangle)
        {
            TraceableObjects.Add(rectangle);
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

        public static Scene CreateCornellBoxScene()
        {
            var scene = new Scene(new Vector2(1920f, 1080f))
            {
                AmbientLight = new Light
                {
                    Color = new Common.Color(1, 1, 1),
                    Intensity = 0.2f
                },
                BackgroundColor = Color.Black
            };

            // Room dimensions
            float roomSize = 800f;
            float halfSize = roomSize / 2f;
            var roomCenter = new Vector3(1000, 500, 500);

            scene.Camera = new Camera(
                position: new Vector3(roomCenter.X, roomCenter.Y, roomCenter.Z - 2 * halfSize),
                viewPort: new Vector2(1920f, 1080f),
                imageSize: new Vector2(1920f, 1080f),
                fieldOfView: 60);

            // Add diffuse lights
            scene.DiffusedLights.Add(new Light
            {
                Position = new Vector3(roomCenter.X, roomCenter.Y - halfSize * 0.8f, roomCenter.Z),
                Color = Color.White,
                Intensity = 1.0f
            });

            // Materials for walls
            var redMaterial = new Material(Color.Red, 0.2f, 20, 0f);
            var greenMaterial = new Material(Color.Green, 0.2f, 20, 0f);
            var whiteMaterial = new Material(Color.White, 0f, 20, 0f);
            var blueMaterial = new Material(Color.Cyan, 0.2f, 20, 0f);
            var yellowMaterial = new Material(Color.Yellow, 0.2f, 20, 0f);

            // Create the walls of the Cornell box

            // Back wall (blue)
            //scene.AddRectangle(Rectangle.CreateWall(
            //    new Vector3(roomCenter.X, roomCenter.Y, roomCenter.Z + halfSize),
            //    new Vector3(0, 0, -1),
            //    roomSize,
            //    blueMaterial));

            // Left wall (purple)
            scene.AddRectangle(Rectangle.CreateWall(
                new Vector3(roomCenter.X - halfSize, roomCenter.Y, roomCenter.Z),
                new Vector3(1, 0, 0),
                roomSize,
                redMaterial));

            // Right wall (yellow)
            scene.AddRectangle(Rectangle.CreateWall(
                new Vector3(roomCenter.X + halfSize, roomCenter.Y, roomCenter.Z),
                new Vector3(-1, 0, 0),
                roomSize,
                yellowMaterial));

            // Floor (white)
            scene.AddRectangle(Rectangle.CreateWall(
                new Vector3(roomCenter.X, roomCenter.Y + halfSize, roomCenter.Z),
                new Vector3(0, -1, 0),
                roomSize,
                whiteMaterial));

            // Ceiling (cyan)
            scene.AddRectangle(Rectangle.CreateWall(
                new Vector3(roomCenter.X, roomCenter.Y - halfSize, roomCenter.Z),
                new Vector3(0, 1, 0),
                roomSize,
                blueMaterial));

            // Add spheres

            // White reflective sphere
            scene.AddSphere(
                new Vector3(roomCenter.X - halfSize * 0.3f, roomCenter.Y + halfSize * 0.5f, roomCenter.Z),
                100,
                new Material(Color.White, 0.7f, 50, 0f));

            // Gold sphere
            scene.AddSphere(
                new Vector3(roomCenter.X + halfSize * 0.3f, roomCenter.Y + halfSize * 0.5f, roomCenter.Z),
                100,
                new Material(new Color(1.0f, 0.8f, 0.2f), 0.5f, 100, 0f));

            // Small cyan sphere
            scene.AddSphere(
                new Vector3(roomCenter.X, roomCenter.Y + halfSize * 0.7f, roomCenter.Z - halfSize * 0.3f),
                50,
                new Material(Color.Cyan, 0f, 20, 0f));

            return scene;
        }
    }
}
