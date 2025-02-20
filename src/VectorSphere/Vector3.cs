namespace VectorSphere;

internal struct Vector3(int x, int y, int z)
{
    public int X => x;

    public int Y => y;

    public int Z => z;

    public static Vector3 operator +(Vector3 a, Vector3 b) => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Vector3 operator -(Vector3 a, Vector3 b) => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    public static Vector3 operator *(Vector3 a, int scalar) => new(a.X * scalar, a.Y * scalar, a.Z * scalar);

    public double EuclideanDistance(Vector3 other)
    {
        var distance = this - other;
        return Math.Sqrt(distance.X * distance.X + distance.Y * distance.Y + distance.Z * distance.Z);
    }
}
