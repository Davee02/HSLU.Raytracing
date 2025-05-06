using Common.Objects;
using System.Numerics;

namespace Common.BVH;
internal static class AABBHelper
{
    // Compute a bounding box for a list of objects
    internal static AABB ComputeBoundingBox(List<ITraceableObject> objects)
    {
        if (objects.Count == 0)
        {
            return new AABB(Vector3.Zero, Vector3.Zero);
        }

        var initialBox = GetObjectBounds(objects[0]);
        var min = initialBox.Min;
        var max = initialBox.Max;

        for (int i = 1; i < objects.Count; i++)
        {
            var box = GetObjectBounds(objects[i]);
            min = Vector3.Min(min, box.Min);
            max = Vector3.Max(max, box.Max);
        }

        return new AABB(min, max);
    }

    // Helper to get bounds for different object types
    internal static AABB GetObjectBounds(ITraceableObject obj)
    {
        // Handle different object types
        if (obj is Triangle triangle)
        {
            return new AABB(
            [
                triangle.Origin,
                triangle.Origin + triangle.V,
                triangle.Origin + triangle.W
            ]);
        }
        else if (obj is Sphere sphere)
        {
            var radius = new Vector3(sphere.Radius);
            return new AABB(sphere.Center - radius, sphere.Center + radius);
        }
        else
        {
            throw new NotSupportedException($"Unsupported object type: {obj.GetType()}");
        }
    }
}
