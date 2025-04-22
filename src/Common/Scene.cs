using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Numerics;
using System.Text;

namespace Common
{
    public struct Scene(Vector2 imageSize) : IDisposable
    {
        public RenderSettings RenderSettings { get; set; } = new();

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


        public readonly void AddRectangle(Vector3 position, Vector3 normal, Vector3 up, float width, float height, float thickness, Material material)
        {
            var rectangle = new Rectangle(position, normal, up, width, height, thickness, material);
            AddRectangle(rectangle);
        }

        public readonly void AddRectangle(Rectangle rectangle)
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

        public string PrintInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Image resolution: {ImageSize.X}x{ImageSize.Y}");
            sb.AppendLine($"Number of spheres: {Spheres.Count}");
            sb.AppendLine($"Number of planes: {Planes.Count}");
            sb.AppendLine($"Number of triangles: {Triangles.Count}");
            return sb.ToString();
        }


        public readonly void Render()
        {
            var thisScene = this; // needed for the lambda function
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
            }));
        }

        public static Scene CreateCornellBoxScene()
        {
            var scene = new Scene(new Vector2(1280f / 2, 720f / 2))
            {
                AmbientLight = new Light
                {
                    Color = new Color(1, 1, 1),
                    AttenuationC = 0.2f
                },
                BackgroundColor = Color.Black
            };

            // Room dimensions
            float roomSize = 800f;
            float halfSize = roomSize / 2f;
            var roomCenter = new Vector3(1000, 500, 500);

            scene.Camera = new Camera(
                position: new Vector3(roomCenter.X, roomCenter.Y, roomCenter.Z - (2 * halfSize)),
                lookAt: new Vector3(1000, 500, 500),
                up: new Vector3(0, -1, 0),
                imageWidth: scene.ImageSize.X,
                imageHeight: scene.ImageSize.Y,
                fieldOfView: 60,
                sampleCount: 1);

            // Add diffuse lights
            scene.DiffusedLights.Add(new Light
            {
                Position = new Vector3(roomCenter.X, roomCenter.Y - 200, 0),
                Color = Color.White,
                AttenuationA = 1e-6f,
                AttenuationC = 0.7f,
            });

            // Materials for walls
            var redMaterial = new Material(Color.Red, 0.2f, 20, 0f);
            var greenMaterial = new Material(Color.Green, 0.2f, 20, 0f);
            var whiteMaterial = new Material(Color.White, 0f, 20, 0f);
            var glassMaterial = new Material(new Color(0.8f, 0.8f, 1.0f), 0.1f, 50, 0.8f, 10.5f);
            var cyanMaterial = new Material(Color.Cyan, 0.2f, 20, 0f);
            var yellowMaterial = new Material(Color.Yellow, 0.2f, 20, 0f);

            // Create the walls of the Cornell box

            // Back wall (transparent white)
            scene.AddRectangle(Rectangle.CreateWall(
                new Vector3(roomCenter.X, roomCenter.Y, roomCenter.Z + halfSize),
                new Vector3(0, 0, -1),
                roomSize,
                50,
                glassMaterial));

            // Left wall (purple)
            scene.AddRectangle(Rectangle.CreateWall(
                new Vector3(roomCenter.X - halfSize, roomCenter.Y, roomCenter.Z),
                new Vector3(1, 0, 0),
                roomSize,
                5,
                redMaterial));

            // Right wall (yellow)
            scene.AddRectangle(Rectangle.CreateWall(
                new Vector3(roomCenter.X + halfSize, roomCenter.Y, roomCenter.Z),
                new Vector3(-1, 0, 0),
                roomSize,
                5,
                yellowMaterial));

            // Floor (white)
            scene.AddRectangle(Rectangle.CreateWall(
                new Vector3(roomCenter.X, roomCenter.Y + halfSize, roomCenter.Z),
                new Vector3(0, -1, 0),
                roomSize,
                5,
                whiteMaterial));

            // Ceiling (cyan)
            scene.AddRectangle(Rectangle.CreateWall(
                new Vector3(roomCenter.X, roomCenter.Y - halfSize, roomCenter.Z),
                new Vector3(0, 1, 0),
                roomSize,
                5,
                cyanMaterial));

            // Add spheres

            // White reflective sphere
            scene.AddSphere(
                new Vector3(roomCenter.X - (halfSize * 0.3f), roomCenter.Y + (halfSize * 0.5f), roomCenter.Z),
                100,
                new Material(Color.White, 0.7f, 50, 0f));

            // Transmissive sphere
            scene.AddSphere(
                new Vector3(roomCenter.X + (halfSize * 0.3f), roomCenter.Y + (halfSize * 0.5f), 600),
                100,
                new Material(Color.Red, 0f, 100, 0.9f, 1.2f));

            // Sphere behind transmissive sphere
            scene.AddSphere(
                new Vector3(roomCenter.X + (halfSize * 0.3f), roomCenter.Y + (halfSize * 0.5f), 800),
                50,
                new Material(Color.Green, 0f, 20, 0f));

            // Small cyan sphere
            scene.AddSphere(
                new Vector3(roomCenter.X, roomCenter.Y + (halfSize * 0.7f), roomCenter.Z - (halfSize * 0.3f)),
                50,
                new Material(Color.Cyan, 0f, 20, 0f));

            // Blue sphere in the back
            scene.AddSphere(
                new Vector3(roomCenter.X, 500, 1000),
                100,
                new Material(Color.Blue, 0f, 20, 0f));

            return scene;
        }
    }
}
