namespace Common
{
    public readonly struct Triangle(Vector3 origin, Vector3 v, Vector3 w, Color color) : ITraceableObject
    {
        public Vector3 Origin { get; } = origin;

        public Vector3 V { get; } = v;

        public Vector3 W { get; } = w;

        public Color Color { get; } = color;

        public bool Intersect(Ray ray, out float lambda)
        {
            lambda = 0;

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

            (lambda, var tau, var mu) = inverse.Multiply(B);
            
            var intersects = lambda > 0 && tau >= 0 && mu >= 0 && tau + mu <= 1;
            return intersects;
        }

        public Vector3 SurfaceNormal(Vector3 _) => W.CrossProduct(V).Normalize();
    }
}
