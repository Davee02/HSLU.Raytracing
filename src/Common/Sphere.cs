namespace Common
{
    public readonly struct Sphere(Vector3 center, float radius, Color color)
    {
        public Vector3 Center { get; } = center;

        public float Radius { get; } = radius;

        public Color Color { get; } = color;

        public bool Intersect(Ray ray, out float lambda)
        {
            lambda = 0;

            var v = ray.Origin - Center;
            var a = ray.Direction.ScalarProduct(ray.Direction);
            var b = 2 * v.ScalarProduct(ray.Direction);
            var c = v.ScalarProduct(v) - (Radius * Radius);
            var discriminant = (b * b) - (4 * a * c);
            if (discriminant < 0)
            {
                return false;
            }

            lambda = (-b - MathF.Sqrt(discriminant)) / (2 * a); // we can always choose the smaller lambda as the intersection point

            return true;
        }
    }
}
