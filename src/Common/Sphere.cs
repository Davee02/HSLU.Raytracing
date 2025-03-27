using System.Numerics;

namespace Common
{
    public readonly struct Sphere(Vector3 center, float radius, Material material) : ITraceableObject
    {
        public Vector3 Center { get; } = center;

        public float Radius { get; } = radius;

        public Material Material { get; } = material;

        public bool TryIntersect(Ray ray, ref Hit hit)
        {
            var v = ray.Origin - Center;
            var a = Vector3.Dot(ray.Direction, ray.Direction);
            var b = 2 * Vector3.Dot(v, ray.Direction);
            var c = Vector3.Dot(v, v) - (Radius * Radius);

            var discriminant = (b * b) - (4 * a * c);
            if (discriminant < 0)
            {
                return false;
            }

            var lambda = (-b - MathF.Sqrt(discriminant)) / (2 * a); // we can always choose the smaller lambda as the intersection point

            if (lambda < 0)
            {
                // the intersection point is behind the camera
                return false;
            }

            lambda -= ITraceableObject.eps; // move the intersection point a bit towards the camera to avoid self-intersection

            var intersectionPoint = ray.Origin + (ray.Direction * lambda);
            var surfaceNormal = Vector3.Normalize(intersectionPoint - Center);
            hit = new Hit(intersectionPoint, surfaceNormal, Material, lambda);

            return true;
        }
    }
}
