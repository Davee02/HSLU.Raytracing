namespace Common;
public struct Material(Color color, float reflectivity = 0f, float shininess = 0f, float transparency = 0f, float refractionIndex = 1f)
{
    public Color Color { get; set; } = color;

    public float Reflectivity { get; set; } = reflectivity;

    public float Shininess { get; set; } = shininess;

    public float Transparency { get; set; } = transparency;

    public float RefractionIndex { get; set; } = refractionIndex;
}

public static class RefractiveIndices
{
    public const float Vacuum = 1.0f;
    public const float Air = 1.000293f;
    public const float Ice = 1.31f;
    public const float Water = 1.333f;
    public const float Glass = 1.5f;
    public const float Diamond = 2.42f;
}