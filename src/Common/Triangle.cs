namespace Common
{
    public readonly struct Triangle(Vector3 p1, Vector3 p2, Vector3 p3, Color color)
    {
        public Vector3 P1 { get; } = p1;

        public Vector3 P2 { get; } = p2;

        public Vector3 P3 { get; } = p3;

        public Color Color { get; } = color;

        public bool Intersect(Ray ray, out float lambda)
        {
            lambda = 0;

            var u = ray.Direction;
            var v = P2 - P1;
            var w = P3 - P1;

            var A = new Matrix3x3(
                u.X, -v.X, -w.X,
                u.Y, -v.Y, -w.Y,
                u.Z, -v.Z, -w.Z);

            var b = P1 - ray.Origin;

            var hasInverse = A.TryInverse(out var inverse);
            if (!hasInverse)
            {
                return false;
            }

            (lambda, var tau, var mu) = inverse.Multiply(b);
            
            var intersects = lambda > 0 && tau >= 0 && mu >= 0 && tau + mu <= 1;
            return intersects;
        }
    }
}
