namespace Common;
public struct Material(Color color, float reflectivity, float shininess)
{
    public Color Color { get; set; } = color;

    public float Reflectivity { get; set; } = reflectivity;

    public float Shininess { get; set; } = shininess;
}
