namespace Common;
public static class ColorUtils
{
    public static Color Normalize(this Color color)
    {
        // normalize color values to the range [0, 1] by taking the maximum of the color components
        var max = Math.Max(color.R, Math.Max(color.G, color.B));

        return max < 1 
            ? color // don't normalize if max is less than 1
            : new Color(color.R / max, color.G / max, color.B / max);
    }
}
