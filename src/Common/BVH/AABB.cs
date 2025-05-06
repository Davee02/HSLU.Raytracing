using System.Numerics;

namespace Common.BVH;
public readonly struct AABB
{
    public Vector3 Min { get; }

    public Vector3 Max { get; }

    public AABB(Vector3 min, Vector3 max)
    {
        Min = min;
        Max = max;
    }

    // Create an AABB containing multiple points
    public AABB(IEnumerable<Vector3> points)
    {
        Min = new Vector3(float.MaxValue);
        Max = new Vector3(float.MinValue);

        foreach (var point in points)
        {
            Min = Vector3.Min(Min, point);
            Max = Vector3.Max(Max, point);
        }
    }

    // Create a bounding box that encompasses two bounding boxes
    public static AABB Union(AABB a, AABB b)
    {
        return new AABB(
            Vector3.Min(a.Min, b.Min),
            Vector3.Max(a.Max, b.Max)
        );
    }

    // Test for ray-box intersection
    public bool Intersect(Ray ray, out float tMin, out float tMax)
    {
        tMin = float.MinValue;
        tMax = float.MaxValue;

        // For each axis (x, y, z)
        for (int i = 0; i < 3; i++)
        {
            float rayOriginComponent = i == 0 
                ? ray.Origin.X 
                : (i == 1 
                    ? ray.Origin.Y 
                    : ray.Origin.Z);
            float rayDirectionComponent = i == 0 
                ? ray.Direction.X 
                : (i == 1 
                    ? ray.Direction.Y 
                    : ray.Direction.Z);
            float boxMinComponent = i == 0 
                ? Min.X 
                    : (i == 1 
                    ? Min.Y 
                    : Min.Z);
            float boxMaxComponent = i == 0 
                ? Max.X 
                : (i == 1 
                    ? Max.Y 
                    : Max.Z);

            if (MathF.Abs(rayDirectionComponent) < float.Epsilon)
            {
                // Ray is parallel to slab. No hit if origin is outside the slab
                if (rayOriginComponent < boxMinComponent || rayOriginComponent > boxMaxComponent)
                {
                    return false;
                }
            }
            else
            {
                // Calculate intersection with slab
                float invD = 1.0f / rayDirectionComponent;
                float t0 = (boxMinComponent - rayOriginComponent) * invD;
                float t1 = (boxMaxComponent - rayOriginComponent) * invD;

                // Swap if necessary
                if (t0 > t1)
                {
                    (t0, t1) = (t1, t0);
                }

                // Update tMin and tMax
                tMin = MathF.Max(tMin, t0);
                tMax = MathF.Min(tMax, t1);

                // Check if there's a valid intersection interval
                if (tMin > tMax)
                {
                    return false;
                }
            }
        }

        return true;
    }
}
