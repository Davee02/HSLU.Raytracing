using System.Numerics;

namespace Common
{
    public readonly struct Rectangle
    {
        public Rectangle(Vector3 origin, Vector3 w, Vector3 v, Material material)
        {
            // Create two triangles that form the rectangle
            Triangle1 = new Triangle(origin, w, v, material);
            Triangle2 = new Triangle(origin + w + v, -w, -v, material);
        }

        public readonly Triangle Triangle1 { get; }

        public readonly Triangle Triangle2 { get; }
    }
}
