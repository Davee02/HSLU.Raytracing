using Common.Objects;
using System.Numerics;

namespace Common.BVH;
public class BVH
{
    private const int MaxObjectsPerLeaf = 6;

    public BVHNode Root { get; }

    public int Depth { get; private set; }

    public BVH(List<ITraceableObject> sceneObjects)
    {
        Root = BuildBVH(sceneObjects, 0);
    }

    private BVHNode BuildBVH(List<ITraceableObject> objects, int depth)
    {
        if (depth > Depth)
        {
            Depth = depth;
        }

        // If few enough objects, create a leaf node
        if (objects.Count <= MaxObjectsPerLeaf)
        {
            return new BVHNode(objects);
        }

        // Choose a splitting axis (x, y, or z)
        int axis = ChooseSplitAxis(objects);

        // Sort objects along the chosen axis
        SortObjectsByAxis(objects, axis);

        // Split in the middle
        int mid = objects.Count / 2;
        var leftObjects = objects.GetRange(0, mid);
        var rightObjects = objects.GetRange(mid, objects.Count - mid);

        // Recursively build left and right subtrees
        var left = BuildBVH(leftObjects, depth + 1);
        var right = BuildBVH(rightObjects, depth + 1);

        // Create and return an internal node
        return new BVHNode(left, right);
    }

    private static int ChooseSplitAxis(List<ITraceableObject> objects)
    {
        // Compute the bounding box of all objects
        var bounds = AABBHelper.ComputeBoundingBox(objects);
        var extent = bounds.Max - bounds.Min;

        // Choose the axis with the largest extent
        if (extent.X > extent.Y && extent.X > extent.Z)
        {
            return 0; // X-axis
        }
        else if (extent.Y > extent.Z)
        {
            return 1; // Y-axis
        }
        else
        {
            return 2; // Z-axis
        }
    }

    private static void SortObjectsByAxis(List<ITraceableObject> objects, int axis)
    {
        objects.Sort((a, b) =>
        {
            var boxA = GetObjectCentroid(a);
            var boxB = GetObjectCentroid(b);

            float valueA = axis == 0 ? boxA.X : (axis == 1 ? boxA.Y : boxA.Z);
            float valueB = axis == 0 ? boxB.X : (axis == 1 ? boxB.Y : boxB.Z);

            return valueA.CompareTo(valueB);
        });
    }

    private static Vector3 GetObjectCentroid(ITraceableObject obj)
    {
        // Get the centroid for different object types
        if (obj is Triangle triangle)
        {
            return triangle.Origin + ((triangle.V + triangle.W) / 3.0f);
        }
        else if (obj is Sphere sphere)
        {
            return sphere.Center;
        }
        else
        {
            throw new NotSupportedException($"Unsupported object type: {obj.GetType()}");
        }
    }
}
