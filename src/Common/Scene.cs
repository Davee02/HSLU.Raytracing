using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Common
{
    public struct Scene(int width, int height, int depth) : IDisposable
    {
        public int Width { get; set; } = width;

        public int Height { get; set; } = height;

        public int Depth { get; set; } = depth;

        public List<Sphere> Spheres { get; } = [];

        public Light DiffusedLight { get; set; }

        public Light AmbientLight { get; set; }

        public Color BackgroundColor { get; set; } = new Color(0, 0, 0);

        public Image<Rgba32> Bitmap { get; } = new Image<Rgba32>(width, height);

        public readonly void AddObject(Sphere sphere) => Spheres.Add(sphere);

        public void Dispose()
        {
            Bitmap?.Dispose();
        }

        public void GenerateRandomSpheres(int num)
        {
            var random = new Random();
            for (int i = 0; i < num; i++)
            {
                var center = new Vector3(random.Next(0, Width), random.Next(0, Height), random.Next(50, 200));
                var radius = random.Next(10, 100);
                var color = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
                AddObject(new Sphere(center, radius, color));
            }
        }
    }
}
