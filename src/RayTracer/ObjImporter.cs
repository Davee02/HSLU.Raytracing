using Common;
using System.Globalization;
using System.Numerics;
using Vector2 = System.Numerics.Vector2;

namespace RayTracer;
public static class ObjImporter
{
    public static List<Triangle> ImportObj(string filePath, Material defaultMaterial)
    {
        var objects = new List<Triangle>();
        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();
        var textureCoords = new List<Vector2>();

        // Add a dummy vertex at index 0 because OBJ indices are 1-based
        vertices.Add(new Vector3(0, 0, 0));
        normals.Add(new Vector3(0, 0, 0));
        textureCoords.Add(new Vector2(0, 0));

        try
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                string trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith("#"))
                {
                    continue; // Skip comments and empty lines
                }

                string[] parts = trimmedLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0)
                {
                    continue;
                }

                switch (parts[0].ToLower())
                {
                    case "v": // Vertex
                        if (parts.Length >= 4)
                        {
                            float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                            float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                            float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                            vertices.Add(new Vector3(x, y, z));
                        }
                        break;

                    case "vn": // Vertex normal
                        if (parts.Length >= 4)
                        {
                            float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                            float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                            float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                            var normal = new Vector3(x, y, z);
                            normals.Add(-normal);
                        }
                        break;

                    case "vt": // Texture coordinate
                        if (parts.Length >= 3)
                        {
                            float u = float.Parse(parts[1], CultureInfo.InvariantCulture);
                            float v = float.Parse(parts[2], CultureInfo.InvariantCulture);
                            textureCoords.Add(new Vector2(u, v));
                        }
                        break;

                    case "f": // Face
                        if (parts.Length >= 4) // At least 3 vertices to form a face
                        {
                            ProcessFace(parts, vertices, objects, defaultMaterial);
                        }
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading OBJ file: {ex.Message}");
        }

        return objects;
    }

    private static void ProcessFace(string[] parts, List<Vector3> vertices, List<Triangle> objects, Material material)
    {
        // Handle triangulation for faces with more than 3 vertices
        for (int i = 1; i < parts.Length - 2; i++)
        {
            // Create a triangle for each three consecutive vertices
            int v1Index = ParseVertexIndex(parts[1]);
            int v2Index = ParseVertexIndex(parts[i + 1]);
            int v3Index = ParseVertexIndex(parts[i + 2]);

            if (v1Index >= 0 && v1Index < vertices.Count &&
                v2Index >= 0 && v2Index < vertices.Count &&
                v3Index >= 0 && v3Index < vertices.Count)
            {
                var origin = vertices[v1Index];
                var w = vertices[v2Index] - origin; // Edge vector 1
                var v = vertices[v3Index] - origin; // Edge vector 2

                var triangle = new Triangle(origin, v, w, material);
                objects.Add(triangle);
            }
        }
    }

    private static int ParseVertexIndex(string indexStr)
    {
        // Handle the various face formats (v, v/vt, v/vt/vn, v//vn)
        string[] indices = indexStr.Split('/');
        if (indices.Length > 0 && int.TryParse(indices[0], out int vIndex))
        {
            // OBJ indices are 1-based, convert to 0-based
            return vIndex;
        }
        return -1;
    }
}

