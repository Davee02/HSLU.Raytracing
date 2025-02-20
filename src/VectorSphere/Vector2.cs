namespace VectorSphere;

internal struct Vector2(int x, int y)
{
    public int X => x;

    public int Y => y;

    public static Vector2 operator +(Vector2 a, Vector2 b) => new (a.X + b.X, a.Y + b.Y);

    public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.X - b.X, a.Y - b.Y);

    public static Vector2 operator *(Vector2 a, int scalar) => new(a.X * scalar, a.Y * scalar);

    public double EuclideanDistance(Vector2 other)
    {
        var distance = this - other;
        return Math.Sqrt(distance.X * distance.X + distance.Y * distance.Y);
    }
}
