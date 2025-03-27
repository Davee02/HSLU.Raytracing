using System.Numerics;

namespace Common
{
    public readonly struct Plane(Vector3 position, Vector3 rotationAnglesDegrees, Material material) : ITraceableObject
    {
        public Vector3 Position { get; } = position;

        public Vector3 Normal { get; } = CalculateNormal(rotationAnglesDegrees);

        public Material Material { get; } = material;

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
            return Vector3.Normalize(Vector3.Transform(baseNormal, rotationMatrix));
        }

        public bool TryIntersect(Ray ray, ref Hit hit)
        {
            // Calculate the denominator: dot product of ray direction and plane normal
            var denominator = Vector3.Dot(ray.Direction, Normal);

            // If denominator is close to 0, ray is parallel to the plane (no intersection)
            if (Math.Abs(denominator) < 1e-6)
            {
                return false;
            }

            // Calculate distance from ray origin to the plane
            // Using the plane equation: dot(Normal, P - Position) = 0
            // Where P is any point on the plane
            var numerator = Vector3.Dot(Normal, Position - ray.Origin);

            // Calculate intersection distance
            var lambda = numerator / denominator;

            // Intersection happens if lambda is positive (in front of the camera)
            if (lambda < 0)
            {
                return false;
            }

            lambda -= ITraceableObject.eps; // move the intersection point a bit towards the camera to avoid self-intersection

            var intersectionPoint = ray.Origin + (ray.Direction * lambda);
            hit = new Hit(intersectionPoint, Normal, Material, lambda);

            return true;
        }
    }
}