using Common.Objects;
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

            var lambda1 = (-b - MathF.Sqrt(discriminant)) / (2 * a);
            var lambda2 = (-b + MathF.Sqrt(discriminant)) / (2 * a);

            // Get the closest valid intersection
            float lambda;
            bool entering;

            if (lambda1 > 0)
            {
                // First intersection is valid (ray starts outside the sphere)
                lambda = lambda1;
                entering = true;
            }
            else if (lambda2 > 0)
            {
                // First intersection is invalid but second is valid
                // (ray starts inside the sphere)
                lambda = lambda2;
                entering = false;
            }
            else
            {
                // Both intersections are behind the ray origin
                return false;
            }

            // Calculate intersection point with adjusted lambda
            // Apply epsilon in the right direction based on whether we're entering or exiting
            lambda = entering ? lambda - ITraceableObject.eps : lambda + ITraceableObject.eps;

            var intersectionPoint = ray.Origin + (ray.Direction * lambda);
            var surfaceNormal = Vector3.Normalize(intersectionPoint - Center);

            // Ensure normal points against the ray direction for correct shading
            if (!entering)
            {
                surfaceNormal = -surfaceNormal;
            }

            hit = new Hit(intersectionPoint, surfaceNormal, Material, lambda);

            return true;
        }
    }
}