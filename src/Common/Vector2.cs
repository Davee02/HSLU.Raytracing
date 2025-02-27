namespace Common;

public readonly struct Vector2(int x, int y)
{
    public readonly int X => x;

    public readonly int Y => y;

    public double Length => Math.Sqrt(X * X + Y * Y);

    public static Vector2 operator +(Vector2 a, Vector2 b) => new (a.X + b.X, a.Y + b.Y);

    public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.X - b.X, a.Y - b.Y);

    public static Vector2 operator *(Vector2 a, int scalar) => new(a.X * scalar, a.Y * scalar);

    public readonly double EuclideanDistance(Vector2 other)
    {
        var distance = this - other;
        return Math.Sqrt(distance.X * distance.X + distance.Y * distance.Y);
    }

    public double ScalarProduct(Vector2 other) => (this.X * other.X) + (this.Y * other.Y);

    public double ScalarProduct(Vector2 b, double angle) => this.Length * b.Length * Math.Cos(angle);
}
