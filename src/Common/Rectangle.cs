namespace Common
{
    public readonly struct Rectangle
    {
        public Rectangle(Vector3 origin, Vector3 widthVector, Vector3 heightVector, Color color)
        {
            // Define the four corners of the rectangle
            Vector3 p1 = origin;
            Vector3 p2 = origin + widthVector;
            Vector3 p3 = origin + heightVector;
            Vector3 p4 = origin + widthVector + heightVector;

            // Create two triangles that form the rectangle
            Triangle1 = new Triangle(p1, p2, p3, color);
            Triangle2 = new Triangle(p2, p4, p3, color);
        }

        public readonly Triangle Triangle1 { get; }

        public readonly Triangle Triangle2 { get; }
    }
}
