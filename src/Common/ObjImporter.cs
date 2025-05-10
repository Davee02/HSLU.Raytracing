using Common.Objects;
using System.Globalization;
using System.Numerics;

namespace Common;
public static class ObjImporter
{
    public static IEnumerable<Material> LoadMaterialsFromFile(string filePath)
    {
        try
        {
            var currentMaterial = Material.Default;
            bool materialStarted = false;
            var materials = new List<Material>();

            string[] lines = File.ReadAllLines(filePath);

            foreach (var line in lines)
            {
                string trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith('#'))
                    continue;

                string[] parts = trimmedLine.Split([' '], StringSplitOptions.RemoveEmptyEntries);
                string keyword = parts[0].ToLower();

                switch (keyword)
                {
                    case "newmtl":
                        if (materialStarted)
                        {
                            materials.Add(currentMaterial);
                        }

                        materialStarted = true;
                        currentMaterial = Material.Default;
                        currentMaterial.Name = parts[1];
                        break;

                    case "ka": // Ambient color
                        if (parts.Length >= 4)
                            currentMaterial.AmbientColor = ParseColor(parts[1], parts[2], parts[3]);
                        break;

                    case "kd": // Diffuse color
                        if (parts.Length >= 4)
                            currentMaterial.DiffuseColor = ParseColor(parts[1], parts[2], parts[3]);
                        break;

                    case "ks": // Specular color
                        if (parts.Length >= 4)
                            currentMaterial.SpecularColor = ParseColor(parts[1], parts[2], parts[3]);
                        break;

                    case "ke": // Emissive color
                        if (parts.Length >= 4)
                            currentMaterial.EmissiveColor = ParseColor(parts[1], parts[2], parts[3]);
                        break;

                    case "ns": // Specular exponent (shininess)
                        if (parts.Length >= 2)
                            currentMaterial.Shininess = float.Parse(parts[1], CultureInfo.InvariantCulture);
                        break;

                    case "ni": // Optical density (refraction index)
                        if (parts.Length >= 2)
                            currentMaterial.RefractionIndex = float.Parse(parts[1], CultureInfo.InvariantCulture);
                        break;

                    case "d": // Dissolve (transparency)
                        if (parts.Length >= 2)
                        {
                            float dissolve = float.Parse(parts[1], CultureInfo.InvariantCulture);
                            currentMaterial.Transparency = 1.0f - dissolve; // Convert dissolve to transparency
                        }
                        break;

                    case "illum": // Illumination model
                        if (parts.Length >= 2)
                        {
                            int illumModel = int.Parse(parts[1]);
                            // Set reflectivity based on illumination model
                            if (illumModel is >= 3 and <= 7)
                            {
                                currentMaterial.Reflectivity = 0.8f; // Default reflectivity for reflective models
                            }
                        }
                        break;
                }
            }

            if (materialStarted)
            {
                materials.Add(currentMaterial);
            }

            return materials;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading MTL file: {ex.Message}");
            throw;
        }
    }

    public static IEnumerable<Triangle> LoadFromFile(string objFilePath)
    {
        if (string.IsNullOrEmpty(objFilePath) || !File.Exists(objFilePath))
        {
            throw new FileNotFoundException($"OBJ file not found: {objFilePath}");
        }

        var vertices = new List<Vector3>();
        var normals = new List<Vector3>();
        var triangles = new List<Triangle>();

        // Variables to track the current material
        Dictionary<string, Material> materials = [];
        var currentMaterial = new Material();
        var objDirectory = Path.GetDirectoryName(objFilePath);

        try
        {
            string[] lines = File.ReadAllLines(objFilePath);

            foreach (var line in lines)
            {
                string trimmedLine = line.Trim();
                if (string.IsNullOrEmpty(trimmedLine) || trimmedLine.StartsWith('#'))
                    continue;

                string[] parts = trimmedLine.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string keyword = parts[0].ToLower();

                switch (keyword)
                {
                    case "mtllib": // Material library
                        if (parts.Length >= 2 && !string.IsNullOrEmpty(objDirectory))
                        {
                            string mtlFilePath = Path.Combine(objDirectory, parts[1]);
                            if (File.Exists(mtlFilePath))
                            {
                                var loadedMaterials = LoadMaterialsFromFile(mtlFilePath);
                                materials = loadedMaterials.ToDictionary(m => m.Name, m => m);
                            }
                            else
                            {
                                throw new FileNotFoundException($"MTL file not found: {mtlFilePath}");
                            }
                        }
                        break;

                    case "v": // Vertex
                        if (parts.Length >= 4)
                        {
                            float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                            float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                            float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                            vertices.Add(new Vector3(x, y, z));
                        }
                        break;

                    case "vn": // Normal
                        if (parts.Length >= 4)
                        {
                            float x = float.Parse(parts[1], CultureInfo.InvariantCulture);
                            float y = float.Parse(parts[2], CultureInfo.InvariantCulture);
                            float z = float.Parse(parts[3], CultureInfo.InvariantCulture);
                            normals.Add(Vector3.Normalize(new Vector3(x, y, z))); // Ensure normals are normalized
                        }
                        break;

                    case "f": // Face
                        if (parts.Length >= 4) // At least 3 vertices for a triangle
                        {
                            // OBJ indices are 1-based, so subtract 1
                            var faceIndices = new List<(int vertexIndex, int normalIndex)>();

                            for (int i = 1; i < parts.Length; i++)
                            {
                                string[] indices = parts[i].Split('/');
                                int vertexIndex = int.Parse(indices[0]) - 1;

                                // Handle normal index if present
                                int normalIndex = -1;
                                if (indices.Length >= 3 && !string.IsNullOrEmpty(indices[2]))
                                {
                                    normalIndex = int.Parse(indices[2]) - 1;
                                }

                                faceIndices.Add((vertexIndex, normalIndex));
                            }

                            // Create triangles for triangulated mesh
                            if (faceIndices.Count == 3) // It's already a triangle
                            {
                                var triangle = CreateTriangle(vertices, normals, faceIndices, currentMaterial);
                                triangles.Add(triangle);
                            }
                            else if (faceIndices.Count > 3) // Polygon with more than 3 vertices
                            {
                                throw new NotImplementedException("Polygon triangulation is not implemented. Please provide a triangulated mesh.");

                            }
                        }
                        break;

                    case "usemtl": // Use material
                        if (parts.Length >= 2)
                        {
                            string materialName = parts[1];
                            if (materials != null && materials.TryGetValue(materialName, out Material value))
                            {
                                currentMaterial = value;
                            }
                            else
                            {
                                throw new Exception($"Material '{materialName}' not found in the loaded materials.");
                            }
                        }
                        break;
                }
            }

            return triangles;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading OBJ file: {ex.Message}");
            throw;
        }
    }

    private static Color ParseColor(string r, string g, string b)
    {
        float red = float.Parse(r, CultureInfo.InvariantCulture);
        float green = float.Parse(g, CultureInfo.InvariantCulture);
        float blue = float.Parse(b, CultureInfo.InvariantCulture);

        return new Color(red, green, blue);
    }

    private static Triangle CreateTriangle(
         List<Vector3> vertices,
         List<Vector3> normals,
         List<(int vertexIndex, int normalIndex)> indices,
         Material material)
    {
        var origin = vertices[indices[0].vertexIndex];
        var v = vertices[indices[1].vertexIndex] - origin;
        var w = vertices[indices[2].vertexIndex] - origin;

        // Use face normal if provided in the OBJ file
        // If all vertices have the same normal, use that
        // Otherwise, calculate the normal
        Vector3 normal;

        if (indices[0].normalIndex >= 0 && indices[1].normalIndex >= 0 && indices[2].normalIndex >= 0)
        {
            normal = Vector3.Normalize(
                normals[indices[0].normalIndex] +
                normals[indices[1].normalIndex] +
                normals[indices[2].normalIndex]
            );
        }
        else
        {
            throw new Exception("No normals provided for triangle vertices. Cannot create triangle.");
        }

        return new Triangle(origin, v, w, normal, material);
    }
}

