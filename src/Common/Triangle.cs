using System.Numerics;

namespace Common
{
    public readonly struct Triangle(Vector3 origin, Vector3 v, Vector3 w, Material material) : ITraceableObject
    {
        public Vector3 Origin { get; } = origin;

        public Vector3 V { get; } = v;

        public Vector3 W { get; } = w;

        public Material Material { get; } = material;

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

            if(!intersects)
            {
                return false;
            }

            lambda -= ITraceableObject.eps; // move the intersection point a bit towards the camera to avoid self-intersection

            var intersectionPoint = ray.Origin + (ray.Direction * lambda);
            var surfaceNormal = Vector3.Normalize(Vector3.Cross(W, V));
            hit = new Hit(intersectionPoint, surfaceNormal, Material, lambda);

            return true;
        }
    }
}
