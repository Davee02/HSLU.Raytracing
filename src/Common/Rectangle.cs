using System.Numerics;

namespace Common
{
    public readonly struct Rectangle : ITraceableObject
    {
        public Vector3 Position { get; } // Center of the rectangle
        
        public Vector3 Normal { get; } // Normal vector (perpendicular to rectangle)

        public Vector3 Width { get; }

        public Vector3 Height { get; }

        public float WidthExtent { get; } // Half width

        public float HeightExtent { get; } // Half height

        public Material Material { get; }

        public Rectangle(Vector3 position, Vector3 normal, Vector3 up, float width, float height, Material material)
        {
            Position = position;
            Normal = Vector3.Normalize(normal);

            // Calculate width direction (right vector) using cross product of up and normal
            Width = Vector3.Normalize(Vector3.Cross(up, Normal));

            // Calculate true height direction (up vector) using cross product of normal and width
            Height = Vector3.Normalize(Vector3.Cross(Normal, Width));

            WidthExtent = width / 2f;
            HeightExtent = height / 2f;
            Material = material;
        }

        // Convenience constructor to create full planes for the Cornell box
        public static Rectangle CreateWall(Vector3 center, Vector3 normal, float size, Material material)
        {
            var up = normal.Y != 0 ? new Vector3(0, 0, 1) : new Vector3(0, 1, 0);
            return new Rectangle(center, normal, up, size, size, material);
        }

        public bool TryIntersect(Ray ray, ref Hit hit)
        {
            // Calculate the denominator: dot product of ray direction and rectangle normal
            var denominator = Vector3.Dot(ray.Direction, Normal);

            // If denominator is close to 0, ray is parallel to the rectangle (no intersection)
            if (Math.Abs(denominator) < 1e-6)
            {
                return false;
            }

            // Calculate distance from ray origin to the rectangle plane
            var numerator = Vector3.Dot(Normal, Position - ray.Origin);

            // Calculate intersection distance
            var lambda = numerator / denominator;

            // Intersection happens if lambda is positive (in front of the camera)
            if (lambda < 0)
            {
                return false;
            }

            // Whether we're entering or exiting depends on the normal and ray direction
            bool entering = denominator < 0; // Ray is coming from the front of the surface

            // Apply epsilon correction based on entering/exiting
            lambda = entering ? lambda - ITraceableObject.eps : lambda + ITraceableObject.eps;

            // Calculate the intersection point
            var intersectionPoint = ray.Origin + (ray.Direction * lambda);

            // Check if the intersection point is within the rectangle bounds
            var relativePos = intersectionPoint - Position;
            var projWidth = Vector3.Dot(relativePos, Width);
            var projHeight = Vector3.Dot(relativePos, Height);

            if (Math.Abs(projWidth) > WidthExtent || Math.Abs(projHeight) > HeightExtent)
            {
                return false; // Outside rectangle bounds
            }

            // Create the hit object
            hit = new Hit(intersectionPoint, Normal, Material, lambda);
            return true;
        }
    }
}