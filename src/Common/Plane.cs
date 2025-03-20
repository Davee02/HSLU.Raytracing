using System.Numerics;

namespace Common
{
    public readonly struct Plane(Vector3 position, Vector3 rotationAnglesDegrees, Color color) : ITraceableObject
    {
        public Vector3 Position { get; } = position;

        public Vector3 Normal { get; } = CalculateNormal(rotationAnglesDegrees);

        public Color Color { get; } = color;

        private static Vector3 CalculateNormal(Vector3 rotationAnglesDegrees)
        {
            // Convert rotation angles from degrees to radians
            var rotationAnglesRadians = new Vector3(
                rotationAnglesDegrees.X.ToRadians(),
                rotationAnglesDegrees.Y.ToRadians(),
                rotationAnglesDegrees.Z.ToRadians());

            // Start with a normal pointing up (0, -1, 0)
            var baseNormal = new Vector3(0, -1, 0);

            // Create a rotation matrix
            var rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(rotationAnglesRadians.Y, rotationAnglesRadians.X, rotationAnglesRadians.Z);

            // Apply rotation to the normal
            return baseNormal.Transform(rotationMatrix).Normalize();
        }

        public bool Intersect(Ray ray, out float lambda)
        {
            // Calculate the denominator: dot product of ray direction and plane normal
            var denominator = ray.Direction.ScalarProduct(Normal);

            // If denominator is close to 0, ray is parallel to the plane (no intersection)
            if (Math.Abs(denominator) < 1e-6)
            {
                lambda = 0;
                return false;
            }

            // Calculate distance from ray origin to the plane
            // Using the plane equation: dot(Normal, P - Position) = 0
            // Where P is any point on the plane
            var numerator = Normal.ScalarProduct(Position - ray.Origin);

            // Calculate intersection distance
            lambda = numerator / denominator;

            // Intersection happens if lambda is positive (in front of the camera)
            return lambda > 0;
        }

        public Vector3 SurfaceNormal(Vector3 _) => Normal;
    }
}