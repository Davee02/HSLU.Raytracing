using System.Numerics;

namespace Common
{
    public readonly struct Triangle(Vector3 origin, Vector3 v, Vector3 w, Vector3 normal, Material material) : ITraceableObject
    {
        public Vector3 Origin { get; } = origin;

        public Vector3 V { get; } = v;

        public Vector3 W { get; } = w;

        public Vector3 Normal { get; } = normal;

        public Material Material { get; } = material;

        // Constructor without an explicit normal (calculates it)
        public Triangle(Vector3 origin, Vector3 v, Vector3 w, Material material)
            : this(origin, v, w, Vector3.Normalize(Vector3.Cross(w, v)), material)
        {
        }

        public bool TryIntersect(Ray ray, ref Hit hit)
        {
            var u = ray.Direction;
            var A = new Matrix3x3(
                u.X, -V.X, -W.X,
                u.Y, -V.Y, -W.Y,
                u.Z, -V.Z, -W.Z);
            var B = Origin - ray.Origin;

            var hasInverse = A.TryInverse(out var inverse);
            if (!hasInverse)
            {
                return false;
            }

            var tmp = inverse.Multiply(B);
            var (lambda, tau, mu) = (tmp.X, tmp.Y, tmp.Z);

            var intersects = lambda > 0 && tau >= 0 && mu >= 0 && tau + mu <= 1;
            if (!intersects)
            {
                return false;
            }

            lambda -= ITraceableObject.eps; // move the intersection point a bit towards the camera to avoid self-intersection
            var intersectionPoint = ray.Origin + (ray.Direction * lambda);

            // Use the provided normal instead of calculating it
            var surfaceNormal = Normal;

            // Check if we're hitting the back face of the triangle
            // If so, we need to flip the normal for proper refraction
            var fromFront = Vector3.Dot(ray.Direction, surfaceNormal) < 0;
            if (!fromFront)
            {
                surfaceNormal = -surfaceNormal;
            }
            hit = new Hit(intersectionPoint, surfaceNormal, Material, lambda);
            return true;
        }
    }
}
