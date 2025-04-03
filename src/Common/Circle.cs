namespace Common
{
    public readonly struct Circle(MyVector2 center, float radius, Color color)
    {
        public MyVector2 Center { get; } = center;

        public float Radius { get; } = radius;

        public Color Color { get; } = color;

        public bool ContainsPixel(MyVector2 pixel)
        {
            return Center.EuclideanDistance(pixel) <= Radius;
        }
    }
}
