using System.Numerics;

namespace Common;
public readonly struct Hit(Vector3 position, Vector3 normal, Material material, float lambda)
{
    public Vector3 Position { get; } = position;

    public Vector3 Normal { get; } = normal;

    public Material Material { get; } = material;

    public float Lambda { get; } = lambda;
}
