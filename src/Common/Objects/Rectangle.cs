using System.Numerics;
namespace Common.Objects
{
    public readonly struct Rectangle
    {
        private readonly Triangle[] _triangles;

        public Material Material { get; }

        /// <summary>
        /// Constructs a rectangle with thickness using triangles for internal representation.
        /// </summary>
        /// <param name="position">Center position of the front face</param>
        /// <param name="normal">Direction the front face points</param>
        /// <param name="up">Up direction used to calculate orientation</param>
        /// <param name="width">Width of the rectangle</param>
        /// <param name="height">Height of the rectangle</param>
        /// <param name="thickness">Thickness of the rectangle</param>
        /// <param name="material">Material properties</param>
        public Rectangle(Vector3 position, Vector3 normal, Vector3 up, float width, float height, float thickness, Material material)
        {
            Material = material;

            // Normalize vectors
            normal = Vector3.Normalize(normal);

            // Calculate width direction (right vector) using cross product of up and normal
            var widthDir = Vector3.Normalize(Vector3.Cross(up, normal));

            // Calculate true height direction (up vector) using cross product of normal and width
            var heightDir = Vector3.Normalize(Vector3.Cross(normal, widthDir));

            // Half extents
            var halfWidth = width / 2f;
            var halfHeight = height / 2f;

            // Create the 8 corners of the box
            Vector3 frontCenter = position;
            Vector3 backCenter = position + normal * thickness;

            // Front face corners (when looking at front face)
            Vector3 frontBL = frontCenter - widthDir * halfWidth - heightDir * halfHeight;
            Vector3 frontBR = frontCenter + widthDir * halfWidth - heightDir * halfHeight;
            Vector3 frontTR = frontCenter + widthDir * halfWidth + heightDir * halfHeight;
            Vector3 frontTL = frontCenter - widthDir * halfWidth + heightDir * halfHeight;

            // Back face corners (when looking at back face)
            Vector3 backBL = backCenter - widthDir * halfWidth - heightDir * halfHeight;
            Vector3 backBR = backCenter + widthDir * halfWidth - heightDir * halfHeight;
            Vector3 backTR = backCenter + widthDir * halfWidth + heightDir * halfHeight;
            Vector3 backTL = backCenter - widthDir * halfWidth + heightDir * halfHeight;

            // Create all 12 triangles (2 for each face)
            _triangles = new Triangle[12];

            // Front face (face with normal pointing to -normal)
            // Desired outward normal: -normal
            _triangles[0] = new Triangle(frontBL, frontBR - frontBL, frontTL - frontBL, material);
            _triangles[1] = new Triangle(frontBR, frontTR - frontBR, frontTL - frontBR, material);

            // Back face (face with normal pointing to normal)
            // Desired outward normal: normal
            _triangles[2] = new Triangle(backBL, backTL - backBL, backBR - backBL, material);
            _triangles[3] = new Triangle(backBR, backTL - backBR, backTR - backBR, material);

            // Left face (face with normal pointing to -widthDir)
            // Desired outward normal: -widthDir
            _triangles[4] = new Triangle(frontBL, frontTL - frontBL, backBL - frontBL, material);
            _triangles[5] = new Triangle(backBL, frontTL - backBL, backTL - backBL, material);

            // Right face (face with normal pointing to widthDir)
            // Desired outward normal: widthDir
            _triangles[6] = new Triangle(frontBR, backBR - frontBR, frontTR - frontBR, material);
            _triangles[7] = new Triangle(frontTR, backBR - frontTR, backTR - frontTR, material);

            // Top face (face with normal pointing to heightDir)
            // Desired outward normal: heightDir
            _triangles[8] = new Triangle(frontTL, frontTR - frontTL, backTL - frontTL, material);
            _triangles[9] = new Triangle(backTL, frontTR - backTL, backTR - backTL, material);

            // Bottom face (face with normal pointing to -heightDir)
            // Desired outward normal: -heightDir
            _triangles[10] = new Triangle(frontBL, backBL - frontBL, frontBR - frontBL, material);
            _triangles[11] = new Triangle(frontBR, backBL - frontBR, backBR - frontBR, material);
        }

        /// <summary>
        /// Creates a rectangular wall with equal width and height.
        /// </summary>
        public static Rectangle CreateWall(Vector3 center, Vector3 normal, float size, float thickness, Material material)
        {
            // Choose an up vector that's not parallel to the normal
            var up = normal.Y != 0 ? new Vector3(0, 0, 1) : new Vector3(0, 1, 0);
            return new Rectangle(center, normal, up, size, size, thickness, material);
        }

        /// <summary>
        /// Provides access to the triangles for debugging or rendering.
        /// </summary>
        public IReadOnlyList<Triangle> Triangles => _triangles;
    }
}