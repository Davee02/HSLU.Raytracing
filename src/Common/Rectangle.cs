namespace Common
{
    public readonly struct Rectangle
    {
        public Rectangle(Vector3 origin, Vector3 w, Vector3 v, Color color)
        {
            // Create two triangles that form the rectangle
            Triangle1 = new Triangle(origin, w, v, color);
            Triangle2 = new Triangle(origin + w + v, -w, -v, color);
        }

        public readonly Triangle Triangle1 { get; }

        public readonly Triangle Triangle2 { get; }
    }
}
