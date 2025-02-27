namespace Common
{
    public readonly struct Circle(Vector2 center, float radius, Color color)
    {
        public Vector2 Center { get; } = center;

        public float Radius { get; } = radius;

        public Color Color { get; } = color;

        public bool ContainsPixel(Vector2 pixel)
        {
            return Center.EuclideanDistance(pixel) <= Radius;
        }
    }
}
