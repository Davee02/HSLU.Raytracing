namespace Common;
public struct Material(Color diffuseColor, float reflectivity = 0f, float shininess = 0f, float transparency = 0f, float refractionIndex = 1f)
{
    public string Name { get; set; } = string.Empty;

    public Color DiffuseColor { get; set; } = diffuseColor;

    public Color SpecularColor { get; set; } = Color.White;

    public Color AmbientColor { get; set; } = Color.Black;

    public Color EmissiveColor { get; set; } = Color.Black;

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