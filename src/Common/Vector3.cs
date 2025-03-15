using System.Numerics;

namespace Common;

public readonly struct Vector3(float x, float y, float z)
{
    public readonly float X => x;

    public readonly float Y => y;

    public readonly float Z => z;

    public float Length
        => MathF.Sqrt(this.LengthSquared);

    public float LengthSquared
        => (X * X) + (Y * Y) + (Z * Z);

    public static Vector3 operator +(Vector3 a, Vector3 b)
        => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Vector3 operator -(Vector3 a, Vector3 b)
        => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static Vector3 operator -(Vector3 a)
        => new(-a.X, -a.Y, -a.Z);

    public static Vector3 operator *(Vector3 a, int scalar)
        => new(a.X * scalar, a.Y * scalar, a.Z * scalar);

    public static Vector3 operator *(Vector3 a, float scalar)
        => new(a.X * scalar, a.Y * scalar, a.Z * scalar);

    public void Deconstruct(out float x, out float y, out float z)
    {
        x = X;
        y = Y;
        z = Z;
    }

    public readonly float EuclideanDistance(Vector3 other)
    {
        var distance = this - other;
        return MathF.Sqrt((distance.X * distance.X) + (distance.Y * distance.Y) + (distance.Z * distance.Z));
    }

    public float ScalarProduct(Vector3 other)
        => (this.X * other.X) + (this.Y * other.Y) + (this.Z * other.Z);

    public float ScalarProduct(Vector3 other, float angle)
        => this.Length * other.Length * MathF.Cos(angle);

    public Vector3 Normalize()
    {
        var length = this.Length;
        return new Vector3(X / length, Y / length, Z / length);
    }

    public Vector3 CrossProduct(Vector3 other)
        => new((Y * other.Z) - (Z * other.Y), (Z * other.X) - (X * other.Z), (X * other.Y) - (Y * other.X));

    /// <summary>
    /// Applies a homogeneous 4x4 transformation matrix to this vector.
    /// </summary>
    public Vector3 Transform(Matrix4x4 matrix)
    {
        float xNew = (X * matrix.M11) + (Y * matrix.M21) + (Z * matrix.M31) + matrix.M41;
        float yNew = (X * matrix.M12) + (Y * matrix.M22) + (Z * matrix.M32) + matrix.M42;
        float zNew = (X * matrix.M13) + (Y * matrix.M23) + (Z * matrix.M33) + matrix.M43;

        return new(xNew, yNew, zNew);
    }
}
