namespace Common;
public readonly struct Hit(System.Numerics.Vector3 position, System.Numerics.Vector3 normal, Color color, float lambda)
{
    public System.Numerics.Vector3 Position { get; } = position;

    public System.Numerics.Vector3 Normal { get; } = normal;

    public Color Color { get; } = color;

    public float Lambda { get; } = lambda;
}
