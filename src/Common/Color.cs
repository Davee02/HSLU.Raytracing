using SixLabors.ImageSharp.PixelFormats;
using System.Numerics;

namespace Common
{
    public struct Color(float r, float g, float b)
    {
        public static readonly Color Black = new(0, 0, 0);
        public static readonly Color White = new(1, 1, 1);
        public static readonly Color Red = new(1, 0, 0);
        public static readonly Color Green = new(0, 1, 0);
        public static readonly Color Blue = new(0, 0, 1);
        public static readonly Color Yellow = new(1, 1, 0);
        public static readonly Color Cyan = new(0, 1, 1);
        public static readonly Color Magenta = new(1, 0, 1);
        public static readonly Color Gray = new(0.5f, 0.5f, 0.5f);
        public static readonly Color LightGray = new(0.75f, 0.75f, 0.75f);
        public static readonly Color DarkGray = new(0.25f, 0.25f, 0.25f);

        public float R { get; set; } = r;

        public float G { get; set; } = g;

        public float B { get; set; } = b;

        public static Color operator +(Color a, Color b) => new(a.R + b.R, a.G + b.G, a.B + b.B);

        public readonly Rgba32 ToRgba32() => new (R, G, B);

        public readonly Vector4 ToVector4() => new(R, G, B, 1);

        public static Color operator *(Color color, float scalar) => new(color.R * scalar, color.G * scalar, color.B * scalar);

        public static Color operator *(Color color, int scalar) => new(color.R * scalar, color.G * scalar, color.B * scalar);

        public static Color operator /(Color color, int scalar) => new(color.R / scalar, color.G / scalar, color.B / scalar);

        public static Color operator *(Color color1, Color color2) => new(color1.R * color2.R, color1.G * color2.G, color1.B * color2.B);

        public static bool operator ==(Color left, Color right) => left.R == right.R && left.G == right.G && left.B == right.B;

        public static bool operator !=(Color left, Color right) => !(left == right);
    }
}
