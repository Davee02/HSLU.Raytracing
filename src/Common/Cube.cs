namespace Common
{
    public readonly struct Cube
    {
        private readonly Triangle[] _triangles;

        /// <summary>
        /// Constructs a cube starting at <paramref name="origin"/> 
        /// with each edge of length <paramref name="size"/>.
        /// All faces have the same <paramref name="color"/>.
        /// </summary>
        public Cube(Vector3 origin, float size, Color color)
        {
            // Define the eight corners of the cube
            //    p0-------p1
            //     |        |
            //     |        |
            //    p2-------p3   (z=0 plane)
            //
            //    p4-------p5
            //     |        |
            //     |        |
            //    p6-------p7   (z=size plane)
            //
            var p0 = origin;
            var p1 = origin + new Vector3(size, 0, 0);
            var p2 = origin + new Vector3(0, size, 0);
            var p3 = origin + new Vector3(size, size, 0);
            var p4 = origin + new Vector3(0, 0, size);
            var p5 = origin + new Vector3(size, 0, size);
            var p6 = origin + new Vector3(0, size, size);
            var p7 = origin + new Vector3(size, size, size);

            // Build each face as two triangles
            // Front face (z=0 plane)
            var frontA = new Triangle(p0, p1, p2, color);
            var frontB = new Triangle(p1, p3, p2, color);

            // Back face (z=size plane)
            var backA = new Triangle(p4, p5, p6, color);
            var backB = new Triangle(p5, p7, p6, color);

            // Left face (x=0 plane)
            var leftA = new Triangle(p0, p2, p4, color);
            var leftB = new Triangle(p2, p6, p4, color);

            // Right face (x=size plane)
            var rightA = new Triangle(p1, p3, p5, color);
            var rightB = new Triangle(p3, p7, p5, color);

            // Top face (y=size plane)
            var topA = new Triangle(p2, p3, p6, color);
            var topB = new Triangle(p3, p7, p6, color);

            // Bottom face (y=0 plane)
            var bottomA = new Triangle(p0, p1, p4, color);
            var bottomB = new Triangle(p1, p5, p4, color);

            _triangles =
            [
                frontA, frontB,
                backA, backB,
                leftA, leftB,
                rightA, rightB,
                topA, topB,
                bottomA, bottomB
            ];
        }

        public IReadOnlyList<Triangle> Triangles => _triangles;
    }
}
