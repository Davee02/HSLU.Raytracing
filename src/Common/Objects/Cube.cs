using System.Numerics;

namespace Common.Objects
{
    public readonly struct Cube
    {
        private readonly Triangle[] _triangles = new Triangle[12];

        public IReadOnlyList<Triangle> Triangles => _triangles;

        /// <summary>
        /// Constructs a cube starting at <paramref name="position"/> 
        /// with each edge of length <paramref name="sideLength"/>.
        /// All faces have the same <paramref name="material"/>.
        /// Yaw (rotation around the Y axis), pitch (X axis), and roll (Z axis) are defined in the vector <paramref name="rotationAnglesDegrees"/>.
        /// </summary>
        public Cube(Vector3 position, float sideLength, Vector3 rotationAnglesDegrees, Material material)
        {
            var rotationAnglesRadians = new Vector3(
                rotationAnglesDegrees.X.ToRadians(),
                rotationAnglesDegrees.Y.ToRadians(),
                rotationAnglesDegrees.Z.ToRadians());

            float x = position.X;
            float y = position.Y;
            float z = position.Z;
            float L = sideLength;

            // Compute the center of the cube
            var center = position + new Vector3(sideLength / 2f, sideLength / 2f, sideLength / 2f);

            // Create a rotation matrix about the Y-axis (for example).
            // You can also use CreateRotationX or CreateRotationZ, or a combination.
            var rotationMatrix = Matrix4x4.CreateFromYawPitchRoll(rotationAnglesRadians.Y, rotationAnglesRadians.X, rotationAnglesRadians.Z);

            // Helper function to rotate a vertex around the cube's center
            Vector3 Rotate(Vector3 v)
            {
                // Step 1: translate to center-based coords
                Vector3 translated = v - center;

                // Step 2: apply rotation
                Vector3 rotated = Vector3.Transform(translated, rotationMatrix);

                // Step 3: translate back
                return rotated + center;
            }

            // Define the 8 vertices of the cube.
            // Assuming origin is the minimum corner (lowest x, y, z).
            var v0 = new Vector3(x, y, z);
            var v1 = new Vector3(x + L, y, z);
            var v2 = new Vector3(x + L, y + L, z);
            var v3 = new Vector3(x, y + L, z);
            var v4 = new Vector3(x, y, z + L);
            var v5 = new Vector3(x + L, y, z + L);
            var v6 = new Vector3(x + L, y + L, z + L);
            var v7 = new Vector3(x, y + L, z + L);

            // Rotate each vertex
            v0 = Rotate(v0);
            v1 = Rotate(v1);
            v2 = Rotate(v2);
            v3 = Rotate(v3);
            v4 = Rotate(v4);
            v5 = Rotate(v5);
            v6 = Rotate(v6);
            v7 = Rotate(v7);

            // Front face (face with z = z)
            // Desired outward normal: (0, 0, 1)
            // Use a clockwise ordering when viewed from the front.
            _triangles[0] = new Triangle(v0, v1 - v0, v3 - v0, material);
            _triangles[1] = new Triangle(v1, v2 - v1, v3 - v1, material);

            // Back face (face with z = z + L)
            // Desired outward normal: (0, 0, -1)
            // Order the vertices clockwise when viewed from the back.
            _triangles[2] = new Triangle(v4, v6 - v4, v5 - v4, material);
            _triangles[3] = new Triangle(v4, v7 - v4, v6 - v4, material);

            // Left face (face with x = x)
            // Desired outward normal: (1, 0, 0)
            _triangles[4] = new Triangle(v0, v3 - v0, v7 - v0, material);
            _triangles[5] = new Triangle(v0, v7 - v0, v4 - v0, material);

            // Right face (face with x = x + L)
            // Desired outward normal: (-1, 0, 0)
            _triangles[6] = new Triangle(v1, v5 - v1, v6 - v1, material);
            _triangles[7] = new Triangle(v1, v6 - v1, v2 - v1, material);

            // Top face (face with y = y + L)
            // Desired outward normal: (0, -1, 0)
            _triangles[8] = new Triangle(v3, v2 - v3, v6 - v3, material);
            _triangles[9] = new Triangle(v3, v6 - v3, v7 - v3, material);

            // Bottom face (face with y = y)
            // Desired outward normal: (0, 1, 0)
            _triangles[10] = new Triangle(v0, v4 - v0, v5 - v0, material);
            _triangles[11] = new Triangle(v0, v5 - v0, v1 - v0, material);
        }
    }
}
