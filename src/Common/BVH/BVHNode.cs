using Common.Objects;

namespace Common.BVH;
public class BVHNode
{
    public AABB BoundingBox { get; }

    public BVHNode? Left { get; }

    public BVHNode? Right { get; }

    public List<ITraceableObject>? Objects { get; } // Only leaf nodes have objects

    // Constructor for leaf nodes
    public BVHNode(List<ITraceableObject> objects)
    {
        Objects = objects;
        BoundingBox = AABBHelper.ComputeBoundingBox(objects);
        Left = null;
        Right = null;
    }

    // Constructor for internal nodes
    public BVHNode(BVHNode left, BVHNode right)
    {
        Left = left;
        Right = right;
        BoundingBox = AABB.Union(left.BoundingBox, right.BoundingBox);
        Objects = null;
    }
}
