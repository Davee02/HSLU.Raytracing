namespace Common;

public readonly struct MyVector2(int x, int y)
{
    public readonly int X => x;

    public readonly int Y => y;

    public double Length => Math.Sqrt((X * X) + (Y * Y));

    public static MyVector2 operator +(MyVector2 a, MyVector2 b) => new (a.X + b.X, a.Y + b.Y);

    public static MyVector2 operator -(MyVector2 a, MyVector2 b) => new(a.X - b.X, a.Y - b.Y);

    public static MyVector2 operator *(MyVector2 a, int scalar) => new(a.X * scalar, a.Y * scalar);

    public readonly double EuclideanDistance(MyVector2 other)
    {
        var distance = this - other;
        return Math.Sqrt((distance.X * distance.X) + (distance.Y * distance.Y));
    }

    public double ScalarProduct(MyVector2 other) => (this.X * other.X) + (this.Y * other.Y);

    public double ScalarProduct(MyVector2 b, double angle) => this.Length * b.Length * Math.Cos(angle);
}
