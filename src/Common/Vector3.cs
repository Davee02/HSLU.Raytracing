namespace Common;

public readonly struct Vector3(int x, int y, int z)
{
    public readonly int X => x;

    public readonly int Y => y;

    public readonly int Z => z;

    public double Length => Math.Sqrt(this.LengthSquared);

    public float LengthSquared => (X * X) + (Y * Y) + (Z * Z);

    public static Vector3 operator +(in Vector3 a, in Vector3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Vector3 operator -(in Vector3 a, Vector3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static Vector3 operator *(in Vector3 a, in int scalar) => new(a.X * scalar, a.Y * scalar, a.Z * scalar);

    public readonly double EuclideanDistance(in Vector3 other)
    {
        var distance = this - other;
        return Math.Sqrt(distance.X * distance.X + distance.Y * distance.Y + distance.Z * distance.Z);
    }

    public float ScalarProduct(in Vector3 other) => (this.X * other.X) + (this.Y * other.Y) + (this.Z * other.Z);

    public double ScalarProduct(in Vector3 other, float angle) => this.Length * other.Length * Math.Cos(angle);
}
