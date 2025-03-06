using SixLabors.ImageSharp.PixelFormats;
using System.Numerics;

namespace Common
{
    public readonly struct Color(float r, float g, float b)
    {
        public readonly float R => r;

        public readonly float G => g;

        public readonly float B => b;

        public static Color operator +(Color a, Color b) => new(a.R + b.R, a.G + b.G, a.B + b.B);

        public Rgba32 ToRgba32() => new (R, G, B);

        public Vector4 ToVector4() => new(R, G, B, 1);

        public static Color operator *(Color color, float scalar) => new(color.R * scalar, color.G * scalar, color.B * scalar);

        public static Color operator *(Color color, int scalar) => new(color.R * scalar, color.G * scalar, color.B * scalar);

        public static Color operator *(Color color1, Color color2) => new(color1.R * color2.R, color1.G * color2.G, color1.B * color2.B);
    }
}
